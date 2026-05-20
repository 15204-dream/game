class EndingManager {
    constructor() {
        this.unlockedEndings = new Set();
        this.loadEndings();
    }
    
    getAllEndings() {
        return [
            { id: 'yangyang_true', name: '阳阳鸭 - 真爱结局', desc: '你和阳阳鸭终于走到了一起', guest: 'yangyang', type: 'true' },
            { id: 'yangyang_friend', name: '阳阳鸭 - 友情结局', desc: '你和阳阳鸭成为了最好的朋友', guest: 'yangyang', type: 'friend' },
            { id: 'yangyang_bad', name: '阳阳鸭 - 错过结局', desc: '你和阳阳鸭擦肩而过', guest: 'yangyang', type: 'bad' },
            
            { id: 'lengleng_true', name: '冷冷喵 - 真爱结局', desc: '你融化了冷冷喵冰冷的心', guest: 'lengleng', type: 'true' },
            { id: 'lengleng_friend', name: '冷冷喵 - 友情结局', desc: '你成为了冷冷喵唯一的朋友', guest: 'lengleng', type: 'friend' },
            { id: 'lengleng_bad', name: '冷冷喵 - 错过结局', desc: '冷冷喵再次关上了心门', guest: 'lengleng', type: 'bad' },
            
            { id: 'momo_true', name: '墨墨熊 - 真爱结局', desc: '墨墨熊为你脱下了白大褂', guest: 'momo', type: 'true' },
            { id: 'momo_friend', name: '墨墨熊 - 友情结局', desc: '你是墨墨熊最重要的病人', guest: 'momo', type: 'friend' },
            { id: 'momo_bad', name: '墨墨熊 - 错过结局', desc: '墨墨熊回到了手术室', guest: 'momo', type: 'bad' },
            
            { id: 'zhezhe_true', name: '哲哲兔 - 真爱结局', desc: '哲哲兔为你写了一首情歌', guest: 'zhezhe', type: 'true' },
            { id: 'zhezhe_friend', name: '哲哲兔 - 友情结局', desc: '你是哲哲兔的第一个听众', guest: 'zhezhe', type: 'friend' },
            { id: 'zhezhe_bad', name: '哲哲兔 - 错过结局', desc: '哲哲兔独自弹完了最后一曲', guest: 'zhezhe', type: 'bad' },
            
            { id: 'nuannuan_true', name: '暖暖猪 - 真爱结局', desc: '暖暖猪终于找到了家', guest: 'nuannuan', type: 'true' },
            { id: 'nuannuan_friend', name: '暖暖猪 - 友情结局', desc: '你和暖暖猪都是小太阳', guest: 'nuannuan', type: 'friend' },
            { id: 'nuannuan_bad', name: '暖暖猪 - 错过结局', desc: '暖暖猪继续温暖别人', guest: 'nuannuan', type: 'bad' },
            
            { id: 'yangyang2_true', name: '扬扬狗 - 真爱结局', desc: '扬扬狗的失眠终于好了', guest: 'yangyang2', type: 'true' },
            { id: 'yangyang2_friend', name: '扬扬狗 - 友情结局', desc: '你是扬扬狗最忠实的粉丝', guest: 'yangyang2', type: 'friend' },
            { id: 'yangyang2_bad', name: '扬扬狗 - 错过结局', desc: '扬扬狗回到了聚光灯下', guest: 'yangyang2', type: 'bad' },
            
            { id: 'mimi_true', name: '蜜蜜兔 - 真爱结局', desc: '蜜蜜兔学会了做饭', guest: 'mimi', type: 'true' },
            { id: 'mimi_friend', name: '蜜蜜兔 - 友情结局', desc: '你是蜜蜜兔的专属美食家', guest: 'mimi', type: 'friend' },
            { id: 'mimi_bad', name: '蜜蜜兔 - 错过结局', desc: '蜜蜜兔继续寻找美食', guest: 'mimi', type: 'bad' },
            
            { id: 'qingqing_true', name: '晴晴猫 - 真爱结局', desc: '晴晴猫脱下了高跟鞋', guest: 'qingqing', type: 'true' },
            { id: 'qingqing_friend', name: '晴晴猫 - 友情结局', desc: '你是晴晴猫唯一认可的人', guest: 'qingqing', type: 'friend' },
            { id: 'qingqing_bad', name: '晴晴猫 - 错过结局', desc: '晴晴猫继续在职场奋斗', guest: 'qingqing', type: 'bad' },
            
            { id: 'shuangshuang_true', name: '霜霜狼 - 真爱结局', desc: '霜霜狼重新发动了引擎', guest: 'shuangshuang', type: 'true' },
            { id: 'shuangshuang_friend', name: '霜霜狼 - 友情结局', desc: '你是霜霜狼的最佳搭档', guest: 'shuangshuang', type: 'friend' },
            { id: 'shuangshuang_bad', name: '霜霜狼 - 错过结局', desc: '霜霜狼独自踏上旅途', guest: 'shuangshuang', type: 'bad' },
            
            { id: 'nuanyang_true', name: '暖阳羊 - 真爱结局', desc: '暖阳羊终于找到了要找的人', guest: 'nuanyang', type: 'true' },
            { id: 'nuanyang_friend', name: '暖阳羊 - 友情结局', desc: '你是暖阳羊镜头里最美的风景', guest: 'nuanyang', type: 'friend' },
            { id: 'nuanyang_bad', name: '暖阳羊 - 错过结局', desc: '暖阳羊继续流浪', guest: 'nuanyang', type: 'bad' },
            
            { id: 'yaya_true', name: '雅雅鹿 - 真爱结局', desc: '雅雅鹿认定你是前世的缘分', guest: 'yaya', type: 'true' },
            { id: 'yaya_friend', name: '雅雅鹿 - 友情结局', desc: '你是雅雅鹿最好的旅伴', guest: 'yaya', type: 'friend' },
            { id: 'yaya_bad', name: '雅雅鹿 - 错过结局', desc: '雅雅鹿继续考古之旅', guest: 'yaya', type: 'bad' },
            
            { id: 'qingqing2_true', name: '轻轻云 - 真爱结局', desc: '轻轻云为你留在了人间', guest: 'qingqing2', type: 'true' },
            { id: 'qingqing2_friend', name: '轻轻云 - 友情结局', desc: '你是轻轻云唯一的知己', guest: 'qingqing2', type: 'friend' },
            { id: 'qingqing2_bad', name: '轻轻云 - 错过结局', desc: '轻轻云回到了山间', guest: 'qingqing2', type: 'bad' },
            
            { id: 'harem', name: '后宫结局', desc: '所有嘉宾都爱上了你', type: 'special' },
            { id: 'lonely', name: '孤独结局', desc: '你谁也没有选择', type: 'special' },
            { id: 'secret', name: '隐藏结局', desc: '发现了这个节目的秘密', type: 'secret' }
        ];
    }
    
    checkEndingConditions(guestData, storyFlags, choices) {
        const guests = guestData.getGuests();
        let maxAffection = 0;
        let maxGuest = null;
        
        for (let guest of guests) {
            if (guest.affection > maxAffection) {
                maxAffection = guest.affection;
                maxGuest = guest;
            }
        }
        
        let highCount = 0;
        for (let guest of guests) {
            if (guest.affection >= 80) highCount++;
        }
        
        if (highCount >= 12) {
            return this.getEndingById('harem');
        }
        
        if (maxAffection < 20) {
            return this.getEndingById('lonely');
        }
        
        if (maxGuest) {
            if (maxAffection >= 80) {
                return this.getEndingById(`${maxGuest.id}_true`);
            } else if (maxAffection >= 50) {
                return this.getEndingById(`${maxGuest.id}_friend`);
            } else {
                return this.getEndingById(`${maxGuest.id}_bad`);
            }
        }
        
        return this.getEndingById('lonely');
    }
    
    getEndingById(id) {
        return this.getAllEndings().find(e => e.id === id);
    }
    
    unlockEnding(id) {
        this.unlockedEndings.add(id);
        this.saveEndings();
    }
    
    isUnlocked(id) {
        return this.unlockedEndings.has(id);
    }
    
    getUnlockedCount() {
        return this.unlockedEndings.size;
    }
    
    getTotalCount() {
        return this.getAllEndings().length;
    }
    
    saveEndings() {
        try {
            localStorage.setItem('heartDuck_endings', JSON.stringify([...this.unlockedEndings]));
        } catch (e) {
            console.error('Failed to save endings:', e);
        }
    }
    
    loadEndings() {
        try {
            const saved = localStorage.getItem('heartDuck_endings');
            if (saved) {
                this.unlockedEndings = new Set(JSON.parse(saved));
            }
        } catch (e) {
            console.error('Failed to load endings:', e);
        }
    }
}

export default EndingManager;
