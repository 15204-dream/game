import Button from '../ui/Button.js';
import DialogBox from '../ui/DialogBox.js';

class GuestModeState {
    constructor(game) {
        this.game = game;
        this.buttons = [];
        this.currentDialog = null;
        this.gameStarted = false;
        this.day = 1;
        this.isInit = false;
    }
    
    onEnter(data) {
        if (!this.isInit) {
            this.init();
            this.isInit = true;
        }
        
        this.game.guiManager.clearElements();
        if (!this.gameStarted) {
            this.startIntroduction();
        }
    }
    
    init() {
        this.backButton = new Button(
            20, 20, 120, 40,
            '返回',
            () => {
                this.game.stateManager.setState('modeSelect');
            }
        );
    }
    
    startIntroduction() {
        this.gameStarted = true;
        this.game.guiManager.addElement(this.backButton);
        
        const dialog = new DialogBox(
            80, 380, 800, 200,
            '欢迎来到《糟糕！是心动鸭！》恋综节目！\n\n在接下来的30天里，你将和其他12位嘉宾一起生活，寻找属于你的真爱...',
            '旁白',
            [],
            () => {
                this.showNextIntro();
            }
        );
        
        this.game.guiManager.showDialog(dialog);
    }
    
    showNextIntro() {
        const dialog = new DialogBox(
            80, 380, 800, 200,
            '首先，让我们来认识一下各位嘉宾吧！',
            '旁白',
            ['好的！', '迫不及待了！'],
            (index, option) => {
                this.showGuestIntroduction();
            }
        );
        
        this.game.guiManager.showDialog(dialog);
    }
    
    showGuestIntroduction() {
        const guests = this.game.guestData.getGuests();
        let guestList = '12位嘉宾：\n';
        for (let guest of guests) {
            guestList += `${guest.nickname} - ${guest.occupation}\n`;
        }
        
        const dialog = new DialogBox(
            80, 380, 800, 200,
            guestList,
            '旁白',
            ['开始旅程！'],
            (index, option) => {
                this.showMainGame();
            }
        );
        
        this.game.guiManager.showDialog(dialog);
    }
    
    showMainGame() {
        this.game.guiManager.hideDialog();
        this.setupGameButtons();
    }
    
    setupGameButtons() {
        const centerX = 480;
        
        this.buttons.push(new Button(
            centerX - 150, 180, 300, 60,
            '开始剧情',
            () => {
                this.startStory();
            },
            '#3a6a3a',
            '#4a8a4a'
        ));
        
        this.buttons.push(new Button(
            centerX - 150, 260, 300, 60,
            '查看嘉宾',
            () => {
                this.showGuestList();
            }
        ));
        
        this.buttons.push(new Button(
            centerX - 150, 340, 300, 60,
            '今日活动',
            () => {
                console.log('今日活动');
            }
        ));
        
        this.buttons.push(new Button(
            centerX - 150, 420, 300, 60,
            '自由行动',
            () => {
                console.log('自由行动');
            }
        ));
        
        for (let button of this.buttons) {
            this.game.guiManager.addElement(button);
        }
    }
    
    startStory() {
        this.game.guiManager.clearElements();
        this.game.guiManager.addElement(this.backButton);
        this.game.storyEngine.reset();
        this.showStoryNode();
    }
    
    showStoryNode() {
        const node = this.game.storyEngine.getCurrentNode();
        
        if (!node) {
            this.game.stateManager.setState('menu');
            return;
        }
        
        if (node.isEnd) {
            this.backToMenuFromStory();
            return;
        }
        
        if (node.affectionGain) {
            for (let guestId in node.affectionGain) {
                this.game.guestData.updateAffection(guestId, node.affectionGain[guestId]);
            }
        }
        
        const choiceTexts = node.choices ? node.choices.map(c => c.text) : [];
        
        const dialog = new DialogBox(
            80, 380, 800, 200,
            node.text,
            node.speaker,
            choiceTexts,
            (index) => {
                this.game.storyEngine.makeChoice(index);
                this.game.guiManager.hideDialog();
                this.showStoryNode();
            }
        );
        
        this.game.guiManager.showDialog(dialog);
    }
    
    backToMenuFromStory() {
        this.game.guiManager.clearElements();
        this.game.stateManager.setState('menu');
    }
    
    showGuestList() {
        this.game.guiManager.hideDialog();
        this.viewingGuest = false;
        this.currentGuest = null;
        for (let button of this.buttons) {
            this.game.guiManager.removeElement(button);
        }
        
        if (this.guestDetailButtons) {
            for (let btn of this.guestDetailButtons) {
                this.game.guiManager.removeElement(btn);
            }
            this.guestDetailButtons = [];
        }
        
        const guests = this.game.guestData.getGuests();
        this.guestButtons = [];
        
        for (let i = 0; i < guests.length; i++) {
            const guest = guests[i];
            const row = Math.floor(i / 3);
            const col = i % 3;
            const x = 80 + col * 260;
            const y = 120 + row * 100;
            
            const btn = new Button(
                x, y, 240, 80,
                `${guest.nickname}\n${guest.occupation}`,
                () => {
                    this.showGuestDetail(guest);
                }
            );
            this.guestButtons.push(btn);
            this.game.guiManager.addElement(btn);
        }
        
        const backBtn = new Button(
            380, 540, 200, 50,
            '返回',
            () => {
                this.game.guiManager.hideDialog();
                for (let btn of this.guestButtons) {
                    this.game.guiManager.removeElement(btn);
                }
                this.game.guiManager.removeElement(backBtn);
                for (let button of this.buttons) {
                    this.game.guiManager.addElement(button);
                }
            }
        );
        this.guestButtons.push(backBtn);
        this.game.guiManager.addElement(backBtn);
    }
    
    showGuestDetail(guest) {
        this.currentGuest = guest;
        this.viewingGuest = true;
        
        for (let btn of this.guestButtons) {
            this.game.guiManager.removeElement(btn);
        }
        
        this.guestDetailButtons = [];
        
        const chatBtn = new Button(
            420, 520, 150, 50,
            '聊天',
            () => {
                this.startChat(guest);
            },
            '#5a3a7a',
            '#7a5a9a'
        );
        this.guestDetailButtons.push(chatBtn);
        this.game.guiManager.addElement(chatBtn);
        
        const backBtn = new Button(
            590, 520, 150, 50,
            '返回',
            () => {
                this.viewingGuest = false;
                this.currentGuest = null;
                for (let btn of this.guestDetailButtons) {
                    this.game.guiManager.removeElement(btn);
                }
                this.showGuestList();
            }
        );
        this.guestDetailButtons.push(backBtn);
        this.game.guiManager.addElement(backBtn);
    }
    
    render(renderer) {
        const centerX = 480;
        
        renderer.drawPixelText(
            '嘉宾模式',
            centerX,
            50,
            '#ffcc00',
            28,
            'center'
        );
        
        renderer.drawPixelText(
            `第 ${this.day} 天`,
            800,
            30,
            '#aaaacc',
            18,
            'right'
        );
        
        renderer.drawPixelRect(
            60, 80, 840, 2,
            '#3a3a5a'
        );
        
        if (this.viewingGuest && this.currentGuest) {
            this.renderGuestDetail(renderer);
        }
    }
    
    renderGuestDetail(renderer) {
        const guest = this.currentGuest;
        
        renderer.drawPixelRect(160, 100, 640, 480, '#1a1a2e', '#4a4a6a', 4);
        
        this.game.resourceManager.drawPixelCharacter(
            this.renderer.ctx,
            guest,
            220,
            140,
            160
        );
        
        renderer.drawPixelText(guest.nickname, 450, 130, '#ffcc00', 28, 'left');
        renderer.drawPixelText(`本名：${guest.realName}`, 450, 170, '#fff', 16, 'left');
        renderer.drawPixelText(`年龄：${guest.age}岁`, 450, 200, '#fff', 16, 'left');
        renderer.drawPixelText(`职业：${guest.occupation}`, 450, 230, '#fff', 16, 'left');
        
        renderer.drawPixelText('性格：', 180, 320, '#ffcc00', 16, 'left');
        renderer.drawPixelText(guest.personality.join('、'), 180, 350, '#aaaacc', 14, 'left');
        
        renderer.drawPixelText('喜欢：', 180, 390, '#ffcc00', 16, 'left');
        renderer.drawPixelText(guest.likes.join('、'), 180, 420, '#aaaacc', 14, 'left');
        
        renderer.drawPixelText('好感度：', 180, 460, '#ffcc00', 16, 'left');
        this.renderAffectionBar(renderer, 280, 460, guest);
        
        renderer.drawPixelText(
            `${guest.affection}/100 - ${this.game.guestData.getRelationLevelName(guest.relationLevel)}`,
            500,
            460,
            '#aaaacc',
            14,
            'left'
        );
    }
    
    renderAffectionBar(renderer, x, y, guest) {
        const width = 200;
        const height = 20;
        
        renderer.drawPixelRect(x, y, width, height, '#1a1a1a', '#4a4a6a', 2);
        
        const fillWidth = Math.max(0, Math.min(width, (guest.affection / 100) * width));
        let fillColor = '#aaa';
        if (guest.affection >= 80) fillColor = '#ff4757';
        else if (guest.affection >= 60) fillColor = '#ff6b81';
        else if (guest.affection >= 40) fillColor = '#ffa502';
        else if (guest.affection >= 20) fillColor = '#ffd700';
        
        renderer.drawPixelRect(x + 2, y + 2, fillWidth - 4, height - 4, fillColor);
    }
    
    startChat(guest) {
        this.currentGuest = guest;
        this.game.guiManager.hideDialog();
        this.showAIChat(guest, `嗨！我是${guest.nickname}，很高兴认识你！`);
    }
    
    showAIChat(guest, message) {
        const dialog = new DialogBox(
            80, 380, 800, 200,
            message,
            guest.nickname,
            ['你好！', '我也很高兴！', '你今天怎么样？', '聊点别的...'],
            async (index, option) => {
                this.game.guestData.updateAffection(guest.id, 3);
                this.game.guiManager.hideDialog();
                await this.showAIResponse(guest, option);
            }
        );
        
        this.game.guiManager.showDialog(dialog);
    }
    
    async showAIResponse(guest, userMessage) {
        this.showThinkingDialog();
        
        const aiResponse = await this.game.aiEngine.generateResponse(
            guest, 
            userMessage,
            guest.id
        );
        
        this.game.guestData.updateAffection(guest.id, 2);
        this.showAIChat(guest, aiResponse);
    }
    
    showThinkingDialog() {
        const dialog = new DialogBox(
            80, 380, 800, 200,
            '正在思考中...',
            '系统',
            [],
            null
        );
        this.game.guiManager.showDialog(dialog);
    }
    

    
    update(dt, data) {
    }
    
    render(renderer, data) {
        const centerX = 480;
        
        renderer.drawPixelText(
            '嘉宾模式',
            centerX,
            50,
            '#ffcc00',
            28,
            'center'
        );
        
        renderer.drawPixelText(
            `第 ${this.day} 天`,
            800,
            30,
            '#aaaacc',
            18,
            'right'
        );
        
        renderer.drawPixelRect(
            60, 80, 840, 2,
            '#3a3a5a'
        );
    }
    
    onExit() {
        this.game.guiManager.clearElements();
    }
}

export default GuestModeState;
