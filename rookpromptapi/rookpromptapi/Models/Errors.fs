namespace rookpromptapi.Models

type ErrorCode =
    | EmailAlreadyExists = 1

type Error = {
    ErrorCode: ErrorCode
    Detail: string
}

type SysResult<'a> = Result<'a, Error>

module SysResult =
    let Error (ec: ErrorCode) (detail: string) =
        Result.Error { ErrorCode = ec; Detail = detail }

    let FromErrorCode (ec: ErrorCode) =
        Error ec (ErrorCode.GetName(ec))

    let Ok (r: 'a): SysResult<'a> =
        Result.Ok r