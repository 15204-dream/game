class InputManager {
    constructor(canvas) {
        this.canvas = canvas;
        this.keys = {};
        this.mouse = { x: 0, y: 0, clicked: false, down: false };
        this.justClicked = false;
        
        this.init();
    }
    
    init() {
        window.addEventListener('keydown', (e) => {
            this.keys[e.code] = true;
        });
        
        window.addEventListener('keyup', (e) => {
            this.keys[e.code] = false;
        });
        
        this.canvas.addEventListener('mousemove', (e) => {
            const rect = this.canvas.getBoundingClientRect();
            this.mouse.x = e.clientX - rect.left;
            this.mouse.y = e.clientY - rect.top;
        });
        
        this.canvas.addEventListener('mousedown', (e) => {
            this.mouse.down = true;
            this.justClicked = true;
        });
        
        this.canvas.addEventListener('mouseup', (e) => {
            this.mouse.down = false;
            this.mouse.clicked = false;
        });
        
        this.canvas.addEventListener('click', (e) => {
            this.mouse.clicked = true;
        });
    }
    
    update() {
        this.justClicked = false;
    }
    
    isKeyPressed(code) {
        return this.keys[code] || false;
    }
    
    isMouseDown() {
        return this.mouse.down;
    }
    
    isMouseClicked() {
        return this.mouse.clicked;
    }
    
    isJustClicked() {
        return this.justClicked;
    }
    
    getMousePosition() {
        return { x: this.mouse.x, y: this.mouse.y };
    }
    
    isPointInRect(px, py, rx, ry, rw, rh) {
        return px >= rx && px <= rx + rw && py >= ry && py <= ry + rh;
    }
}

export default InputManager;
