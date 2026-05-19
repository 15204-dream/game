using UnityEngine;
using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭
{
    /// <summary>
    /// 角色数据基类 - 定义恋综角色的基本信息
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        /// <summary>
        /// 角色唯一标识符
        /// </summary>
        [SerializeField]
        private string id;

        /// <summary>
        /// 角色名称
        /// </summary>
        [SerializeField]
        private string name;

        /// <summary>
        /// 角色性别
        /// </summary>
        [SerializeField]
        private string gender;

        /// <summary>
        /// 年龄
        /// </summary>
        [SerializeField]
        private int age;

        /// <summary>
        /// 职业
        /// </summary>
        [SerializeField]
        private string occupation;

        /// <summary>
        /// 性格标签列表
        /// </summary>
        [SerializeField]
        private List<string> personalityTags = new List<string>();

        /// <summary>
        /// 兴趣标签列表
        /// </summary>
        [SerializeField]
        private List<string> interestTags = new List<string>();

        /// <summary>
        /// 角色头像资源路径
        /// </summary>
        [SerializeField]
        private string avatarPath;

        /// <summary>
        /// 角色立绘资源路径
        /// </summary>
        [SerializeField]
        private string portraitPath;

        /// <summary>
        /// 角色背景故事
        /// </summary>
        [SerializeField]
        [TextArea(3, 10)]
        private string backstory;

        /// <summary>
        /// 理想型描述
        /// </summary>
        [SerializeField]
        [TextArea(2, 5)]
        private string idealTypeDescription;

        /// <summary>
        /// 喜欢的兴趣标签
        /// </summary>
        [SerializeField]
        private List<string> preferredInterests = new List<string>();

        /// <summary>
        /// 讨厌的兴趣标签
        /// </summary>
        [SerializeField]
        private List<string> dislikedInterests = new List<string>();

        /// <summary>
        /// 心动阈值难度 (1-5, 5最难攻略)
        /// </summary>
        [SerializeField]
        [Range(1, 5)]
        private int difficultyRating = 3;

        /// <summary>
        /// 是否可攻略
        /// </summary>
        [SerializeField]
        private bool isRomanceable = true;

        /// <summary>
        /// 特殊事件列表
        /// </summary>
        [SerializeField]
        private List<string> specialEventIds = new List<string>();

        /// <summary>
        /// 初始好感度
        /// </summary>
        [SerializeField]
        private int initialAffection = Constants.INITIAL_AFFECTION;

        /// <summary>
        /// 语音资源路径
        /// </summary>
        [SerializeField]
        private string voicePath;

        /// <summary>
        /// 角色颜色（用于UI显示）
        /// </summary>
        [SerializeField]
        private Color characterColor = Color.white;

        #region 属性访问器

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Gender
        {
            get => gender;
            set => gender = value;
        }

        public int Age
        {
            get => age;
            set => age = Mathf.Clamp(value, 18, 100);
        }

        public string Occupation
        {
            get => occupation;
            set => occupation = value;
        }

        public List<string> PersonalityTags
        {
            get => personalityTags;
            set => personalityTags = value;
        }

        public List<string> InterestTags
        {
            get => interestTags;
            set => interestTags = value;
        }

        public string AvatarPath
        {
            get => avatarPath;
            set => avatarPath = value;
        }

        public string PortraitPath
        {
            get => portraitPath;
            set => portraitPath = value;
        }

        public string Backstory
        {
            get => backstory;
            set => backstory = value;
        }

        public string IdealTypeDescription
        {
            get => idealTypeDescription;
            set => idealTypeDescription = value;
        }

        public List<string> PreferredInterests
        {
            get => preferredInterests;
            set => preferredInterests = value;
        }

        public List<string> DislikedInterests
        {
            get => dislikedInterests;
            set => dislikedInterests = value;
        }

        public int DifficultyRating
        {
            get => difficultyRating;
            set => difficultyRating = Mathf.Clamp(value, 1, 5);
        }

        public bool IsRomanceable
        {
            get => isRomanceable;
            set => isRomanceable = value;
        }

        public List<string> SpecialEventIds
        {
            get => specialEventIds;
            set => specialEventIds = value;
        }

        public int InitialAffection
        {
            get => initialAffection;
            set => initialAffection = Mathf.Clamp(value, 0, Constants.MAX_AFFECTION);
        }

        public string VoicePath
        {
            get => voicePath;
            set => voicePath = value;
        }

        public Color CharacterColor
        {
            get => characterColor;
            set => characterColor = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CharacterData()
        {
            id = "";
            name = "";
            gender = "未知";
            age = 25;
            occupation = "";
            personalityTags = new List<string>();
            interestTags = new List<string>();
            avatarPath = "";
            portraitPath = "";
            backstory = "";
            idealTypeDescription = "";
            preferredInterests = new List<string>();
            dislikedInterests = new List<string>();
            difficultyRating = 3;
            isRomanceable = true;
            specialEventIds = new List<string>();
            initialAffection = Constants.INITIAL_AFFECTION;
            voicePath = "";
            characterColor = Color.white;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查角色是否匹配理想型
        /// </summary>
        /// <param name="otherPersonalityTags">其他角色的性格标签</param>
        /// <param name="otherInterestTags">其他角色的兴趣标签</param>
        /// <returns>匹配度 (0-100)</returns>
        public int CalculateIdealMatchScore(List<string> otherPersonalityTags, List<string> otherInterestTags)
        {
            int score = 0;

            foreach (var interest in preferredInterests)
            {
                if (otherInterestTags.Contains(interest))
                {
                    score += 20;
                }
            }

            foreach (var interest in dislikedInterests)
            {
                if (otherInterestTags.Contains(interest))
                {
                    score -= 15;
                }
            }

            return Mathf.Clamp(score, 0, 100);
        }

        /// <summary>
        /// 获取性格标签字符串
        /// </summary>
        /// <returns>逗号分隔的性格标签</returns>
        public string GetPersonalityTagsString()
        {
            return string.Join(", ", personalityTags);
        }

        /// <summary>
        /// 获取兴趣标签字符串
        /// </summary>
        /// <returns>逗号分隔的兴趣标签</returns>
        public string GetInterestTagsString()
        {
            return string.Join(", ", interestTags);
        }

        /// <summary>
        /// 是否为女性角色
        /// </summary>
        /// <returns>是否为女性</returns>
        public bool IsFemale()
        {
            return gender == "女" || gender == "Female";
        }

        /// <summary>
        /// 是否为男性角色
        /// </summary>
        /// <returns>是否为男性</returns>
        public bool IsMale()
        {
            return gender == "男" || gender == "Male";
        }

        /// <summary>
        /// 获取难度等级描述
        /// </summary>
        /// <returns>难度描述</returns>
        public string GetDifficultyDescription()
        {
            return difficultyRating switch
            {
                1 => "非常简单",
                2 => "简单",
                3 => "普通",
                4 => "困难",
                5 => "非常困难",
                _ => "未知"
            };
        }

        /// <summary>
        /// 添加性格标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddPersonalityTag(string tag)
        {
            if (!personalityTags.Contains(tag))
            {
                personalityTags.Add(tag);
            }
        }

        /// <summary>
        /// 添加兴趣标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddInterestTag(string tag)
        {
            if (!interestTags.Contains(tag))
            {
                interestTags.Add(tag);
            }
        }

        /// <summary>
        /// 复制角色数据
        /// </summary>
        /// <returns>复制的新对象</returns>
        public CharacterData Clone()
        {
            CharacterData clone = new CharacterData
            {
                id = this.id,
                name = this.name,
                gender = this.gender,
                age = this.age,
                occupation = this.occupation,
                personalityTags = new List<string>(this.personalityTags),
                interestTags = new List<string>(this.interestTags),
                avatarPath = this.avatarPath,
                portraitPath = this.portraitPath,
                backstory = this.backstory,
                idealTypeDescription = this.idealTypeDescription,
                preferredInterests = new List<string>(this.preferredInterests),
                dislikedInterests = new List<string>(this.dislikedInterests),
                difficultyRating = this.difficultyRating,
                isRomanceable = this.isRomanceable,
                specialEventIds = new List<string>(this.specialEventIds),
                initialAffection = this.initialAffection,
                voicePath = this.voicePath,
                characterColor = this.characterColor
            };
            return clone;
        }

        #endregion

        #region Unity特定方法

        /// <summary>
        /// 获取头像Sprite（如果资源存在）
        /// </summary>
        /// <returns>头像Sprite或null</returns>
        public Sprite GetAvatarSprite()
        {
            if (string.IsNullOrEmpty(avatarPath)) return null;
            
            try
            {
                return Resources.Load<Sprite>(avatarPath);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取立绘Sprite（如果资源存在）
        /// </summary>
        /// <returns>立绘Sprite或null</returns>
        public Sprite GetPortraitSprite()
        {
            if (string.IsNullOrEmpty(portraitPath)) return null;
            
            try
            {
                return Resources.Load<Sprite>(portraitPath);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    /// <summary>
    /// 约会对象数据类
    /// </summary>
    [System.Serializable]
    public class DatingCharacterData : CharacterData
    {
        /// <summary>
        /// 约会时间
        /// </summary>
        [SerializeField]
        private DateTime datingTime;

        /// <summary>
        /// 约会地点
        /// </summary>
        [SerializeField]
        private string datingLocation;

        /// <summary>
        /// 约会状态
        /// </summary>
        [SerializeField]
        private bool isDating;

        /// <summary>
        /// 约会次数
        /// </summary>
        [SerializeField]
        private int dateCount;

        public DateTime DatingTime
        {
            get => datingTime;
            set => datingTime = value;
        }

        public string DatingLocation
        {
            get => datingLocation;
            set => datingLocation = value;
        }

        public bool IsDating
        {
            get => isDating;
            set => isDating = value;
        }

        public int DateCount
        {
            get => dateCount;
            set => dateCount = Mathf.Max(0, value);
        }
    }
}
