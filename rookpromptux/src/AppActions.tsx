export enum MainViewType {
    FirstNavigation = "FirstNavigation",
    ShowPrompts = "ShowPrompts",
    ShowEntries = "ShowEntries",
    RandomPrompt = "RandomPrompt",
}

export enum ActionType {
    SetMainView = "SetMainView",
    GetRandomPrompt = "GetRandomPrompt",
}
export interface AppState {
    mainView: MainViewType;
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

export type ActionDispatcher = (action: AppAction) => void;

export interface BaseComponentProperties {
    dispatcher: ActionDispatcher;
};

export class ActionHandler {
    handlers = new Map<ActionType, ActionDispatcher>();

    add<T extends AppAction>(actionType: ActionType, handler: (action: T) => void) {
        this.handlers.set(actionType, (action) => { handler(action as T); });
        return this;
    }

    handle = (action: AppAction): void => {
        let handler = this.handlers.get(action.action);
        if (handler !== undefined) {
            handler(action);
        }
        else {
            throw new Error(`Invalid action:  ${action.action}`);
        }
    }

}