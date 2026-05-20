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
                    { text: '和墨墨熊打招呼', next: 'talk_momo_meet', affection: { guest: 3, value: 5 } },
                    { text: '和蜜蜜兔打招呼', next: 'talk_mimi', affection: { guest: 7, value: 5 } },
                    { text: '和晴晴猫打招呼', next: 'talk_qingqing', affection: { guest: 8, value: 5 } },
                    { text: '和暖暖羊打招呼', next: 'talk_nuanyang', affection: { guest: 10, value: 5 } }
                ]
            },
            talk_yangyang: {
                id: 'talk_yangyang',
                text: '阳阳鸭热情地朝你挥手："嗨！我是阳阳鸭！要不要一起去打篮球？"',
                choices: [
                    { text: '好呀！', next: 'activity_basketball', affection: { guest: 1, value: 10 } },
                    { text: '今天有点累...', next: 'free_time_day1', affection: { guest: 1, value: 2 } }
                ]
            },
            talk_lengleng: {
                id: 'talk_lengleng',
                text: '冷冷喵微微点头："你好。"（虽然表情冷淡，但你注意到他耳朵红了）',
                choices: [
                    { text: '你好~', next: 'talk_lengleng_2', affection: { guest: 2, value: 8 } },
                    { text: '（安静地坐下）', next: 'free_time_day1', affection: { guest: 2, value: 5 } }
                ]
            },
            talk_lengleng_2: {
                id: 'talk_lengleng_2',
                text: '（似乎放松了一点）"...要喝点什么吗？我带了一些不错的茶。"',
                choices: [
                    { text: '好呀，谢谢！', next: 'free_time_day1', affection: { guest: 2, value: 12 } },
                    { text: '不用麻烦了', next: 'free_time_day1', affection: { guest: 2, value: 5 } }
                ]
            },
            talk_momo_meet: {
                id: 'talk_momo_meet',
                text: '墨墨熊正坐在角落画画，听到声音抬头看你："啊...你好..."',
                choices: [
                    { text: '在画什么呢？', next: 'talk_momo_art', affection: { guest: 3, value: 10 } },
                    { text: '你好呀~', next: 'free_time_day1', affection: { guest: 3, value: 5 } }
                ]
            },
            talk_momo_art: {
                id: 'talk_momo_art',
                text: '墨墨熊脸微红："只是...随便画画花园的风景而已..."',
                choices: [
                    { text: '画得很漂亮！', next: 'free_time_day1', affection: { guest: 3, value: 15 } },
                    { text: '可以教我吗？', next: 'free_time_day1', affection: { guest: 3, value: 12 } }
                ]
            },
            talk_mimi: {
                id: 'talk_mimi',
                text: '蜜蜜兔蹦蹦跳跳地跑过来："哇！你终于来啦！我们一起去拍照吧~"',
                choices: [
                    { text: '好呀！', next: 'activity_photo', affection: { guest: 7, value: 10 } },
                    { text: '先休息一下', next: 'free_time_day1', affection: { guest: 7, value: 3 } }
                ]
            },
            talk_qingqing: {
                id: 'talk_qingqing',
                text: '晴晴猫优雅地微笑："你好，我是晴晴猫。很高兴认识你。"',
                choices: [
                    { text: '很高兴认识你！', next: 'talk_qingqing_2', affection: { guest: 8, value: 8 } },
                    { text: '你真漂亮', next: 'free_time_day1', affection: { guest: 8, value: 12 } }
                ]
            },
            talk_qingqing_2: {
                id: 'talk_qingqing_2',
                text: '（轻笑）"你嘴真甜~ 晚上有个欢迎派对，要来吗？"',
                choices: [
                    { text: '当然！', next: 'free_time_day1', affection: { guest: 8, value: 10 } }
                ]
            },
            talk_nuanyang: {
                id: 'talk_nuanyang',
                text: '暖暖羊温柔地笑着："你好~ 我是暖暖羊。要来点小点心吗？"',
                choices: [
                    { text: '好呀，谢谢！', next: 'free_time_day1', affection: { guest: 10, value: 10 } },
                    { text: '你太客气了', next: 'free_time_day1', affection: { guest: 10, value: 5 } }
                ]
            },
            activity_basketball: {
                id: 'activity_basketball',
                text: '你们一起打了篮球，阳阳鸭特别开心！"太棒了！你打得真好！下次我们再一起玩吧！"',
                choices: [
                    { text: '继续探索', next: 'free_time_day1' }
                ]
            },
            activity_photo: {
                id: 'activity_photo',
                text: '你们拍了好多有趣的照片，蜜蜜兔特别满意！"这张最好看！我们要永远记住今天！"',
                choices: [
                    { text: '继续探索', next: 'free_time_day1' }
                ]
            },
            free_time_day1: {
                id: 'free_time_day1',
                text: '现在是自由活动时间，你想做什么？',
                choices: [
                    { text: '在客厅聊天', next: 'living_room_day1' },
                    { text: '去花园散步', next: 'garden_day1' },
                    { text: '去厨房看看', next: 'kitchen_day1' },
                    { text: '和哲哲兔聊聊', next: 'talk_zhezhe', affection: { guest: 4, value: 5 } },
                    { text: '回房间休息', next: 'day1_end' }
                ]
            },
            living_room_day1: {
                id: 'living_room_day1',
                text: '客厅里扬扬狗正在弹吉他，霜霜狼和雅雅鹿在旁边聊天~',
                choices: [
                    { text: '听扬扬狗弹吉他', next: 'talk_yangyangdog', affection: { guest: 6, value: 8 } },
                    { text: '加入霜霜狼和雅雅鹿', next: 'talk_group', affection: { guest: 9, value: 5 } },
                    { text: '去别处看看', next: 'free_time_day1' }
                ]
            },
            talk_yangyangdog: {
                id: 'talk_yangyangdog',
                text: '扬扬狗停下来："嘿！要来听听我写的新歌吗？"',
                choices: [
                    { text: '好啊！', next: 'free_time_day1', affection: { guest: 6, value: 15 } },
                    { text: '下次吧', next: 'free_time_day1', affection: { guest: 6, value: 5 } }
                ]
            },
            talk_group: {
                id: 'talk_group',
                text: '霜霜狼和雅雅鹿正在讨论摄影和书籍，看到你来很开心~',
                choices: [
                    { text: '你们在聊什么？', next: 'free_time_day1', affection: { guest: 9, value: 8, guest2: 11, value: 8 } },
                    { text: '我先逛逛', next: 'free_time_day1' }
                ]
            },
            garden_day1: {
                id: 'garden_day1',
                text: '花园很漂亮，墨墨熊和轻轻云在这里~',
                choices: [
                    { text: '过去看看', next: 'talk_momo_qingyun', affection: { guest: 3, value: 5, guest2: 12, value: 5 } },
                    { text: '静静地欣赏', next: 'free_time_day1' }
                ]
            },
            talk_momo_qingyun: {
                id: 'talk_momo_qingyun',
                text: '轻轻云在冥想，墨墨熊在画画，画面很和谐~',
                choices: [
                    { text: '不打扰他们了', next: 'free_time_day1' },
                    { text: '和墨墨熊说话', next: 'talk_momo', affection: { guest: 3, value: 8 } }
                ]
            },
            talk_momo: {
                id: 'talk_momo',
                text: '墨墨熊有些害羞："...被你看到了，画得不好..."',
                choices: [
                    { text: '画得很棒！', next: 'free_time_day1', affection: { guest: 3, value: 15 } },
                    { text: '可以教我吗？', next: 'free_time_day1', affection: { guest: 3, value: 12 } }
                ]
            },
            kitchen_day1: {
                id: 'kitchen_day1',
                text: '暖暖猪正在厨房里准备晚餐，香气扑鼻~',
                choices: [
                    { text: '需要帮忙吗？', next: 'help_cook_day1', affection: { guest: 5, value: 10 } },
                    { text: '好香啊！', next: 'free_time_day1', affection: { guest: 5, value: 5 } }
                ]
            },
            help_cook_day1: {
                id: 'help_cook_day1',
                text: '暖暖猪开心地笑了："太好了！来帮我一起准备吧~ 大家一定会喜欢的！"',
                choices: [
                    { text: '好的！', next: 'free_time_day1', affection: { guest: 5, value: 15 } }
                ]
            },
            talk_zhezhe: {
                id: 'talk_zhezhe',
                text: '哲哲兔正抱着一本书："啊、你好！我在看科幻小说，要一起讨论吗？"',
                choices: [
                    { text: '好啊！', next: 'free_time_day1', affection: { guest: 4, value: 15 } },
                    { text: '我不太懂科幻', next: 'free_time_day1', affection: { guest: 4, value: 5 } }
                ]
            },
            day1_end: {
                id: 'day1_end',
                text: '第一天就这样结束了...你会选择给谁发心动短信呢？',
                choices: [
                    { text: '发给阳阳鸭', next: 'day2_start', affection: { guest: 1, value: 20 } },
                    { text: '发给冷冷喵', next: 'day2_start', affection: { guest: 2, value: 20 } },
                    { text: '发给墨墨熊', next: 'day2_start', affection: { guest: 3, value: 20 } },
                    { text: '发给哲哲兔', next: 'day2_start', affection: { guest: 4, value: 20 } },
                    { text: '发给暖暖猪', next: 'day2_start', affection: { guest: 5, value: 20 } },
                    { text: '发给扬扬狗', next: 'day2_start', affection: { guest: 6, value: 20 } },
                    { text: '发给蜜蜜兔', next: 'day2_start', affection: { guest: 7, value: 20 } },
                    { text: '发给晴晴猫', next: 'day2_start', affection: { guest: 8, value: 20 } },
                    { text: '发给霜霜狼', next: 'day2_start', affection: { guest: 9, value: 20 } },
                    { text: '发给暖暖羊', next: 'day2_start', affection: { guest: 10, value: 20 } },
                    { text: '发给雅雅鹿', next: 'day2_start', affection: { guest: 11, value: 20 } },
                    { text: '发给轻轻云', next: 'day2_start', affection: { guest: 12, value: 20 } }
                ]
            },
            day2_start: {
                id: 'day2_start',
                text: '第二天！新的一天开始了！大家都收到了心动短信，气氛变得微妙起来~ 今天有个特别的约会活动！',
                choices: [
                    { text: '开始新的一天', next: 'day2_choice' }
                ]
            },
            day2_choice: {
                id: 'day2_choice',
                text: '你可以选择和一位嘉宾单独约会！你想和谁一起呢？',
                choices: [
                    { text: '和阳阳鸭去运动', next: 'date_yangyang', affection: { guest: 1, value: 10 } },
                    { text: '和冷冷喵去咖啡馆', next: 'date_lengleng', affection: { guest: 2, value: 10 } },
                    { text: '和墨墨熊去美术馆', next: 'date_momo', affection: { guest: 3, value: 10 } },
                    { text: '和哲哲兔去科技馆', next: 'date_zhezhe', affection: { guest: 4, value: 10 } },
                    { text: '和暖暖猪去美食街', next: 'date_nuannuan', affection: { guest: 5, value: 10 } },
                    { text: '和扬扬狗去音乐会', next: 'date_yangyangdog', affection: { guest: 6, value: 10 } },
                    { text: '和蜜蜜兔去游乐园', next: 'date_mimi', affection: { guest: 7, value: 10 } },
                    { text: '和晴晴猫去逛街', next: 'date_qingqing', affection: { guest: 8, value: 10 } },
                    { text: '和霜霜狼去摄影', next: 'date_shuangshuang', affection: { guest: 9, value: 10 } },
                    { text: '和暖暖羊去野餐', next: 'date_nuanyang', affection: { guest: 10, value: 10 } },
                    { text: '和雅雅鹿去书店', next: 'date_yaya', affection: { guest: 11, value: 10 } },
                    { text: '和轻轻云去禅修', next: 'date_qingyun', affection: { guest: 12, value: 10 } }
                ]
            },
            date_yangyang: {
                id: 'date_yangyang',
                text: '你们在运动场玩得很开心！阳阳鸭认真地看着你："和你在一起真的很快乐..."',
                choices: [
                    { text: '我也是！', next: 'day2_end', affection: { guest: 1, value: 25 } },
                    { text: '（脸红）', next: 'day2_end', affection: { guest: 1, value: 20 } }
                ]
            },
            date_lengleng: {
                id: 'date_lengleng',
                text: '在安静的咖啡馆里，冷冷喵终于开口："其实...我不常和人说这么多话..."',
                choices: [
                    { text: '我很开心能和你聊天', next: 'day2_end', affection: { guest: 2, value: 25 } },
                    { text: '你人很好', next: 'day2_end', affection: { guest: 2, value: 20 } }
                ]
            },
            date_momo: {
                id: 'date_momo',
                text: '在美术馆里，墨墨熊红着脸："这是我第一次和别人一起来..."',
                choices: [
                    { text: '那我很荣幸', next: 'day2_end', affection: { guest: 3, value: 25 } },
                    { text: '以后可以常来', next: 'day2_end', affection: { guest: 3, value: 20 } }
                ]
            },
            date_zhezhe: {
                id: 'date_zhezhe',
                text: '哲哲兔兴奋地给你讲解各种科技展品："你看这个！超酷的对吧！"',
                choices: [
                    { text: '你懂好多！', next: 'day2_end', affection: { guest: 4, value: 25 } },
                    { text: '真的很有趣', next: 'day2_end', affection: { guest: 4, value: 20 } }
                ]
            },
            date_nuannuan: {
                id: 'date_nuannuan',
                text: '暖暖猪喂你吃了一口甜点："怎么样！好吃吗？这是我最推荐的！"',
                choices: [
                    { text: '太好吃了！', next: 'day2_end', affection: { guest: 5, value: 25 } },
                    { text: '你推荐的都好', next: 'day2_end', affection: { guest: 5, value: 20 } }
                ]
            },
            date_yangyangdog: {
                id: 'date_yangyangdog',
                text: '扬扬狗认真地对你说："这首歌...是为今天写的..."',
                choices: [
                    { text: '我很喜欢', next: 'day2_end', affection: { guest: 6, value: 25 } },
                    { text: '你好有才华', next: 'day2_end', affection: { guest: 6, value: 20 } }
                ]
            },
            date_mimi: {
                id: 'date_mimi',
                text: '蜜蜜兔拉着你一起坐过山车，下来后她紧紧抓着你的手："太刺激了！"',
                choices: [
                    { text: '没事吧？', next: 'day2_end', affection: { guest: 7, value: 25 } },
                    { text: '再来一次？', next: 'day2_end', affection: { guest: 7, value: 20 } }
                ]
            },
            date_qingqing: {
                id: 'date_qingqing',
                text: '晴晴猫给你挑了一件衣服："这件一定适合你！去试试看~"',
                choices: [
                    { text: '听你的！', next: 'day2_end', affection: { guest: 8, value: 25 } },
                    { text: '你眼光真好', next: 'day2_end', affection: { guest: 8, value: 20 } }
                ]
            },
            date_shuangshuang: {
                id: 'date_shuangshuang',
                text: '霜霜狼专注地拍照："你看这张！光线正好..."',
                choices: [
                    { text: '拍得真好', next: 'day2_end', affection: { guest: 9, value: 25 } },
                    { text: '可以教我吗？', next: 'day2_end', affection: { guest: 9, value: 20 } }
                ]
            },
            date_nuanyang: {
                id: 'date_nuanyang',
                text: '暖暖羊温柔地给你倒茶："慢慢吃，别着急~"',
                choices: [
                    { text: '谢谢你', next: 'day2_end', affection: { guest: 10, value: 25 } },
                    { text: '你真好', next: 'day2_end', affection: { guest: 10, value: 20 } }
                ]
            },
            date_yaya: {
                id: 'date_yaya',
                text: '雅雅鹿给你推荐了一本书："这本书...我读了很多遍..."',
                choices: [
                    { text: '我会好好读的', next: 'day2_end', affection: { guest: 11, value: 25 } },
                    { text: '可以一起讨论', next: 'day2_end', affection: { guest: 11, value: 20 } }
                ]
            },
            date_qingyun: {
                id: 'date_qingyun',
                text: '轻轻云轻声说："内心平静，才能看到真正的风景..."',
                choices: [
                    { text: '我好像懂了', next: 'day2_end', affection: { guest: 12, value: 25 } },
                    { text: '和你在一起很安心', next: 'day2_end', affection: { guest: 12, value: 20 } }
                ]
            },
            day2_end: {
                id: 'day2_end',
                text: '约会结束了！今晚要再发一次心动短信~ 经过两天的相处，你心里有答案了吗？',
                choices: [
                    { text: '发给阳阳鸭', next: 'day3_start', affection: { guest: 1, value: 20 } },
                    { text: '发给冷冷喵', next: 'day3_start', affection: { guest: 2, value: 20 } },
                    { text: '发给墨墨熊', next: 'day3_start', affection: { guest: 3, value: 20 } },
                    { text: '发给哲哲兔', next: 'day3_start', affection: { guest: 4, value: 20 } },
                    { text: '发给暖暖猪', next: 'day3_start', affection: { guest: 5, value: 20 } },
                    { text: '发给扬扬狗', next: 'day3_start', affection: { guest: 6, value: 20 } },
                    { text: '发给蜜蜜兔', next: 'day3_start', affection: { guest: 7, value: 20 } },
                    { text: '发给晴晴猫', next: 'day3_start', affection: { guest: 8, value: 20 } },
                    { text: '发给霜霜狼', next: 'day3_start', affection: { guest: 9, value: 20 } },
                    { text: '发给暖暖羊', next: 'day3_start', affection: { guest: 10, value: 20 } },
                    { text: '发给雅雅鹿', next: 'day3_start', affection: { guest: 11, value: 20 } },
                    { text: '发给轻轻云', next: 'day3_start', affection: { guest: 12, value: 20 } }
                ]
            },
            day3_start: {
                id: 'day3_start',
                text: '最后一天！这是最后的机会了！今晚的告白会上，你要做出最终选择！',
                choices: [
                    { text: '迎接最终章', next: 'final_choice' }
                ]
            },
            final_choice: {
                id: 'final_choice',
                text: '最终的告白时刻！你的心属于谁？选择你想要表白的对象！',
                choices: [
                    { text: '向阳阳鸭告白', next: 'ending_check', affection: { guest: 1, value: 30 } },
                    { text: '向冷冷喵告白', next: 'ending_check', affection: { guest: 2, value: 30 } },
                    { text: '向墨墨熊告白', next: 'ending_check', affection: { guest: 3, value: 30 } },
                    { text: '向哲哲兔告白', next: 'ending_check', affection: { guest: 4, value: 30 } },
                    { text: '向暖暖猪告白', next: 'ending_check', affection: { guest: 5, value: 30 } },
                    { text: '向扬扬狗告白', next: 'ending_check', affection: { guest: 6, value: 30 } },
                    { text: '向蜜蜜兔告白', next: 'ending_check', affection: { guest: 7, value: 30 } },
                    { text: '向晴晴猫告白', next: 'ending_check', affection: { guest: 8, value: 30 } },
                    { text: '向霜霜狼告白', next: 'ending_check', affection: { guest: 9, value: 30 } },
                    { text: '向暖暖羊告白', next: 'ending_check', affection: { guest: 10, value: 30 } },
                    { text: '向雅雅鹿告白', next: 'ending_check', affection: { guest: 11, value: 30 } },
                    { text: '向轻轻云告白', next: 'ending_check', affection: { guest: 12, value: 30 } },
                    { text: '选择单身', next: 'ending_single' }
                ]
            },
            ending_check: {
                id: 'ending_check',
                text: '让我们看看你的心意能否传达...',
                choices: [
                    { text: '查看结局', next: 'calculate_ending' }
                ]
            },
            ending_single: {
                id: 'ending_single',
                text: '单身贵族结局！你选择了专注于自己，在心动小屋收获了珍贵的友情！这30天真的很开心~',
                choices: [
                    { text: '返回主菜单', next: 'menu' }
                ]
            },
            calculate_ending: {
                id: 'calculate_ending',
                text: '正在计算你的结局...',
                choices: [
                    { text: '...', next: 'show_ending' }
                ]
            },
            show_ending: {
                id: 'show_ending',
                text: '恭喜你通关了！你达成了属于自己的心动结局！更多结局等待你去探索~',
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
