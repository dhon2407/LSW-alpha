using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using Cooking.Recipe;
using GameTime;
using UnityEngine.SceneManagement;
using InventorySystem;
using PlayerStats;

using CharacterVisual = CharacterData.Wrappers.CharacterInfoWrapper;

namespace GameFile
{
    [DefaultExecutionOrder(100)]
    public class SaveManager : MonoBehaviour
    {
        private readonly int MainGameBuildIndex = 1;
        private static SaveManager instance;

        public float PlayTime;
        public string FileName => CurrentSaveName;
        public SaveData Data => CurrentSaveData;

        private string CurrentSaveName = null;
        private SaveData CurrentSaveData = null;
        private bool onPlay;

        private string filePath => Application.persistentDataPath + "/" + CurrentSaveName + Save.fileExtension;

        

#if UNITY_EDITOR && ODIN_SUPPORTED
        [Sirenix.OdinInspector.Button]
        public void ToggleAllowSaving()
        {
            bool currentState = UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true);
            UnityEditor.EditorPrefs.SetBool("ShouldGameSave", !currentState);
            Debug.Log("Save/Load allowed: " + !currentState);
        }
#endif
        public void SetCurrentSave(string filename, SaveData data)
        {
            CurrentSaveName = filename;
            CurrentSaveData = data;
        }

        public void LoadGame()
        {

#if UNITY_EDITOR && ODIN_SUPPORTED
            // Specific so game won't save/load for Lyrcaxis
            if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", true))
            {
                Debug.Log("Not Loading :)");
                return;
            }
#endif

            if (CurrentSaveData != null)
            {
                PlayTime = CurrentSaveData.RealPlayTime;
                GameLibOfMethods.player.transform.localPosition = new Vector3(CurrentSaveData.playerX, CurrentSaveData.playerY, 1);

                GameTime.Clock.SetTime(CurrentSaveData.time);
                GameTime.Calendar.Initialize(CurrentSaveData.days, CurrentSaveData.weekday, CurrentSaveData.season);
                Weather.WeatherSystem.Initialize(CurrentSaveData.weather);

                Stats.SetMoney = CurrentSaveData.money;
                Bank.Instance.MoneyInBank = (float)CurrentSaveData.moneyInBank;
                Bank.Instance.PercentagePerDay = CurrentSaveData.percentagePerDay;
                Bank.Instance.UpdateBalance();
                Stats.XpMultiplier = CurrentSaveData.xpMultiplayer;
                Stats.PriceMultiplier = CurrentSaveData.priceMultiplayer;
                Stats.RepairSpeed = CurrentSaveData.repairSpeed;

                SpriteControler.Instance.visuals = CurrentSaveData.characterVisuals.GetVisuals();

                Inventory.Initialize(CurrentSaveData.inventoryItems, CurrentSaveData.containerItems);

                JobManager.JobData = CurrentSaveData.job;

                Stats.Initialize(CurrentSaveData.playerSkillsData, CurrentSaveData.playerStatusData);

                if (CurrentSaveData.CompletedMissions != null)
                    MissionHandler.CompletedMissions = new List<string>(CurrentSaveData.CompletedMissions);
                if (CurrentSaveData.CurrentMissions != null)
                    MissionHandler.CurrentMissions = new List<string>(CurrentSaveData.CurrentMissions);

                MissionHandler.missionHandler.ReactivateOld();

                //henrique - working on the save system
                //Debug.LogWarning("Upgrades not loading.");
                //Upgrades.SetData(CurrentSaveData.upgrades);
                if (CurrentSaveData.upgrades != null)
                {
                    StartCoroutine(LoadUpgrades(CurrentSaveData));
                }

                HomePCInteractions.instance.savedProgress = CurrentSaveData.computerActivitiesProgress;

                CookingHandler.SetCookedRecipes(CurrentSaveData.cookedRecipes);

                CareerUi.Instance.UpdateJobUi();
            }
            else if (Stats.Ready) //Loaded directly from game scene
            {
                Stats.Initialize();
                Inventory.Initialize();
                CookingHandler.SetCookedRecipes(null);
                CareerUi.Instance.UpdateJobUi();
            }

            onPlay = true;
            StatusUIUpdater.UpdateEverything();
        }

        IEnumerator LoadUpgrades(SaveData save)
        {
            yield return new WaitForSeconds(.2f);
            var validators = FindObjectsOfType<Objects.Utilities.UpgradeValidator>(); //henrique - working on the save system
            foreach (var item in validators)
            {
                if (!save.upgrades.ContainsKey(item.ValidatorType)) break;
                Objects.Utilities.UpgradeValidator.UpgradeObjectSettings step = item.ObjectSettings[save.upgrades[item.ValidatorType]];
                item.ApplyUpgrade(step.Prefab);
            }
        }

        public void CreateSaveFile(string filename, CharacterData.CharacterInfo charInfo)
        {
            CurrentSaveName = filename;
            CurrentSaveData = new SaveData
            {
                characterVisuals = new CharacterVisual(charInfo),
                time = 28800f, // 28800 seconds = 8hrs = 8am
                season = Calendar.Season.Spring,
            };

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            DataFormat format = DataFormat.Binary;
            var bytes = SerializationUtility.SerializeValue(CurrentSaveData, format);
            File.WriteAllBytes(filePath, bytes);
        }

        public void SaveGame()
        {
            #region Specific so game won't save/load for Lyrcaxis
#if UNITY_EDITOR && ODIN_SUPPORTED
            // Specific so game won't save/load for Lyrcaxis
            if (!UnityEditor.EditorPrefs.GetBool("ShouldGameSave", false))
            {
                Debug.Log("Not Saving :)");
                return;
            }
#endif
            #endregion

            CurrentSaveData = CreateSaveGameObject();

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            DataFormat format = DataFormat.Binary;
            var bytes = SerializationUtility.SerializeValue(CurrentSaveData, format);
            File.WriteAllBytes(filePath, bytes);
        }

        private SaveData CreateSaveGameObject()
        {
            Debug.LogWarning("Upgrades not saving.");

            return new SaveData
            {
                RealPlayTime = PlayTime,

                playerX = GameLibOfMethods.player.transform.localPosition.x,
                playerY = GameLibOfMethods.player.transform.localPosition.y,

                time = GameTime.Clock.Time,
                days = GameTime.Calendar.Day,
                season = GameTime.Calendar.CurrentSeason,
                weekday = GameTime.Calendar.CurrentWeekday,
                weather = Weather.WeatherSystem.CurrentSaveWeather,

                money = Stats.Money,
                moneyInBank = Bank.Instance.MoneyInBank,
                percentagePerDay = Bank.Instance.PercentagePerDay,
                xpMultiplayer = Stats.XpMultiplier,
                priceMultiplayer = Stats.PriceMultiplier,
                repairSpeed = Stats.RepairSpeed,

                characterVisuals = new CharacterVisual(SpriteControler.Instance.visuals),

                inventoryItems = Inventory.BagItems,
                containerItems = Inventory.ContainerContents,

                job = JobManager.JobData,

                playerSkillsData = Stats.SkillsData,
                playerStatusData = Stats.StatusData,
                CompletedMissions = new List<string>(MissionHandler.CompletedMissions),
                CurrentMissions = new List<string>(MissionHandler.CurrentMissions),

                upgrades = new Dictionary<Objects.Utilities.UpgradeType, int>(UpgradesSavHelper()),
                computerActivitiesProgress = HomePCInteractions.instance.savedProgress,

                cookedRecipes = CookingHandler.CookedRecipes,
            };
        }

        public Dictionary<Objects.Utilities.UpgradeType, int> UpgradesSavHelper() //henrique - fixing the loaing upgrades
        {
            var currentUpgrades = new Dictionary<Objects.Utilities.UpgradeType, int>();
            var validators = FindObjectsOfType<Objects.Utilities.UpgradeValidator>(); 
            foreach (var item in validators)
            {
                currentUpgrades.Add(item.ValidatorType, item.ObjectSettings.FindIndex(x => x.Prefab == item.CurrentObject));
            }
            return currentUpgrades;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
            GameFile.Data.Initialize(this);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            onPlay = false;
            SceneManager.activeSceneChanged +=
             delegate
             {
                 if (SceneManager.GetActiveScene().buildIndex == MainGameBuildIndex)
                     LoadGame();
             };

            //LOADED DIRECTLY FROM MAIN SCENE
            if (CurrentSaveData == null && SceneManager.GetActiveScene().buildIndex == MainGameBuildIndex)
                LoadGame();
        }

        private void FixedUpdate()
        {
            if (onPlay)
                PlayTime += GameTime.Time.deltaTime;
        }
    }

    public static class Data
    {
        private static SaveManager manager = null;

        public static string TutorialKey { get; } = "Show Tutorial";

        public static bool Ready => (manager != null);

        public static SaveData CurrentSaveData => manager.Data;
        public static string Filename => manager.FileName;

        public static void Initialize(SaveManager saveManager)
        {
            manager = saveManager;
        }

        public static void Set(SaveData data, string filename)
        {
            manager.SetCurrentSave(filename, data);
        }

        public static void Save()
        {
            manager.SaveGame();
        }

        public static void Create(string filename, CharacterData.CharacterInfo charInfo)
        {
            PlayerPrefs.SetInt(TutorialKey, 1);
            manager.CreateSaveFile(filename, charInfo);
        }
    }
}