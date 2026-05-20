class StoryEngine {
    constructor() {
        this.currentNode = null;
        this.visitedNodes = [];
        this.choices = {};
        this.storyFlags = {};
    }
    
    loadStory(storyData) {
        this.storyData = storyData;
        this.currentNode = storyData.startNode;
    }
    
    getCurrentNode() {
        return this.storyData.nodes[this.currentNode];
    }
    
    makeChoice(choiceIndex) {
        const node = this.getCurrentNode();
        if (!node || !node.choices || choiceIndex >= node.choices.length) {
            return false;
        }
        
        const choice = node.choices[choiceIndex];
        this.choices[this.currentNode] = choiceIndex;
        this.visitedNodes.push(this.currentNode);
        
        if (choice.flag) {
            this.storyFlags[choice.flag] = true;
        }
        
        this.currentNode = choice.nextNode;
        return true;
    }
    
    setFlag(flag, value = true) {
        this.storyFlags[flag] = value;
    }
    
    getFlag(flag) {
        return this.storyFlags[flag] || false;
    }
    
    reset() {
        this.currentNode = this.storyData.startNode;
        this.visitedNodes = [];
        this.choices = {};
        this.storyFlags = {};
    }
    
    getChoicesMade() {
        return { ...this.choices };
    }
    
    getProgress() {
        return {
            currentNode: this.currentNode,
            visitedNodes: [...this.visitedNodes],
            flags: { ...this.storyFlags }
        };
    }
}

export default StoryEngine;
