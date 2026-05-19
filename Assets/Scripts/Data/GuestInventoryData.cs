using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        Gift,
        Clothing,
        Accessory,
        Tool,
        Consumable,
        KeyItem,
        QuestItem,
        Decoration
    }

    /// <summary>
    /// 物品稀有度
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Unique
    }

    /// <summary>
    /// 物品数据
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        public string itemId;
        public string itemName;
        public string description;
        public ItemType itemType;
        public ItemRarity rarity;
        public string iconPath;
        public int maxStack;
        public int buyPrice;
        public int sellPrice;
        public bool isTradeable;
        public bool isDestroyable;
        public List<string> tags = new List<string>();
        public List<ItemEffect> effects = new List<ItemEffect>();
        public string usageHint;
    }

    /// <summary>
    /// 物品效果
    /// </summary>
    [System.Serializable]
    public class ItemEffect
    {
        public EffectTarget target;
        public EffectType effectType;
        public float value;
        public int duration;
        public string description;
    }

    /// <summary>
    /// 效果目标
    /// </summary>
    public enum EffectTarget
    {
        Self,
        TargetCharacter,
        AllCharacters,
        Environment
    }

    /// <summary>
    /// 背包槽位数据
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        public int slotId;
        public string itemId;
        public int quantity;
        public bool isLocked;
        public bool isEmpty => string.IsNullOrEmpty(itemId);
    }

    /// <summary>
    /// 服装装备槽位
    /// </summary>
    [System.Serializable]
    public class EquipmentSlot
    {
        public EquipmentSlotType slotType;
        public string equippedItemId;
        public bool isEmpty => string.IsNullOrEmpty(equippedItemId);
    }

    /// <summary>
    /// 装备槽位类型
    /// </summary>
    public enum EquipmentSlotType
    {
        Head,
        Top,
        Bottom,
        Shoes,
        Accessory1,
        Accessory2,
        Outfit
    }

    /// <summary>
    /// 时装套装数据
    /// </summary>
    [System.Serializable]
    public class OutfitSet
    {
        public string setId;
        public string setName;
        public List<string> itemIds = new List<string>();
        public List<ItemEffect> setBonusEffects = new List<ItemEffect>();
        public int completionBonus;
        public bool isUnlocked;
        public int equippedPieces;
    }

    /// <summary>
    /// 收藏品数据
    /// </summary>
    [System.Serializable]
    public class CollectibleData
    {
        public string collectibleId;
        public string collectibleName;
        public CollectibleCategory category;
        public int totalInCategory;
        public int collectedCount;
        public List<string> collectedIds = new List<string>();
        public float completionPercentage;
    }

    /// <summary>
    /// 收藏品分类
    /// </summary>
    public enum CollectibleCategory
    {
        Gift,
        Clothing,
        Photo,
        Memory,
        Achievement
    }

    /// <summary>
    /// 礼物偏好数据
    /// </summary>
    [System.Serializable]
    public class GiftPreference
    {
        public string characterId;
        public List<string> lovedGiftTags = new List<string>();
        public List<string> likedGiftTags = new List<string>();
        public List<string> neutralGiftTags = new List<string>();
        public List<string> dislikedGiftTags = new List<string>();
        public List<string> hatedGiftTags = new List<string>();
        public List<string> favoriteItems = new List<string>();
        public List<string> leastFavoriteItems = new List<string>();
    }

    /// <summary>
    /// 背包数据 - 嘉宾模式使用
    /// </summary>
    [System.Serializable]
    public class GuestInventoryData
    {
        [Header("背包容量")]
        public int maxInventorySlots = 50;
        public int currentSlotCount;
        public List<InventorySlot> inventorySlots = new List<InventorySlot>();

        [Header("装备槽位")]
        public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
        public OutfitSet currentOutfit = null;

        [Header("时装收藏")]
        public List<OutfitSet> unlockedOutfitSets = new List<OutfitSet>();
        public List<string> ownedClothingItems = new List<string>();
        public List<string> equippedCostumeIds = new List<string>();

        [Header("收藏品")]
        public List<CollectibleData> collectibles = new List<CollectibleData>();
        public int totalCollectiblesFound;
        public int totalCollectibles;

        [Header("礼物数据")]
        public List<GiftPreference> characterGiftPreferences = new List<GiftPreference>();
        public int totalGiftsGiven;
        public int successfulGifts;

        [Header("货币和资源")]
        public int coins;
        public int gems;
        public int hearts;
        public int starPoints;

        [Header("特殊物品")]
        public List<string> keyItems = new List<string>();
        public List<string> questItems = new List<string>();
        public List<string> achievements = new List<string>();

        [Header("消耗品使用统计")]
        public Dictionary<string, int> consumableUsageCount = new Dictionary<string, int>();
        public int totalConsumablesUsed;

        [Header("商店数据")]
        public List<string> purchasedItemIds = new List<string>();
        public Dictionary<string, int> purchaseCount = new Dictionary<string, int>();
        public int totalSpent;
        public int totalEarned;
    }

    /// <summary>
    /// 背包工具类
    /// </summary>
    public static class GuestInventoryHelper
    {
        public static bool AddItemToInventory(GuestInventoryData inventory, string itemId, int quantity)
        {
            var existingSlot = inventory.inventorySlots.Find(s => s.itemId == itemId && !s.isEmpty);
            if (existingSlot != null)
            {
                existingSlot.quantity += quantity;
                return true;
            }

            var emptySlot = inventory.inventorySlots.Find(s => s.isEmpty);
            if (emptySlot != null)
            {
                emptySlot.itemId = itemId;
                emptySlot.quantity = quantity;
                return true;
            }

            if (inventory.inventorySlots.Count < inventory.maxInventorySlots)
            {
                var newSlot = new InventorySlot
                {
                    slotId = inventory.inventorySlots.Count,
                    itemId = itemId,
                    quantity = quantity
                };
                inventory.inventorySlots.Add(newSlot);
                return true;
            }

            return false;
        }

        public static bool RemoveItemFromInventory(GuestInventoryData inventory, string itemId, int quantity)
        {
            var slot = inventory.inventorySlots.Find(s => s.itemId == itemId && !s.isEmpty);
            if (slot == null || slot.quantity < quantity)
            {
                return false;
            }

            slot.quantity -= quantity;
            if (slot.quantity <= 0)
            {
                slot.itemId = null;
                slot.quantity = 0;
            }

            return true;
        }

        public static int GetItemCount(GuestInventoryData inventory, string itemId)
        {
            var slot = inventory.inventorySlots.Find(s => s.itemId == itemId && !s.isEmpty);
            return slot?.quantity ?? 0;
        }

        public static ItemRarity GetRarityColor(ItemRarity rarity)
        {
            return rarity;
        }

        public static int GetRaritySellMultiplier(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 1;
                case ItemRarity.Uncommon: return 2;
                case ItemRarity.Rare: return 5;
                case ItemRarity.Epic: return 10;
                case ItemRarity.Legendary: return 25;
                case ItemRarity.Unique: return 50;
                default: return 1;
            }
        }

        public static bool CanEquipItem(EquipmentSlotType slotType, ItemData item, GuestInventoryData inventory)
        {
            if (item.itemType != ItemType.Clothing && item.itemType != ItemType.Accessory)
            {
                return false;
            }

            return true;
        }
    }
}
