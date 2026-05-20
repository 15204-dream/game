export default class GUIManager {
    constructor() {
        this.elements = [];
        this.visible = true;
    }

    addElement(element) {
        this.elements.push(element);
        return element;
    }

    removeElement(element) {
        const index = this.elements.indexOf(element);
        if (index > -1) {
            this.elements.splice(index, 1);
        }
    }

    clear() {
        this.elements = [];
    }

    update(inputManager) {
        if (!this.visible) return;
        this.elements.forEach(element => {
            if (element.update) {
                element.update(inputManager);
            }
        });
    }

    render(ctx, uiGenerator, fontRenderer) {
        if (!this.visible) return;
        this.elements.forEach(element => {
            if (element.render) {
                element.render(ctx, uiGenerator, fontRenderer);
            }
        });
    }

    setVisible(visible) {
        this.visible = visible;
    }
}
