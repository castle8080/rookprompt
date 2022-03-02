namespace rookpromptapi.Services.Impl

open System
open System.Text
open System.Threading.Tasks
open System.Security.Cryptography 

open rookpromptapi.Services
open rookpromptapi.Models
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open Microsoft.IdentityModel.Tokens

type JWTTokenService(
    secretService: ISecretService,
    secretName: string,
    tokenExpiresHours: float,
    secretExpiresHours: float) =

    let newRawSecret() =
        use rng = RandomNumberGenerator.Create()
        let secretBytes = Array.zeroCreate<byte> 64
        rng.GetBytes(secretBytes)
        Convert.ToBase64String secretBytes

    let newSecret (): Secret =
        {
            Id = secretName
            Secret = newRawSecret()
            Description = "Secret for creating tokens."
            Expires = DateTime.UtcNow.AddHours secretExpiresHours
            Updated = DateTime.UtcNow
            Created = DateTime.UtcNow
        }

    let getSecret () =
        async {
            match! secretService.FindById secretName with
                | None ->
                    let secret = newSecret()
                    return! secretService.Create secret
                | Some(secret) ->
                    if secret.Expires < DateTime.UtcNow then
                        let secret = newSecret()
                        return! secretService.Create secret
                    else
                        return secret
        }

    let generateToken (secret: Secret) (user: User) =
        let secretBytes = Convert.FromBase64String secret.Secret

        let claims =
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                new Claim(ClaimTypes.NameIdentifier, user.Name)
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString("o"))
            ]
            @ (user.Roles |> List.map (function r -> new Claim(ClaimTypes.Role, r)))

        let creds = new SigningCredentials(
            new SymmetricSecurityKey(secretBytes),
            SecurityAlgorithms.HmacSha256
        )

        let std = new SecurityTokenDescriptor()
        std.Subject <- new ClaimsIdentity(Array.ofList claims)
        std.Expires <- DateTime.UtcNow.AddHours tokenExpiresHours
        std.SigningCredentials <- creds

        let token = new JwtSecurityToken(
            "rookpromptapi",
            "rookpromptapi",
            claims,
            expires = std.Expires,
            signingCredentials = creds
        )
        
        let tokenHandler = new JwtSecurityTokenHandler()
        tokenHandler.WriteToken(token)

    let validate (secret: Secret) (token: string) =
        let secretBytes = Convert.FromBase64String secret.Secret

        let tvp = new TokenValidationParameters()
        tvp.ValidateIssuerSigningKey <- true
        tvp.IssuerSigningKey <- new SymmetricSecurityKey(secretBytes)
        tvp.ValidateIssuer <- false
        tvp.ValidateAudience <- false
        tvp.ClockSkew <- TimeSpan.Zero

        let tokenHandler = new JwtSecurityTokenHandler()
        let (claimsPrincipal, validatedToken) = tokenHandler.ValidateToken(token, tvp)
        let jwt = validatedToken :?> JwtSecurityToken

        let user: User = {
            Id = ""
            Name = ""
            Email = ""
            Roles = []
            Created = DateTime.UtcNow
            Updated = DateTime.UtcNow
        }

        let applyClaim (user: User) (claim: Claim) =
            match claim.Type with
                | JwtRegisteredClaimNames.Sub ->
                    { user with Id = claim.Value }
                | ClaimTypes.NameIdentifier ->
                    { user with Name = claim.Value }
                | JwtRegisteredClaimNames.Email ->
                    { user with Email = claim.Value }
                | ClaimTypes.Role ->
                    { user with Roles = claim.Value :: user.Roles }
                | _ ->
                    user

        let user = jwt.Claims |> Seq.fold applyClaim user
        let user = { user with Roles = user.Roles |> List.rev }
        user

    interface ITokenService with
        member this.Create(user: User): string Async =
            async {
                let! secret = getSecret()
                return (generateToken secret user)
            }

        member this.Validate(token: string): User Async =
            async {
                let! secret = getSecret()
                return (validate secret token)
            }