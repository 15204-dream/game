import Button from '../ui/Button.js';
import GuestModeState from './GuestModeState.js';

class ModeSelectState {
    constructor(game) {
        this.game = game;
        this.buttons = [];
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
            centerX - 170, 250, 340, 80,
            '嘉宾模式',
            () => {
                this.game.stateManager.setState('guestMode');
            },
            '#5a3a7a',
            '#7a5a9a'
        ));
        
        this.buttons.push(new Button(
            centerX - 170, 360, 340, 80,
            '导演模式 (开发中)',
            () => {
                console.log('导演模式开发中');
            },
            '#3a4a5a',
            '#4a5a6a'
        ));
        
        this.buttons.push(new Button(
            centerX - 100, 480, 200, 50,
            '返回菜单',
            () => {
                this.game.stateManager.setState('menu');
            }
        ));
    }
    
    update(dt, data) {
    }
    
    render(renderer, data) {
        const centerX = 480;
        
        renderer.drawPixelText(
            '选择你的身份',
            centerX,
            120,
            '#ffcc00',
            32,
            'center'
        );
        
        renderer.drawPixelText(
            '嘉宾模式：作为嘉宾参与节目，寻找真爱！',
            centerX,
            190,
            '#aaaacc',
            16,
            'center'
        );
    }
    
    onExit() {
    }
}

export default ModeSelectState;
