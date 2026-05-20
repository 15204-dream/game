using UnityEngine;
using System.Collections.Generic;

public class PixelArtResourceManager : MonoBehaviour
{
    public static PixelArtResourceManager Instance { get; private set; }
    
    [Header("Pixel Icons")]
    public Sprite[] uiIcons;
    
    [Header("Character Sprites")]
    public Sprite[] characterSprites;
    
    [Header("Scene Backgrounds")]
    public Sprite[] sceneBackgrounds;
    
    private Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> characterDictionary = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> sceneDictionary = new Dictionary<string, Sprite>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDictionaries()
    {
        InitializeIconDictionary();
        InitializeCharacterDictionary();
        InitializeSceneDictionary();
    }
    
    private void InitializeIconDictionary()
    {
        string[] iconNames = {
            "heart", "star", "diamond", "message", "clock", "settings", "home",
            "heart_outline", "camera", "users", "star_outline", "gift", "phone",
            "trophy", "menu", "play", "pause", "volume", "mute", "lightbulb", "help"
        };
        
        for (int i = 0; i < iconNames.Length && i < uiIcons.Length; i++)
        {
            iconDictionary[iconNames[i]] = uiIcons[i];
        }
    }
    
    private void InitializeCharacterDictionary()
    {
        string[] characterNames = {
            "male_1", "male_2", "male_3", "male_4", "male_5", "male_6",
            "female_1", "female_2", "female_3", "female_4", "female_5", "female_6",
            "director_male", "director_female",
            "player_male", "player_female", "player_male_alt", "player_female_alt"
        };
        
        for (int i = 0; i < characterNames.Length && i < characterSprites.Length; i++)
        {
            characterDictionary[characterNames[i]] = characterSprites[i];
        }
    }
    
    private void InitializeSceneDictionary()
    {
        string[] sceneNames = {
            "sunset_villa", "living_room", "date_beach"
        };
        
        for (int i = 0; i < sceneNames.Length && i < sceneBackgrounds.Length; i++)
        {
            sceneDictionary[sceneNames[i]] = sceneBackgrounds[i];
        }
    }
    
    public Sprite GetIcon(string iconName)
    {
        if (iconDictionary.TryGetValue(iconName, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Icon '{iconName}' not found in dictionary");
        return null;
    }
    
    public Sprite GetCharacter(string characterName)
    {
        if (characterDictionary.TryGetValue(characterName, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Character '{characterName}' not found in dictionary");
        return null;
    }
    
    public Sprite GetScene(string sceneName)
    {
        if (sceneDictionary.TryGetValue(sceneName, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Scene '{sceneName}' not found in dictionary");
        return null;
    }
    
    public Sprite GetCharacterById(int characterId)
    {
        string[] characterIds = {
            "male_1", "male_2", "male_3", "male_4", "male_5", "male_6",
            "female_1", "female_2", "female_3", "female_4", "female_5", "female_6"
        };
        
        if (characterId >= 0 && characterId < characterIds.Length)
        {
            return GetCharacter(characterIds[characterId]);
        }
        return null;
    }
    
    public Sprite GetPlayerSprite(bool isFemale, bool isAlt)
    {
        string key = isFemale ? 
            (isAlt ? "player_female_alt" : "player_female") : 
            (isAlt ? "player_male_alt" : "player_male");
        return GetCharacter(key);
    }
    
    public Sprite GetDirectorSprite(bool isFemale)
    {
        string key = isFemale ? "director_female" : "director_male";
        return GetCharacter(key);
    }
}