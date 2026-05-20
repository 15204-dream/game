export default class Button {
    constructor(x, y, width, height, text, color = '#E94560', callback = null) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.text = text;
        this.color = color;
        this.callback = callback;
        this.hovered = false;
        this.pressed = false;
    }

    update(inputManager) {
        const mouse = inputManager.getMousePosition();
        const wasHovered = this.hovered;
        this.hovered = inputManager.isPointInRect(mouse.x, mouse.y, this);

        if (this.hovered && inputManager.isMouseClicked() && this.callback) {
            this.callback();
        }
    }

    render(ctx, uiGenerator, fontRenderer) {
        uiGenerator.drawButton(this.x, this.y, this.width, this.height, this.text, this.color, this.hovered);
        
        const textColor = this.hovered ? '#E94560' : '#1A1A2E';
        const textWidth = fontRenderer.measureTextWidth(this.text, 1.2);
        const textX = this.x + (this.width - textWidth) / 2;
        const textY = this.y + (this.height - 20) / 2;
        fontRenderer.drawText(this.text, textX, textY, textColor, 1.2);
    }

    getBounds() {
        return { x: this.x, y: this.y, width: this.width, height: this.height };
    }
}
