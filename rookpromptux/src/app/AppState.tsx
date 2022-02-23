export enum MainViewType {
    FirstNavigation = "FirstNavigation",
    ShowPrompts = "ShowPrompts",
    ShowEntries = "ShowEntries",
    RandomPrompt = "RandomPrompt",
    Login = "Login",
}

export interface AppState {
    mainView: MainViewType;
}