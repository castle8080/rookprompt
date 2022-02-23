import React, { ChangeEvent, FormEvent } from 'react';

import { AppContextRef, LoginAction } from '../app/AppActions';

interface LoginState {
    username: string;
    password: string;
}

export class Login extends React.Component<{}, LoginState> {
    static contextType = AppContextRef;

    constructor(props: {}) {
        super(props);
        this.state = {
            username: "",
            password: "",
        };
    }

    onUsernameChange = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({"username": e.target.value});
    }

    onPasswordChange = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({"password": e.target.value});
    }

    onLogin = (e: FormEvent<HTMLFormElement>) => {
        console.log("Do the login!");
        let action = new LoginAction(this.state.username, this.state.password);
        this.context.dispatcher(action);
    }

    render() {
        return (
            <div>
                <form onSubmit={this.onLogin}>
                    <label>Username: </label>
                    <input type="text" size={20} value={this.state.username} onChange={this.onUsernameChange}/>
                    <br/>
                    <label>Password: </label>
                    <input type="password" size={20} value={this.state.password} onChange={this.onPasswordChange}/>
                    <br/>
                    <input type="submit" value="Sign In"/>
                </form>
            </div>
        );
    }
}