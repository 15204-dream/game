export default class UIElementGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }

    drawButton(x, y, width, height, text, color = '#E94560', hover = false) {
        const ctx = this.ctx;
        ctx.imageSmoothingEnabled = false;

        const fillColor = hover ? this.adjustColor(color, 20) : color;
        
        ctx.fillStyle = fillColor;
        ctx.fillRect(x, y, width, height);

        ctx.fillStyle = this.adjustColor(color, -30);
        ctx.fillRect(x, y + height - 4, width, 4);
        ctx.fillRect(x + width - 4, y, 4, height);

        ctx.fillStyle = this.adjustColor(color, 30);
        ctx.fillRect(x, y, width, 4);
        ctx.fillRect(x, y, 4, height);

        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(x + 4, y + 4, width - 8, height - 8);

        return { x, y, width, height };
    }

    drawDialogBox(x, y, width, height) {
        const ctx = this.ctx;
        ctx.imageSmoothingEnabled = false;

        ctx.fillStyle = '#16213E';
        ctx.fillRect(x, y, width, height);

        ctx.fillStyle = '#E94560';
        ctx.fillRect(x, y, width, 4);
        ctx.fillRect(x, y + height - 4, width, 4);
        ctx.fillRect(x, y, 4, height);
        ctx.fillRect(x + width - 4, y, 4, height);

        ctx.fillStyle = '#0F3460';
        ctx.fillRect(x + 4, y + 4, width - 8, height - 8);

        return { x, y, width, height };
    }

    drawAffectionBar(x, y, width, height, value, maxValue = 100) {
        const ctx = this.ctx;
        const percentage = value / maxValue;

        ctx.fillStyle = '#2C2C2C';
        ctx.fillRect(x, y, width, height);

        ctx.fillStyle = '#1A1A1A';
        ctx.fillRect(x + 2, y + 2, width - 4, height - 4);

        let fillColor;
        if (percentage < 0.3) fillColor = '#E94560';
        else if (percentage < 0.6) fillColor = '#FFD93D';
        else if (percentage < 0.85) fillColor = '#6BCB77';
        else fillColor = '#FF69B4';

        ctx.fillStyle = fillColor;
        ctx.fillRect(x + 2, y + 2, (width - 4) * percentage, height - 4);

        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(x + 2, y + 2, (width - 4) * percentage, 2);

        return { x, y, width, height };
    }

    drawHeartIcon(x, y, size, filled = true) {
        const ctx = this.ctx;
        const color = filled ? '#E94560' : '#444444';
        
        ctx.fillStyle = color;
        ctx.fillRect(x - size * 0.3, y - size * 0.2, size * 0.3, size * 0.3);
        ctx.fillRect(x, y - size * 0.2, size * 0.3, size * 0.3);
        ctx.fillRect(x - size * 0.45, y - size * 0.05, size * 0.9, size * 0.3);
        ctx.fillRect(x - size * 0.3, y + size * 0.1, size * 0.6, size * 0.2);
        ctx.fillRect(x - size * 0.15, y + size * 0.3, size * 0.3, size * 0.15);

        if (filled) {
            ctx.fillStyle = '#FFB6C1';
            ctx.fillRect(x - size * 0.2, y - size * 0.1, size * 0.1, size * 0.1);
        }

        return { x, y, size };
    }

    drawStarIcon(x, y, size, filled = true) {
        const ctx = this.ctx;
        const color = filled ? '#FFD700' : '#444444';
        
        ctx.fillStyle = color;
        ctx.fillRect(x, y - size * 0.4, size * 0.2, size * 0.3);
        ctx.fillRect(x - size * 0.4, y, size * 0.3, size * 0.2);
        ctx.fillRect(x + size * 0.1, y, size * 0.3, size * 0.2);
        ctx.fillRect(x - size * 0.3, y + size * 0.1, size * 0.6, size * 0.2);
        ctx.fillRect(x - size * 0.2, y + size * 0.3, size * 0.4, size * 0.2);
        ctx.fillRect(x - size * 0.1, y + size * 0.5, size * 0.2, size * 0.2);

        return { x, y, size };
    }

    drawPortraitFrame(x, y, size) {
        const ctx = this.ctx;

        ctx.fillStyle = '#8B4513';
        ctx.fillRect(x - 8, y - 8, size + 16, size + 16);

        ctx.fillStyle = '#DAA520';
        ctx.fillRect(x - 4, y - 4, size + 8, size + 8);

        ctx.fillStyle = '#1A1A2E';
        ctx.fillRect(x, y, size, size);

        return { x, y, size };
    }

    drawDecorationBorder(x, y, width, height, color = '#E94560') {
        const ctx = this.ctx;

        ctx.fillStyle = color;
        for (let i = 0; i < width; i += 16) {
            ctx.fillRect(x + i, y, 8, 8);
            ctx.fillRect(x + i + 4, y + height - 8, 8, 8);
        }
        for (let i = 0; i < height; i += 16) {
            ctx.fillRect(x, y + i, 8, 8);
            ctx.fillRect(x + width - 8, y + i + 4, 8, 8);
        }

        this.drawCornerDecoration(x, y, color);
        this.drawCornerDecoration(x + width - 16, y, color);
        this.drawCornerDecoration(x, y + height - 16, color);
        this.drawCornerDecoration(x + width - 16, y + height - 16, color);

        return { x, y, width, height };
    }

    drawCornerDecoration(x, y, color) {
        const ctx = this.ctx;
        ctx.fillStyle = color;
        ctx.fillRect(x, y, 16, 16);
        ctx.fillStyle = this.adjustColor(color, 30);
        ctx.fillRect(x + 4, y + 4, 8, 8);
    }

    drawTextBox(x, y, width, height) {
        const ctx = this.ctx;

        ctx.fillStyle = '#FFFDD0';
        ctx.fillRect(x, y, width, height);

        ctx.fillStyle = '#8B4513';
        ctx.fillRect(x, y, width, 2);
        ctx.fillRect(x, y + height - 2, width, 2);
        ctx.fillRect(x, y, 2, height);
        ctx.fillRect(x + width - 2, y, 2, height);

        ctx.fillStyle = '#FFE4B5';
        for (let i = y + 20; i < y + height - 10; i += 20) {
            ctx.fillRect(x + 10, i, width - 20, 1);
        }

        return { x, y, width, height };
    }

    adjustColor(color, amount) {
        const hex = color.replace('#', '');
        const r = Math.max(0, Math.min(255, parseInt(hex.substr(0, 2), 16) + amount));
        const g = Math.max(0, Math.min(255, parseInt(hex.substr(2, 2), 16) + amount));
        const b = Math.max(0, Math.min(255, parseInt(hex.substr(4, 2), 16) + amount));
        return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
    }
}
