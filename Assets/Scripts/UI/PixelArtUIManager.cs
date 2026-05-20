using UnityEngine;
using UnityEngine.UI;
using System;

public class PixelArtUIManager : MonoBehaviour
{
    [Header("Pixel Style Settings")]
    public int pixelScale = 4;
    public Color primaryColor = new Color(1f, 0.427f, 0.616f);
    public Color secondaryColor = new Color(1f, 0.843f, 0f);
    public Color backgroundColor = new Color(0.961f, 0.961f, 0.824f);
    
    [Header("UI References")]
    public Button[] pixelButtons;
    public Text[] pixelTexts;
    public Image[] pixelImages;
    
    private void Awake()
    {
        ApplyPixelStyle();
    }
    
    private void ApplyPixelStyle()
    {
        ApplyButtonStyle();
        ApplyTextStyle();
        ApplyImageStyle();
    }
    
    private void ApplyButtonStyle()
    {
        foreach (Button button in pixelButtons)
        {
            if (button != null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = primaryColor;
                    ApplyPixelBorder(image);
                }
                
                Text text = button.GetComponentInChildren<Text>();
                if (text != null)
                {
                    ApplyPixelFont(text);
                }
            }
        }
    }
    
    private void ApplyTextStyle()
    {
        foreach (Text text in pixelTexts)
        {
            if (text != null)
            {
                ApplyPixelFont(text);
            }
        }
    }
    
    private void ApplyImageStyle()
    {
        foreach (Image image in pixelImages)
        {
            if (image != null)
            {
                ApplyPixelBorder(image);
            }
        }
    }
    
    private void ApplyPixelFont(Text text)
    {
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 12 * pixelScale;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
    }
    
    private void ApplyPixelBorder(Image image)
    {
        RectTransform rt = image.rectTransform;
        rt.sizeDelta = new Vector2(
            Mathf.Round(rt.sizeDelta.x / pixelScale) * pixelScale,
            Mathf.Round(rt.sizeDelta.y / pixelScale) * pixelScale
        );
    }
    
    public void SetHeartIcon(Image target)
    {
        if (target != null)
        {
            target.sprite = PixelArtResourceManager.Instance.GetIcon("heart");
            target.color = Color.white;
        }
    }
    
    public void SetStarIcon(Image target)
    {
        if (target != null)
        {
            target.sprite = PixelArtResourceManager.Instance.GetIcon("star");
            target.color = Color.white;
        }
    }
    
    public void SetDiamondIcon(Image target)
    {
        if (target != null)
        {
            target.sprite = PixelArtResourceManager.Instance.GetIcon("diamond");
            target.color = Color.white;
        }
    }
    
    public void SetCharacterPortrait(int characterId, Image target)
    {
        if (target != null)
        {
            Sprite sprite = PixelArtResourceManager.Instance.GetCharacterById(characterId);
            if (sprite != null)
            {
                target.sprite = sprite;
                target.preserveAspect = true;
            }
        }
    }
    
    public void SetSceneBackground(string sceneName, Image target)
    {
        if (target != null)
        {
            Sprite sprite = PixelArtResourceManager.Instance.GetScene(sceneName);
            if (sprite != null)
            {
                target.sprite = sprite;
                target.preserveAspect = false;
            }
        }
    }
    
    public void CreatePixelButton(Transform parent, Vector2 position, string text, Action onClick)
    {
        GameObject buttonObj = new GameObject("PixelButton");
        buttonObj.transform.SetParent(parent);
        
        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(128 * pixelScale, 32 * pixelScale);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = primaryColor;
        
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchoredPosition = Vector2.zero;
        textRt.sizeDelta = rt.sizeDelta;
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        ApplyPixelFont(textComp);
        textComp.alignment = TextAnchor.MiddleCenter;
    }
}