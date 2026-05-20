export default class CoverGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }

    generateCover(width, height) {
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        ctx.imageSmoothingEnabled = false;

        this.drawCoverBackground(ctx, width, height);
        this.drawCoverTitle(ctx, width, height);
        this.drawCoverHearts(ctx, width, height);
        this.drawCoverCharacters(ctx, width, height);

        return canvas;
    }

    drawCoverBackground(ctx, width, height) {
        // 渐变背景
        const gradient = ctx.createLinearGradient(0, 0, 0, height);
        gradient.addColorStop(0, '#16213E');
        gradient.addColorStop(0.5, '#0F3460');
        gradient.addColorStop(1, '#1A1A2E');
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, width, height);

        // 像素风格星星
        for (let i = 0; i < 50; i++) {
            const x = Math.random() * width;
            const y = Math.random() * height * 0.6;
            const size = 2 + Math.random() * 3;
            ctx.fillStyle = `rgba(255, 255, 255, ${0.3 + Math.random() * 0.5})`;
            ctx.fillRect(x, y, size, size);
        }

        // 像素化的心形装饰
        for (let i = 0; i < 15; i++) {
            const x = Math.random() * width;
            const y = Math.random() * height * 0.7;
            const size = 10 + Math.random() * 20;
            ctx.fillStyle = `rgba(233, 69, 96, ${0.2 + Math.random() * 0.3})`;
            this.drawPixelHeart(ctx, x, y, size);
        }
    }

    drawPixelHeart(ctx, x, y, size) {
        const s = size / 10;
        ctx.fillRect(x - 2*s, y - s, 2*s, 2*s);
        ctx.fillRect(x + 0, y - s, 2*s, 2*s);
        ctx.fillRect(x - 3*s, y + 0, 6*s, 2*s);
        ctx.fillRect(x - 2*s, y + 2*s, 4*s, 2*s);
        ctx.fillRect(x - 1*s, y + 4*s, 2*s, s);
    }

    drawCoverTitle(ctx, width, height) {
        // 主标题背景框
        ctx.fillStyle = 'rgba(233, 69, 96, 0.8)';
        ctx.fillRect(width * 0.1, height * 0.15, width * 0.8, height * 0.12);
        
        // 边框
        ctx.fillStyle = '#FFB6C1';
        ctx.fillRect(width * 0.1, height * 0.15, width * 0.8, 4);
        ctx.fillRect(width * 0.1, height * 0.15 + height * 0.12 - 4, width * 0.8, 4);
        ctx.fillRect(width * 0.1, height * 0.15, 4, height * 0.12);
        ctx.fillRect(width * 0.9 - 4, height * 0.15, 4, height * 0.12);

        // 副标题
        ctx.fillStyle = '#FFD93D';
        const subTitle = "Oops! I'm in Love!";
        const subTitleSize = 24;
        ctx.font = `${subTitleSize}px 'Courier New', monospace`;
        ctx.textAlign = 'center';
        ctx.fillText(subTitle, width / 2, height * 0.14);
        
        // 中文标题
        ctx.fillStyle = '#FFFFFF';
        const title = "糟糕！是心动鸭！";
        const titleSize = 48;
        ctx.font = `bold ${titleSize}px 'Courier New', monospace`;
        ctx.textAlign = 'center';
        ctx.fillText(title, width / 2, height * 0.23);
    }

    drawCoverHearts(ctx, width, height) {
        // 装饰性大爱心
        this.drawBigHeart(ctx, width * 0.15, height * 0.4, 40, '#E94560');
        this.drawBigHeart(ctx, width * 0.85, height * 0.4, 40, '#FF69B4');
        this.drawBigHeart(ctx, width * 0.2, height * 0.55, 30, '#FFB6C1');
        this.drawBigHeart(ctx, width * 0.8, height * 0.55, 30, '#FF6B6B');
    }

    drawBigHeart(ctx, x, y, size, color) {
        ctx.fillStyle = color;
        const s = size / 10;
        ctx.fillRect(x - 2*s, y - s, 2*s, 2*s);
        ctx.fillRect(x + 0, y - s, 2*s, 2*s);
        ctx.fillRect(x - 3*s, y + 0, 6*s, 2*s);
        ctx.fillRect(x - 2*s, y + 2*s, 4*s, 2*s);
        ctx.fillRect(x - 1*s, y + 4*s, 2*s, s);
        
        // 高光
        ctx.fillStyle = 'rgba(255, 255, 255, 0.5)';
        ctx.fillRect(x - 2*s, y - s, s, s);
    }

    drawCoverCharacters(ctx, width, height) {
        // 中央展示区
        const centerY = height * 0.5;
        
        // 像素风格装饰边框
        ctx.fillStyle = 'rgba(255, 215, 0, 0.3)';
        ctx.fillRect(width * 0.1, centerY - 10, width * 0.8, 4);
        ctx.fillRect(width * 0.1, centerY + 160, width * 0.8, 4);
        
        // 像素风格的嘉宾剪影（简单化）
        const colors = ['#FF6B6B', '#4ECDC4', '#9B59B6', '#3498DB', '#F39C12', '#E74C3C', 
                        '#FF69B4', '#9B59B6', '#2C3E50', '#27AE60', '#1ABC9C', '#E8DAEF'];
        
        for (let i = 0; i < 6; i++) {
            const x = width * 0.15 + i * (width * 0.7 / 5);
            this.drawPixelCharacter(ctx, x, centerY + 60, colors[i], i < 3 ? 'male' : 'female');
        }
        
        for (let i = 0; i < 6; i++) {
            const x = width * 0.15 + i * (width * 0.7 / 5);
            this.drawPixelCharacter(ctx, x, centerY + 130, colors[i + 6], i >= 3 ? 'female' : 'male');
        }
    }

    drawPixelCharacter(ctx, x, y, color, gender) {
        const unit = 4;
        
        // 身体
        ctx.fillStyle = color;
        ctx.fillRect(x - 3*unit, y, 6*unit, 8*unit);
        
        // 头
        ctx.fillStyle = gender === 'male' ? '#FFD5B5' : '#FFE0C5';
        ctx.fillRect(x - 2*unit, y - 5*unit, 4*unit, 5*unit);
        
        // 头发
        ctx.fillStyle = this.adjustColor(color, -40);
        ctx.fillRect(x - 2*unit, y - 6*unit, 4*unit, 2*unit);
        ctx.fillRect(x - 3*unit, y - 5*unit, unit, 2*unit);
        ctx.fillRect(x + 2*unit, y - 5*unit, unit, 2*unit);
        
        // 眼睛
        ctx.fillStyle = '#2C1810';
        ctx.fillRect(x - unit, y - 4*unit, unit, unit);
        ctx.fillRect(x + 0, y - 4*unit, unit, unit);
        
        // 腿
        ctx.fillStyle = this.adjustColor(color, -20);
        ctx.fillRect(x - 2*unit, y + 8*unit, 2*unit, 3*unit);
        ctx.fillRect(x + 0, y + 8*unit, 2*unit, 3*unit);
    }

    adjustColor(color, amount) {
        const hex = color.replace('#', '');
        const r = Math.max(0, Math.min(255, parseInt(hex.substr(0, 2), 16) + amount));
        const g = Math.max(0, Math.min(255, parseInt(hex.substr(2, 2), 16) + amount));
        const b = Math.max(0, Math.min(255, parseInt(hex.substr(4, 2), 16) + amount));
        return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
    }
}
