class Renderer {
    constructor(ctx, width, height) {
        this.ctx = ctx;
        this.width = width;
        this.height = height;
        
        this.colors = {
            background: '#0f0f23',
            panel: '#1a1a2e',
            panelBorder: '#4a4a6a',
            text: '#ffffff',
            textSecondary: '#aaaacc',
            accent: '#ffcc00',
            accentSecondary: '#ff6b6b',
            success: '#4ecdc4',
            heart: '#ff4757',
            star: '#ffd700'
        };
    }
    
    clear() {
        this.ctx.fillStyle = this.colors.background;
        this.ctx.fillRect(0, 0, this.width, this.height);
    }
    
    renderBackground() {
        this.ctx.fillStyle = '#121228';
        for (let i = 0; i < this.width; i += 32) {
            for (let j = 0; j < this.height; j += 32) {
                if ((i + j) % 64 === 0) {
                    this.ctx.fillRect(i, j, 32, 32);
                }
            }
        }
    }
    
    drawRect(x, y, w, h, color) {
        this.ctx.fillStyle = color;
        this.ctx.fillRect(x, y, w, h);
    }
    
    drawPixelRect(x, y, w, h, fillColor, borderColor = null, borderWidth = 2) {
        if (fillColor) {
            this.ctx.fillStyle = fillColor;
            this.ctx.fillRect(x, y, w, h);
        }
        if (borderColor) {
            this.ctx.strokeStyle = borderColor;
            this.ctx.lineWidth = borderWidth;
            this.ctx.strokeRect(x + borderWidth/2, y + borderWidth/2, w - borderWidth, h - borderWidth);
        }
    }
    
    drawText(text, x, y, color = '#fff', size = 16, align = 'left') {
        this.ctx.fillStyle = color;
        this.ctx.font = `${size}px "Courier New", monospace`;
        this.ctx.textAlign = align;
        this.ctx.fillText(text, x, y);
    }
    
    drawPixelText(text, x, y, color = '#fff', size = 16, align = 'left') {
        this.ctx.fillStyle = color;
        this.ctx.font = `bold ${size}px "Courier New", monospace`;
        this.ctx.textAlign = align;
        this.ctx.textBaseline = 'top';
        this.ctx.imageSmoothingEnabled = false;
        this.ctx.fillText(text, x, y);
    }
    
    drawHeart(x, y, size, filled = true) {
        this.ctx.save();
        this.ctx.translate(x, y);
        this.ctx.scale(size / 32, size / 32);
        
        if (filled) {
            this.ctx.fillStyle = this.colors.heart;
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
    
    drawStar(x, y, size, filled = true) {
        this.ctx.save();
        this.ctx.translate(x, y);
        this.ctx.scale(size / 32, size / 32);
        
        const spikes = 5;
        let rot = Math.PI / 2 * 3;
        let cx = 16;
        let cy = 16;
        let outerRadius = 16;
        let innerRadius = 7;
        let step = Math.PI / spikes;
        
        if (filled) {
            this.ctx.fillStyle = this.colors.star;
            this.ctx.beginPath();
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
}

export default Renderer;
