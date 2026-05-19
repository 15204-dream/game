using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HeartHook.Game.Ending.UI
{
    public class EndingReplay : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image previewImage;
        [SerializeField] private Slider progressSlider;

        [Header("时间轴")]
        [SerializeField] private Transform timelineContainer;
        [SerializeField] private GameObject checkpointPrefab;
        [SerializeField] private TextMeshProUGUI currentTimeText;
        [SerializeField] private TextMeshProUGUI totalTimeText;

        [Header("按钮")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button speedButton;

        [Header("播放配置")]
        [SerializeField] private float[] playbackSpeeds = { 0.5f, 1f, 1.5f, 2f };
        [SerializeField] private int currentSpeedIndex = 1;

        private EndingData currentEnding;
        private List<ReplayCheckpoint> checkpoints = new List<ReplayCheckpoint>();
        private bool isPlaying;
        private float currentTime;
        private float totalDuration = 60f;
        private float playbackSpeed = 1f;

        public event Action OnReplayClosed;
        public event Action<EndingData> OnReplayRequested;

        private void Start()
        {
            InitializeButtons();
            gameObject.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(Play);
                playButton.gameObject.SetActive(true);
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(Pause);
                pauseButton.gameObject.SetActive(false);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(Restart);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }

            if (speedButton != null)
            {
                speedButton.onClick.AddListener(CycleSpeed);
                UpdateSpeedButtonText();
            }
        }

        public void ShowReplay(EndingData ending)
        {
            if (ending == null)
            {
                Debug.LogWarning("[EndingReplay] 结局数据为空！");
                return;
            }

            currentEnding = ending;
            gameObject.SetActive(true);
            isPlaying = false;
            currentTime = 0f;

            PrepareReplay(ending);
            CreateCheckpoints(ending);
            UpdateUI();
        }

        private void PrepareReplay(EndingData ending)
        {
            if (titleText != null)
            {
                titleText.text = $"回放：{ending.EndingName}";
            }

            if (descriptionText != null)
            {
                descriptionText.text = ending.Description;
            }

            if (previewImage != null && ending.BackgroundImage != null)
            {
                previewImage.sprite = ending.BackgroundImage;
            }

            totalDuration = CalculateTotalDuration(ending);
            if (totalTimeText != null)
            {
                totalTimeText.text = FormatTime(totalDuration);
            }

            if (progressSlider != null)
            {
                progressSlider.maxValue = totalDuration;
                progressSlider.value = 0f;
            }
        }

        private float CalculateTotalDuration(EndingData ending)
        {
            float baseDuration = 30f;
            if (!string.IsNullOrEmpty(ending.Summary))
            {
                baseDuration += ending.Summary.Length * 0.05f;
            }
            return Mathf.Min(baseDuration, 120f);
        }

        private void CreateCheckpoints(EndingData ending)
        {
            if (timelineContainer == null || checkpointPrefab == null)
                return;

            checkpoints.Clear();
            foreach (Transform child in timelineContainer)
            {
                Destroy(child.gameObject);
            }

            int checkpointCount = 5;
            for (int i = 0; i < checkpointCount; i++)
            {
                float time = (totalDuration / (checkpointCount - 1)) * i;
                var checkpoint = CreateCheckpoint(time, $"CP_{i + 1}");
                checkpoints.Add(checkpoint);
            }
        }

        private ReplayCheckpoint CreateCheckpoint(float time, string id)
        {
            var go = Instantiate(checkpointPrefab, timelineContainer);
            var checkpoint = go.AddComponent<ReplayCheckpoint>();

            checkpoint.Initialize(time, id, OnCheckpointClicked);
            return checkpoint;
        }

        private void OnCheckpointClicked(ReplayCheckpoint checkpoint)
        {
            currentTime = checkpoint.Time;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (currentTimeText != null)
            {
                currentTimeText.text = FormatTime(currentTime);
            }

            if (progressSlider != null)
            {
                progressSlider.value = currentTime;
            }
        }

        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            currentTime += Time.deltaTime * playbackSpeed;

            if (currentTime >= totalDuration)
            {
                currentTime = totalDuration;
                Pause();
            }

            UpdateUI();
        }

        public void Play()
        {
            if (currentEnding == null)
                return;

            isPlaying = true;

            if (playButton != null)
            {
                playButton.gameObject.SetActive(false);
            }

            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(true);
            }

            OnReplayRequested?.Invoke(currentEnding);
        }

        public void Pause()
        {
            isPlaying = false;

            if (playButton != null)
            {
                playButton.gameObject.SetActive(true);
            }

            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(false);
            }
        }

        public void Restart()
        {
            currentTime = 0f;
            UpdateUI();
            Play();
        }

        private void CycleSpeed()
        {
            currentSpeedIndex = (currentSpeedIndex + 1) % playbackSpeeds.Length;
            playbackSpeed = playbackSpeeds[currentSpeedIndex];
            UpdateSpeedButtonText();
        }

        private void UpdateSpeedButtonText()
        {
            if (speedButton != null)
            {
                var text = speedButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = $"{playbackSpeed}x";
                }
            }
        }

        public void SeekTo(float time)
        {
            currentTime = Mathf.Clamp(time, 0f, totalDuration);
            UpdateUI();
        }

        private void Close()
        {
            Pause();
            gameObject.SetActive(false);
            OnReplayClosed?.Invoke();
        }

        public bool IsPlaying => isPlaying;
        public float CurrentTime => currentTime;
        public float TotalDuration => totalDuration;
    }

    public class ReplayCheckpoint : MonoBehaviour
    {
        public float Time { get; private set; }
        public string Id { get; private set; }

        private Action<ReplayCheckpoint> onClicked;

        public void Initialize(float time, string id, Action<ReplayCheckpoint> callback)
        {
            Time = time;
            Id = id;
            onClicked = callback;

            var button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            onClicked?.Invoke(this);
        }
    }
}
