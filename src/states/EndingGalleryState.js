import Button from '../ui/Button.js';

class EndingGalleryState {
    constructor(game) {
        this.game = game;
        this.buttons = [];
        this.scrollOffset = 0;
    }
    
    onEnter(data) {
        this.buttons = [];
        this.game.guiManager.clearElements();
        
        const backBtn = new Button(
            20, 20, 120, 50,
            '返回',
            () => {
                this.game.stateManager.setState('menu');
            }
        );
        this.buttons.push(backBtn);
        this.game.guiManager.addElement(backBtn);
    }
    
    update(dt, data) {
    }
    
    render(renderer, data) {
        const centerX = 480;
        
        renderer.drawPixelText(
            '结局图鉴',
            centerX,
            50,
            '#ffcc00',
            32,
            'center'
        );
        
        const unlocked = this.game.endingManager.getUnlockedCount();
        const total = this.game.endingManager.getTotalCount();
        renderer.drawPixelText(
            `已解锁 ${unlocked}/${total}`,
            centerX,
            90,
            '#aaaacc',
            18,
            'center'
        );
        
        const endings = this.game.endingManager.getAllEndings();
        let y = 130;
        
        for (let i = 0; i < endings.length; i++) {
            const ending = endings[i];
            const isUnlocked = this.game.endingManager.isUnlocked(ending.id);
            
            renderer.drawPixelRect(100, y, 760, 70, '#1a1a2e', '#4a4a6a', 3);
            
            if (isUnlocked) {
                let color = '#aaa';
                if (ending.type === 'true') color = '#ff4757';
                else if (ending.type === 'friend') color = '#4ecdc4';
                else if (ending.type === 'special') color = '#ffcc00';
                else if (ending.type === 'secret') color = '#a078c0';
                
                renderer.drawPixelText(ending.name, 120, y + 18, color, 18, 'left');
                renderer.drawPixelText(ending.desc, 120, y + 42, '#aaaacc', 14, 'left');
            } else {
                renderer.drawPixelText('???', 120, y + 25, '#555', 24, 'left');
                renderer.drawPixelText('尚未解锁', 180, y + 28, '#555', 16, 'left');
            }
            
            y += 80;
            if (y > 550) break;
        }
    }
    
    onExit() {
    }
}

export default EndingGalleryState;
