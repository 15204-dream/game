using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PixelArtLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public Image heartIcon;
    public Image starIcon;
    public Image diamondIcon;
    public Image messageIcon;
    
    [Header("Character Portraits")]
    public Image[] characterPortraits;
    
    [Header("Scene Background")]
    public Image sceneBackground;
    
    [Header("Director Icons")]
    public Image maleDirectorIcon;
    public Image femaleDirectorIcon;
    
    private void Start()
    {
        StartCoroutine(LoadResourcesAsync());
    }
    
    private IEnumerator LoadResourcesAsync()
    {
        yield return new WaitForEndOfFrame();
        
        if (PixelArtResourceManager.Instance != null)
        {
            LoadUIIcons();
            LoadCharacterPortraits();
            LoadDirectorIcons();
            LoadSceneBackground();
        }
        else
        {
            Debug.LogError("PixelArtResourceManager not found in scene");
        }
    }
    
    private void LoadUIIcons()
    {
        if (heartIcon != null)
            heartIcon.sprite = PixelArtResourceManager.Instance.GetIcon("heart");
        
        if (starIcon != null)
            starIcon.sprite = PixelArtResourceManager.Instance.GetIcon("star");
        
        if (diamondIcon != null)
            diamondIcon.sprite = PixelArtResourceManager.Instance.GetIcon("diamond");
        
        if (messageIcon != null)
            messageIcon.sprite = PixelArtResourceManager.Instance.GetIcon("message");
    }
    
    private void LoadCharacterPortraits()
    {
        for (int i = 0; i < characterPortraits.Length; i++)
        {
            Sprite sprite = PixelArtResourceManager.Instance.GetCharacterById(i);
            if (sprite != null && characterPortraits[i] != null)
            {
                characterPortraits[i].sprite = sprite;
                characterPortraits[i].preserveAspect = true;
            }
        }
    }
    
    private void LoadDirectorIcons()
    {
        if (maleDirectorIcon != null)
            maleDirectorIcon.sprite = PixelArtResourceManager.Instance.GetDirectorSprite(false);
        
        if (femaleDirectorIcon != null)
            femaleDirectorIcon.sprite = PixelArtResourceManager.Instance.GetDirectorSprite(true);
    }
    
    private void LoadSceneBackground()
    {
        if (sceneBackground != null)
        {
            sceneBackground.sprite = PixelArtResourceManager.Instance.GetScene("sunset_villa");
            sceneBackground.preserveAspect = false;
        }
    }
    
    public void ChangeSceneBackground(string sceneName)
    {
        if (sceneBackground != null)
        {
            Sprite sprite = PixelArtResourceManager.Instance.GetScene(sceneName);
            if (sprite != null)
            {
                sceneBackground.sprite = sprite;
            }
        }
    }
    
    public void SetPlayerPortrait(bool isFemale, bool isAlt, Image targetImage)
    {
        if (targetImage != null)
        {
            Sprite sprite = PixelArtResourceManager.Instance.GetPlayerSprite(isFemale, isAlt);
            if (sprite != null)
            {
                targetImage.sprite = sprite;
                targetImage.preserveAspect = true;
            }
        }
    }
}