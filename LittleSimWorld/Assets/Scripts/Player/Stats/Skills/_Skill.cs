using Boo.Lang;
using UI.Notifications;
using UnityEngine;

namespace PlayerStats
{
    public abstract class Skill
    {
        protected static readonly Data defaultData = new Data
        {
            level = 0,
            maxLvl = 10,
            xp = 0,
            expTable = new List<int>
            {
                100,
                200,
                400,
                400,
                600,
                800,
                1200,
                1600,
                2000,
                3000,
                5000,
                10000,
                20000,
            }
        };

        protected Data data = defaultData;

        public Data GetData() => data;
        public int CurrentLevel => data.level;
        public Type type { get; protected set; }
        public string name { get; protected set; }

        protected abstract void Initialize();
        protected abstract Sprite GetSkillSprite();
        
        public Skill()
        {
            Initialize();
        }

        public virtual void AddXP(float amount)
        {
            data.xp += (amount * (Stats.XpMultiplier + Stats.BonusXpMultiplier));

            if (data.xp >= data.RequiredXP && data.level < data.maxLvl)
                LevelUp();

              //TODO TO BE EXTRACTED
                UIManager.Instance.LevelingSkill.text = name + ": " + data.level;
                UIManager.Instance.XPbar.fillAmount = data.xp / data.RequiredXP;
                UIManager.Instance.CurrentSkillImage.sprite = GetSkillSprite();
                UIManager.Instance.ShowLevelingSkill().Start();

                LayerMask mask = new LayerMask();
                mask |= (1 << UIManager.Instance.CurrentLevelingSkillIcon.layer);
                
                BubbleSpawner.Instance.SpawnBubble(UIManager.Instance.CurrentLevelingSkillIcon.transform,
                    UIManager.Instance.CurrentLevelingSkillIcon.transform.position + new Vector3(UnityEngine.Random.Range(-2, 2),
                    UnityEngine.Random.Range(-2, 2)),
                    GetSkillSprite(), mask, 
                    Color.green,
                    type
                    );
            
        }

        protected virtual void Effect()
        {
            Stats.PlayLevelUpEffects();
        }

        protected virtual void LevelUp()
        {
            data.xp = 0;
            data.level += 1;
            Stats.InvokeLevelUp(type);
            Effect();
            GameLibOfMethods.CreateFloatingText($"{name} Leveled UP!", 3);
            PlayerChatLog.Instance.AddChatMessege(name + " level UP!");
            
            Notify.Show($"Congratulations, you have reached level {data.level} in {name}!", GetSkillSprite());
        }

        [System.Serializable]
        public struct Data
        {
            public int level;
            public int maxLvl;
            public float xp;
            public List<int> expTable;

            public float RequiredXP => GetRequireExp();
            private float GetRequireExp()
            {
                if (expTable == null || expTable.Count == 0)
                    return 0;

                return expTable[Mathf.Clamp(level, 0, expTable.Count - 1)];
            }
        }

        [System.Serializable]
        public enum Type
        {
            Unknown = -1,
            
            Strength,
            Fitness,
            Intelligence,
            Cooking,
            Charisma,
            Repair,
            Writing,
            Mixing
        };
    }


}
