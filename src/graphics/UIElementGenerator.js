class UIElementGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }
    
    drawPixelButton(x, y, width, height, text, color = '#2a2a4e', hoverColor = '#3a3a6e', isHovered = false) {
        const bgColor = isHovered ? hoverColor : color;
        this.ctx.fillStyle = bgColor;
        this.ctx.fillRect(x, y, width, height);
        
        this.ctx.strokeStyle = '#4a4a6e';
        this.ctx.lineWidth = 3;
        this.ctx.strokeRect(x + 1, y + 1, width - 2, height - 2);
        
        this.ctx.fillStyle = '#fff';
        this.ctx.font = 'bold 16px "Courier New"';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(text, x + width / 2, y + height / 2);
    }
    
    drawDialogBox(x, y, width, height, speaker = null) {
        this.ctx.fillStyle = '#1a1a2e';
        this.ctx.fillRect(x, y, width, height);
        
        this.ctx.strokeStyle = '#4a4a6e';
        this.ctx.lineWidth = 4;
        this.ctx.strokeRect(x + 2, y + 2, width - 4, height - 4);
        
        if (speaker) {
            this.ctx.fillStyle = '#ffcc00';
            this.ctx.fillRect(x, y - 35, 150, 35);
            
            this.ctx.strokeStyle = '#cc9900';
            this.ctx.lineWidth = 2;
            this.ctx.strokeRect(x + 1, y - 34, 148, 33);
            
            this.ctx.fillStyle = '#1a1a2e';
            this.ctx.font = 'bold 14px "Courier New"';
            this.ctx.textAlign = 'center';
            this.ctx.fillText(speaker, x + 75, y - 15);
        }
    }
    
    drawAffectionBar(x, y, width, height, value, maxValue = 100) {
        this.ctx.fillStyle = '#1a1a1a';
        this.ctx.fillRect(x, y, width, height);
        
        this.ctx.strokeStyle = '#4a4a6a';
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(x, y, width, height);
        
        const fillWidth = (value / maxValue) * (width - 4);
        
        let fillColor = '#aaa';
        if (value >= 80) fillColor = '#ff4757';
        else if (value >= 60) fillColor = '#ff6b8b';
        else if (value >= 40) fillColor = '#ffd93d';
        else if (value >= 20) fillColor = '#fffacd';
        
        this.ctx.fillStyle = fillColor;
        this.ctx.fillRect(x + 2, y + 2, fillWidth, height - 4);
    }
    
    drawHeartIcon(x, y, size = 32, filled = true, color = null) {
        this.ctx.save();
        this.ctx.translate(x, y);
        const scale = size / 32;
        this.ctx.scale(scale, scale);
        
        this.ctx.fillStyle = color || '#ff4757';
        
        if (filled) {
            this.ctx.beginPath();
            this.ctx.moveTo(16, 6);
            this.ctx.bezierCurveTo(16, 0, 6, 0, 6, 12);
            this.ctx.bezierCurveTo(6, 20, 16, 28, 16, 32);
            this.ctx.bezierCurveTo(16, 28, 26, 20, 26, 12);
            this.ctx.bezierCurveTo(26, 0, 16, 0, 16, 6);
            this.ctx.fill();
        }
        
        this.ctx.strokeStyle = '#8b0000';
        this.ctx.lineWidth = 2;
        this.ctx.beginPath();
        this.ctx.moveTo(16, 6);
        this.ctx.bezierCurveTo(16, 0, 6, 0, 6, 12);
        this.ctx.bezierCurveTo(6, 20, 16, 28, 16, 32);
        this.ctx.bezierCurveTo(16, 28, 26, 20, 26, 12);
        this.ctx.bezierCurveTo(26, 0, 16, 0, 16, 6);
        this.ctx.stroke();
        
        this.ctx.restore();
    }
    
    drawStarIcon(x, y, size = 32, filled = true) {
        this.ctx.save();
        this.ctx.translate(x, y);
        const scale = size / 32;
        this.ctx.scale(scale, scale);
        
        if (filled) {
            this.ctx.fillStyle = '#ffd700';
            this.ctx.beginPath();
            
            const spikes = 5;
            let rot = Math.PI / 2 * 3;
            let cx = 16;
            let cy = 16;
            let outerRadius = 16;
            let innerRadius = 7;
            let step = Math.PI / spikes;
            
            this.ctx.moveTo(cx, cy - outerRadius);
            for (let i = 0; i < spikes; i++) {
                this.ctx.lineTo(cx + Math.cos(rot) * outerRadius, cy + Math.sin(rot) * outerRadius);
                rot += step;
                this.ctx.lineTo(cx + Math.cos(rot) * innerRadius, cy + Math.sin(rot) * innerRadius);
                rot += step;
            }
            this.ctx.lineTo(cx, cy - outerRadius);
            this.ctx.closePath();
            this.ctx.fill();
        }
        
        this.ctx.restore();
    }
    
    drawPanel(x, y, width, height, title = null) {
        this.ctx.fillStyle = '#1a1a2e';
        this.ctx.fillRect(x, y, width, height);
        
        this.ctx.strokeStyle = '#4a4a6e';
        this.ctx.lineWidth = 3;
        this.ctx.strokeRect(x + 1, y + 1, width - 2, height - 2);
        
        if (title) {
            this.ctx.fillStyle = '#ffcc00';
            this.ctx.font = 'bold 18px "Courier New"';
            this.ctx.textAlign = 'center';
            this.ctx.fillText(title, x + width / 2, y + 25);
        }
    }
    
    drawChoiceButton(x, y, width, height, text, index, isSelected = false) {
        const bgColor = isSelected ? '#3a3a6e' : '#2a2a4e';
        this.ctx.fillStyle = bgColor;
        this.ctx.fillRect(x, y, width, height);
        
        this.ctx.strokeStyle = '#4a4a6e';
        this.ctx.lineWidth = 3;
        this.ctx.strokeRect(x + 1, y + 1, width - 2, height - 2);
        
        this.ctx.fillStyle = '#fff';
        this.ctx.font = '14px "Courier New"';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(text, x + width / 2, y + height / 2);
    }
}

export default UIElementGenerator;
