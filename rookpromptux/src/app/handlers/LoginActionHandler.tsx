import { Component } from 'react';
import { ActionHandler } from '../AppActionHandler';
import { AppAction, LoginAction } from '../AppActions';
import { AppState } from '../AppState';

export class LoginActionHandler implements ActionHandler<LoginAction> { 
    handle(action: LoginAction, app: React.Component<any, AppState>): void {
        console.log(`Login action processing: ${action.username}`);
    }
}