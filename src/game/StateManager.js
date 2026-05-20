export default class StateManager {
    constructor() {
        this.states = {
            MENU: 'menu',
            MODE_SELECT: 'mode_select',
            GUEST_MODE: 'guest_mode',
            DIRECTOR_MODE: 'director_mode',
            ENDING_GALLERY: 'ending_gallery'
        };
        this.currentState = this.states.MENU;
        this.previousState = null;
    }

    setState(newState) {
        this.previousState = this.currentState;
        this.currentState = newState;
        console.log(`State changed to: ${newState}`);
    }

    getCurrentState() {
        return this.currentState;
    }

    goBack() {
        if (this.previousState) {
            this.setState(this.previousState);
        }
    }

    isInState(state) {
        return this.currentState === state;
    }
}
