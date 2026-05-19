using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Appearance
{
    [Serializable]
    public class CharacterColor
    {
        public Color hairColor = Color.black;
        public Color eyeColor = Color.black;
        public Color skinColor = new Color(1f, 0.9f, 0.8f);
        public Color outfitColor = Color.white;
    }

    [Serializable]
    public class CharacterBodyData
    {
        public float height = 1.7f;
        public float weight = 65f;
        public float bustSize = 0f;
        public float shoulderWidth = 0.45f;
    }

    [Serializable]
    public class CharacterFaceData
    {
        public float eyeSize = 1f;
        public float eyeDistance = 0.035f;
        public float noseSize = 1f;
        public float mouthSize = 1f;
        public float faceShape = 0.5f;
    }

    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class CharacterAppearance : MonoBehaviour
    {
        [Header("外观数据")]
        [SerializeField] private string characterId;
        [SerializeField] private CharacterColor colors = new CharacterColor();
        [SerializeField] private CharacterBodyData bodyData = new CharacterBodyData();
        [SerializeField] private CharacterFaceData faceData = new CharacterFaceData();

        [Header("服装配置")]
        [SerializeField] private List<OutfitConfig> availableOutfits = new List<OutfitConfig>();
        [SerializeField] private int currentOutfitIndex = 0;

        [Header("配饰配置")]
        [SerializeField] private List<AccessoryConfig> accessories = new List<AccessoryConfig>();
        [SerializeField] private bool showGlasses = false;
        [SerializeField] private bool showHat = false;
        [SerializeField] private bool showEarrings = false;

        [Header("渲染器引用")]
        [SerializeField] private SkinnedMeshRenderer bodyRenderer;
        [SerializeField] private SkinnedMeshRenderer outfitRenderer;
        [SerializeField] private List<Renderer> accessoryRenderers = new List<Renderer>();

        [Serializable]
        public class OutfitConfig
        {
            public string outfitName;
            public Material outfitMaterial;
            public Color primaryColor;
            public Color secondaryColor;
        }

        [Serializable]
        public class AccessoryConfig
        {
            public string accessoryName;
            public GameObject accessoryPrefab;
            public Transform attachPoint;
            public bool isActive = false;
        }

        private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();

        protected virtual void Awake()
        {
            InitializeRenderers();
            InitializeMaterials();
        }

        private void InitializeRenderers()
        {
            if (bodyRenderer == null)
            {
                bodyRenderer = GetComponent<SkinnedMeshRenderer>();
            }

            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.tag == "Outfit")
                {
                    outfitRenderer = renderer;
                }
            }
        }

        private void InitializeMaterials()
        {
            if (bodyRenderer != null)
            {
                Material bodyMat = new Material(bodyRenderer.material);
                materialCache["body"] = bodyMat;
                bodyRenderer.material = bodyMat;
            }

            if (outfitRenderer != null)
            {
                Material outfitMat = new Material(outfitRenderer.material);
                materialCache["outfit"] = outfitMat;
                outfitRenderer.material = outfitMat;
            }
        }

        public virtual void ApplyAppearance()
        {
            ApplyBodyColors();
            ApplyBodyShape();
            ApplyFaceFeatures();
            ApplyOutfit();
            ApplyAccessories();
        }

        protected virtual void ApplyBodyColors()
        {
            if (materialCache.ContainsKey("body"))
            {
                Material mat = materialCache["body"];
                mat.SetColor("_Color", colors.skinColor);
                mat.SetColor("_HairColor", colors.hairColor);
            }
        }

        protected virtual void ApplyBodyShape()
        {
            if (bodyRenderer == null) return;

            bodyRenderer.transform.localScale = new Vector3(
                1f + (bodyData.shoulderWidth - 0.45f) * 0.2f,
                bodyData.height / 1.7f,
                1f
            );
        }

        protected virtual void ApplyFaceFeatures()
        {
        }

        public virtual void ChangeHairColor(Color newColor)
        {
            colors.hairColor = newColor;
            if (materialCache.ContainsKey("body"))
            {
                materialCache["body"].SetColor("_HairColor", newColor);
            }
        }

        public virtual void ChangeEyeColor(Color newColor)
        {
            colors.eyeColor = newColor;
        }

        public virtual void ChangeOutfit(int outfitIndex)
        {
            if (outfitIndex >= 0 && outfitIndex < availableOutfits.Count)
            {
                currentOutfitIndex = outfitIndex;
                ApplyOutfit();
            }
        }

        protected virtual void ApplyOutfit()
        {
            if (currentOutfitIndex >= 0 && currentOutfitIndex < availableOutfits.Count)
            {
                OutfitConfig outfit = availableOutfits[currentOutfitIndex];
                if (materialCache.ContainsKey("outfit"))
                {
                    Material mat = materialCache["outfit"];
                    mat.SetColor("_Color", outfit.primaryColor);
                }
                colors.outfitColor = outfit.primaryColor;
            }
        }

        public virtual void ToggleAccessory(string accessoryName, bool show)
        {
            switch (accessoryName.ToLower())
            {
                case "glasses":
                    showGlasses = show;
                    break;
                case "hat":
                    showHat = show;
                    break;
                case "earrings":
                    showEarrings = show;
                    break;
            }
            ApplyAccessories();
        }

        protected virtual void ApplyAccessories()
        {
            if (accessories == null || accessories.Count == 0) return;

            foreach (var accessory in accessories)
            {
                if (accessory.accessoryPrefab != null)
                {
                    bool shouldShow = false;
                    switch (accessory.accessoryName.ToLower())
                    {
                        case "glasses":
                            shouldShow = showGlasses;
                            break;
                        case "hat":
                            shouldShow = showHat;
                            break;
                        case "earrings":
                            shouldShow = showEarrings;
                            break;
                    }

                    if (accessory.accessoryPrefab.activeSelf != shouldShow)
                    {
                        accessory.accessoryPrefab.SetActive(shouldShow);
                    }
                }
            }
        }

        public virtual void AddAccessory(AccessoryConfig newAccessory)
        {
            if (!accessories.Exists(a => a.accessoryName == newAccessory.accessoryName))
            {
                accessories.Add(newAccessory);
            }
        }

        public virtual void RemoveAccessory(string accessoryName)
        {
            accessories.RemoveAll(a => a.accessoryName == accessoryName);
        }

        public virtual void SetCharacterId(string id)
        {
            characterId = id;
        }

        public virtual string GetCharacterId()
        {
            return characterId;
        }

        #region 属性访问器

        public CharacterColor Colors => colors;
        public CharacterBodyData BodyData => bodyData;
        public CharacterFaceData FaceData => faceData;
        public int CurrentOutfitIndex => currentOutfitIndex;
        public int OutfitCount => availableOutfits.Count;

        #endregion

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public virtual void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var mat in materialCache.Values)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }
            materialCache.Clear();
        }
    }
}
