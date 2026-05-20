class BackgroundGenerator {
    constructor(ctx, width, height) {
        this.ctx = ctx;
        this.width = width;
        this.height = height;
    }
    
    drawPatternBackground(pattern = 'checkerboard', colors = ['#0f0f23', '#121228']) {
        const tileSize = 32;
        
        for (let x = 0; x < this.width; x += tileSize) {
            for (let y = 0; y < this.height; y += tileSize) {
                let colorIndex = 0;
                
                if (pattern === 'checkerboard') {
                    colorIndex = ((x / tileSize) + (y / tileSize)) % 2;
                } else if (pattern === 'vertical') {
                    colorIndex = Math.floor(x / tileSize) % 2;
                } else if (pattern === 'horizontal') {
                    colorIndex = Math.floor(y / tileSize) % 2;
                }
                
                this.ctx.fillStyle = colors[colorIndex];
                this.ctx.fillRect(x, y, tileSize, tileSize);
            }
        }
    }
    
    drawHeartBackground(time = 0) {
        this.ctx.fillStyle = '#1a1a2e';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        const heartCount = 12;
        for (let i = 0; i < heartCount; i++) {
            const x = (this.width / heartCount) * i + 50;
            const y = this.height / 2 + Math.sin(time * 0.001 + i * 0.5) * 100;
            const size = 20 + Math.sin(time * 0.002 + i) * 10;
            const alpha = 0.3 + Math.sin(time * 0.001 + i) * 0.2;
            
            this.ctx.globalAlpha = alpha;
            this.drawHeart(x, y, size);
        }
        
        this.ctx.globalAlpha = 1;
    }
    
    drawHeart(x, y, size) {
        this.ctx.save();
        this.ctx.translate(x, y);
        const scale = size / 32;
        this.ctx.scale(scale, scale);
        
        this.ctx.fillStyle = '#ff4757';
        
        this.ctx.beginPath();
        this.ctx.moveTo(16, 6);
        this.ctx.bezierCurveTo(16, 0, 6, 0, 6, 12);
        this.ctx.bezierCurveTo(6, 20, 16, 28, 16, 32);
        this.ctx.bezierCurveTo(16, 28, 26, 20, 26, 12);
        this.ctx.bezierCurveTo(26, 0, 16, 0, 16, 6);
        this.ctx.fill();
        
        this.ctx.restore();
    }
    
    drawLivingRoomBackground(time = 0) {
        const gradient = this.ctx.createLinearGradient(0, 0, 0, this.height);
        gradient.addColorStop(0, '#4a3f6b');
        gradient.addColorStop(0.5, '#3a2f5b');
        gradient.addColorStop(1, '#2a1f4b');
        this.ctx.fillStyle = gradient;
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        this.ctx.fillStyle = '#8a7fa8';
        this.ctx.fillRect(0, this.height * 0.6, this.width, this.height * 0.4);
        
        this.ctx.fillStyle = '#a078c0';
        this.ctx.fillRect(0, this.height * 0.6, this.width, 4);
        
        this.ctx.fillStyle = '#c0a0d0';
        this.ctx.fillRect(200, 80, 200, 250);
        this.ctx.fillStyle = '#806090';
        this.ctx.fillRect(200, 80, 200, 6);
        this.ctx.fillRect(297, 80, 6, 250);
        this.ctx.fillRect(200, 205, 200, 6);
        
        this.ctx.fillStyle = '#7a5a4a';
        this.ctx.fillRect(150, this.height * 0.55, 300, 80);
        this.ctx.fillStyle = '#8a6a5a';
        this.ctx.fillRect(150, this.height * 0.55, 300, 10);
        
        this.ctx.fillStyle = '#ff6b8b';
        this.ctx.fillRect(200, this.height * 0.52, 30, 25);
        this.ctx.fillStyle = '#ffd93d';
        this.ctx.fillRect(300, this.height * 0.53, 40, 20);
        
        this.ctx.fillStyle = '#5a7a8a';
        this.ctx.fillRect(600, this.height * 0.45, 160, 200);
        this.ctx.fillStyle = '#7a9aaa';
        this.ctx.fillRect(600, this.height * 0.45, 160, 20);
        
        this.ctx.fillStyle = '#2a1a0a';
        this.ctx.fillRect(700, 50, 100, 100);
        this.ctx.fillStyle = '#4a3a2a';
        this.ctx.fillRect(695, 45, 110, 8);
        this.ctx.fillRect(695, 45, 8, 110);
        this.ctx.fillRect(797, 45, 8, 110);
        this.ctx.fillRect(695, 147, 110, 8);
    }
    
    drawNightBackground(time = 0) {
        const gradient = this.ctx.createLinearGradient(0, 0, 0, this.height);
        gradient.addColorStop(0, '#0a0a1f');
        gradient.addColorStop(0.5, '#1a1a2e');
        gradient.addColorStop(1, '#2a2a3e');
        this.ctx.fillStyle = gradient;
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        this.ctx.fillStyle = '#fff';
        for (let i = 0; i < 80; i++) {
            const x = (i * 137) % this.width;
            const y = (i * 89) % (this.height * 0.6);
            const size = 1 + Math.sin(time * 0.001 + i) * 0.5;
            this.ctx.fillRect(x, y, size, size);
        }
        
        this.ctx.fillStyle = '#fff';
        this.ctx.beginPath();
        this.ctx.arc(750, 100, 50, 0, Math.PI * 2);
        this.ctx.fill();
        
        this.ctx.fillStyle = '#0a0a1f';
        this.ctx.beginPath();
        this.ctx.arc(770, 90, 45, 0, Math.PI * 2);
        this.ctx.fill();
    }
    
    drawGameLogo(x, y, scale = 1, time = 0) {
        const bounce = Math.sin(time * 0.003) * 5;
        
        this.ctx.save();
        this.ctx.translate(x, y + bounce);
        this.ctx.scale(scale, scale);
        
        this.ctx.fillStyle = '#ffcc00';
        
        this.ctx.fillRect(0, 0, 120, 30);
        this.ctx.fillRect(10, 10, 100, 30);
        
        this.ctx.fillStyle = '#ff4757';
        this.ctx.fillRect(10, -5, 10, 15);
        this.ctx.fillRect(100, -5, 10, 15);
        
        this.ctx.restore();
    }
}

export default BackgroundGenerator;
