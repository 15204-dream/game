export default class EndingManager {
    constructor() {
        this.endings = this.initEndings();
        this.unlockedEndings = this.loadUnlockedEndings();
    }

    initEndings() {
        const endings = [];
        
        const guests = [
            '阳阳鸭', '冷冷喵', '墨墨熊', '哲哲兔', '暖暖猪', '扬扬狗',
            '蜜蜜兔', '晴晴猫', '霜霜狼', '暖暖羊', '雅雅鹿', '轻轻云'
        ];

        guests.forEach((guest, index) => {
            endings.push({
                id: `love_${guest}_1`,
                type: 'love',
                title: `${guest}的甜蜜结局`,
                description: `你和${guest}最终走到了一起，过上了幸福的生活~`,
                guestId: index + 1,
                affectionRequired: 90
            });
            endings.push({
                id: `love_${guest}_2`,
                type: 'love',
                title: `${guest}的羞涩结局`,
                description: `虽然有些害羞，但你和${guest}确认了彼此的心意`,
                guestId: index + 1,
                affectionRequired: 70
            });
            endings.push({
                id: `love_${guest}_3`,
                type: 'love',
                title: `${guest}的遗憾结局`,
                description: `有些话没有说出口，这或许是最好的结局...`,
                guestId: index + 1,
                affectionRequired: 40
            });
        });

        endings.push({
            id: 'friend_best',
            type: 'friend',
            title: '挚友结局',
            description: '你在心动小屋收获了最珍贵的友谊！',
            friendshipRequired: 80
        });
        endings.push({
            id: 'friend_group',
            type: 'friend',
            title: '团体友谊结局',
            description: '12人成为了永远的好朋友！',
            friendshipRequired: 60
        });
        endings.push({
            id: 'single_elite',
            type: 'single',
            title: '单身贵族结局',
            description: '一个人也可以很精彩！',
            singleRequired: true
        });

        endings.push({
            id: 'director_hit',
            type: 'director',
            title: '爆款综艺结局',
            description: '节目收视率破纪录，成为现象级综艺！',
            ratingRequired: 95
        });
        endings.push({
            id: 'director_critical',
            type: 'director',
            title: '口碑佳作结局',
            description: '虽然收视一般，但收获了极佳的口碑！',
            ratingRequired: 75
        });
        endings.push({
            id: 'director_controversy',
            type: 'director',
            title: '争议话题结局',
            description: '节目话题度爆表，褒贬不一',
            ratingRequired: 50
        });
        endings.push({
            id: 'director_flop',
            type: 'director',
            title: '平淡收场结局',
            description: '节目平稳收官，虽不火爆但也算圆满',
            ratingRequired: 30
        });

        endings.push({
            id: 'secret_1',
            type: 'secret',
            title: '心动秘密1',
            description: '你发现了某位嘉宾的小秘密...',
            secretRequired: true
        });
        endings.push({
            id: 'secret_2',
            type: 'secret',
            title: '心动秘密2',
            description: '另一个秘密被你发现了！',
            secretRequired: true
        });
        endings.push({
            id: 'true_love',
            type: 'special',
            title: '真命天子/天女结局',
            description: '完美的爱情，完美的结局！',
            specialRequired: true
        });

        return endings;
    }

    loadUnlockedEndings() {
        try {
            const saved = localStorage.getItem('unlockedEndings');
            return saved ? JSON.parse(saved) : [];
        } catch (e) {
            return [];
        }
    }

    saveUnlockedEndings() {
        localStorage.setItem('unlockedEndings', JSON.stringify(this.unlockedEndings));
    }

    unlockEnding(endingId) {
        if (!this.unlockedEndings.includes(endingId)) {
            this.unlockedEndings.push(endingId);
            this.saveUnlockedEndings();
        }
    }

    isUnlocked(endingId) {
        return this.unlockedEndings.includes(endingId);
    }

    getEndingById(id) {
        return this.endings.find(e => e.id === id);
    }

    getAllEndings() {
        return this.endings;
    }

    getUnlockedEndings() {
        return this.endings.filter(e => this.unlockedEndings.includes(e.id));
    }

    getProgress() {
        return {
            total: this.endings.length,
            unlocked: this.unlockedEndings.length,
            percentage: Math.round((this.unlockedEndings.length / this.endings.length) * 100)
        };
    }
}
