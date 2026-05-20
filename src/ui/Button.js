class Button {
    constructor(x, y, width, height, text, callback, color = null, hoverColor = null) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.text = text;
        this.callback = callback;
        this.color = color || '#2a2a4e';
        this.hoverColor = hoverColor || '#3a3a6e';
        this.borderColor = '#4a4a6a';
        this.textColor = '#ffffff';
        this.isHovered = false;
        this.isClicked = false;
    }
    
    update(dt, inputManager) {
        const mouse = inputManager.getMousePosition();
        this.isHovered = inputManager.isPointInRect(mouse.x, mouse.y, this.x, this.y, this.width, this.height);
        
        if (this.isHovered && inputManager.isJustClicked()) {
            this.isClicked = true;
            if (this.callback) {
                this.callback();
            }
        } else {
            this.isClicked = false;
        }
    }
    
    render(renderer) {
        const bgColor = this.isHovered ? this.hoverColor : this.color;
        renderer.drawPixelRect(this.x, this.y, this.width, this.height, bgColor, this.borderColor, 3);
        
        const textX = this.x + this.width / 2;
        const textY = this.y + this.height / 2 - 8;
        renderer.drawPixelText(this.text, textX, textY, this.textColor, 16, 'center');
    }
    
    setPosition(x, y) {
        this.x = x;
        this.y = y;
    }
    
    setText(text) {
        this.text = text;
    }
}

export default Button;
