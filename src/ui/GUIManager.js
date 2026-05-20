import Button from './Button.js';
import MenuState from '../states/MenuState.js';

class GUIManager {
    constructor(renderer, inputManager) {
        this.renderer = renderer;
        this.inputManager = inputManager;
        this.elements = [];
        this.dialogQueue = [];
        this.currentDialog = null;
    }
    
    addElement(element) {
        this.elements.push(element);
    }
    
    removeElement(element) {
        const index = this.elements.indexOf(element);
        if (index > -1) {
            this.elements.splice(index, 1);
        }
    }
    
    clearElements() {
        this.elements = [];
    }
    
    update(dt) {
        for (let element of this.elements) {
            element.update(dt, this.inputManager);
        }
        
        if (this.currentDialog) {
            this.currentDialog.update(dt, this.inputManager);
        }
    }
    
    render() {
        for (let element of this.elements) {
            element.render(this.renderer);
        }
        
        if (this.currentDialog) {
            this.currentDialog.render(this.renderer);
        }
    }
    
    showDialog(dialog) {
        this.currentDialog = dialog;
    }
    
    hideDialog() {
        this.currentDialog = null;
    }
}

export default GUIManager;
