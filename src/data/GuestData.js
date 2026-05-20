export default class GuestData {
    constructor() {
        this.guests = [
            {
                id: 1,
                nickname: '阳阳鸭',
                realName: '林阳',
                gender: 'male',
                age: 26,
                occupation: '体育老师/健身教练',
                personality: '热情开朗，爱运动，充满活力，有点直男但温柔',
                background: '从小热爱运动，大学时是校篮球队队长，现在在中学当体育老师',
                likes: '打篮球、健身、户外运动、做饭',
                secret: '其实很怕黑，晚上睡觉要开小夜灯',
                color: '#FF6B6B',
                accentColor: '#FFE66D'
            },
            {
                id: 2,
                nickname: '冷冷喵',
                realName: '顾冷',
                gender: 'male',
                age: 29,
                occupation: '科技公司CEO',
                personality: '外表冷酷，内心温柔，不善于表达情感，工作狂',
                background: '白手起家创立科技公司，年少有为但压力很大',
                likes: '工作、阅读、古典音乐、品酒',
                secret: '其实很喜欢看动画片，家里堆满了手办',
                color: '#4ECDC4',
                accentColor: '#95E1D3'
            },
            {
                id: 3,
                nickname: '墨墨熊',
                realName: '陈墨',
                gender: 'male',
                age: 27,
                occupation: '自由插画师',
                personality: '浪漫敏感，细腻温柔，有点内向但内心丰富',
                background: '美术学院毕业，作品在网上小有名气，喜欢到处旅行采风',
                likes: '画画、摄影、旅行、咖啡馆',
                secret: '其实有点社交恐惧，人多的时候会紧张',
                color: '#9B59B6',
                accentColor: '#D7BDE2'
            },
            {
                id: 4,
                nickname: '哲哲兔',
                realName: '王哲',
                gender: 'male',
                age: 25,
                occupation: '在读博士/科研工作者',
                personality: '理性思维，逻辑清晰，但生活技能满点，对感情一窍不通，呆萌可爱',
                background: '天才少年，一路跳级读到博士，在研究所工作',
                likes: '科研、编程、科幻小说、解谜游戏',
                secret: '恋爱经验为零，连女生的手都没牵过',
                color: '#3498DB',
                accentColor: '#85C1E9'
            },
            {
                id: 5,
                nickname: '暖暖猪',
                realName: '李温暖',
                gender: 'male',
                age: 28,
                occupation: '餐厅主厨',
                personality: '温柔体贴，治愈系，很会照顾人，脾气好',
                background: '出身餐饮世家，自己经营一家米其林推荐餐厅',
                likes: '烹饪、美食、看美食节目、养宠物',
                secret: '其实不会游泳，小时候溺水过一次',
                color: '#F39C12',
                accentColor: '#F9E79F'
            },
            {
                id: 6,
                nickname: '扬扬狗',
                realName: '张扬',
                gender: 'male',
                age: 24,
                occupation: '独立音乐人/乐队主唱',
                personality: '自由随性，敢爱敢恨，有点叛逆，热爱音乐',
                background: '热爱音乐，在酒吧驻唱，有自己的乐队，有一些粉丝',
                likes: '音乐、吉他、摇滚、旅行演出',
                secret: '其实很传统，想找个能安定下来的人',
                color: '#E74C3C',
                accentColor: '#F1948A'
            },
            {
                id: 7,
                nickname: '蜜蜜兔',
                realName: '田蜜',
                gender: 'female',
                age: 22,
                occupation: '大学生/兼职网红',
                personality: '活泼开朗，爱撒娇，充满活力，有点小脾气',
                background: '艺术学院在读，在网上分享日常很受欢迎',
                likes: '购物、美食、拍照、旅行',
                secret: '其实私下里是个游戏宅，喜欢打游戏',
                color: '#FF69B4',
                accentColor: '#FFB6C1'
            },
            {
                id: 8,
                nickname: '晴晴猫',
                realName: '苏晴',
                gender: 'female',
                age: 30,
                occupation: '时尚杂志编辑',
                personality: '知性优雅，独立自信，情商高，有点强势',
                background: '留学归来，在知名时尚杂志工作，阅人无数',
                likes: '时尚、红酒、旅行、阅读',
                secret: '其实很渴望被保护，私下很黏人',
                color: '#9B59B6',
                accentColor: '#D2B4DE'
            },
            {
                id: 9,
                nickname: '霜霜狼',
                realName: '凌霜',
                gender: 'female',
                age: 25,
                occupation: '摄影师/机车爱好者',
                personality: '飒爽直率，敢爱敢恨，不扭捏',
                background: '旅行摄影师，去过很多地方，喜欢冒险',
                likes: '摄影、机车、旅行、极限运动',
                secret: '其实很怕鬼，不敢看恐怖片',
                color: '#2C3E50',
                accentColor: '#95A5A6'
            },
            {
                id: 10,
                nickname: '暖暖羊',
                realName: '何暖',
                gender: 'female',
                age: 27,
                occupation: '儿科护士',
                personality: '温柔体贴，善解人意，有耐心，爱心泛滥',
                background: '医院儿科护士，很喜欢小朋友',
                likes: '照顾人、小动物、阅读、烘焙',
                secret: '其实有恐血症，当护士克服了很多困难',
                color: '#27AE60',
                accentColor: '#82E0AA'
            },
            {
                id: 11,
                nickname: '雅雅鹿',
                realName: '林书雅',
                gender: 'female',
                age: 26,
                occupation: '图书馆管理员/作家',
                personality: '智慧内敛，博览群书，有点内向但内心丰富',
                background: '中文系毕业，在图书馆工作，业余写小说',
                likes: '阅读、写作、古典音乐、喝茶',
                secret: '其实写的是言情小说，笔名在网上很火但没人知道',
                color: '#1ABC9C',
                accentColor: '#A3E4D7'
            },
            {
                id: 12,
                nickname: '轻轻云',
                realName: '云轻',
                gender: 'female',
                age: 24,
                occupation: '瑜伽老师/茶师',
                personality: '佛系平和，不争不抢，内心强大',
                background: '从小学习瑜伽和茶道，想开自己的工作室',
                likes: '瑜伽、茶道、冥想、大自然',
                secret: '其实脾气其实很倔，认定的事不会改变',
                color: '#E8DAEF',
                accentColor: '#D5F5E3'
            }
        ];
    }

    getGuests() {
        return this.guests;
    }

    getGuestById(id) {
        return this.guests.find(g => g.id === id);
    }
}
