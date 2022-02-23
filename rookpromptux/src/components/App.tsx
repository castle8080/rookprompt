import React from 'react';
import logo from './logo.svg';
import './App.css';
import { FirstNavigation } from './FirstNavigation';
import { RandomPrompt } from './RandomPrompt';
import { Login } from './Login';
import {
  AppContext, AppContextRef,
  ActionType, AppAction,
  SetMainViewAction, GetRandomPromptAction, LoginAction
} from '../app/AppActions';
import { AppState, MainViewType } from '../app/AppState';
import { ActionHandlerDispatcher } from '../app/AppActionHandler';
import { SetMainViewActionHandler } from '../app/handlers/SetMainViewActionHandler';
import { GetRandomPromptActionHandler } from '../app/handlers/GetRandomPromptActionHandler';
import { LoginActionHandler } from '../app/handlers/LoginActionHandler';

class App extends React.Component<{}, AppState> {
    appContext: AppContext;

    constructor(props: {}) {
        super(props);
        this.state = { "mainView": MainViewType.Login };
        this.appContext = new AppContext(
            (action) => this.actionHandler.handle(action, this)
        );
    }

    actionHandler = new ActionHandlerDispatcher()
        .add<SetMainViewAction>(ActionType.SetMainView, new SetMainViewActionHandler())
        .add<GetRandomPromptAction>(ActionType.GetRandomPrompt, new GetRandomPromptActionHandler())
        .add<LoginAction>(ActionType.Login, new LoginActionHandler());

    render() {
        return (
            <AppContextRef.Provider value={this.appContext}>
                <div className="App">
                    {this.renderMainView()}
                </div>
            </AppContextRef.Provider>
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
            case MainViewType.Login:
                return this.renderLoginView();
            default:
              return this.renderLoginView();
        }
    }

    renderLoginView() {
        return (
            <Login/>
        );
    }

    renderFirstNavigationView() {
        return (
            <FirstNavigation name="Bob"/>
        );
    }

    renderRandomPromptView() {
        return (
            <RandomPrompt/>
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
