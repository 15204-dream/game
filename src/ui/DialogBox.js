class DialogBox {
    constructor(x, y, width, height, text, speaker = null, options = [], onOptionSelect = null) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.text = text;
        this.speaker = speaker;
        this.options = options;
        this.onOptionSelect = onOptionSelect;
        
        this.displayText = '';
        this.textIndex = 0;
        this.textSpeed = 30;
        this.textTimer = 0;
        this.isTyping = true;
        this.selectedOption = 0;
        this.showOptions = false;
    }
    
    update(dt, inputManager) {
        if (this.isTyping) {
            this.textTimer += dt * 1000;
            if (this.textTimer >= this.textSpeed) {
                this.textTimer = 0;
                if (this.textIndex < this.text.length) {
                    this.displayText += this.text[this.textIndex];
                    this.textIndex++;
                } else {
                    this.isTyping = false;
                    if (this.options.length > 0) {
                        this.showOptions = true;
                    }
                }
            }
            
            if (inputManager.isJustClicked()) {
                this.skipTyping();
            }
        } else if (this.showOptions) {
            const mouse = inputManager.getMousePosition();
            
            for (let i = 0; i < this.options.length; i++) {
                const optionY = this.y + this.height + 20 + i * 50;
                if (inputManager.isPointInRect(mouse.x, mouse.y, this.x, optionY, this.width, 45)) {
                    this.selectedOption = i;
                    if (inputManager.isJustClicked()) {
                        this.selectOption(i);
                    }
                    break;
                }
            }
        }
    }
    
    skipTyping() {
        this.displayText = this.text;
        this.textIndex = this.text.length;
        this.isTyping = false;
        if (this.options.length > 0) {
            this.showOptions = true;
        }
    }
    
    selectOption(index) {
        if (this.onOptionSelect) {
            this.onOptionSelect(index, this.options[index]);
        }
    }
    
    render(renderer) {
        renderer.drawPixelRect(this.x, this.y, this.width, this.height, '#1a1a2e', '#4a4a6a', 4);
        
        if (this.speaker) {
            renderer.drawPixelRect(this.x, this.y - 35, 150, 35, '#ffcc00', '#cc9900', 2);
            renderer.drawPixelText(this.speaker, this.x + 75, this.y - 30, '#1a1a2e', 16, 'center');
        }
        
        const textX = this.x + 20;
        const textY = this.y + 20;
        renderer.drawPixelText(this.displayText, textX, textY, '#ffffff', 14);
        
        if (this.showOptions) {
            for (let i = 0; i < this.options.length; i++) {
                const optionY = this.y + this.height + 20 + i * 50;
                const bgColor = i === this.selectedOption ? '#3a3a6e' : '#2a2a4e';
                renderer.drawPixelRect(this.x, optionY, this.width, 45, bgColor, '#4a4a6a', 3);
                renderer.drawPixelText(this.options[i], this.x + this.width / 2, optionY + 12, '#ffffff', 14, 'center');
            }
        }
        
        if (!this.isTyping && !this.showOptions) {
            const blinkY = this.y + this.height - 30;
            renderer.drawPixelText('点击继续...', this.x + this.width - 120, blinkY, '#ffcc00', 12);
        }
    }
}

export default DialogBox;
