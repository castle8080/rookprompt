import React from 'react';
import logo from './logo.svg';
import './App.css';
import { FirstNavigation } from './FirstNavigation';
import { RandomPrompt } from './RandomPrompt';
import {
  ActionHandler, ActionType, AppState, AppAction,
  SetMainViewAction, MainViewType, GetRandomPromptAction
} from './AppActions';

class App extends React.Component<{}, AppState> {

    constructor(props: {}) {
      super(props);
      this.state = {"mainView": MainViewType.FirstNavigation};
    }

    handleSetMainView = (action: SetMainViewAction) => {
        this.setState({"mainView": action.view});
    }

    handleGetRandomPrompt = async (action: GetRandomPromptAction) => {
        console.log("Get a random prompt!");
        let r = await (await fetch("/weatherforecast")).json();
        console.log("Have response", r);
    };

    actionHandler = new ActionHandler()
        .add<SetMainViewAction>(ActionType.SetMainView, this.handleSetMainView)
        .add<GetRandomPromptAction>(ActionType.GetRandomPrompt, this.handleGetRandomPrompt);

    render() {
      return (
        <div className="App">
          {this.renderMainView()}
        </div>
      );
    }

    renderMainView() {
        switch (this.state.mainView) {
            case MainViewType.FirstNavigation:
                return this.renderFirstNavigationView();
            case MainViewType.RandomPrompt:
              return this.renderRandomPromptView();
            case MainViewType.ShowPrompts:
              return this.renderShowPrompts();
            case MainViewType.ShowEntries:
              return this.renderShowEntries();
            default:
              return this.renderFirstNavigationView();
        }
    }

    renderFirstNavigationView() {
        return (
            <FirstNavigation name="Bob" dispatcher={this.actionHandler.handle}/>
        );
    }

    renderRandomPromptView() {
        return (
            <RandomPrompt dispatcher={this.actionHandler.handle}/>
        );
    }

    renderShowPrompts() {
      return (
          <div>Show the prompts!</div>
      );
    }

    renderShowEntries() {
      return (
          <div>Show the entries!</div>
      );
    }
}

export default App;
