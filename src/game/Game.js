import StateManager from './StateManager.js';
import InputManager from './InputManager.js';
import GuestData from '../data/GuestData.js';
import AIDialogueEngine from '../ai/AIDialogueEngine.js';
import EndingManager from '../ending/EndingManager.js';
import StoryData from '../story/StoryData.js';
import PixelFontRenderer from '../graphics/PixelFontRenderer.js';
import CharacterSpriteGenerator from '../graphics/CharacterSpriteGenerator.js';
import BackgroundGenerator from '../graphics/BackgroundGenerator.js';
import UIElementGenerator from '../graphics/UIElementGenerator.js';
import CoverGenerator from '../graphics/CoverGenerator.js';
import GUIManager from '../ui/GUIManager.js';
import Button from '../ui/Button.js';

export default class Game {
    constructor(canvas) {
        this.canvas = canvas;
        this.ctx = canvas.getContext('2d');
        this.ctx.imageSmoothingEnabled = false;
        
        this.width = canvas.width;
        this.height = canvas.height;
        
        this.stateManager = new StateManager();
        this.inputManager = new InputManager(canvas);
        this.guestData = new GuestData();
        this.aiDialogueEngine = new AIDialogueEngine();
        this.endingManager = new EndingManager();
        this.storyData = new StoryData();
        
        this.fontRenderer = new PixelFontRenderer(this.ctx);
        this.backgroundGenerator = new BackgroundGenerator(this.ctx);
        this.uiGenerator = new UIElementGenerator(this.ctx);
        
        this.characterSprites = null;
        this.guiManager = new GUIManager();
        this.coverCanvas = null;
        
        this.gameData = {
            affection: {},
            currentStoryNode: 'start',
            day: 1,
            mode: null,
            playerCharacter: null,
            finalChoice: null,
            currentEndingText: ''
        };
        
        this.guests = this.guestData.getGuests();
        this.guests.forEach(guest => {
            this.gameData.affection[guest.id] = 0;
        });
        
        this.currentTime = 0;
        this.running = false;
        this.lastTime = 0;
        this.deltaTime = 0;
    }

    async init() {
        const spriteGenerator = new CharacterSpriteGenerator(this.ctx);
        this.characterSprites = spriteGenerator.generateAllCharacters(this.guests, 128);
        
        // 生成封面
        const coverGenerator = new CoverGenerator(this.ctx);
        this.coverCanvas = coverGenerator.generateCover(this.width, this.height);
        
        this.setupMenuUI();
        
        this.running = true;
        this.lastTime = performance.now();
        this.gameLoop();
        
        const loading = document.getElementById('loading');
        if (loading) {
            loading.style.display = 'none';
        }
    }

    setupMenuUI() {
        this.guiManager.clear();

        const centerX = this.width / 2;

        this.guiManager.addElement(new Button(
            centerX - 100, 350, 200, 60,
            '开始游戏',
            '#FFB6C1',
            () => {
                this.stateManager.setState(this.stateManager.states.MODE_SELECT);
                this.setupModeSelectUI();
            }
        ));

        this.guiManager.addElement(new Button(
            centerX - 100, 430, 200, 60,
            '结局画廊',
            '#87CEEB',
            () => {
                this.stateManager.setState(this.stateManager.states.ENDING_GALLERY);
                this.setupEndingGalleryUI();
            }
        ));
    }

    setupModeSelectUI() {
        this.guiManager.clear();

        const centerX = this.width / 2;

        this.guiManager.addElement(new Button(
            centerX - 100, 300, 200, 60,
            '嘉宾模式',
            '#FFB6C1',
            () => {
                this.gameData.mode = 'guest';
                this.stateManager.setState(this.stateManager.states.GUEST_MODE);
                this.setupGuestModeUI();
            }
        ));

        this.guiManager.addElement(new Button(
            centerX - 100, 380, 200, 60,
            '导演模式',
            '#87CEEB',
            () => {
                this.gameData.mode = 'director';
                this.stateManager.setState(this.stateManager.states.DIRECTOR_MODE);
            }
        ));

        this.guiManager.addElement(new Button(
            centerX - 100, 460, 200, 60,
            '返回',
            '#DCDCDC',
            () => {
                this.stateManager.setState(this.stateManager.states.MENU);
                this.setupMenuUI();
            }
        ));
    }

    setupGuestModeUI() {
        this.guiManager.clear();
        this.gameData.currentStoryNode = 'start';
        this.updateStoryButtons();
    }

    setupEndingGalleryUI() {
        this.guiManager.clear();

        const centerX = this.width / 2;

        this.guiManager.addElement(new Button(
            centerX - 100, 650, 200, 60,
            '返回',
            '#DCDCDC',
            () => {
                this.stateManager.setState(this.stateManager.states.MENU);
                this.setupMenuUI();
            }
        ));
    }

    update() {
        this.currentTime++;
        this.guiManager.update(this.inputManager);
        
        const currentState = this.stateManager.getCurrentState();
        
        if (currentState === this.stateManager.states.MODE_SELECT) {
            if (this.guiManager.elements.length === 0) {
                this.setupModeSelectUI();
            }
        } else if (currentState === this.stateManager.states.ENDING_GALLERY) {
            if (this.guiManager.elements.length === 0) {
                this.setupEndingGalleryUI();
            }
        }
    }

    render() {
        this.ctx.clearRect(0, 0, this.width, this.height);
        
        const currentState = this.stateManager.getCurrentState();
        
        if (currentState === this.stateManager.states.MENU) {
            this.renderMenu();
        } else if (currentState === this.stateManager.states.MODE_SELECT) {
            this.renderModeSelect();
        } else if (currentState === this.stateManager.states.GUEST_MODE) {
            this.renderGuestMode();
        } else if (currentState === this.stateManager.states.DIRECTOR_MODE) {
            this.renderDirectorMode();
        } else if (currentState === this.stateManager.states.ENDING_GALLERY) {
            this.renderEndingGallery();
        }
        
        this.guiManager.render(this.ctx, this.uiGenerator, this.fontRenderer);
    }

    renderMenu() {
        // 显示游戏封面
        if (this.coverCanvas) {
            this.ctx.drawImage(this.coverCanvas, 0, 0, this.width, this.height);
        } else {
            this.backgroundGenerator.drawHeartBackground(this.currentTime);
        }
        
        // 稍微调整一下，让按钮更显眼
        const centerX = this.width / 2;
        
        // 如果有封面，在底部加上一点渐变让按钮更清晰
        if (this.coverCanvas) {
            const gradient = this.ctx.createLinearGradient(0, this.height * 0.65, 0, this.height);
            gradient.addColorStop(0, 'rgba(22, 33, 62, 0)');
            gradient.addColorStop(1, 'rgba(22, 33, 62, 0.9)');
            this.ctx.fillStyle = gradient;
            this.ctx.fillRect(0, this.height * 0.65, this.width, this.height * 0.35);
        }
        
        // 额外显示12个嘉宾头像
        this.renderCharacterRow(100, 480, [1, 2, 3, 4, 5, 6]);
        this.renderCharacterRow(100, 590, [7, 8, 9, 10, 11, 12]);
    }

    renderModeSelect() {
        this.backgroundGenerator.drawLivingRoomBackground(this.currentTime);

        const centerX = this.width / 2;
        this.fontRenderer.drawText('选择游戏模式', centerX - 150, 200, '#FFFFFF', 1.5);

        this.uiGenerator.drawDecorationBorder(centerX - 250, 250, 500, 300, '#FFB6C1');
    }

    renderGuestMode() {
        this.backgroundGenerator.drawLivingRoomBackground(this.currentTime);
        this.renderStory();
    }

    renderDirectorMode() {
        this.backgroundGenerator.drawHeartBackground(this.currentTime);

        const centerX = this.width / 2;
        this.fontRenderer.drawText('导演模式', centerX - 120, 200, '#FFB6C1', 1.8);
        this.fontRenderer.drawText('即将上线！', centerX - 100, 300, '#FFFFFF', 1.5);

        // 避免重复添加按钮，只在没有元素时添加
        if (this.guiManager.elements.length === 0) {
            this.guiManager.addElement(new Button(
                centerX - 100, 400, 200, 60,
                '返回',
                '#DCDCDC',
                () => {
                    this.stateManager.setState(this.stateManager.states.MODE_SELECT);
                    this.setupModeSelectUI();
                }
            ));
        }
    }

    renderEndingGallery() {
        this.ctx.fillStyle = '#1A1A2E';
        this.ctx.fillRect(0, 0, this.width, this.height);

        const centerX = this.width / 2;
        this.fontRenderer.drawText('结局画廊', centerX - 120, 80, '#FFB6C1', 1.8);

        const progress = this.endingManager.getProgress();
        const progressText = `进度: ${progress.unlocked} / ${progress.total}`;
        this.fontRenderer.drawText(progressText, centerX - 120, 130, '#87CEEB', 1.2);
        
        const endings = this.endingManager.getAllEndings();
        let x = 50;
        let y = 180;
        
        endings.forEach((ending, index) => {
            const isUnlocked = this.endingManager.isUnlocked(ending.id);
            
            if (isUnlocked) {
                this.uiGenerator.drawHeartIcon(x + 20, y + 20, 15, true);
            } else {
                this.uiGenerator.drawHeartIcon(x + 20, y + 20, 15, false);
            }
            
            const title = isUnlocked ? ending.title : '???';
            this.fontRenderer.drawText(title, x + 50, y + 15, isUnlocked ? '#FFFFFF' : '#DCDCDC', 1);
            
            x += 240;
            if (x > this.width - 100) {
                x = 50;
                y += 80;
            }
        });
    }

    renderStory() {
        // 显示天数
        this.renderDayDisplay();
        
        const node = this.storyData.getStoryNode(this.gameData.currentStoryNode);
        if (!node) {
            this.gameData.currentStoryNode = 'start';
            return;
        }
        
        // 特殊节点处理
        if (this.gameData.currentStoryNode === 'calculate_ending') {
            this.calculateEnding();
            return;
        }
        
        const centerX = this.width / 2;
        
        this.uiGenerator.drawDialogBox(50, 450, this.width - 100, 200);
        
        // 自定义结局文本
        if (this.gameData.currentStoryNode === 'show_ending' && this.gameData.currentEndingText) {
            this.fontRenderer.drawTextWithWrap(this.gameData.currentEndingText, 80, 480, this.width - 160, '#FFB6C1', 1.2);
        } else {
            this.fontRenderer.drawTextWithWrap(node.text, 80, 480, this.width - 160, '#FFFFFF', 1.2);
        }
        
        // 更新按钮（只在节点变化时）
        this.updateStoryButtons();
        
        this.renderAffectionDisplay();
    }
    
    updateStoryButtons() {
        const node = this.storyData.getStoryNode(this.gameData.currentStoryNode);
        if (!node) return;
        
        // 检查按钮是否需要更新
        const currentNodeId = this.gameData.currentStoryNode;
        if (this.lastStoryNode !== currentNodeId) {
            this.lastStoryNode = currentNodeId;
            
            // 清除旧按钮，添加新按钮
            this.guiManager.clear();
            
            const centerX = this.width / 2;
            let buttonY = 550;
            
            node.choices.forEach((choice, index) => {
                this.guiManager.addElement(new Button(
                    centerX - 200, buttonY, 400, 50,
                    choice.text,
                    '#87CEEB',
                    () => this.handleChoice(choice)
                ));
                buttonY += 60;
            });
        }
    }

    renderDayDisplay() {
        let day = 1;
        const nodeId = this.gameData.currentStoryNode;
        
        if (nodeId.startsWith('day2') || nodeId === 'date_yangyang' || nodeId === 'date_lengleng' || 
            nodeId === 'date_momo' || nodeId === 'date_zhezhe' || nodeId === 'date_nuannuan' || 
            nodeId === 'date_yangyangdog' || nodeId === 'date_mimi' || nodeId === 'date_qingqing' || 
            nodeId === 'date_shuangshuang' || nodeId === 'date_nuanyang' || nodeId === 'date_yaya' || 
            nodeId === 'date_qingyun' || nodeId === 'day2_end') {
            day = 2;
        } else if (nodeId.startsWith('day3') || nodeId === 'final_choice' || 
                   nodeId === 'ending_check' || nodeId === 'calculate_ending' || nodeId === 'show_ending') {
            day = 3;
        }
        
        this.fontRenderer.drawText(`Day ${day}`, 50, 20, '#FFB6C1', 1.5);
    }

    handleChoice(choice) {
        if (choice.affection) {
            this.gameData.affection[choice.affection.guest] += choice.affection.value;
            if (choice.affection.guest2) {
                this.gameData.affection[choice.affection.guest2] += choice.affection.value;
            }
        }
        
        // 记录最终告白选择
        if (this.gameData.currentStoryNode === 'final_choice') {
            if (choice.affection) {
                this.gameData.finalChoice = choice.affection.guest;
            } else {
                this.gameData.finalChoice = 'single';
            }
        }
        
        if (choice.next === 'menu') {
            this.stateManager.setState(this.stateManager.states.MENU);
            this.setupMenuUI();
            this.resetGameData();
        } else {
            // 更新当前节点，lastStoryNode会被清空，确保按钮更新
            this.lastStoryNode = null; // 强制刷新按钮
            this.gameData.currentStoryNode = choice.next;
        }
    }

    calculateEnding() {
        let endingText = '';
        
        if (this.gameData.finalChoice === 'single') {
            this.endingManager.unlockEnding('single_elite');
            endingText = '单身贵族结局！你选择了专注于自己，在心动小屋收获了珍贵的友情！这30天真的很开心~';
        } else if (this.gameData.finalChoice) {
            const guestId = this.gameData.finalChoice;
            const guest = this.guestData.getGuestById(guestId);
            const affection = this.gameData.affection[guestId];
            const nickname = this.getGuestNickname(guestId);
            
            // 根据好感度解锁不同结局
            if (affection >= 80) {
                this.endingManager.unlockEnding(`love_${nickname}_1`);
                endingText = `❤️ 甜蜜结局！你和${nickname}最终走到了一起！\n好感度: ${affection}/100\n${guest.nickname}：\"和你在一起的时光，是我最珍贵的回忆！\"`;
            } else if (affection >= 50) {
                this.endingManager.unlockEnding(`love_${nickname}_2`);
                endingText = `💕 青涩结局！虽然有些害羞，但你和${nickname}确认了彼此的心意！\n好感度: ${affection}/100\n${guest.nickname}：\"...我也喜欢你\"`;
            } else {
                this.endingManager.unlockEnding(`love_${nickname}_3`);
                endingText = `💔 遗憾结局！有些话没有说出口，这或许是最好的结局...\n好感度: ${affection}/100\n${guest.nickname}：\"希望你能幸福\"`;
            }
        }
        
        // 解锁友情结局
        let totalAffection = 0;
        Object.values(this.gameData.affection).forEach(a => totalAffection += a);
        if (totalAffection >= 200) {
            this.endingManager.unlockEnding('friend_group');
        } else if (totalAffection >= 100) {
            this.endingManager.unlockEnding('friend_best');
        }
        
        this.gameData.currentEndingText = endingText;
        this.gameData.currentStoryNode = 'show_ending';
    }

    getGuestNickname(guestId) {
        const guest = this.guestData.getGuestById(guestId);
        return guest ? guest.nickname : 'unknown';
    }

    resetGameData() {
        this.gameData.currentStoryNode = 'start';
        this.gameData.finalChoice = null;
        this.guests.forEach(guest => {
            this.gameData.affection[guest.id] = 0;
        });
    }

    renderAffectionDisplay() {
        let x = 50;
        let y = 30;
        
        this.guests.slice(0, 6).forEach(guest => {
            this.renderAffectionBar(x, y, guest);
            x += 160;
        });
        
        x = 50;
        y = 80;
        
        this.guests.slice(6, 12).forEach(guest => {
            this.renderAffectionBar(x, y, guest);
            x += 160;
        });
    }

    renderAffectionBar(x, y, guest) {
        const affection = this.gameData.affection[guest.id];
        
        if (this.characterSprites[guest.id]) {
            this.ctx.drawImage(this.characterSprites[guest.id], x, y, 40, 40);
        }
        
        this.uiGenerator.drawAffectionBar(x + 50, y + 10, 100, 20, affection);
        
        const heartX = x + 50;
        const heartY = y + 10;
        const numHearts = Math.floor(affection / 20);
        for (let i = 0; i < 5; i++) {
            this.uiGenerator.drawHeartIcon(heartX + i * 22, heartY - 25, 12, i < numHearts);
        }
    }

    renderCharacterRow(startX, y, guestIds) {
        let x = startX;
        guestIds.forEach(id => {
            if (this.characterSprites[id]) {
                this.ctx.drawImage(this.characterSprites[id], x, y, 96, 96);
            }
            x += 136;
        });
    }

    gameLoop() {
        if (!this.running) return;
        
        const now = performance.now();
        this.deltaTime = (now - this.lastTime) / 1000;
        this.lastTime = now;
        
        this.update();
        this.render();
        
        requestAnimationFrame(() => this.gameLoop());
    }
}
