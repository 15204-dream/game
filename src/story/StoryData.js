const StoryData = {
    startNode: 'intro',
    nodes: {
        intro: {
            text: '欢迎来到《糟糕！是心动鸭！》恋综节目！\n\n你是第13位嘉宾，今天是你入住心动小屋的第一天。\n\n你的行李刚放下，就听到了门铃声...',
            speaker: '旁白',
            choices: [
                { text: '马上去开门', nextNode: 'door_1', flag: 'opened_door_first' },
                { text: '先整理一下再去开门', nextNode: 'door_2' },
                { text: '假装没听见', nextNode: 'door_3' }
            ]
        },
        door_1: {
            text: '你快步走向门口，一把打开了门！\n\n门外站着的是阳阳鸭，他手里捧着一个大箱子，看起来是刚到。\n\n"嗨！你就是新来的嘉宾吧？我是阳阳，多多关照！"',
            speaker: '旁白',
            choices: [
                { text: '热情地打招呼', nextNode: 'yangyang_positive', flag: 'yangyang_good_impression' },
                { text: '腼腆地点头', nextNode: 'yangyang_neutral' },
                { text: '帮他拿箱子', nextNode: 'yangyang_helpful', flag: 'yangyang_helped' }
            ]
        },
        door_2: {
            text: '你不紧不慢地整理了一下头发和衣服，然后才去开门。\n\n门外是冷冷喵，她靠在门边，似乎已经等了一会儿。\n\n"...你好。"她淡淡地说。',
            speaker: '旁白',
            choices: [
                { text: '抱歉让你久等了', nextNode: 'lengleng_apology', flag: 'lengleng_polite' },
                { text: '你好，找我有事吗？', nextNode: 'lengleng_neutral' },
                { text: '哇，你好漂亮！', nextNode: 'lengleng_flatter' }
            ]
        },
        door_3: {
            text: '你决定假装没听见，继续整理行李。\n\n过了一会儿，门铃声停了。\n\n又过了几分钟，你听到了敲门声...',
            speaker: '旁白',
            choices: [
                { text: '去开门', nextNode: 'door_1' },
                { text: '继续假装', nextNode: 'no_guest' }
            ]
        },
        yangyang_positive: {
            text: '阳阳鸭被你的热情感染，露出了灿烂的笑容！\n\n"哈哈，你真活泼！走，我带你去认识一下其他人！"',
            speaker: '阳阳鸭',
            affectionGain: { yangyang: 8 },
            choices: [
                { text: '好呀，走吧！', nextNode: 'living_room' }
            ]
        },
        yangyang_neutral: {
            text: '阳阳鸭笑了笑，"嗯，看来你有点害羞呢。没关系，慢慢熟悉就好。"',
            speaker: '阳阳鸭',
            affectionGain: { yangyang: 3 },
            choices: [
                { text: '我们去认识其他人吧', nextNode: 'living_room' }
            ]
        },
        yangyang_helpful: {
            text: '你主动接过了阳阳鸭手中的箱子！\n\n"哇，谢谢！你真是太好了！"他看起来很惊喜。',
            speaker: '阳阳鸭',
            affectionGain: { yangyang: 12 },
            choices: [
                { text: '不客气，走吧', nextNode: 'living_room' }
            ]
        },
        lengleng_apology: {
            text: '冷冷喵的表情稍微缓和了一点。\n\n"没关系，我也刚到不久。"',
            speaker: '冷冷喵',
            affectionGain: { lengleng: 5 },
            choices: [
                { text: '进去坐吧', nextNode: 'living_room' }
            ]
        },
        lengleng_neutral: {
            text: '冷冷喵点了点头，"嗯，我是住在隔壁房间的嘉宾。"',
            speaker: '冷冷喵',
            affectionGain: { lengleng: 2 },
            choices: [
                { text: '那进来聊聊吧', nextNode: 'living_room' }
            ]
        },
        lengleng_flatter: {
            text: '冷冷喵脸颊微红，转过头去。\n\n"...谢谢。"她的声音小了一些。',
            speaker: '冷冷喵',
            affectionGain: { lengleng: 8 },
            choices: [
                { text: '快进来吧', nextNode: 'living_room' }
            ]
        },
        no_guest: {
            text: '敲门声也停了。\n\n你从猫眼往外看，发现是墨墨熊正把一个包裹放在你门口。\n\n包裹上有张便签："欢迎来到心动小屋~"',
            speaker: '旁白',
            choices: [
                { text: '开门去追他', nextNode: 'momo_chase' },
                { text: '先打开包裹看看', nextNode: 'momo_package' }
            ]
        },
        momo_chase: {
            text: '你赶紧打开门追了出去！\n\n墨墨熊没走远，听到声音转过身来。\n\n"啊，你好！抱歉刚才打扰你了..."',
            speaker: '墨墨熊',
            affectionGain: { momo: 5 },
            choices: [
                { text: '没关系，谢谢你的包裹', nextNode: 'living_room' }
            ]
        },
        momo_package: {
            text: '你打开包裹，里面是一些小零食和一张手写的卡片。\n\n卡片上字迹工整："不知道你喜欢什么，准备了点小零食。"',
            speaker: '旁白',
            choices: [
                { text: '去找墨墨熊道谢', nextNode: 'living_room' }
            ]
        },
        living_room: {
            text: '你来到客厅，发现其他嘉宾已经到了不少！\n\n暖暖的小猪玩偶坐在沙发上，蜜蜜兔在自拍，暖阳羊在调试相机...',
            speaker: '旁白',
            choices: [
                { text: '向大家打招呼', nextNode: 'day1_evening' }
            ]
        },
        day1_evening: {
            text: '第一天就这样过去了。\n\n虽然发生了很多事，但你心里有一点点期待...\n\n明天会发生什么呢？',
            speaker: '旁白',
            choices: [
                { text: '进入第二天', nextNode: 'day2_morning' }
            ]
        },
        day2_morning: {
            text: '第二天清晨，阳光透过窗户照进房间...',
            speaker: '旁白',
            choices: [
                { text: '继续剧情', nextNode: 'end_demo' }
            ]
        },
        end_demo: {
            text: '感谢试玩《糟糕！是心动鸭！》！\n\n更多精彩内容，敬请期待完整版！',
            speaker: '旁白',
            choices: [
                { text: '返回主菜单', nextNode: 'back_to_menu' }
            ]
        },
        back_to_menu: {
            text: '',
            speaker: '',
            choices: [],
            isEnd: true
        }
    }
};

export default StoryData;
