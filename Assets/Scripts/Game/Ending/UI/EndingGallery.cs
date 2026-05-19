using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace HeartHook.Game.Ending.UI
{
    public class EndingGallery : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GridLayoutGroup categoryTabs;
        [SerializeField] private GridLayoutGroup endingGrid;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image detailPanel;
        [SerializeField] private TextMeshProUGUI detailTitle;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private Image detailImage;
        [SerializeField] private TextMeshProUGUI detailCondition;

        [Header("按钮")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button lockButton;

        [Header("预制体")]
        [SerializeField] private GameObject endingItemPrefab;
        [SerializeField] private GameObject categoryTabPrefab;

        [Header("配置")]
        [SerializeField] private int itemsPerPage = 8;
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        private List<EndingData> allEndings = new List<EndingData>();
        private List<EndingData> displayedEndings = new List<EndingData>();
        private Dictionary<EndingCategory, List<EndingData>> endingsByCategory = new Dictionary<EndingCategory, List<EndingData>>();
        private EndingCategory currentCategory = EndingCategory.Main;
        private int currentPage = 0;
        private EndingData selectedEnding;
        private List<GameObject> currentItemInstances = new List<GameObject>();

        public event Action OnGalleryClosed;
        public event Action<EndingData> OnEndingSelected;

        private void Start()
        {
            InitializeButtons();
            gameObject.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseGallery);
            }

            if (prevButton != null)
            {
                prevButton.onClick.AddListener(PreviousPage);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextPage);
            }

            if (lockButton != null)
            {
                lockButton.onClick.AddListener(ShowUnlockHint);
            }
        }

        public void OpenGallery()
        {
            gameObject.SetActive(true);
            LoadEndings();
            CreateCategoryTabs();
            FilterByCategory(currentCategory);
            UpdateProgress();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeIn());
            }
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                }
                yield return null;
            }
        }

        private void LoadEndings()
        {
            allEndings.Clear();
            endingsByCategory.Clear();

            if (EndingManager.Instance != null)
            {
                var endings = EndingManager.Instance.GetAllEndings();
                if (endings != null)
                {
                    allEndings.AddRange(endings);
                }
            }

            foreach (EndingCategory category in Enum.GetValues(typeof(EndingCategory)))
            {
                endingsByCategory[category] = new List<EndingData>();
            }

            foreach (var ending in allEndings)
            {
                if (ending != null && endingsByCategory.ContainsKey(ending.Category))
                {
                    endingsByCategory[ending.Category].Add(ending);
                }
            }
        }

        private void CreateCategoryTabs()
        {
            if (categoryTabs == null)
                return;

            foreach (Transform child in categoryTabs.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (EndingCategory category in Enum.GetValues(typeof(EndingCategory)))
            {
                var tab = Instantiate(categoryTabPrefab, categoryTabs.transform);
                var button = tab.GetComponent<Button>();
                var text = tab.GetComponentInChildren<TextMeshProUGUI>();

                if (text != null)
                {
                    text.text = GetCategoryName(category);
                }

                if (button != null)
                {
                    int count = endingsByCategory[category].Count;
                    button.onClick.AddListener(() => FilterByCategory(category));
                }
            }
        }

        private string GetCategoryName(EndingCategory category)
        {
            return category switch
            {
                EndingCategory.Main => "主线结局",
                EndingCategory.Character => "角色结局",
                EndingCategory.Secret => "彩蛋结局",
                EndingCategory.Director => "导演结局",
                _ => category.ToString()
            };
        }

        private void FilterByCategory(EndingCategory category)
        {
            currentCategory = category;
            currentPage = 0;

            displayedEndings = endingsByCategory.ContainsKey(category)
                ? new List<EndingData>(endingsByCategory[category])
                : new List<EndingData>();

            UpdateGrid();
            UpdateNavigationButtons();
        }

        private void UpdateGrid()
        {
            if (endingGrid == null)
                return;

            foreach (var instance in currentItemInstances)
            {
                if (instance != null)
                {
                    Destroy(instance);
                }
            }
            currentItemInstances.Clear();

            int startIndex = currentPage * itemsPerPage;
            int endIndex = Mathf.Min(startIndex + itemsPerPage, displayedEndings.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var ending = displayedEndings[i];
                var item = CreateEndingItem(ending, i);
                currentItemInstances.Add(item);
            }
        }

        private GameObject CreateEndingItem(EndingData ending, int index)
        {
            if (endingItemPrefab == null || endingGrid == null)
                return null;

            var item = Instantiate(endingItemPrefab, endingGrid.transform);

            var image = item.GetComponent<Image>();
            var text = item.GetComponentInChildren<TextMeshProUGUI>();

            if (image != null)
            {
                bool isUnlocked = EndingManager.Instance != null && EndingManager.Instance.IsEndingUnlocked(ending.Id);
                image.color = isUnlocked ? unlockedColor : lockedColor;

                if (isUnlocked && ending.BackgroundImage != null)
                {
                    image.sprite = ending.BackgroundImage;
                }
            }

            if (text != null)
            {
                text.text = ending.GetDisplayName();
            }

            var button = item.GetComponent<Button>();
            if (button != null)
            {
                int capturedIndex = index;
                button.onClick.AddListener(() => SelectEnding(displayedEndings[capturedIndex]));
            }

            return item;
        }

        private void SelectEnding(EndingData ending)
        {
            selectedEnding = ending;
            ShowEndingDetail(ending);
            OnEndingSelected?.Invoke(ending);
        }

        private void ShowEndingDetail(EndingData ending)
        {
            bool isUnlocked = EndingManager.Instance != null && EndingManager.Instance.IsEndingUnlocked(ending.Id);

            if (detailTitle != null)
            {
                detailTitle.text = isUnlocked ? ending.EndingName : "???";
            }

            if (detailDescription != null)
            {
                detailDescription.text = isUnlocked ? ending.Description : "尚未解锁";
            }

            if (detailImage != null && ending.BackgroundImage != null)
            {
                detailImage.sprite = ending.BackgroundImage;
                detailImage.color = isUnlocked ? Color.white : lockedColor;
            }

            if (detailCondition != null)
            {
                detailCondition.text = isUnlocked ? ending.GetUnlockConditionText() : ending.UnlockHint ?? "???";
            }

            if (lockButton != null)
            {
                lockButton.gameObject.SetActive(!isUnlocked);
            }
        }

        private void UpdateNavigationButtons()
        {
            int totalPages = Mathf.CeilToInt((float)displayedEndings.Count / itemsPerPage);

            if (prevButton != null)
            {
                prevButton.interactable = currentPage > 0;
            }

            if (nextButton != null)
            {
                nextButton.interactable = currentPage < totalPages - 1;
            }
        }

        private void UpdateProgress()
        {
            if (progressText == null)
                return;

            int unlocked = 0;
            int total = allEndings.Count;

            foreach (var ending in allEndings)
            {
                if (ending != null && EndingManager.Instance != null && EndingManager.Instance.IsEndingUnlocked(ending.Id))
                {
                    unlocked++;
                }
            }

            progressText.text = $"收集进度：{unlocked}/{total} ({(total > 0 ? (float)unlocked / total * 100 : 0):F1}%)";
        }

        private void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdateGrid();
                UpdateNavigationButtons();
            }
        }

        private void NextPage()
        {
            int totalPages = Mathf.CeilToInt((float)displayedEndings.Count / itemsPerPage);
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                UpdateGrid();
                UpdateNavigationButtons();
            }
        }

        private void ShowUnlockHint()
        {
            if (selectedEnding != null && !string.IsNullOrEmpty(selectedEnding.UnlockHint))
            {
                Debug.Log($"[EndingGallery] 解锁提示: {selectedEnding.UnlockHint}");
            }
        }

        private void CloseGallery()
        {
            StartCoroutine(FadeOutAndClose());
        }

        private IEnumerator FadeOut()
        {
            if (canvasGroup == null)
                yield break;

            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = 1f - (elapsed / duration);
                yield return null;
            }
        }

        private IEnumerator FadeOutAndClose()
        {
            yield return StartCoroutine(FadeOut());
            gameObject.SetActive(false);
            OnGalleryClosed?.Invoke();
        }
    }
}
