using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Core.Resource
{
    /// <summary>
    /// 音频管理器 - 统一管理游戏音频播放
    /// 支持背景音乐、音效、语音等分类管理
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get { return _instance; }
        }

        [Header("音频配置")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private float _defaultBGMVolume = 0.7f;
        [SerializeField] private float _defaultSFXVolume = 1.0f;
        [SerializeField] private float _defaultVoiceVolume = 1.0f;
        [SerializeField] private float _fadeDuration = 1.0f;

        private Dictionary<string, AudioSource> _bgmSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, AudioSource> _sfxSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, AudioSource> _voiceSources = new Dictionary<string, AudioSource>();
        
        private AudioSource _currentBGM;
        private string _currentBGMName = "";
        
        private float _masterVolume = 1.0f;
        private float _bgmVolume = 1.0f;
        private float _sfxVolume = 1.0f;
        private float _voiceVolume = 1.0f;

        public enum AudioType
        {
            BGM,
            SFX,
            Voice
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeAudioManager();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初始化音频管理器
        /// </summary>
        private void InitializeAudioManager()
        {
            CreateAudioSourcePool();
            LoadAudioSettings();
        }

        /// <summary>
        /// 创建音频源池
        /// </summary>
        private void CreateAudioSourcePool()
        {
            for (int i = 0; i < 3; i++)
            {
                var bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("BGM")[0];
                _bgmSources[$"Pool_{i}"] = bgmSource;
            }

            for (int i = 0; i < 10; i++)
            {
                var sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("SFX")[0];
                _sfxSources[$"Pool_{i}"] = sfxSource;
            }

            for (int i = 0; i < 5; i++)
            {
                var voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.loop = false;
                voiceSource.playOnAwake = false;
                voiceSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("Voice")[0];
                _voiceSources[$"Pool_{i}"] = voiceSource;
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayBGM(string clipName, bool loop = true)
        {
            var clip = Resources.Load<AudioClip>($"Audio/BGM/{clipName}");
            if (clip == null)
            {
                Debug.LogWarning($"BGM不存在: {clipName}");
                return;
            }

            if (_currentBGM != null && _currentBGM.isPlaying)
            {
                StartCoroutine(FadeOutBGM());
            }

            _currentBGM = GetAvailableBGMsource();
            _currentBGM.clip = clip;
            _currentBGM.loop = loop;
            _currentBGM.volume = 0f;
            _currentBGM.Play();
            StartCoroutine(FadeInBGM());

            _currentBGMName = clipName;
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBGM()
        {
            if (_currentBGM != null)
            {
                StartCoroutine(FadeOutBGM());
            }
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            if (_currentBGM != null)
            {
                _currentBGM.Pause();
            }
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            if (_currentBGM != null)
            {
                _currentBGM.UnPause();
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySFX(string clipName)
        {
            var clip = Resources.Load<AudioClip>($"Audio/SFX/{clipName}");
            if (clip == null)
            {
                Debug.LogWarning($"SFX不存在: {clipName}");
                return;
            }

            var source = GetAvailableSFXsource();
            source.PlayOneShot(clip, _sfxVolume * _masterVolume);
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        public void PlayVoice(string clipName, Action onComplete = null)
        {
            var clip = Resources.Load<AudioClip>($"Audio/Voice/{clipName}");
            if (clip == null)
            {
                Debug.LogWarning($"Voice不存在: {clipName}");
                onComplete?.Invoke();
                return;
            }

            var source = GetAvailableVoicesource();
            source.clip = clip;
            source.volume = _voiceVolume * _masterVolume;
            source.Play();
        }

        /// <summary>
        /// 淡入BGM
        /// </summary>
        private System.Collections.IEnumerator FadeInBGM()
        {
            float elapsed = 0f;
            float targetVolume = _bgmVolume * _masterVolume;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeDuration;
                _currentBGM.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }

            _currentBGM.volume = targetVolume;
        }

        /// <summary>
        /// 淡出BGM
        /// </summary>
        private System.Collections.IEnumerator FadeOutBGM()
        {
            float elapsed = 0f;
            float startVolume = _currentBGM.volume;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeDuration;
                _currentBGM.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            _currentBGM.Stop();
            _currentBGM.clip = null;
        }

        /// <summary>
        /// 获取可用的BGM源
        /// </summary>
        private AudioSource GetAvailableBGMsource()
        {
            foreach (var source in _bgmSources.Values)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.loop = true;
            _bgmSources[$"Dynamic_{_bgmSources.Count}"] = newSource;
            return newSource;
        }

        /// <summary>
        /// 获取可用的SFX源
        /// </summary>
        private AudioSource GetAvailableSFXsource()
        {
            foreach (var source in _sfxSources.Values)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            var newSource = gameObject.AddComponent<AudioSource>();
            _sfxSources[$"Dynamic_{_sfxSources.Count}"] = newSource;
            return newSource;
        }

        /// <summary>
        /// 获取可用的Voice源
        /// </summary>
        private AudioSource GetAvailableVoicesource()
        {
            foreach (var source in _voiceSources.Values)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            var newSource = gameObject.AddComponent<AudioSource>();
            _voiceSources[$"Dynamic_{_voiceSources.Count}"] = newSource;
            return newSource;
        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveAudioSettings();
        }

        /// <summary>
        /// 设置BGM音量
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            if (_currentBGM != null)
            {
                _currentBGM.volume = _bgmVolume * _masterVolume;
            }
            SaveAudioSettings();
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        /// <summary>
        /// 设置语音音量
        /// </summary>
        public void SetVoiceVolume(float volume)
        {
            _voiceVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        /// <summary>
        /// 更新所有音量
        /// </summary>
        private void UpdateAllVolumes()
        {
            if (_currentBGM != null)
            {
                _currentBGM.volume = _bgmVolume * _masterVolume;
            }
        }

        /// <summary>
        /// 加载音频设置
        /// </summary>
        private void LoadAudioSettings()
        {
            _masterVolume = PlayerPrefs.GetFloat("Audio_Master", 1.0f);
            _bgmVolume = PlayerPrefs.GetFloat("Audio_BGM", _defaultBGMVolume);
            _sfxVolume = PlayerPrefs.GetFloat("Audio_SFX", _defaultSFXVolume);
            _voiceVolume = PlayerPrefs.GetFloat("Audio_Voice", _defaultVoiceVolume);
        }

        /// <summary>
        /// 保存音频设置
        /// </summary>
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("Audio_Master", _masterVolume);
            PlayerPrefs.SetFloat("Audio_BGM", _bgmVolume);
            PlayerPrefs.SetFloat("Audio_SFX", _sfxVolume);
            PlayerPrefs.SetFloat("Audio_Voice", _voiceVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 获取当前BGM名称
        /// </summary>
        public string GetCurrentBGMName()
        {
            return _currentBGMName;
        }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying(AudioType type)
        {
            switch (type)
            {
                case AudioType.BGM:
                    return _currentBGM != null && _currentBGM.isPlaying;
                default:
                    return false;
            }
        }
    }
}
