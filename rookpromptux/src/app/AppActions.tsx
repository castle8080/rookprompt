import React from 'react';

import { MainViewType } from "./AppState";

export enum ActionType {
    SetMainView = "SetMainView",
    GetRandomPrompt = "GetRandomPrompt",
    Login = "Login",
}

export interface AppAction {
    action: ActionType;
}

export class SetMainViewAction implements AppAction {
    action = ActionType.SetMainView;
    view: MainViewType;

    constructor(view: MainViewType) {
        this.view = view;
    }
}

export class GetRandomPromptAction implements AppAction {
    action = ActionType.GetRandomPrompt;
}

export class LoginAction implements AppAction {
    action = ActionType.Login;
    username: string = "";
    password: string = "";

    constructor(username: string, password: string) {
        this.username = username;
        this.password = password;
    }
}

export type ActionDispatcher = (action: AppAction) => void;

export class AppContext {
    dispatcher: ActionDispatcher;

    constructor(dispatcher: ActionDispatcher) {
        this.dispatcher = dispatcher;
    }
}

export const AppContextRef = React.createContext<AppContext>(new AppContext(
    (action) => {
        console.log(`Action received: ${action.action}`);
    }
));