using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Resource
{
    /// <summary>
    /// 本地化管理器 - 管理游戏多语言支持
    /// 支持文本、语音、图像的本地化
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get { return _instance; }
        }

        [Header("本地化配置")]
        [SerializeField] private string _defaultLanguage = "zh-CN";
        [SerializeField] private bool _enableFallback = true;
        [SerializeField] private bool _enableCaching = true;

        private Dictionary<string, string> _currentTranslations = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> _allTranslations = new Dictionary<string, Dictionary<string, string>>();
        private string _currentLanguage = "zh-CN";
        private string _fallbackLanguage = "zh-CN";

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
        }

        public event Action<string> OnLanguageChanged;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeLocalization();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初始化本地化管理器
        /// </summary>
        private void InitializeLocalization()
        {
            LoadAvailableLanguages();
            SetLanguage(_defaultLanguage);
        }

        /// <summary>
        /// 加载可用语言
        /// </summary>
        private void LoadAvailableLanguages()
        {
            _allTranslations["zh-CN"] = LoadLanguageFile("zh-CN");
            _allTranslations["en-US"] = LoadLanguageFile("en-US");
            _allTranslations["ja-JP"] = LoadLanguageFile("ja-JP");
        }

        /// <summary>
        /// 加载语言文件
        /// </summary>
        private Dictionary<string, string> LoadLanguageFile(string language)
        {
            var translations = new Dictionary<string, string>();

            TextAsset textAsset = Resources.Load<TextAsset>($"Localization/{language}");
            if (textAsset != null)
            {
                ParseLanguageFile(textAsset.text, translations);
            }
            else
            {
                LoadDefaultTranslations(translations, language);
            }

            return translations;
        }

        /// <summary>
        /// 解析语言文件
        /// </summary>
        private void ParseLanguageFile(string content, Dictionary<string, string> translations)
        {
            string[] lines = content.Split('\n');
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = line.Split('=');
                if (parts.Length >= 2)
                {
                    string key = parts[0].Trim();
                    string value = line.Substring(line.IndexOf('=') + 1).Trim();
                    translations[key] = value;
                }
            }
        }

        /// <summary>
        /// 加载默认翻译
        /// </summary>
        private void LoadDefaultTranslations(Dictionary<string, string> translations, string language)
        {
            switch (language)
            {
                case "zh-CN":
                    LoadChineseTranslations(translations);
                    break;
                case "en-US":
                    LoadEnglishTranslations(translations);
                    break;
                case "ja-JP":
                    LoadJapaneseTranslations(translations);
                    break;
            }
        }

        /// <summary>
        /// 加载中文翻译
        /// </summary>
        private void LoadChineseTranslations(Dictionary<string, string> translations)
        {
            translations["game_title"] = "糟糕！是心动鸭！";
            translations["main_menu_start"] = "开始游戏";
            translations["main_menu_continue"] = "继续游戏";
            translations["main_menu_settings"] = "设置";
            translations["main_menu_exit"] = "退出游戏";
            translations["mode_guest"] = "嘉宾模式";
            translations["mode_director"] = "导演模式";
            translations["dialogue_continue"] = "继续";
            translations["dialogue_skip"] = "跳过";
            translations["dialogue_save"] = "保存";
            translations["settings_audio"] = "音频设置";
            translations["settings_video"] = "画面设置";
            translations["settings_language"] = "语言设置";
            translations["save_confirm"] = "确定要保存吗？";
            translations["load_confirm"] = "确定要读取存档吗？";
            translations["yes"] = "是";
            translations["no"] = "否";
        }

        /// <summary>
        /// 加载英文翻译
        /// </summary>
        private void LoadEnglishTranslations(Dictionary<string, string> translations)
        {
            translations["game_title"] = "Oh No! It's a Crush!";
            translations["main_menu_start"] = "Start Game";
            translations["main_menu_continue"] = "Continue";
            translations["main_menu_settings"] = "Settings";
            translations["main_menu_exit"] = "Exit";
            translations["mode_guest"] = "Guest Mode";
            translations["mode_director"] = "Director Mode";
            translations["dialogue_continue"] = "Continue";
            translations["dialogue_skip"] = "Skip";
            translations["dialogue_save"] = "Save";
            translations["settings_audio"] = "Audio Settings";
            translations["settings_video"] = "Video Settings";
            translations["settings_language"] = "Language Settings";
            translations["save_confirm"] = "Save game?";
            translations["load_confirm"] = "Load game?";
            translations["yes"] = "Yes";
            translations["no"] = "No";
        }

        /// <summary>
        /// 加载日文翻译
        /// </summary>
        private void LoadJapaneseTranslations(Dictionary<string, string> translations)
        {
            translations["game_title"] = "糟糕！是心動鸭！";
            translations["main_menu_start"] = "ゲームスタート";
            translations["main_menu_continue"] = "続きから";
            translations["main_menu_settings"] = "設定";
            translations["main_menu_exit"] = "終了";
            translations["mode_guest"] = "ゲストモード";
            translations["mode_director"] = "監督モード";
            translations["dialogue_continue"] = "続ける";
            translations["dialogue_skip"] = "スキップ";
            translations["dialogue_save"] = "セーブ";
            translations["settings_audio"] = "オーディオ設定";
            translations["settings_video"] = "映像設定";
            translations["settings_language"] = "言語設定";
            translations["save_confirm"] = "保存しますか？";
            translations["load_confirm"] = "ロードしますか？";
            translations["yes"] = "はい";
            translations["no"] = "いいえ";
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        public void SetLanguage(string language)
        {
            if (!_allTranslations.ContainsKey(language))
            {
                Debug.LogWarning($"语言不支持: {language}");
                return;
            }

            _currentLanguage = language;
            _currentTranslations = _allTranslations[language];

            PlayerPrefs.SetString("Language", language);
            OnLanguageChanged?.Invoke(language);
        }

        /// <summary>
        /// 获取翻译文本
        /// </summary>
        public string Get(string key)
        {
            if (_currentTranslations.ContainsKey(key))
            {
                return _currentTranslations[key];
            }

            if (_enableFallback && _fallbackLanguage != _currentLanguage)
            {
                if (_allTranslations.ContainsKey(_fallbackLanguage) && 
                    _allTranslations[_fallbackLanguage].ContainsKey(key))
                {
                    return _allTranslations[_fallbackLanguage][key];
                }
            }

            return key;
        }

        /// <summary>
        /// 获取带参数的翻译文本
        /// </summary>
        public string GetFormatted(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }

        /// <summary>
        /// 获取所有可用语言
        /// </summary>
        public List<string> GetAvailableLanguages()
        {
            return new List<string>(_allTranslations.Keys);
        }

        /// <summary>
        /// 检查语言是否支持
        /// </summary>
        public bool IsLanguageSupported(string language)
        {
            return _allTranslations.ContainsKey(language);
        }

        /// <summary>
        /// 获取翻译统计
        /// </summary>
        public Dictionary<string, int> GetTranslationStats()
        {
            var stats = new Dictionary<string, int>();
            foreach (var lang in _allTranslations.Keys)
            {
                stats[lang] = _allTranslations[lang].Count;
            }
            return stats;
        }

        /// <summary>
        /// 添加自定义翻译
        /// </summary>
        public void AddTranslation(string language, string key, string value)
        {
            if (!_allTranslations.ContainsKey(language))
            {
                _allTranslations[language] = new Dictionary<string, string>();
            }
            _allTranslations[language][key] = value;

            if (language == _currentLanguage)
            {
                _currentTranslations[key] = value;
            }
        }
    }
}
