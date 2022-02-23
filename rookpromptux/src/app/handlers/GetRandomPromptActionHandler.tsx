import { Component } from 'react';
import { ActionHandler } from '../AppActionHandler';
import { AppAction, GetRandomPromptAction } from '../AppActions';
import { AppState } from '../AppState';

export class GetRandomPromptActionHandler implements ActionHandler<GetRandomPromptAction> { 
    async handle(action: GetRandomPromptAction, app: React.Component<any, AppState>): Promise<void> {
        console.log("Get a random prompt!");
        let r = await (await fetch("/weatherforecast")).json();
        console.log("Have response", r);
    }
}