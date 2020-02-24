using System;
using UnityEngine;

namespace PlayerStats
{
    public abstract class Status
    {
        private const float TenPercent = 0.1f;

        protected static readonly Data defaultData = new Data
        {
            drainPerHour = 0,
            amount = 100,
            maxAmount = 100,
            drainPerHourPunished = -30,
        };

        protected Data data = defaultData;
        public Type type { get; protected set; }
        public float CurrentAmount => data.amount;
        public float MaxAmount => data.maxAmount;
        public Data GetData() => data;

        protected abstract void UpdateData();
        public abstract void ZeroPenalty(float timeScale);

        public Status()
        {
            UpdateData();
        }

        public virtual void Add(float amount)
        {
            float minValue = (JobManager.Instance.isWorking) ? TenPercent * data.maxAmount : 0;
            data.amount = Mathf.Clamp(data.amount + amount, minValue, data.maxAmount);
            if(amount > 0)
            {
                LayerMask mask = new LayerMask();
                mask |= (1 << StatusUIUpdater.instance.statusBarUI.StatusBarSlots[type].IconOfTheStatus.gameObject.layer);
                BubbleSpawner.Instance.SpawnBubble(StatusUIUpdater.instance.statusBarUI.StatusBarSlots[type].IconOfTheStatus.transform,
              StatusUIUpdater.instance.statusBarUI.StatusBarSlots[type].IconOfTheStatus.transform.position + new Vector3(UnityEngine.Random.Range(-2,2), UnityEngine.Random.Range(-2, 2)),
              StatusUIUpdater.instance.statusBarUI.StatusBarSlots[type].IconOfTheStatus.sprite, mask,
               StatusUIUpdater.instance.statusBarUI.StatusBarSlots[type].ColorForBubbles,
               type
              );
            }
          
        }

        public virtual void Set(float amount)
        {
            data.amount = Mathf.Clamp(amount, 0, data.maxAmount);
        }

        public virtual void Drain(float timeScale)
        {
            float multiplier = (GameLibOfMethods.isSleeping) ? 0.5f : 1;
            Add(data.drainPerHour * timeScale * multiplier);
        }

        public virtual void Remove(float amount)
        {
            amount = Mathf.Abs(amount);
            Add(-amount);
        }

        public virtual void AddMax(float amount)
        {
            data.maxAmount += amount;
            UpdateData();
        }

        [System.Serializable]
        public struct Data
        {    
            public float amount;
            public float maxAmount;
            public float drainPerHour;
            public float drainPerHourPunished;
        }

        [System.Serializable]
        public enum Type
        {
            Health,
            Energy,
            Mood,
            Hygiene,
            Bladder,    
            Hunger,
            Thirst
        };
    }
}
