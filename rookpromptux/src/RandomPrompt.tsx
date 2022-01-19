import React from 'react';
import { SetMainViewAction, ActionDispatcher, MainViewType, BaseComponentProperties, GetRandomPromptAction } from './AppActions';

export class RandomPrompt extends React.Component<BaseComponentProperties> {

    componentDidMount() {
        this.props.dispatcher(new GetRandomPromptAction());
    }

    render() {
        return (
            <div>
                Here is a random writing prompt!
            </div>
        );
    }
}