class CharacterSpriteGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }
    
    generateCharacter(guest, size = 128) {
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');
        
        const colors = this.getCharacterColors(guest);
        const scale = size / 128;
        
        ctx.save();
        ctx.scale(scale, scale);
        ctx.imageSmoothingEnabled = false;
        
        this.drawBase(ctx, colors);
        this.drawFace(ctx, colors, guest);
        this.drawHair(ctx, colors, guest);
        this.drawOutfit(ctx, colors, guest);
        this.drawAccessories(ctx, colors, guest);
        
        ctx.restore();
        
        return canvas;
    }
    
    getCharacterColors(guest) {
        const colorMap = {
            yangyang: {
                skin: '#f4c9a0',
                hair: '#4a3728',
                outfit: '#3a7ca5',
                accent: '#ffcc00',
                eyes: '#2a1a0a'
            },
            lengleng: {
                skin: '#f0e6d3',
                hair: '#2c1e3a',
                outfit: '#4a3f6b',
                accent: '#a078c0',
                eyes: '#1a0a2a'
            },
            momo: {
                skin: '#f2d0b0',
                hair: '#3a3a3a',
                outfit: '#2a5a7a',
                accent: '#70a0c0',
                eyes: '#1a2a3a'
            },
            zhezhe: {
                skin: '#f5d5b5',
                hair: '#5a4a3a',
                outfit: '#5a7a8a',
                accent: '#90c0e0',
                eyes: '#3a2a1a'
            },
            nuannuan: {
                skin: '#f9e0c0',
                hair: '#8a6a4a',
                outfit: '#e090a0',
                accent: '#f0c0d0',
                eyes: '#5a4a2a'
            },
            yangyang2: {
                skin: '#f1cba3',
                hair: '#3a2a1a',
                outfit: '#7a5a4a',
                accent: '#c09070',
                eyes: '#2a1a0a'
            },
            mimi: {
                skin: '#f7d7b7',
                hair: '#a07050',
                outfit: '#f0a080',
                accent: '#ffd0c0',
                eyes: '#6a4a3a'
            },
            qingqing: {
                skin: '#efd9c3',
                hair: '#1a1a2a',
                outfit: '#8a3a5a',
                accent: '#d07090',
                eyes: '#2a1a3a'
            },
            shuangshuang: {
                skin: '#eec9b3',
                hair: '#4a2a2a',
                outfit: '#3a3a4a',
                accent: '#808090',
                eyes: '#3a1a1a'
            },
            nuanyang: {
                skin: '#f3cdab',
                hair: '#6a5a4a',
                outfit: '#5a6a5a',
                accent: '#a0b0a0',
                eyes: '#4a3a2a'
            },
            yaya: {
                skin: '#f6d6bc',
                hair: '#5a3a2a',
                outfit: '#7a6a5a',
                accent: '#b0a090',
                eyes: '#3a2a1a'
            },
            qingqing2: {
                skin: '#f0d2b2',
                hair: '#2a2a3a',
                outfit: '#4a5a5a',
                accent: '#809090',
                eyes: '#1a2a2a'
            }
        };
        
        return colorMap[guest.id] || {
            skin: '#f4c9a0',
            hair: '#4a3728',
            outfit: '#666',
            accent: '#aaa',
            eyes: '#1a1a1a'
        };
    }
    
    drawBase(ctx, colors) {
        ctx.fillStyle = colors.skin;
        
        ctx.fillRect(44, 24, 40, 44);
        ctx.fillRect(40, 68, 48, 36);
        
        ctx.fillRect(44, 104, 16, 24);
        ctx.fillRect(68, 104, 16, 24);
    }
    
    drawFace(ctx, colors, guest) {
        ctx.fillStyle = colors.eyes;
        
        if (guest.gender === 'female') {
            ctx.fillRect(50, 38, 6, 6);
            ctx.fillRect(72, 38, 6, 6);
        } else {
            ctx.fillRect(52, 40, 8, 6);
            ctx.fillRect(68, 40, 8, 6);
        }
        
        ctx.fillStyle = '#fff';
        ctx.fillRect(54, 40, 3, 3);
        ctx.fillRect(74, 40, 3, 3);
        
        ctx.fillStyle = colors.eyes;
        ctx.fillRect(50, 48, 10, 2);
        ctx.fillRect(68, 48, 10, 2);
        
        ctx.fillStyle = '#e08090';
        ctx.fillRect(44, 54, 8, 4);
        ctx.fillRect(76, 54, 8, 4);
        
        ctx.strokeStyle = colors.eyes;
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(54, 62);
        ctx.lineTo(74, 62);
        ctx.stroke();
    }
    
    drawHair(ctx, colors, guest) {
        ctx.fillStyle = colors.hair;
        
        ctx.fillRect(36, 16, 56, 24);
        ctx.fillRect(32, 24, 16, 32);
        ctx.fillRect(80, 24, 16, 32);
        ctx.fillRect(40, 12, 48, 20);
        
        if (guest.gender === 'female') {
            ctx.fillRect(28, 32, 12, 40);
            ctx.fillRect(88, 32, 12, 40);
        }
    }
    
    drawOutfit(ctx, colors, guest) {
        ctx.fillStyle = colors.outfit;
        
        ctx.fillRect(40, 66, 48, 44);
        ctx.fillRect(24, 70, 20, 40);
        ctx.fillRect(84, 70, 20, 40);
        
        ctx.fillStyle = colors.accent;
        ctx.fillRect(60, 68, 8, 36);
        
        ctx.fillStyle = colors.outfit;
        ctx.fillRect(44, 108, 16, 4);
        ctx.fillRect(68, 108, 16, 4);
    }
    
    drawAccessories(ctx, colors, guest) {
        const accessoryMap = {
            yangyang: () => {
                ctx.fillStyle = '#ffd700';
                ctx.fillRect(52, 20, 24, 4);
            },
            lengleng: () => {
                ctx.fillStyle = '#1a1a2a';
                ctx.fillRect(44, 36, 16, 10);
                ctx.fillRect(68, 36, 16, 10);
                ctx.fillRect(60, 40, 8, 4);
            },
            momo: () => {
                ctx.fillStyle = '#fff';
                ctx.fillRect(42, 14, 44, 12);
            },
            zhezhe: () => {
                ctx.fillStyle = '#88aacc';
                ctx.fillRect(48, 16, 32, 8);
            },
            nuannuan: () => {
                ctx.fillStyle = '#ffaaaa';
                ctx.fillRect(38, 20, 12, 12);
                ctx.fillRect(78, 20, 12, 12);
            },
            yangyang2: () => {
                ctx.fillStyle = '#ccaa88';
                ctx.fillRect(44, 16, 40, 8);
            },
            mimi: () => {
                ctx.fillStyle = '#ff66aa';
                ctx.fillRect(54, 14, 20, 12);
            },
            qingqing: () => {
                ctx.fillStyle = '#ff4488';
                ctx.fillRect(48, 12, 32, 6);
            },
            shuangshuang: () => {
                ctx.fillStyle = '#444466';
                ctx.fillRect(46, 18, 36, 8);
            },
            nuanyang: () => {
                ctx.fillStyle = '#334433';
                ctx.fillRect(42, 10, 44, 10);
            },
            yaya: () => {
                ctx.fillStyle = '#9988aa';
                ctx.fillRect(50, 12, 28, 10);
            },
            qingqing2: () => {
                ctx.fillStyle = '#667788';
                ctx.fillRect(44, 14, 40, 8);
            }
        };
        
        if (accessoryMap[guest.id]) {
            accessoryMap[guest.id]();
        }
    }
    
    generateAllCharacters(guests, size = 128) {
        const sprites = {};
        guests.forEach(guest => {
            sprites[guest.id] = this.generateCharacter(guest, size);
        });
        return sprites;
    }
}

export default CharacterSpriteGenerator;
