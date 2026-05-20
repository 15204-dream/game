export default class CharacterSpriteGenerator {
    constructor(ctx) {
        this.ctx = ctx;
    }

    generateAllCharacters(guests, size = 128) {
        const sprites = {};
        guests.forEach(guest => {
            sprites[guest.id] = this.generateCharacter(guest, size);
        });
        
        sprites['player_male_1'] = this.generatePlayerCharacter('male', 1, size);
        sprites['player_male_2'] = this.generatePlayerCharacter('male', 2, size);
        sprites['player_female_1'] = this.generatePlayerCharacter('female', 1, size);
        sprites['player_female_2'] = this.generatePlayerCharacter('female', 2, size);
        sprites['director_male_1'] = this.generateDirectorCharacter('male', 1, size);
        sprites['director_male_2'] = this.generateDirectorCharacter('male', 2, size);
        sprites['director_female_1'] = this.generateDirectorCharacter('female', 1, size);
        sprites['director_female_2'] = this.generateDirectorCharacter('female', 2, size);

        return sprites;
    }

    generateCharacter(guest, size = 128) {
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');
        ctx.imageSmoothingEnabled = false;

        const unit = size / 16;
        const skinTone = guest.gender === 'male' ? '#FFD5B5' : '#FFE0C5';
        const hairColor = this.getHairColor(guest.id);
        const mainColor = guest.color;
        const accentColor = guest.accentColor;

        this.drawPixelBackground(ctx, size, mainColor);
        this.drawBody(ctx, unit, guest.gender, mainColor, accentColor);
        this.drawHead(ctx, unit, skinTone, hairColor, guest.gender);
        this.drawFace(ctx, unit, skinTone);
        this.drawAccessory(ctx, unit, guest.id);

        return canvas;
    }

    generatePlayerCharacter(gender, variant, size = 128) {
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');
        ctx.imageSmoothingEnabled = false;

        const unit = size / 16;
        const skinTone = gender === 'male' ? '#FFD5B5' : '#FFE0C5';
        const hairColors = ['#8B4513', '#2C1810', '#FFD700', '#4A3728'];
        const hairColor = hairColors[(variant - 1) * 2] || hairColors[0];
        const mainColors = ['#E94560', '#0F4C75', '#3282B8', '#BBE1FA'];
        const mainColor = mainColors[variant - 1] || mainColors[0];

        this.drawPixelBackground(ctx, size, '#16213E');
        this.drawBody(ctx, unit, gender, mainColor, '#BBE1FA');
        this.drawHead(ctx, unit, skinTone, hairColor, gender);
        this.drawFace(ctx, unit, skinTone);

        return canvas;
    }

    generateDirectorCharacter(gender, variant, size = 128) {
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');
        ctx.imageSmoothingEnabled = false;

        const unit = size / 16;
        const skinTone = gender === 'male' ? '#FFD5B5' : '#FFE0C5';
        const hairColor = '#1A1A2E';
        const mainColor = '#1A1A2E';
        const accentColor = '#E94560';

        this.drawPixelBackground(ctx, size, '#0F3460');
        this.drawDirectorBody(ctx, unit, gender, mainColor, accentColor);
        this.drawHead(ctx, unit, skinTone, hairColor, gender);
        this.drawFace(ctx, unit, skinTone);
        this.drawDirectorCap(ctx, unit, accentColor);

        return canvas;
    }

    drawPixelBackground(ctx, size, color) {
        ctx.fillStyle = color;
        ctx.fillRect(0, 0, size, size);
        
        ctx.fillStyle = this.adjustColor(color, -20);
        for (let i = 0; i < size; i += 8) {
            for (let j = 0; j < size; j += 8) {
                if ((i + j) % 16 === 0) {
                    ctx.fillRect(i, j, 4, 4);
                }
            }
        }
    }

    drawBody(ctx, unit, gender, mainColor, accentColor) {
        ctx.fillStyle = mainColor;
        
        if (gender === 'male') {
            ctx.fillRect(unit * 4, unit * 8, unit * 8, unit * 6);
        } else {
            ctx.fillRect(unit * 4, unit * 8, unit * 8, unit * 6);
            ctx.fillStyle = accentColor;
            ctx.fillRect(unit * 4, unit * 10, unit * 8, unit * 2);
        }

        ctx.fillStyle = this.adjustColor(mainColor, -30);
        ctx.fillRect(unit * 2, unit * 9, unit * 2, unit * 4);
        ctx.fillRect(unit * 12, unit * 9, unit * 2, unit * 4);
    }

    drawDirectorBody(ctx, unit, gender, mainColor, accentColor) {
        ctx.fillStyle = '#2C2C2C';
        ctx.fillRect(unit * 4, unit * 8, unit * 8, unit * 6);
        
        ctx.fillStyle = accentColor;
        ctx.fillRect(unit * 5, unit * 8, unit * 6, unit * 1);
        
        ctx.fillStyle = '#444444';
        ctx.fillRect(unit * 2, unit * 9, unit * 2, unit * 4);
        ctx.fillRect(unit * 12, unit * 9, unit * 2, unit * 4);
    }

    drawHead(ctx, unit, skinTone, hairColor, gender) {
        ctx.fillStyle = hairColor;
        ctx.fillRect(unit * 4, unit * 2, unit * 8, unit * 5);
        ctx.fillRect(unit * 3, unit * 3, unit * 2, unit * 3);
        ctx.fillRect(unit * 11, unit * 3, unit * 2, unit * 3);
        
        if (gender === 'female') {
            ctx.fillRect(unit * 3, unit * 5, unit * 2, unit * 4);
            ctx.fillRect(unit * 11, unit * 5, unit * 2, unit * 4);
        }

        ctx.fillStyle = skinTone;
        ctx.fillRect(unit * 4, unit * 4, unit * 8, unit * 5);
    }

    drawFace(ctx, unit, skinTone) {
        ctx.fillStyle = '#2C1810';
        ctx.fillRect(unit * 5, unit * 5, unit * 2, unit * 2);
        ctx.fillRect(unit * 9, unit * 5, unit * 2, unit * 2);
        
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(unit * 6, unit * 5, unit * 1, unit * 1);
        ctx.fillRect(unit * 10, unit * 5, unit * 1, unit * 1);

        ctx.fillStyle = '#E94560';
        ctx.fillRect(unit * 6, unit * 8, unit * 4, unit * 1);
    }

    drawAccessory(ctx, unit, guestId) {
        const accessories = {
            1: () => this.drawBasketball(ctx, unit),
            2: () => this.drawGlasses(ctx, unit),
            3: () => this.drawPaintbrush(ctx, unit),
            4: () => this.drawBook(ctx, unit),
            5: () => this.drawChefHat(ctx, unit),
            6: () => this.drawGuitar(ctx, unit),
            7: () => this.drawBow(ctx, unit),
            8: () => this.drawNecklace(ctx, unit),
            9: () => this.drawCamera(ctx, unit),
            10: () => this.drawNurseCap(ctx, unit),
            11: () => this.drawBook(ctx, unit),
            12: () => this.drawFlower(ctx, unit)
        };

        if (accessories[guestId]) {
            accessories[guestId]();
        }
    }

    drawBasketball(ctx, unit) {
        ctx.fillStyle = '#FF8C00';
        ctx.fillRect(unit * 12, unit * 12, unit * 3, unit * 3);
        ctx.fillStyle = '#2C1810';
        ctx.fillRect(unit * 13, unit * 12, unit * 1, unit * 3);
        ctx.fillRect(unit * 12, unit * 13, unit * 3, unit * 1);
    }

    drawGlasses(ctx, unit) {
        ctx.fillStyle = '#2C2C2C';
        ctx.fillRect(unit * 4, unit * 5, unit * 3, unit * 2);
        ctx.fillRect(unit * 9, unit * 5, unit * 3, unit * 2);
        ctx.fillRect(unit * 7, unit * 6, unit * 2, unit * 1);
    }

    drawPaintbrush(ctx, unit) {
        ctx.fillStyle = '#8B4513';
        ctx.fillRect(unit * 13, unit * 8, unit * 1, unit * 6);
        ctx.fillStyle = '#E94560';
        ctx.fillRect(unit * 13, unit * 7, unit * 1, unit * 2);
    }

    drawBook(ctx, unit) {
        ctx.fillStyle = '#8B4513';
        ctx.fillRect(unit * 2, unit * 10, unit * 4, unit * 3);
        ctx.fillStyle = '#FFFDD0';
        ctx.fillRect(unit * 3, unit * 11, unit * 2, unit * 1);
    }

    drawChefHat(ctx, unit) {
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(unit * 5, unit * 1, unit * 6, unit * 2);
        ctx.fillRect(unit * 6, unit * 0, unit * 4, unit * 2);
    }

    drawGuitar(ctx, unit) {
        ctx.fillStyle = '#8B4513';
        ctx.fillRect(unit * 1, unit * 8, unit * 2, unit * 8);
        ctx.fillRect(unit * 0, unit * 12, unit * 4, unit * 4);
        ctx.fillStyle = '#E94560';
        ctx.fillRect(unit * 1, unit * 13, unit * 2, unit * 2);
    }

    drawBow(ctx, unit) {
        ctx.fillStyle = '#FF69B4';
        ctx.fillRect(unit * 3, unit * 3, unit * 2, unit * 2);
        ctx.fillRect(unit * 11, unit * 3, unit * 2, unit * 2);
        ctx.fillRect(unit * 6, unit * 3, unit * 4, unit * 1);
    }

    drawNecklace(ctx, unit) {
        ctx.fillStyle = '#FFD700';
        ctx.fillRect(unit * 6, unit * 9, unit * 4, unit * 1);
        ctx.fillRect(unit * 7, unit * 10, unit * 2, unit * 1);
    }

    drawCamera(ctx, unit) {
        ctx.fillStyle = '#2C2C2C';
        ctx.fillRect(unit * 12, unit * 10, unit * 3, unit * 2);
        ctx.fillStyle = '#4169E1';
        ctx.fillRect(unit * 12, unit * 10, unit * 1, unit * 1);
    }

    drawNurseCap(ctx, unit) {
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(unit * 4, unit * 2, unit * 8, unit * 2);
        ctx.fillStyle = '#E94560';
        ctx.fillRect(unit * 7, unit * 2, unit * 2, unit * 2);
    }

    drawFlower(ctx, unit) {
        ctx.fillStyle = '#FF69B4';
        ctx.fillRect(unit * 11, unit * 3, unit * 1, unit * 1);
        ctx.fillRect(unit * 12, unit * 2, unit * 1, unit * 1);
        ctx.fillRect(unit * 13, unit * 3, unit * 1, unit * 1);
        ctx.fillRect(unit * 12, unit * 4, unit * 1, unit * 1);
        ctx.fillStyle = '#FFD700';
        ctx.fillRect(unit * 12, unit * 3, unit * 1, unit * 1);
    }

    drawDirectorCap(ctx, unit, accentColor) {
        ctx.fillStyle = '#1A1A2E';
        ctx.fillRect(unit * 3, unit * 1, unit * 10, unit * 2);
        ctx.fillRect(unit * 4, unit * 0, unit * 8, unit * 2);
        ctx.fillStyle = accentColor;
        ctx.fillRect(unit * 5, unit * 2, unit * 6, unit * 1);
    }

    getHairColor(id) {
        const colors = [
            '#8B4513', '#2C1810', '#4A3728', '#1A1A1A',
            '#654321', '#E6E6FA', '#FFD700', '#8B008B',
            '#1A1A2E', '#2F4F4F', '#228B22', '#DDA0DD'
        ];
        return colors[(id - 1) % colors.length];
    }

    adjustColor(color, amount) {
        const hex = color.replace('#', '');
        const r = Math.max(0, Math.min(255, parseInt(hex.substr(0, 2), 16) + amount));
        const g = Math.max(0, Math.min(255, parseInt(hex.substr(2, 2), 16) + amount));
        const b = Math.max(0, Math.min(255, parseInt(hex.substr(4, 2), 16) + amount));
        return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
    }
}
