using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 对话框组件 - 角色对话和旁白显示
/// </summary>
public class UIDialogueBox : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerPortraitImage;
    [SerializeField] private Image nameTagImage;
    [SerializeField] private Image typingIndicator;
    
    [Header("对话框配置")]
    [SerializeField] private DialogueBoxType boxType = DialogueBoxType.Normal;
    [SerializeField] private bool showSpeakerName = true;
    [SerializeField] private bool showPortrait = true;
    [SerializeField] private TextAnchor textAlignment = TextAnchor.MiddleCenter;
    
    [Header("打字机效果配置")]
    [SerializeField] private bool useTypewriterEffect = true;
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private float fastTypeSpeed = 0.01f;
    [SerializeField] private bool skipTypingOnClick = true;
    
    [Header("动画配置")]
    [SerializeField] private bool useShowAnimation = true;
    [SerializeField] private bool useHideAnimation = true;
    [SerializeField] private float showAnimationDuration = 0.3f;
    [SerializeField] private float hideAnimationDuration = 0.2f;
    [SerializeField] private AnimationCurve showAnimationCurve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve hideAnimationCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    [Header("视觉配置")]
    [SerializeField] private Color backgroundColor = new Color(1f, 0.95f, 0.95f, 0.95f);
    [SerializeField] private Color borderColor = new Color(1f, 0.7f, 0.8f, 1f);
    [SerializeField] private Color nameTagColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color textColor = new Color(0.3f, 0.2f, 0.25f, 1f);
    [SerializeField] private Color nameTextColor = new Color(1f, 0.4f, 0.6f, 1f);
    
    [Header("位置配置")]
    [SerializeField] private DialogueBoxPosition boxPosition = DialogueBoxPosition.Bottom;
    [SerializeField] private float bottomOffset = 50f;
    [SerializeField] private float sideOffset = 30f;
    
    // 私有变量
    private RectTransform rectTransform;
    private Vector2 showPosition;
    private Vector2 hidePosition;
    private bool isShowing = false;
    private bool isTyping = false;
    private string currentText = "";
    private string displayedText = "";
    private Coroutine typingCoroutine;
    private Coroutine animationCoroutine;
    private Queue<DialogueData> dialogueQueue = new Queue<DialogueData>();
    private bool isProcessingQueue = false;
    
    // 对话框类型
    public enum DialogueBoxType
    {
        Normal,     // 普通对话框
        Narration,  // 旁白框
        Thought,    // 心理活动框
        Whisper,    // 悄悄话框
        Shout       // 大喊框
    }
    
    // 对话框位置
    public enum DialogueBoxPosition
    {
        Top,
        Center,
        Bottom,
        Left,
        Right
    }
    
    // 对话数据
    [System.Serializable]
    public class DialogueData
    {
        public string speakerName;
        public string text;
        public Sprite portrait;
        public DialogueBoxType boxType;
        public System.Action onComplete;
        
        public DialogueData(string name, string dialogue, Sprite portraitSprite = null, DialogueBoxType type = DialogueBoxType.Normal)
        {
            speakerName = name;
            text = dialogue;
            portrait = portraitSprite;
            boxType = type;
        }
    }
    
    // 事件
    public System.Action OnDialogueComplete;
    public System.Action OnDialogueStartTyping;
    public System.Action OnDialogueFinishTyping;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 初始化位置
        CalculatePositions();
        
        // 默认隐藏
        if (useHideAnimation)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
    
    private void Start()
    {
        // 初始化视觉样式
        ApplyVisualStyle();
    }
    
    /// <summary>
    /// 显示对话框
    /// </summary>
    public void Show()
    {
        if (isShowing) return;
        
        gameObject.SetActive(true);
        isShowing = true;
        
        if (useShowAnimation && animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        animationCoroutine = StartCoroutine(ShowAnimation());
    }
    
    /// <summary>
    /// 隐藏对话框
    /// </summary>
    public void Hide()
    {
        if (!isShowing) return;
        
        isShowing = false;
        
        if (useHideAnimation)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(HideAnimation());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 显示单条对话
    /// </summary>
    public void ShowDialogue(string speakerName, string text, Sprite portrait = null)
    {
        ShowDialogue(new DialogueData(speakerName, text, portrait, boxType));
    }
    
    /// <summary>
    /// 显示对话数据
    /// </summary>
    public void ShowDialogue(DialogueData data)
    {
        Show();
        
        // 更新内容
        if (speakerNameText != null)
        {
            speakerNameText.text = data.speakerName;
            speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(data.speakerName) && showSpeakerName);
        }
        
        if (nameTagImage != null)
        {
            nameTagImage.gameObject.SetActive(!string.IsNullOrEmpty(data.speakerName) && showSpeakerName);
        }
        
        if (speakerPortraitImage != null)
        {
            if (data.portrait != null)
            {
                speakerPortraitImage.sprite = data.portrait;
                speakerPortraitImage.gameObject.SetActive(showPortrait);
            }
            else
            {
                speakerPortraitImage.gameObject.SetActive(false);
            }
        }
        
        // 设置对话框类型
        SetBoxType(data.boxType);
        
        // 开始打字效果
        if (useTypewriterEffect)
        {
            StartTypingEffect(data.text, data.onComplete);
        }
        else
        {
            SetDialogueText(data.text);
            if (data.onComplete != null)
            {
                data.onComplete.Invoke();
            }
        }
    }
    
    /// <summary>
    /// 队列显示对话
    /// </summary>
    public void QueueDialogue(string speakerName, string text, Sprite portrait = null)
    {
        dialogueQueue.Enqueue(new DialogueData(speakerName, text, portrait));
        
        if (!isProcessingQueue)
        {
            ProcessQueue();
        }
    }
    
    /// <summary>
    /// 清空对话队列
    /// </summary>
    public void ClearQueue()
    {
        dialogueQueue.Clear();
        isProcessingQueue = false;
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        isTyping = false;
    }
    
    /// <summary>
    /// 设置对话框类型
    /// </summary>
    public void SetBoxType(DialogueBoxType type)
    {
        boxType = type;
        ApplyBoxTypeStyle();
    }
    
    /// <summary>
    /// 设置文本内容
    /// </summary>
    public void SetDialogueText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
            displayedText = text;
        }
    }
    
    /// <summary>
    /// 跳过打字效果
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            SetDialogueText(currentText);
            isTyping = false;
            
            if (typingIndicator != null)
            {
                typingIndicator.gameObject.SetActive(false);
            }
            
            OnDialogueFinishTyping?.Invoke();
        }
    }
    
    /// <summary>
    /// 完成当前对话，继续队列
    /// </summary>
    public void CompleteCurrentDialogue()
    {
        if (isTyping)
        {
            SkipTyping();
        }
        else
        {
            ProcessQueue();
        }
    }
    
    /// <summary>
    /// 是否正在显示对话
    /// </summary>
    public bool IsShowing()
    {
        return isShowing;
    }
    
    /// <summary>
    /// 是否有待处理的对话
    /// </summary>
    public bool HasQueuedDialogue()
    {
        return dialogueQueue.Count > 0;
    }
    
    private void StartTypingEffect(string text, System.Action onComplete = null)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        currentText = text;
        isTyping = true;
        
        if (typingIndicator != null)
        {
            typingIndicator.gameObject.SetActive(true);
        }
        
        typingCoroutine = StartCoroutine(TypewriterEffect(text, onComplete));
    }
    
    private IEnumerator TypewriterEffect(string text, System.Action onComplete)
    {
        displayedText = "";
        dialogueText.text = "";
        
        OnDialogueStartTyping?.Invoke();
        
        for (int i = 0; i < text.Length; i++)
        {
            // 检测是否跳过
            float currentSpeed = skipTypingOnClick && Input.GetMouseButtonDown(0) ? fastTypeSpeed : typeSpeed;
            
            displayedText += text[i];
            dialogueText.text = displayedText;
            
            yield return new WaitForSeconds(currentSpeed);
            
            // 跳过剩余文字
            if (skipTypingOnClick && Input.GetMouseButtonDown(0))
            {
                displayedText = text;
                dialogueText.text = text;
                break;
            }
        }
        
        isTyping = false;
        
        if (typingIndicator != null)
        {
            typingIndicator.gameObject.SetActive(false);
        }
        
        OnDialogueFinishTyping?.Invoke();
        onComplete?.Invoke();
        OnDialogueComplete?.Invoke();
    }
    
    private void ProcessQueue()
    {
        if (dialogueQueue.Count > 0)
        {
            isProcessingQueue = true;
            DialogueData data = dialogueQueue.Dequeue();
            
            System.Action onComplete = ProcessQueue;
            ShowDialogue(new DialogueData(data.speakerName, data.text, data.portrait, data.boxType));
        }
        else
        {
            isProcessingQueue = false;
        }
    }
    
    private void CalculatePositions()
    {
        if (rectTransform == null) return;
        
        switch (boxPosition)
        {
            case DialogueBoxPosition.Bottom:
                showPosition = new Vector2(0, bottomOffset);
                hidePosition = new Vector2(0, -rectTransform.sizeDelta.y);
                break;
            case DialogueBoxPosition.Top:
                showPosition = Vector2.zero;
                hidePosition = new Vector2(0, rectTransform.sizeDelta.y);
                break;
            case DialogueBoxPosition.Left:
                showPosition = Vector2.zero;
                hidePosition = new Vector2(-rectTransform.sizeDelta.x, 0);
                break;
            case DialogueBoxPosition.Right:
                showPosition = Vector2.zero;
                hidePosition = new Vector2(rectTransform.sizeDelta.x, 0);
                break;
            default:
                showPosition = Vector2.zero;
                hidePosition = Vector2.zero;
                break;
        }
    }
    
    private IEnumerator ShowAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        
        while (elapsed < showAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / showAnimationDuration;
            float curveValue = showAnimationCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = endScale;
    }
    
    private IEnumerator HideAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;
        
        while (elapsed < hideAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hideAnimationDuration;
            float curveValue = hideAnimationCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = endScale;
        gameObject.SetActive(false);
    }
    
    private void ApplyVisualStyle()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        
        if (borderImage != null)
        {
            borderImage.color = borderColor;
        }
        
        if (dialogueText != null)
        {
            dialogueText.color = textColor;
            dialogueText.alignment = textAlignment;
        }
        
        if (speakerNameText != null)
        {
            speakerNameText.color = nameTextColor;
        }
        
        if (nameTagImage != null)
        {
            nameTagImage.color = nameTagColor;
        }
    }
    
    private void ApplyBoxTypeStyle()
    {
        switch (boxType)
        {
            case DialogueBoxType.Narration:
                backgroundColor = new Color(1f, 1f, 0.95f, 0.9f);
                textAlignment = TextAnchor.UpperCenter;
                if (speakerNameText != null)
                {
                    speakerNameText.gameObject.SetActive(false);
                }
                break;
                
            case DialogueBoxType.Thought:
                backgroundColor = new Color(0.95f, 0.9f, 1f, 0.9f);
                textAlignment = TextAnchor.MiddleCenter;
                break;
                
            case DialogueBoxType.Whisper:
                backgroundColor = new Color(0.98f, 0.95f, 0.98f, 0.85f);
                textAlignment = TextAnchor.MiddleCenter;
                break;
                
            case DialogueBoxType.Shout:
                backgroundColor = new Color(1f, 0.9f, 0.9f, 0.95f);
                textAlignment = TextAnchor.MiddleCenter;
                break;
        }
        
        ApplyVisualStyle();
    }
}
