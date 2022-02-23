import { AppAction, ActionType } from './AppActions';
import { AppState } from './AppState';

export interface ActionHandler<T extends AppAction> {
    handle: (action: T, app: React.Component<any, AppState>) => void;
}

export class ActionHandlerDispatcher implements ActionHandler<AppAction> {
    handlers = new Map<ActionType, ActionHandler<AppAction>>();

    add<T extends AppAction>(actionType: ActionType, handler: ActionHandler<T>) {
        this.handlers.set(actionType, {
            handle: (action, app) => {
                handler.handle(action as T, app);
            }
        });
        return this;
    }

    handle = (action: AppAction, app: React.Component<any, AppState>): void => {
        let handler = this.handlers.get(action.action);
        if (handler !== undefined) {
            handler.handle(action, app);
        }
        else {
            throw new Error(`Invalid action:  ${action.action}`);
        }
    }
}