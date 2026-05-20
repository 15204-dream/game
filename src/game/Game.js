import Renderer from './Renderer.js';
import StateManager from './StateManager.js';
import InputManager from './InputManager.js';
import GUIManager from '../ui/GUIManager.js';
import GuestData from '../data/GuestData.js';
import AIDialogueEngine from '../ai/AIDialogueEngine.js';
import StoryEngine from '../story/StoryEngine.js';
import StoryData from '../story/StoryData.js';
import ResourceManager from '../graphics/ResourceManager.js';
import PixelFontRenderer from '../graphics/PixelFontRenderer.js';
import CharacterSpriteGenerator from '../graphics/CharacterSpriteGenerator.js';
import BackgroundGenerator from '../graphics/BackgroundGenerator.js';
import UIElementGenerator from '../graphics/UIElementGenerator.js';
import EndingManager from '../ending/EndingManager.js';
import MenuState from '../states/MenuState.js';
import ModeSelectState from '../states/ModeSelectState.js';
import GuestModeState from '../states/GuestModeState.js';
import EndingGalleryState from '../states/EndingGalleryState.js';

class Game {
    constructor() {
        this.canvas = document.getElementById('game-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.loadingScreen = document.getElementById('loading-screen');
        
        this.width = 960;
        this.height = 640;
        
        this.isRunning = false;
        this.lastTime = 0;
        this.deltaTime = 0;
        
        this.renderer = null;
        this.stateManager = null;
        this.inputManager = null;
        this.guiManager = null;
        this.guestData = null;
        this.aiEngine = null;
        this.storyEngine = null;
        this.resourceManager = null;
        this.pixelFontRenderer = null;
        this.characterSpriteGenerator = null;
        this.backgroundGenerator = null;
        this.uiElementGenerator = null;
        this.endingManager = null;
        this.characterSprites = null;
        this.currentTime = 0;
    }
    
    async init() {
        console.log('游戏初始化中...');
        
        this.renderer = new Renderer(this.ctx, this.width, this.height);
        this.stateManager = new StateManager();
        this.inputManager = new InputManager(this.canvas);
        this.guiManager = new GUIManager(this.renderer, this.inputManager);
        this.guestData = new GuestData();
        this.aiEngine = new AIDialogueEngine();
        this.storyEngine = new StoryEngine();
        this.storyEngine.loadStory(StoryData);
        this.resourceManager = new ResourceManager();
        this.pixelFontRenderer = new PixelFontRenderer(this.ctx);
        this.characterSpriteGenerator = new CharacterSpriteGenerator(this.ctx);
        this.backgroundGenerator = new BackgroundGenerator(this.ctx, this.width, this.height);
        this.uiElementGenerator = new UIElementGenerator(this.ctx);
        this.endingManager = new EndingManager();
        
        await this.loadAssets();
        
        this.stateManager.registerState('menu', new MenuState(this));
        this.stateManager.registerState('modeSelect', new ModeSelectState(this));
        this.stateManager.registerState('guestMode', new GuestModeState(this));
        this.stateManager.registerState('endingGallery', new EndingGalleryState(this));
        
        this.stateManager.setState('menu');
        
        this.isRunning = true;
        this.hideLoadingScreen();
        
        console.log('游戏初始化完成！');
        
        this.lastTime = performance.now();
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    async loadAssets() {
        await new Promise(resolve => setTimeout(resolve, 1000));
        await this.guestData.load();
        this.characterSprites = this.characterSpriteGenerator.generateAllCharacters(this.guestData.getGuests(), 128);
    }
    
    hideLoadingScreen() {
        this.loadingScreen.style.display = 'none';
    }
    
    gameLoop(currentTime) {
        if (!this.isRunning) return;
        
        this.deltaTime = (currentTime - this.lastTime) / 1000;
        this.lastTime = currentTime;
        
        this.update(this.deltaTime);
        this.render();
        
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    update(dt) {
        this.currentTime += dt * 1000;
        this.stateManager.update(dt);
        this.guiManager.update(dt);
        this.inputManager.update();
    }
    
    render() {
        this.renderer.clear();
        
        const state = this.stateManager.getState();
        if (state === 'menu') {
            this.backgroundGenerator.drawHeartBackground(this.currentTime);
        } else if (state === 'guestMode') {
            this.backgroundGenerator.drawLivingRoomBackground(this.currentTime);
        } else {
            this.renderer.renderBackground();
        }
        
        this.stateManager.render(this.renderer);
        this.guiManager.render();
    }
}

export default Game;
