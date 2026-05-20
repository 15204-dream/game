class StateManager {
    constructor() {
        this.currentState = null;
        this.states = {};
        this.stateData = {};
    }
    
    setState(stateName, data = {}) {
        if (this.currentState && this.states[this.currentState]?.onExit) {
            this.states[this.currentState].onExit();
        }
        
        this.currentState = stateName;
        this.stateData = data;
        
        if (this.states[this.currentState]?.onEnter) {
            this.states[this.currentState].onEnter(data);
        }
    }
    
    registerState(name, state) {
        this.states[name] = state;
    }
    
    update(dt) {
        if (this.currentState && this.states[this.currentState]?.update) {
            this.states[this.currentState].update(dt, this.stateData);
        }
    }
    
    render(renderer) {
        if (this.currentState && this.states[this.currentState]?.render) {
            this.states[this.currentState].render(renderer, this.stateData);
        }
    }
    
    getState() {
        return this.currentState;
    }
    
    getStateData() {
        return this.stateData;
    }
}

export default StateManager;
