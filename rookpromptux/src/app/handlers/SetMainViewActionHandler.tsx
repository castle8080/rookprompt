import { Component } from 'react';
import { ActionHandler } from '../AppActionHandler';
import { AppAction, SetMainViewAction } from '../AppActions';
import { AppState } from '../AppState';

export class SetMainViewActionHandler implements ActionHandler<SetMainViewAction> { 
    handle(action: SetMainViewAction, app: React.Component<any, AppState>): void {
        app.setState({"mainView": action.view});
    }
}