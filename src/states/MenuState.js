import Button from '../ui/Button.js';
import ModeSelectState from './ModeSelectState.js';

class MenuState {
    constructor(game) {
        this.game = game;
        this.buttons = [];
        this.titleY = 100;
        this.titleBounce = 0;
        this.isInit = false;
    }
    
    onEnter(data) {
        if (!this.isInit) {
            this.init();
            this.isInit = true;
        }
        
        this.game.guiManager.clearElements();
        for (let button of this.buttons) {
            this.game.guiManager.addElement(button);
        }
    }
    
    init() {
        const centerX = 480;
        
        this.buttons.push(new Button(
            centerX - 120, 280, 240, 60,
            '开始游戏',
            () => {
                this.game.stateManager.setState('modeSelect');
            },
            '#2a5a2a',
            '#3a7a3a'
        ));
        
        this.buttons.push(new Button(
            centerX - 120, 360, 240, 60,
            '结局图鉴',
            () => {
                this.game.stateManager.setState('endingGallery');
            },
            '#5a3a7a',
            '#7a5a9a'
        ));
        
        this.buttons.push(new Button(
            centerX - 120, 440, 240, 60,
            '游戏设置',
            () => {
                console.log('游戏设置');
            }
        ));
    }
    
    update(dt, data) {
        this.titleBounce += dt * 3;
    }
    
    render(renderer, data) {
        const centerX = 480;
        const bounceOffset = Math.sin(this.titleBounce) * 5;
        
        renderer.drawPixelText(
            '糟糕！是心动鸭！',
            centerX,
            this.titleY + bounceOffset,
            '#ffcc00',
            40,
            'center'
        );
        
        renderer.drawPixelText(
            '一个恋综模拟器',
            centerX,
            170,
            '#aaaacc',
            18,
            'center'
        );
        
        for (let i = 0; i < 6; i++) {
            const heartX = 100 + i * 160;
            const heartY = 550 + Math.sin(this.titleBounce + i) * 10;
            renderer.drawHeart(heartX, heartY, 24, true);
        }
    }
    
    onExit() {
    }
}

export default MenuState;
