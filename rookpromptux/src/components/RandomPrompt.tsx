import React from 'react';
import {
    AppContextRef, SetMainViewAction, ActionDispatcher, GetRandomPromptAction
} from '../app/AppActions';
import { AppState, MainViewType } from '../app/AppState';

export class RandomPrompt extends React.Component {

    static contextType = AppContextRef;

    componentDidMount() {
        this.context.dispatcher(new GetRandomPromptAction());
    }

    render() {
        return (
            <div>
                Here is a random writing prompt!
            </div>
        );
    }
}