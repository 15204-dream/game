class AIDialogueEngine {
    constructor() {
        this.apiKey = 'sk-fe8fdf06158c44b5b6304a8e30dce5a8';
        this.apiBaseUrl = 'https://api.deepseek.com/v1/chat/completions';
        this.conversationHistory = {};
        this.isGenerating = false;
    }
    
    getSystemPrompt(guest) {
        return `你是《糟糕！是心动鸭！》恋综游戏中的角色：${guest.nickname}。
你的真实姓名是${guest.realName}，今年${guest.age}岁，职业是${guest.occupation}。

你的性格特点：${guest.personality.join('、')}。
你喜欢的事物：${guest.likes.join('、')}。
你不喜欢的事物：${guest.dislikes.join('、')}。
你的背景故事：${guest.background}。
你的秘密：${guest.secret}（不要轻易告诉别人）。

请完全沉浸在这个角色中，用${guest.nickname}的身份和语气进行对话。
回复要自然、口语化，符合角色性格，不要太书面化。
回复长度控制在2-4句话。
不要暴露你是AI的身份！

当前你正在参加一个名为《糟糕！是心动鸭！》的恋爱综艺节目，你和其他嘉宾一起在别墅里生活30天，寻找真爱。`;
    }
    
    async generateResponse(guest, userMessage, conversationId = 'default') {
        if (this.isGenerating) {
            return '请稍等，我正在思考...';
        }
        
        this.isGenerating = true;
        
        if (!this.conversationHistory[conversationId]) {
            this.conversationHistory[conversationId] = [];
        }
        
        this.conversationHistory[conversationId].push({
            role: 'user',
            content: userMessage
        });
        
        const messages = [
            {
                role: 'system',
                content: this.getSystemPrompt(guest)
            },
            ...this.conversationHistory[conversationId].slice(-10)
        ];
        
        try {
            const response = await fetch(this.apiBaseUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.apiKey}`
                },
                body: JSON.stringify({
                    model: 'deepseek-chat',
                    messages: messages,
                    temperature: 0.8,
                    max_tokens: 500,
                    top_p: 0.9
                })
            });
            
            if (!response.ok) {
                throw new Error(`API请求失败: ${response.status}`);
            }
            
            const data = await response.json();
            const aiResponse = data.choices[0].message.content.trim();
            
            this.conversationHistory[conversationId].push({
                role: 'assistant',
                content: aiResponse
            });
            
            this.isGenerating = false;
            return aiResponse;
            
        } catch (error) {
            console.error('AI对话生成失败:', error);
            this.isGenerating = false;
            return this.getFallbackResponse(guest);
        }
    }
    
    getFallbackResponse(guest) {
        const responses = [
            `嗯...这个问题让我想想...`,
            `哈哈，你真有趣！`,
            `这个话题有点突然呢...`,
            `能和你聊天真开心！`,
            `让我想想该怎么说...`,
            `你觉得呢？`,
            `哇，这个问题很有意思！`,
            `我也很好奇你的想法~`
        ];
        return responses[Math.floor(Math.random() * responses.length)];
    }
    
    async generateDialogueOptions(guest, currentContext) {
        const options = [
            `聊一聊今天的天气`,
            `问一下${guest.nickname}的兴趣爱好`,
            `分享一下自己的经历`,
            `赞美一下${guest.nickname}`
        ];
        return options;
    }
    
    resetConversation(conversationId = 'default') {
        this.conversationHistory[conversationId] = [];
    }
    
    getConversationHistory(conversationId = 'default') {
        return this.conversationHistory[conversationId] || [];
    }
}

export default AIDialogueEngine;
