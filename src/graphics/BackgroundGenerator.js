export default class BackgroundGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }

    drawHeartBackground(time = 0) {
        const ctx = this.ctx;
        const width = ctx.canvas.width;
        const height = ctx.canvas.height;

        ctx.fillStyle = '#1A1A2E';
        ctx.fillRect(0, 0, width, height);

        const gradient = ctx.createLinearGradient(0, 0, 0, height);
        gradient.addColorStop(0, '#16213E');
        gradient.addColorStop(1, '#0F3460');
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, width, height);

        const hearts = 20;
        for (let i = 0; i < hearts; i++) {
            const x = (i * 73 + time * 0.2) % width;
            const y = (i * 47 + Math.sin(time * 0.01 + i) * 20) % height;
            const size = 10 + (i % 3) * 5;
            const alpha = 0.3 + (Math.sin(time * 0.02 + i * 0.5) + 1) * 0.1;
            
            this.drawHeart(x, y, size, `rgba(233, 69, 96, ${alpha})`);
        }

        const centerX = width / 2;
        const centerY = height / 4;
        const pulseSize = 60 + Math.sin(time * 0.03) * 5;
        this.drawHeart(centerX, centerY, pulseSize, 'rgba(233, 69, 96, 0.8)');
        this.drawHeart(centerX, centerY, pulseSize * 0.7, 'rgba(255, 107, 107, 0.9)');
    }

    drawLivingRoomBackground(time = 0) {
        const ctx = this.ctx;
        const width = ctx.canvas.width;
        const height = ctx.canvas.height;

        ctx.fillStyle = '#F5DEB3';
        ctx.fillRect(0, 0, width, height);

        ctx.fillStyle = '#E8D4A8';
        for (let i = 0; i < width; i += 32) {
            for (let j = 0; j < height; j += 32) {
                if ((i + j) % 64 === 0) {
                    ctx.fillRect(i, j, 32, 32);
                }
            }
        }

        ctx.fillStyle = '#8B4513';
        ctx.fillRect(0, height * 0.65, width, height * 0.35);

        ctx.fillStyle = '#654321';
        for (let i = 0; i < width; i += 64) {
            ctx.fillRect(i, height * 0.65, 2, height * 0.35);
        }

        ctx.fillStyle = '#FF6B6B';
        ctx.fillRect(width * 0.2, height * 0.1, width * 0.6, height * 0.4);
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(width * 0.22, height * 0.12, width * 0.56, height * 0.36);
        
        ctx.fillStyle = '#87CEEB';
        ctx.fillRect(width * 0.25, height * 0.15, width * 0.2, height * 0.3);
        ctx.fillRect(width * 0.55, height * 0.15, width * 0.2, height * 0.3);
        
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(width * 0.44, height * 0.15, width * 0.12, height * 0.3);

        ctx.fillStyle = '#DEB887';
        ctx.fillRect(width * 0.1, height * 0.5, width * 0.35, height * 0.15);
        ctx.fillStyle = '#8B4513';
        ctx.fillRect(width * 0.1, height * 0.62, width * 0.35, height * 0.03);

        ctx.fillStyle = '#D2691E';
        ctx.fillRect(width * 0.55, height * 0.55, width * 0.35, height * 0.1);
        ctx.fillStyle = '#FF0000';
        ctx.fillRect(width * 0.7, height * 0.45, 10, 15);
    }

    drawGardenBackground(time = 0) {
        const ctx = this.ctx;
        const width = ctx.canvas.width;
        const height = ctx.canvas.height;

        const gradient = ctx.createLinearGradient(0, 0, 0, height * 0.4);
        gradient.addColorStop(0, '#87CEEB');
        gradient.addColorStop(1, '#B0E0E6');
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, width, height * 0.5);

        ctx.fillStyle = '#FFD700';
        const sunX = width * 0.85;
        const sunY = height * 0.12;
        this.drawSun(sunX, sunY, 30 + Math.sin(time * 0.02) * 2);

        ctx.fillStyle = '#FFFFFF';
        this.drawCloud(width * 0.15 + Math.sin(time * 0.01) * 10, height * 0.1, 25);
        this.drawCloud(width * 0.45 + Math.sin(time * 0.015) * 15, height * 0.15, 30);
        this.drawCloud(width * 0.7 + Math.sin(time * 0.012) * 12, height * 0.08, 20);

        ctx.fillStyle = '#90EE90';
        ctx.fillRect(0, height * 0.5, width, height * 0.5);

        ctx.fillStyle = '#7CCD7C';
        for (let i = 0; i < 30; i++) {
            const x = (i * 37) % width;
            const y = height * 0.5 + (i * 17) % (height * 0.45);
            this.drawGrass(x, y);
        }

        const flowerColors = ['#FF69B4', '#FF6B6B', '#FFD700', '#9370DB', '#00CED1'];
        for (let i = 0; i < 15; i++) {
            const x = (i * 67) % width;
            const y = height * 0.6 + (i * 23) % (height * 0.35);
            this.drawFlower(x, y, flowerColors[i % flowerColors.length]);
        }

        ctx.fillStyle = '#8B4513';
        ctx.fillRect(width * 0.1, height * 0.55, width * 0.15, height * 0.2);
        ctx.fillStyle = '#228B22';
        ctx.fillRect(width * 0.02, height * 0.35, width * 0.3, height * 0.25);

        ctx.fillStyle = '#808080';
        ctx.fillRect(width * 0.4, height * 0.6, width * 0.2, height * 0.05);
    }

    drawHeart(x, y, size, color) {
        const ctx = this.ctx;
        ctx.fillStyle = color;
        
        ctx.fillRect(x - size * 0.3, y - size * 0.2, size * 0.3, size * 0.3);
        ctx.fillRect(x, y - size * 0.2, size * 0.3, size * 0.3);
        ctx.fillRect(x - size * 0.45, y - size * 0.05, size * 0.9, size * 0.3);
        ctx.fillRect(x - size * 0.3, y + size * 0.1, size * 0.6, size * 0.2);
        ctx.fillRect(x - size * 0.15, y + size * 0.3, size * 0.3, size * 0.15);
    }

    drawSun(x, y, size) {
        const ctx = this.ctx;
        ctx.fillStyle = '#FFD700';
        ctx.fillRect(x - size/2, y - size/2, size, size);
        
        ctx.fillStyle = '#FFA500';
        for (let i = 0; i < 8; i++) {
            const angle = (i / 8) * Math.PI * 2;
            const rayX = x + Math.cos(angle) * (size * 0.8);
            const rayY = y + Math.sin(angle) * (size * 0.8);
            ctx.fillRect(rayX - 3, rayY - 3, 6, 6);
        }
    }

    drawCloud(x, y, size) {
        const ctx = this.ctx;
        ctx.fillRect(x - size, y, size * 0.6, size * 0.4);
        ctx.fillRect(x - size * 0.5, y - size * 0.3, size * 0.8, size * 0.5);
        ctx.fillRect(x, y, size * 0.6, size * 0.4);
        ctx.fillRect(x + size * 0.3, y - size * 0.1, size * 0.5, size * 0.3);
    }

    drawGrass(x, y) {
        const ctx = this.ctx;
        ctx.fillStyle = '#7CCD7C';
        ctx.fillRect(x, y, 2, 8);
        ctx.fillRect(x + 3, y - 2, 2, 10);
        ctx.fillRect(x + 6, y + 1, 2, 7);
    }

    drawFlower(x, y, color) {
        const ctx = this.ctx;
        ctx.fillStyle = '#228B22';
        ctx.fillRect(x, y, 2, 12);
        
        ctx.fillStyle = color;
        ctx.fillRect(x - 4, y - 4, 4, 4);
        ctx.fillRect(x + 2, y - 4, 4, 4);
        ctx.fillRect(x - 2, y - 6, 4, 4);
        ctx.fillRect(x - 2, y - 2, 4, 4);
        
        ctx.fillStyle = '#FFD700';
        ctx.fillRect(x, y - 4, 2, 2);
    }
}
