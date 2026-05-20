export default class PixelFontRenderer {
    constructor(ctx) {
        this.ctx = ctx;
        this.pixelSize = 2;
        this.spacing = 6;
        this.lineSpacing = 16;
    }

    drawPixel(x, y, color, size = this.pixelSize) {
        this.ctx.fillStyle = color;
        this.ctx.fillRect(Math.floor(x), Math.floor(y), size, size);
    }

    drawChar(char, x, y, color = '#FFFFFF', scale = 1) {
        const size = this.pixelSize * scale;
        const fontMap = this.getFontMap();
        const charData = fontMap[char.toLowerCase()] || fontMap['?'];
        
        if (!charData) return 0;

        const charWidth = charData[0].length * size;
        
        charData.forEach((row, rowIndex) => {
            row.split('').forEach((pixel, colIndex) => {
                if (pixel === '1') {
                    this.drawPixel(
                        x + colIndex * size,
                        y + rowIndex * size,
                        color,
                        size
                    );
                }
            });
        });

        return charWidth;
    }

    drawText(text, x, y, color = '#FFFFFF', scale = 1) {
        let currentX = x;
        const charSpacing = this.spacing * scale;
        
        text.split('').forEach((char, index) => {
            if (char === ' ') {
                currentX += charSpacing * 2;
                return;
            }
            
            const charWidth = this.drawChar(char, currentX, y, color, scale);
            currentX += charWidth + charSpacing;
        });

        return currentX - x;
    }

    drawTextWithWrap(text, x, y, maxWidth, color = '#FFFFFF', scale = 1) {
        const words = text.split('');
        let currentX = x;
        let currentY = y;
        const charSpacing = this.spacing * scale;
        const lineHeight = this.lineSpacing * scale;

        words.forEach((char) => {
            if (char === '\n') {
                currentX = x;
                currentY += lineHeight;
                return;
            }
            
            if (char === ' ') {
                currentX += charSpacing * 2;
                return;
            }

            const charWidth = this.getCharWidth(char, scale);
            if (currentX + charWidth > x + maxWidth && currentX > x) {
                currentX = x;
                currentY += lineHeight;
            }

            this.drawChar(char, currentX, currentY, color, scale);
            currentX += charWidth + charSpacing;
        });

        return currentY + lineHeight;
    }

    getCharWidth(char, scale = 1) {
        const fontMap = this.getFontMap();
        const charData = fontMap[char.toLowerCase()] || fontMap['?'];
        if (!charData) return 0;
        return charData[0].length * this.pixelSize * scale;
    }

    measureTextWidth(text, scale = 1) {
        let width = 0;
        const charSpacing = this.spacing * scale;
        
        text.split('').forEach((char) => {
            if (char === ' ') {
                width += charSpacing * 2;
            } else {
                width += this.getCharWidth(char, scale) + charSpacing;
            }
        });

        return width;
    }

    getFontMap() {
        return {
            'a': [' 11 ', '1  1', '1111', '1  1', '1  1'],
            'b': ['111 ', '1  1', '111 ', '1  1', '111 '],
            'c': [' 11 ', '1  1', '1   ', '1  1', ' 11 '],
            'd': ['111 ', '1  1', '1  1', '1  1', '111 '],
            'e': ['1111', '1   ', '111 ', '1   ', '1111'],
            'f': ['1111', '1   ', '111 ', '1   ', '1   '],
            'g': [' 11 ', '1  1', '1 11', '1  1', ' 11 '],
            'h': ['1  1', '1  1', '1111', '1  1', '1  1'],
            'i': [' 11 ', '  1 ', '  1 ', '  1 ', ' 11 '],
            'j': ['  11', '   1', '   1', '1  1', ' 11 '],
            'k': ['1  1', '1 1 ', '11  ', '1 1 ', '1  1'],
            'l': ['1   ', '1   ', '1   ', '1   ', '1111'],
            'm': ['1   1', '11 11', '1 1 1', '1   1', '1   1'],
            'n': ['1   1', '11  1', '1 1 1', '1  11', '1   1'],
            'o': [' 11 ', '1  1', '1  1', '1  1', ' 11 '],
            'p': ['111 ', '1  1', '111 ', '1   ', '1   '],
            'q': [' 11 ', '1  1', '1  1', '1 11', ' 111'],
            'r': ['111 ', '1  1', '111 ', '1 1 ', '1  1'],
            's': [' 111', '1   ', ' 11 ', '   1', '111 '],
            't': ['11111', '  1  ', '  1  ', '  1  ', '  1  '],
            'u': ['1  1', '1  1', '1  1', '1  1', ' 11 '],
            'v': ['1  1', '1  1', '1  1', ' 11 ', '  1  '],
            'w': ['1   1', '1   1', '1 1 1', '11 11', '1   1'],
            'x': ['1  1', ' 11 ', '  1 ', ' 11 ', '1  1'],
            'y': ['1  1', '1  1', ' 11 ', '  1 ', ' 1  '],
            'z': ['1111', '   1', '  1 ', ' 1  ', '1111'],
            '0': [' 11 ', '1  1', '1  1', '1  1', ' 11 '],
            '1': [' 11 ', '1 1 ', '  1 ', '  1 ', '1111'],
            '2': [' 11 ', '1  1', '  1 ', ' 1  ', '1111'],
            '3': [' 11 ', '1  1', '  11', '1  1', ' 11 '],
            '4': ['  1 ', ' 11 ', '1 1 ', '1111', '  1 '],
            '5': ['1111', '1   ', '111 ', '   1', '111 '],
            '6': [' 11 ', '1   ', '111 ', '1  1', ' 11 '],
            '7': ['1111', '   1', '  1 ', ' 1  ', ' 1  '],
            '8': [' 11 ', '1  1', ' 11 ', '1  1', ' 11 '],
            '9': [' 11 ', '1  1', ' 111', '   1', ' 11 '],
            '!': [' 1 ', ' 1 ', ' 1 ', '   ', ' 1 '],
            '?': [' 11 ', '1  1', '  1 ', '   ', '  1 '],
            '.': ['   ', '   ', '   ', '   ', ' 1 '],
            ',': ['   ', '   ', '   ', ' 1 ', '1  '],
            ':': ['   ', ' 1 ', '   ', ' 1 ', '   '],
            '-': ['   ', '   ', '111', '   ', '   '],
            '+': ['   ', ' 1 ', '111', ' 1 ', '   '],
            '=': ['   ', '111', '   ', '111', '   '],
            '(': ['  1', ' 1 ', ' 1 ', ' 1 ', '  1'],
            ')': ['1  ', ' 1 ', ' 1 ', ' 1 ', '1  '],
            '/': ['   1', '  1 ', ' 1  ', '1   ', '    '],
            '\\': ['1   ', ' 1  ', '  1 ', '   1', '    '],
            '"': ['1 1', '1 1', '   ', '   ', '   '],
            "'": [' 1 ', ' 1 ', '   ', '   ', '   '],
            ' ': ['    ', '    ', '    ', '    ', '    ']
        };
    }
}
