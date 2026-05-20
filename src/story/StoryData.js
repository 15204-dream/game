export default class StoryData {
    constructor() {
        this.storyNodes = this.initStoryNodes();
    }

    initStoryNodes() {
        return {
            start: {
                id: 'start',
                text: '欢迎来到《糟糕！是心动鸭！》恋综！你准备好开始这段心动之旅了吗？',
                choices: [
                    { text: '准备好了！', next: 'intro_1' },
                    { text: '等一下...', next: 'intro_2' }
                ]
            },
            intro_1: {
                id: 'intro_1',
                text: '太棒了！让我们一起入住心动小屋吧~',
                choices: [
                    { text: '进入小屋', next: 'meet_guests' }
                ]
            },
            intro_2: {
                id: 'intro_2',
                text: '别紧张！这会是一段美好的旅程~',
                choices: [
                    { text: '那好吧', next: 'meet_guests' }
                ]
            },
            meet_guests: {
                id: 'meet_guests',
                text: '心动小屋里已经有几位嘉宾到了，你想先和谁打招呼？',
                choices: [
                    { text: '和阳阳鸭打招呼', next: 'talk_yangyang', affection: { guest: 1, value: 5 } },
                    { text: '和冷冷喵打招呼', next: 'talk_lengleng', affection: { guest: 2, value: 5 } },
                    { text: '和蜜蜜兔打招呼', next: 'talk_mimi', affection: { guest: 7, value: 5 } },
                    { text: '和晴晴猫打招呼', next: 'talk_qingqing', affection: { guest: 8, value: 5 } }
                ]
            },
            talk_yangyang: {
                id: 'talk_yangyang',
                text: '阳阳鸭热情地朝你挥手："嗨！我是阳阳鸭！要不要一起去打篮球？"',
                choices: [
                    { text: '好呀！', next: 'activity_basketball', affection: { guest: 1, value: 10 } },
                    { text: '今天有点累...', next: 'free_time', affection: { guest: 1, value: 2 } }
                ]
            },
            talk_lengleng: {
                id: 'talk_lengleng',
                text: '冷冷喵微微点头："你好。"（虽然表情冷淡，但你注意到他耳朵红了）',
                choices: [
                    { text: '你好~', next: 'talk_lengleng_2', affection: { guest: 2, value: 8 } },
                    { text: '（安静地坐下）', next: 'free_time', affection: { guest: 2, value: 5 } }
                ]
            },
            talk_lengleng_2: {
                id: 'talk_lengleng_2',
                text: '（似乎放松了一点）"...要喝点什么吗？"',
                choices: [
                    { text: '好呀，谢谢！', next: 'free_time', affection: { guest: 2, value: 10 } }
                ]
            },
            talk_mimi: {
                id: 'talk_mimi',
                text: '蜜蜜兔蹦蹦跳跳地跑过来："哇！你终于来啦！我们一起去拍照吧~"',
                choices: [
                    { text: '好呀！', next: 'activity_photo', affection: { guest: 7, value: 10 } },
                    { text: '先休息一下', next: 'free_time', affection: { guest: 7, value: 3 } }
                ]
            },
            talk_qingqing: {
                id: 'talk_qingqing',
                text: '晴晴猫优雅地微笑："你好，我是晴晴猫。很高兴认识你。"',
                choices: [
                    { text: '很高兴认识你！', next: 'talk_qingqing_2', affection: { guest: 8, value: 8 } },
                    { text: '你真漂亮', next: 'free_time', affection: { guest: 8, value: 12 } }
                ]
            },
            talk_qingqing_2: {
                id: 'talk_qingqing_2',
                text: '（轻笑）"你嘴真甜~ 晚上有个欢迎派对，要来吗？"',
                choices: [
                    { text: '当然！', next: 'free_time', affection: { guest: 8, value: 10 } }
                ]
            },
            activity_basketball: {
                id: 'activity_basketball',
                text: '你们一起打了篮球，阳阳鸭特别开心！',
                choices: [
                    { text: '继续探索', next: 'free_time' }
                ]
            },
            activity_photo: {
                id: 'activity_photo',
                text: '你们拍了好多有趣的照片，蜜蜜兔特别满意！',
                choices: [
                    { text: '继续探索', next: 'free_time' }
                ]
            },
            free_time: {
                id: 'free_time',
                text: '现在是自由活动时间，你想做什么？',
                choices: [
                    { text: '在客厅聊天', next: 'living_room' },
                    { text: '去花园散步', next: 'garden' },
                    { text: '去厨房看看', next: 'kitchen' },
                    { text: '回房间休息', next: 'bedroom' }
                ]
            },
            living_room: {
                id: 'living_room',
                text: '客厅里有几位嘉宾在聊天，氛围很轻松~',
                choices: [
                    { text: '加入聊天', next: 'free_time' },
                    { text: '去别处看看', next: 'free_time' }
                ]
            },
            garden: {
                id: 'garden',
                text: '花园很漂亮，墨墨熊正在这里画画...',
                choices: [
                    { text: '过去看看', next: 'talk_momo', affection: { guest: 3, value: 8 } },
                    { text: '静静地欣赏', next: 'free_time', affection: { guest: 3, value: 5 } }
                ]
            },
            talk_momo: {
                id: 'talk_momo',
                text: '墨墨熊有些害羞："...被你看到了，画得不好..."',
                choices: [
                    { text: '画得很棒！', next: 'free_time', affection: { guest: 3, value: 15 } },
                    { text: '可以教我吗？', next: 'free_time', affection: { guest: 3, value: 12 } }
                ]
            },
            kitchen: {
                id: 'kitchen',
                text: '暖暖猪正在厨房里准备晚餐，香气扑鼻~',
                choices: [
                    { text: '需要帮忙吗？', next: 'help_cook', affection: { guest: 5, value: 10 } },
                    { text: '好香啊！', next: 'free_time', affection: { guest: 5, value: 5 } }
                ]
            },
            help_cook: {
                id: 'help_cook',
                text: '暖暖猪开心地笑了："太好了！来帮我一起准备吧~"',
                choices: [
                    { text: '好的！', next: 'free_time', affection: { guest: 5, value: 15 } }
                ]
            },
            bedroom: {
                id: 'bedroom',
                text: '你回到房间休息，明天会是新的一天！',
                choices: [
                    { text: '结束第一天', next: 'day1_end' }
                ]
            },
            day1_end: {
                id: 'day1_end',
                text: '第一天就这样结束了...你会选择给谁发心动短信呢？',
                choices: [
                    { text: '发给阳阳鸭', next: 'ending_preview', affection: { guest: 1, value: 20 } },
                    { text: '发给冷冷喵', next: 'ending_preview', affection: { guest: 2, value: 20 } },
                    { text: '发给墨墨熊', next: 'ending_preview', affection: { guest: 3, value: 20 } },
                    { text: '发给蜜蜜兔', next: 'ending_preview', affection: { guest: 7, value: 20 } },
                    { text: '发给晴晴猫', next: 'ending_preview', affection: { guest: 8, value: 20 } }
                ]
            },
            ending_preview: {
                id: 'ending_preview',
                text: '心动短信已发送！（这只是试玩版本，完整版有更多剧情和50+结局哦~）',
                choices: [
                    { text: '返回主菜单', next: 'menu' }
                ]
            }
        };
    }

    getStoryNode(nodeId) {
        return this.storyNodes[nodeId];
    }

    getAllNodes() {
        return this.storyNodes;
    }
}
