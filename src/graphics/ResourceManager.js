class ResourceManager {
    constructor() {
        this.images = {};
        this.loaded = 0;
        this.total = 0;
    }
    
    drawPixelCharacter(ctx, guest, x, y, size = 128) {
        const colors = this.getCharacterColors(guest);
        
        ctx.save();
        ctx.imageSmoothingEnabled = false;
        
        this.drawCharacterBase(ctx, x, y, size, colors);
        this.drawCharacterFace(ctx, x, y, size, colors, guest);
        this.drawCharacterHair(ctx, x, y, size, colors);
        this.drawCharacterOutfit(ctx, x, y, size, colors);
        
        ctx.restore();
    }
    
    getCharacterColors(guest) {
        const colorMap = {
            yangyang: { skin: '#f4c9a0', hair: '#4a3728', outfit: '#3a7ca5', accent: '#ffcc00' },
            lengleng: { skin: '#f0e6d3', hair: '#2c1e3a', outfit: '#4a3f6b', accent: '#a078c0' },
            momo: { skin: '#f2d0b0', hair: '#3a3a3a', outfit: '#2a5a7a', accent: '#70a0c0' },
            zhezhe: { skin: '#f5d5b5', hair: '#5a4a3a', outfit: '#5a7a8a', accent: '#90c0e0' },
            nuannuan: { skin: '#f9e0c0', hair: '#8a6a4a', outfit: '#e090a0', accent: '#f0c0d0' },
            yangyang2: { skin: '#f1cba3', hair: '#3a2a1a', outfit: '#7a5a4a', accent: '#c09070' },
            mimi: { skin: '#f7d7b7', hair: '#a07050', outfit: '#f0a080', accent: '#ffd0c0' },
            qingqing: { skin: '#efd9c3', hair: '#1a1a2a', outfit: '#8a3a5a', accent: '#d07090' },
            shuangshuang: { skin: '#eec9b3', hair: '#4a2a2a', outfit: '#3a3a4a', accent: '#808090' },
            nuanyang: { skin: '#f3cdab', hair: '#6a5a4a', outfit: '#5a6a5a', accent: '#a0b0a0' },
            yaya: { skin: '#f6d6bc', hair: '#5a3a2a', outfit: '#7a6a5a', accent: '#b0a090' },
            qingqing2: { skin: '#f0d2b2', hair: '#2a2a3a', outfit: '#4a5a5a', accent: '#809090' }
        };
        
        return colorMap[guest.id] || { skin: '#f4c9a0', hair: '#4a3728', outfit: '#666', accent: '#aaa' };
    }
    
    drawCharacterBase(ctx, x, y, size, colors) {
        const scale = size / 128;
        ctx.save();
        ctx.translate(x, y);
        ctx.scale(scale, scale);
        
        ctx.fillStyle = colors.skin;
        ctx.fillRect(40, 20, 48, 44);
        
        ctx.fillRect(44, 64, 40, 36);
        
        ctx.fillRect(44, 100, 16, 28);
        ctx.fillRect(68, 100, 16, 28);
        
        ctx.restore();
    }
    
    drawCharacterFace(ctx, x, y, size, colors, guest) {
        const scale = size / 128;
        ctx.save();
        ctx.translate(x, y);
        ctx.scale(scale, scale);
        
        ctx.fillStyle = '#1a1a1a';
        ctx.fillRect(50, 36, 8, 8);
        ctx.fillRect(70, 36, 8, 8);
        
        ctx.fillStyle = '#fff';
        ctx.fillRect(52, 38, 3, 3);
        ctx.fillRect(72, 38, 3, 3);
        
        ctx.fillStyle = '#d08090';
        ctx.fillRect(46, 48, 10, 4);
        ctx.fillRect(72, 48, 10, 4);
        
        ctx.strokeStyle = '#a05060';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(56, 56);
        ctx.lineTo(72, 56);
        ctx.stroke();
        
        ctx.restore();
    }
    
    drawCharacterHair(ctx, x, y, size, colors) {
        const scale = size / 128;
        ctx.save();
        ctx.translate(x, y);
        ctx.scale(scale, scale);
        
        ctx.fillStyle = colors.hair;
        
        ctx.fillRect(36, 12, 56, 20);
        ctx.fillRect(32, 20, 16, 28);
        ctx.fillRect(80, 20, 16, 28);
        ctx.fillRect(40, 8, 48, 16);
        
        ctx.restore();
    }
    
    drawCharacterOutfit(ctx, x, y, size, colors) {
        const scale = size / 128;
        ctx.save();
        ctx.translate(x, y);
        ctx.scale(scale, scale);
        
        ctx.fillStyle = colors.outfit;
        ctx.fillRect(40, 62, 48, 40);
        ctx.fillRect(28, 66, 16, 32);
        ctx.fillRect(84, 66, 16, 32);
        
        ctx.fillStyle = colors.accent;
        ctx.fillRect(58, 68, 12, 20);
        
        ctx.restore();
    }
    
    drawHeart(ctx, x, y, size, filled = true, color = null) {
        ctx.save();
        ctx.translate(x, y);
        const scale = size / 32;
        ctx.scale(scale, scale);
        
        ctx.fillStyle = color || '#ff4757';
        
        if (filled) {
            ctx.beginPath();
            ctx.moveTo(16, 6);
            ctx.bezierCurveTo(16, 0, 6, 0, 6, 12);
            ctx.bezierCurveTo(6, 20, 16, 28, 16, 32);
            ctx.bezierCurveTo(16, 28, 26, 20, 26, 12);
            ctx.bezierCurveTo(26, 0, 16, 0, 16, 6);
            ctx.fill();
        }
        
        ctx.strokeStyle = '#8b0000';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(16, 6);
        ctx.bezierCurveTo(16, 0, 6, 0, 6, 12);
        ctx.bezierCurveTo(6, 20, 16, 28, 16, 32);
        ctx.bezierCurveTo(16, 28, 26, 20, 26, 12);
        ctx.bezierCurveTo(26, 0, 16, 0, 16, 6);
        ctx.stroke();
        
        ctx.restore();
    }
    
    drawStar(ctx, x, y, size, filled = true) {
        ctx.save();
        ctx.translate(x, y);
        const scale = size / 32;
        ctx.scale(scale, scale);
        
        const spikes = 5;
        let rot = Math.PI / 2 * 3;
        let cx = 16;
        let cy = 16;
        let outerRadius = 16;
        let innerRadius = 7;
        let step = Math.PI / spikes;
        
        if (filled) {
            ctx.fillStyle = '#ffd700';
            ctx.beginPath();
            ctx.moveTo(cx, cy - outerRadius);
            for (let i = 0; i < spikes; i++) {
                ctx.lineTo(cx + Math.cos(rot) * outerRadius, cy + Math.sin(rot) * outerRadius);
                rot += step;
                ctx.lineTo(cx + Math.cos(rot) * innerRadius, cy + Math.sin(rot) * innerRadius);
                rot += step;
            }
            ctx.lineTo(cx, cy - outerRadius);
            ctx.closePath();
            ctx.fill();
        }
        
        ctx.restore();
    }
}

export default ResourceManager;
