export default class AIDialogueEngine {
    constructor() {
        this.apiKey = 'sk-fe8fdf06158c44b5b6304a8e30dce5a8';
        this.apiUrl = 'https://api.deepseek.com/v1/chat/completions';
        this.conversationHistory = [];
        this.fallbackDialogues = this.initFallbackDialogues();
    }

    initFallbackDialogues() {
        return {
            greetings: [
                '你好呀！很高兴认识你~',
                '嗨！今天过得怎么样？',
                '哈喽，看到你真开心！'
            ],
            casual: [
                '今天天气真好呢！',
                '你喜欢这里吗？',
                '你平时喜欢做什么？'
            ],
            positive: [
                '谢谢你的夸奖~',
                '和你聊天真开心！',
                '你真的很有趣呢！'
            ],
            shy: [
                '...谢谢你',
                '嗯...那个...',
                '（脸红）'
            ]
        };
    }

    generatePrompt(guest, context) {
        return `你现在在扮演恋爱综艺《糟糕！是心动鸭！》中的嘉宾【${guest.nickname}】。

嘉宾信息：
- 本名：${guest.realName}
- 年龄：${guest.age}岁
- 职业：${guest.occupation}
- 性格：${guest.personality}
- 背景：${guest.background}
- 喜好：${guest.likes}

当前场景：${context.scene}
与玩家的关系：${context.relationship}
好感度：${context.affection}/100

请根据以上信息，生成符合该嘉宾人设的对话。
要求：
1. 对话要自然流畅
2. 符合嘉宾性格
3. 考虑当前场景和关系
4. 用中文回复，不超过50字
5. 不要包含任何AI相关的说明`;
    }

    async generateDialogue(guest, context) {
        try {
            const prompt = this.generatePrompt(guest, context);
            
            const response = await fetch(this.apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.apiKey}`
                },
                body: JSON.stringify({
                    model: 'deepseek-chat',
                    messages: [
                        { role: 'system', content: prompt },
                        { role: 'user', content: context.lastPlayerMessage || '你好' }
                    ],
                    max_tokens: 100,
                    temperature: 0.8
                })
            });

            if (response.ok) {
                const data = await response.json();
                const dialogue = data.choices[0].message.content.trim();
                this.conversationHistory.push({ role: 'assistant', content: dialogue });
                return dialogue;
            } else {
                throw new Error('API request failed');
            }
        } catch (error) {
            console.log('使用备用对话:', error);
            return this.getFallbackDialogue(context);
        }
    }

    getFallbackDialogue(context) {
        const types = Object.keys(this.fallbackDialogues);
        const randomType = types[Math.floor(Math.random() * types.length)];
        const dialogues = this.fallbackDialogues[randomType];
        return dialogues[Math.floor(Math.random() * dialogues.length)];
    }

    addUserMessage(message) {
        this.conversationHistory.push({ role: 'user', content: message });
    }

    clearHistory() {
        this.conversationHistory = [];
    }
}
