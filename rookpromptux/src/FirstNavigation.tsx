import React from 'react';
import { SetMainViewAction, ActionDispatcher, MainViewType, BaseComponentProperties } from './AppActions';

interface FirstNavigationParameters extends BaseComponentProperties {
    name: string;
}

interface FirstNavigationState {
    name: string;
}

export class FirstNavigation extends React.Component<FirstNavigationParameters, FirstNavigationState> {

    constructor(props: FirstNavigationParameters) {
        super(props);
        this.state = {
            name: props.name
        };
    }

    onRandomPrompt = (e: React.MouseEvent) => {
        e.preventDefault();
        console.log(this);
        console.log("on random prompt.");
        this.setState({ name: "George Lucas" })
        this.props.dispatcher(new SetMainViewAction(MainViewType.RandomPrompt));
    }

    onViewPrompts = (e: React.MouseEvent) => {
        e.preventDefault();
        console.log("Show prompts");
        this.props.dispatcher(new SetMainViewAction(MainViewType.ShowPrompts));
    }

    onViewWritingEntries = (e: React.MouseEvent) => {
        e.preventDefault();
        console.log("view writing entries.");
        this.props.dispatcher(new SetMainViewAction(MainViewType.ShowEntries));
    }

    render() {
        return (
        <div>
            Hi, {this.state.name}!
            <p/>
            <ul>
                <li><a href='#random_prompt' onClick={this.onRandomPrompt}>Random Prompt</a></li>
                <li><a href='#show_prompts' onClick={this.onViewPrompts}>View Prompts</a></li>
                <li><a href='#show_prompts' onClick={this.onViewWritingEntries}>View Writing Entries</a></li>
            </ul>
        </div>
        );
    }
}