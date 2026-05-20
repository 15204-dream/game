class GuestData {
    constructor() {
        this.guests = [];
        this.loaded = false;
    }
    
    async load() {
        this.guests = [
            {
                id: 'yangyang',
                nickname: '阳阳鸭',
                realName: '林阳',
                gender: 'male',
                age: 26,
                occupation: '创业公司CEO',
                personality: ['阳光', '自信', '有领导力', '偶尔霸道'],
                likes: ['篮球', '黑咖啡', '挑战', '效率'],
                dislikes: ['拖延症', '虚伪', '胡萝卜'],
                background: '白手起家的年轻企业家，有过一段失败的恋情',
                secret: '其实很怕黑，晚上睡觉要留一盏小夜灯',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'lengleng',
                nickname: '冷冷喵',
                realName: '苏冷',
                gender: 'female',
                age: 24,
                occupation: '悬疑小说作家',
                personality: ['高冷', '神秘', '观察力强', '内心柔软'],
                likes: ['雨天', '红茶', '悬疑电影', '猫'],
                dislikes: ['喧闹', '八卦', '香菜'],
                background: '年少成名的作家，很少在公众面前露面',
                secret: '写的爱情故事都是基于自己的幻想',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'momo',
                nickname: '墨墨熊',
                realName: '陈墨',
                gender: 'male',
                age: 28,
                occupation: '脑外科医生',
                personality: ['温柔', '稳重', '细心', '有点完美主义'],
                likes: ['古典音乐', '烹饪', '整洁', '游泳'],
                dislikes: ['粗心大意', '快餐', '血腥电影'],
                background: '医学世家出身，对自己要求极高',
                secret: '晕血，但靠意志力克服了',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'zhezhe',
                nickname: '哲哲兔',
                realName: '李哲',
                gender: 'male',
                age: 22,
                occupation: '音乐学院学生',
                personality: ['害羞', '温柔', '有才华', '敏感'],
                likes: ['钢琴', '向日葵', '牛奶', '安静'],
                dislikes: ['争吵', '恐怖的东西', '被关注'],
                background: '音乐神童，有社交焦虑',
                secret: '一直在偷偷写情歌',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'nuannuan',
                nickname: '暖暖猪',
                realName: '温暖',
                gender: 'female',
                age: 25,
                occupation: '幼儿园老师',
                personality: ['温柔', '善良', '有耐心', '偶尔迷糊'],
                likes: ['小孩子', '甜点', '毛绒玩具', '粉色'],
                dislikes: ['暴力', '被讨厌', '苦味'],
                background: '在孤儿院长大，想给别人温暖',
                secret: '怕黑，不敢一个人睡',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'yangyang2',
                nickname: '扬扬狗',
                realName: '张扬',
                gender: 'male',
                age: 27,
                occupation: '知名演员',
                personality: ['外向', '幽默', '体贴', '有点自恋'],
                likes: ['健身', '美食', '拍照', '热闹'],
                dislikes: ['被忽略', '寂寞', '肥肉'],
                background: '童星出身，在娱乐圈打拼多年',
                secret: '有严重的失眠症',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'mimi',
                nickname: '蜜蜜兔',
                realName: '唐蜜',
                gender: 'female',
                age: 23,
                occupation: '美食博主',
                personality: ['活泼', '开朗', '直率', '有点馋'],
                likes: ['美食', '旅游', '拍照', '交朋友'],
                dislikes: ['饿肚子', '难吃的食物', '下雨'],
                background: '富二代，但是想靠自己成功',
                secret: '不会做饭',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'qingqing',
                nickname: '晴晴猫',
                realName: '夏晴',
                gender: 'female',
                age: 27,
                occupation: '时尚杂志主编',
                personality: ['强势', '干练', '女王气场', '内心小女人'],
                likes: ['时尚', '红酒', '艺术品', '健身'],
                dislikes: ['土气', '没品位', '迟到'],
                background: '从底层打拼到主编位置',
                secret: '喜欢看少女漫画',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'shuangshuang',
                nickname: '霜霜狼',
                realName: '寒霜',
                gender: 'female',
                age: 29,
                occupation: '赛车手',
                personality: ['酷', '豪爽', '讲义气', '独立'],
                likes: ['速度', '机车', '啤酒', '冒险'],
                dislikes: ['磨磨唧唧', '规则', '懦弱'],
                background: '前职业赛车手，因为事故退役',
                secret: '其实很怕蜘蛛',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'nuanyang',
                nickname: '暖阳羊',
                realName: '何阳',
                gender: 'male',
                age: 30,
                occupation: '摄影师',
                personality: ['温和', '文艺', '有魅力', '有点神秘'],
                likes: ['摄影', '旅行', '咖啡', '日落'],
                dislikes: ['数码照片', '嘈杂', '虚伪'],
                background: '环游世界的摄影师，经历丰富',
                secret: '在寻找一个人',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'yaya',
                nickname: '雅雅鹿',
                realName: '孟雅',
                gender: 'female',
                age: 26,
                occupation: '考古学家',
                personality: ['知性', '认真', '博学', '路痴'],
                likes: ['历史', '文物', '读书', '花茶'],
                dislikes: ['破坏文物', '谎言', '嘈杂'],
                background: '考古世家，参与过重大发掘',
                secret: '相信有前世今生',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            },
            {
                id: 'qingqing2',
                nickname: '轻轻云',
                realName: '云轻',
                gender: 'male',
                age: 25,
                occupation: '茶艺师',
                personality: ['淡泊', '宁静', '温润', '飘逸'],
                likes: ['茶道', '书法', '古琴', '大自然'],
                dislikes: ['争执', '烟酒', '快餐'],
                background: '出身中医世家，精通茶道',
                secret: '是个隐藏的电竞高手',
                affection: 0,
                relationLevel: 0,
                unlocked: true
            }
        ];
        
        this.loaded = true;
        console.log('嘉宾数据加载完成！');
    }
    
    getGuests() {
        return this.guests;
    }
    
    getGuestById(id) {
        return this.guests.find(g => g.id === id);
    }
    
    getGuestsByGender(gender) {
        return this.guests.filter(g => g.gender === gender);
    }
    
    updateAffection(guestId, amount) {
        const guest = this.getGuestById(guestId);
        if (guest) {
            guest.affection = Math.max(0, Math.min(100, guest.affection + amount));
            this.updateRelationLevel(guestId);
        }
    }
    
    updateRelationLevel(guestId) {
        const guest = this.getGuestById(guestId);
        if (guest) {
            if (guest.affection >= 80) guest.relationLevel = 4;
            else if (guest.affection >= 60) guest.relationLevel = 3;
            else if (guest.affection >= 40) guest.relationLevel = 2;
            else if (guest.affection >= 20) guest.relationLevel = 1;
            else guest.relationLevel = 0;
        }
    }
    
    getRelationLevelName(level) {
        const names = ['陌生人', '认识', '朋友', '好感', '心动'];
        return names[level] || '陌生人';
    }
}

export default GuestData;
