using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using PlayerStats.Buffs;
using PlayerStats.Buffs.Core;
using GameClock = GameTime.Clock;
using PlayerSkillsData = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill.Data>;
using PlayerStatusData = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status.Data>;
using PlayerSkills = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill>;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;

namespace PlayerStats
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private static PlayerStatsManager instance;

        [SerializeField]
        private ParticleSystem levelUpParticles = null;

        private PlayerSkills playerSkills;
        private PlayerStatus playerStatus;
        private PlayerStatsUpdater statsUpdater;
        
        public float money { get; private set; } = 2000;
        public float priceMultiplier { get; private set; } = 1;
        public float xpMultiplier { get; private set; } = 1;
        public float bonusXpMultiplier { get; private set; } = 0;
        public float moveSpeed { get; private set; } = 1;
        public float repairSpeed { get; private set; } = 1;
        public int totalLevel { get; private set; } = 0;

        public void Money(float value) => money = value;
        public void PriceMultiplier(float value) => priceMultiplier = value;
        public void XpMultiplier(float value) => xpMultiplier = value;
        public void BonusXpMultiplier(float value) => bonusXpMultiplier = value;
        public void MoveSpeed(float value) => moveSpeed = value;
        public void RepairSpeed(float value) => repairSpeed = value;
        public PlayerStatus PlayerStatus => playerStatus;
        public PlayerSkills PlayerSkills => playerSkills;
        public static Dictionary<Status, bool> statWarning = new Dictionary<Status, bool>();

        public Skill GetSkill(Skill.Type type)
        {
            if (playerSkills.ContainsKey(type))
                return playerSkills[type];

            throw new UnityException($"No such skill[{type}] available");
        }

        public Status GetStatus(Status.Type type)
        {
            if (playerStatus.ContainsKey(type))
                return playerStatus[type];

            throw new UnityException($"No such status[{type}] available");
        }

        public void UpdateData(PlayerSkillsData skillsData, PlayerStatusData statusData)
        {
            playerSkills = InitializeSkills(skillsData);
            playerStatus = InitializeStatus(statusData);
        }
        
        public void PlayLevelUpEffects()
        {
            levelUpParticles.Play();
        }

        public void IncreaseStatusMaxAmount(Status.Type type, float amount)
        {
            playerStatus[type].AddMax(amount);
        }

        public float GetStatusMaxAmount(Status.Type type)
        {
            return playerStatus[type].MaxAmount;
        }

        public class OnSkillChange : UnityEvent<Skill.Type> { }
        public class OnMoneyChange : UnityEvent<float> { }


        public void AddStatusAmount(Status.Type type, float amount)
        {
            playerStatus[type].Add(StatBuffModifier(type, amount));
        }

        public float GetStatusAmount(Status.Type type)
        {
            return playerStatus[type].CurrentAmount;
        }

        public void AddSkillXP(Skill.Type type, float amount)
        {
            playerSkills[type].AddXP(SkillBuffModifier(type, amount));
        }
        
        public int GetSkillLevel(Skill.Type type)
        {
            return playerSkills[type].CurrentLevel;
        }

        public PlayerSkillsData CreateSkillsData()
        {
            var data = new PlayerSkillsData();
            foreach (var skills in playerSkills)
                data[skills.Key] = skills.Value.GetData();

            return data;
        }

        public PlayerStatusData CreateStatusData()
        {
            var data = new PlayerStatusData();
            foreach (var stat in playerStatus)
                data[stat.Key] = stat.Value.GetData();

            return data;
        }
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            statsUpdater = new PlayerStatsUpdater();

            instance = this;
            Stats.SetManager(this);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (playerStatus != null)
                statsUpdater.Update(playerStatus, (GameTime.Time.deltaTime / GameClock.Speed) * GameClock.TimeMultiplier);
        }

        private PlayerSkills InitializeSkills(PlayerSkillsData data)
        {
            var playerSkills = new PlayerSkills()
            {
                [Skill.Type.Charisma] =
                    (data == null) ? new Charisma() : new Charisma(data[Skill.Type.Charisma]),
                [Skill.Type.Cooking] =
                    (data == null) ? new Cooking() : new Cooking(data[Skill.Type.Cooking]),
                [Skill.Type.Fitness] =
                    (data == null) ? new Fitness() : new Fitness(data[Skill.Type.Fitness]),
                [Skill.Type.Intelligence] =
                    (data == null) ? new Intelligence() : new Intelligence(data[Skill.Type.Intelligence]),
                [Skill.Type.Repair] =
                    (data == null) ? new Repair() : new Repair(data[Skill.Type.Repair]),
                [Skill.Type.Strength] =
                    (data == null) ? new Strength() : new Strength(data[Skill.Type.Strength]),
                [Skill.Type.Writing] =
                    (data == null) ? new Writing() : new Writing(data[Skill.Type.Writing]),
                [Skill.Type.Mixing] =
                    (data == null) ? new Mixing() : new Mixing(data[Skill.Type.Mixing]),
            };

            return playerSkills;
        }

        private PlayerStatus InitializeStatus(PlayerStatusData data)
        {
            var playerStatus = new PlayerStatus()
            {
                [Status.Type.Bladder] =
                    (data == null) ? new Bladder() : new Bladder(data[Status.Type.Bladder]),
                [Status.Type.Energy] =
                    (data == null) ? new Energy() : new Energy(data[Status.Type.Energy]),
                [Status.Type.Health] =
                    (data == null) ? new Health() : new Health(data[Status.Type.Health]),
                [Status.Type.Hunger] =
                    (data == null) ? new Hunger() : new Hunger(data[Status.Type.Hunger]),
                [Status.Type.Hygiene] =
                    (data == null) ? new Hygiene() : new Hygiene(data[Status.Type.Hygiene]),
                [Status.Type.Mood] =
                    (data == null) ? new Mood() : new Mood(data[Status.Type.Mood]),
                [Status.Type.Thirst] =
                    (data == null) ? new Thirst() : new Thirst(data[Status.Type.Thirst]),
            };

            return playerStatus;
        }
        
        private static float StatBuffModifier(Status.Type type, float amount)
        {
            switch (type)
            {
                case Status.Type.Mood when amount > 0 && PlayerBuff.HasBuff<Freedom>():
                    var statIncModifier = new StatIncreaseArgs(amount);
                    PlayerBuff.GetBuff<Freedom>()?.TakeEffect(statIncModifier);
                    amount = statIncModifier.Amount;
                    break;
                case Status.Type.Health when PlayerBuff.HasBuff<Suppression>():
                {
                    var statDecModifier = new StatDecreaseArgs(amount);
                    PlayerBuff.GetBuff<Suppression>()?.TakeEffect(statDecModifier);
                    amount = statDecModifier.Amount;
                    break;
                }
            }

            return amount;
        }
        
        private static float SkillBuffModifier(Skill.Type type, float amount)
        {
            if (PlayerBuff.HasBuff<Productive>())
            {
                var statIncModifier = new StatIncreaseArgs(amount);
                PlayerBuff.GetBuff<Productive>()?.TakeEffect(statIncModifier);
                amount = statIncModifier.Amount;
            }

            return amount;
        }
    }
 
    public static class Stats
    {
        private static PlayerStatsManager manager = null;

        /// <summary>
        /// Stat instance is Ready
        /// </summary>
        public static bool Ready => (manager != null);
        /// <summary>
        /// Stat data initialized
        /// </summary>
        public static bool Initialized => (manager.PlayerSkills != null && manager.PlayerStatus != null);
        /// <summary>
        /// Current player money
        /// </summary>
        public static float Money  => (manager.money);
        /// <summary>
        /// Current player status
        /// </summary>
        public static PlayerStatus PlayerStatus => manager.PlayerStatus;
        /// <summary>
        /// Current player skills
        /// </summary>
        public static PlayerSkills PlayerSkills => manager.PlayerSkills;

        public static PlayerSkillsData SkillsData => manager.CreateSkillsData();
        public static PlayerStatusData StatusData => manager.CreateStatusData();

        public static PlayerStatsManager.OnSkillChange OnSkillLevelUp;
        public static PlayerStatsManager.OnSkillChange OnSkillUpdate;
        public static PlayerStatsManager.OnMoneyChange OnMoneyChange;

        public static float PriceMultiplier { set => manager.PriceMultiplier(value); get => manager.priceMultiplier; }
        public static float XpMultiplier { set => manager.XpMultiplier(value); get => manager.xpMultiplier; }
        public static float BonusXpMultiplier { set => manager.BonusXpMultiplier(value); get => manager.bonusXpMultiplier; }
        public static float MoveSpeed { set => manager.MoveSpeed(value); get => manager.moveSpeed; }
        public static float RepairSpeed { set => manager.RepairSpeed(value); get => manager.repairSpeed; }
        public static float SetMoney { set => manager.Money(value); }

        /// <summary>
        /// Initializing Player Stats Manager
        /// </summary>
        /// <param name="statsManager">Player stats manager instance</param>
        public static void SetManager(PlayerStatsManager statsManager)
        {
            manager = statsManager;
            InitializeEvents();
        }

        /// <summary>
        /// Initializing New Stats (New players)
        /// </summary>
        public static void Initialize()
        {
            manager.UpdateData(null, null);
        }

        /// <summary>
        /// Initializing Player Stats
        /// </summary>
        /// <param name="playerSkills">Player skills data</param>
        /// <param name="playerStatus">Player stats data</param>
        public static void Initialize(PlayerSkillsData playerSkills, PlayerStatusData playerStatus)
        {
            manager.UpdateData(playerSkills, playerStatus);
        }

        /// <summary>
        /// Play levelup effects on player
        /// </summary>
        public static void PlayLevelUpEffects()
        {
            manager.PlayLevelUpEffects();
        }

        #region Skills Functions
        /// <summary>
        /// Set player skill
        /// </summary>
        /// <param name="type">Skill type</param>
        public static Skill Skill(Skill.Type type)
        {
            return manager.GetSkill(type);
        }

        /// <summary>
        /// Add experience to player skill
        /// </summary>
        /// <param name="type">Skill type</param>
        /// <param name="amount">Experience amount</param>
        public static void AddXP(Skill.Type type, float amount)
        {
            manager.AddSkillXP(type, amount);
            OnSkillUpdate.Invoke(type);
        }

        /// <summary>
        /// Get skill level
        /// </summary>
        /// <param name="type">Skill type</param>
        public static int SkillLevel(Skill.Type type)
        {
            return manager.GetSkillLevel(type);
        }

        /// <summary>
        /// Invoke level up skill event
        /// </summary>
        /// <param name="type">Skill type</param>
        public static void InvokeLevelUp(Skill.Type type)
        {
            OnSkillLevelUp.Invoke(type);
        }
        #endregion

        #region Status Functions
        /// <summary>
        /// Get player status
        /// </summary>
        /// <param name="type">Status type</param>
        public static Status Status(Status.Type type)
        {
            return manager.GetStatus(type);
        }

        /// <summary>
        /// Add value to current player status
        /// </summary>
        /// <param name="type">Status type</param>
        /// <param name="amount">Amount to add</param>
        public static void Add(Status.Type type, float amount)
        {
            manager.AddStatusAmount(type, Mathf.Abs(amount));
        }

        /// <summary>
        /// Reduce value to current player status
        /// </summary>
        /// <param name="type">Status type</param>
        /// <param name="amount">Amount to reduce</param>
        public static void Remove(Status.Type type, float amount)
        {
            manager.AddStatusAmount(type, -1 * Mathf.Abs(amount));
        }

        /// <summary>
        /// Get status current amount value
        /// </summary>
        /// <param name="type">Status type</param>
        public static float GetAmount(Status.Type type)
        {
            return manager.GetStatusAmount(type);
        }

        /// <summary>
        /// Get status maximum value
        /// </summary>
        /// <param name="type">Status type</param>
        public static float MaxAmount(Status.Type type)
        {
            return manager.GetStatusMaxAmount(type);
        }

        /// <summary>
        /// Increase status maximum value
        /// </summary>
        /// <param name="type">Status type</param>
        /// <param name="amount">Amount to increase</param>
        public static void IncreaseMaxAmount(Status.Type type, float amount)
        {
            manager.IncreaseStatusMaxAmount(type, amount);
        }
        #endregion

        /// <summary>
        /// Add money to player
        /// </summary>
        /// <param name="amount">Amount to give</param>
        public static void AddMoney(float amount)
        {
            amount = Mathf.Abs(amount);
            manager.Money(Mathf.Clamp(Money + amount, 0, float.MaxValue));
            OnMoneyChange.Invoke(manager.money);
        }

        /// <summary>
        /// Removes money from player
        /// </summary>
        /// <param name="amount">Amount to get</param>
        public static void RemoveMoney(float amount)
        {
            amount = Mathf.Abs(amount);
            manager.Money(Mathf.Clamp(Money - amount, 0, float.MaxValue));
            OnMoneyChange.Invoke(manager.money);
        }

        /// <summary>
        /// Invokes update skill event
        /// </summary>
        public static void UpdateSkillsData()
        {
            foreach (var skillType in PlayerSkills.Keys)
                OnSkillUpdate.Invoke(skillType);
        }

        /// <summary>
        /// Initialize skill events
        /// </summary>
        private static void InitializeEvents()
        {
            OnSkillLevelUp = new PlayerStatsManager.OnSkillChange();
            OnSkillUpdate = new PlayerStatsManager.OnSkillChange();
            OnMoneyChange = new PlayerStatsManager.OnMoneyChange();

            OnSkillLevelUp.AddListener((type) => CareerUi.Instance.UpdateJobUi());
        }
    }
}
