using InventorySystem;
using System.Collections.Generic;
using GameTime;
using UnityEngine;

using CharacterVisual = CharacterData.Wrappers.CharacterInfoWrapper;
using ItemList = System.Collections.Generic.List<InventorySystem.ItemList.ItemInfo>;
using PlayerSkillsData = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill.Data>;
using PlayerStatusData = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status.Data>;

namespace GameFile
{
    [System.Serializable]
    public class SaveData
    {
        public float RealPlayTime;
        
        public float playerX;
        public float playerY;
        
        public float time;
        public int days = 1;
        public Calendar.Season season;
        public Calendar.Weekday weekday;
        public int weather;

        public float money = 2000;
        public double moneyInBank;
        public float percentagePerDay;
        public float xpMultiplayer = 1;
        public float priceMultiplayer = 1;
        public float repairSpeed;

        public CharacterVisual characterVisuals;

        public ItemList inventoryItems;
        public Dictionary<int, ItemList> containerItems;

        public JobData job;

        public PlayerSkillsData playerSkillsData;
        public PlayerStatusData playerStatusData;
      
        public List<string> CompletedMissions = new List<string>();
        public List<string> CurrentMissions = new List<string>();

        public Dictionary<Objects.Utilities.UpgradeType, int> upgrades; //henrique - working on the save system

        public List<ItemCode> cookedRecipes;

        public float[] computerActivitiesProgress;
    }

    public static class Save
    {
        public static readonly string fileExtension = ".save_";
    }
}
