using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;
using System.Collections;
using MEC;
using PlayerStats.Buffs.Core;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemConsumable", menuName = "Items/Consumable")]
    public class ItemConsumable : ActiveItem
    {
        public ConsumableType type;
        [Header("Consumption Gain")]
        public float Hunger;
        public float Mood;
        public float Health;
        public float Energy;
        public float Thirst;
        public float Bladder;
        [Space]
        public float HungerPercentBonus;
        public float MoodPercentBonus;
        public float HealthPercentBonus;
        public float EnergyPercentBonus;
        public float ThirstPercentBonus;

        public override void Cancel()
        {
            cancel = true;
        }

        public override void Use()
        {
            /* ApplyStatGains();
             ApplyBonusGains();*/
            cancel = false;
            GradualUse().Start(Segment.FixedUpdate);
           
        }
        public IEnumerator<float> GradualUse()
        {
            float t = 0;
            float GetProgress() => (t += GameTime.Time.deltaTime) / UsageTime;

             void ApplyGradualBonusGains()
            {
                Stats.Status(Type.Hunger).Add((Stats.Status(Type.Hunger).MaxAmount * HungerPercentBonus) / UsageTime *Time.deltaTime);
                Stats.Status(Type.Energy).Add((Stats.Status(Type.Energy).MaxAmount * EnergyPercentBonus) / UsageTime * Time.deltaTime);
                Stats.Status(Type.Health).Add((Stats.Status(Type.Health).MaxAmount * HealthPercentBonus) / UsageTime * Time.deltaTime);
                Stats.Status(Type.Mood).Add((Stats.Status(Type.Mood).MaxAmount * MoodPercentBonus) / UsageTime * Time.deltaTime);
                Stats.Status(Type.Thirst).Add((Stats.Status(Type.Thirst).MaxAmount * ThirstPercentBonus) / UsageTime * Time.deltaTime);
            }
            void ApplyGradualStatGains()
            {
                Stats.Status(Type.Hunger).Add((Hunger / UsageTime) * Time.deltaTime);
                Stats.Status(Type.Energy).Add((Energy / UsageTime) * Time.deltaTime);
                Stats.Status(Type.Health).Add((Health / UsageTime) * Time.deltaTime);
                Stats.Status(Type.Mood).Add((Mood / UsageTime) * Time.deltaTime);
                Stats.Status(Type.Thirst).Add((Thirst / UsageTime) * Time.deltaTime);
                Stats.Status(Type.Bladder).Add((Bladder / UsageTime) * Time.deltaTime);
            }

            while (GetProgress() < 1 && !cancel)
            {
                ApplyGradualBonusGains();
                ApplyGradualStatGains();

                yield return 0f;
            }
            
            PlayerBuff.GetRandomBuff();

            yield return 0f; 
        }

        private void ApplyStatGains()
        {
            Stats.Status(Type.Hunger).Add(Hunger);
            Stats.Status(Type.Energy).Add(Energy);
            Stats.Status(Type.Health).Add(Health);
            Stats.Status(Type.Mood).Add(Mood);
            Stats.Status(Type.Thirst).Add(Thirst);
            Stats.Status(Type.Bladder).Add(Bladder);
        }

        private void ApplyBonusGains()
        {
            Stats.Status(Type.Hunger).Add(Stats.Status(Type.Hunger).MaxAmount * HungerPercentBonus);
            Stats.Status(Type.Energy).Add(Stats.Status(Type.Energy).MaxAmount * EnergyPercentBonus);
            Stats.Status(Type.Health).Add(Stats.Status(Type.Health).MaxAmount * HealthPercentBonus);
            Stats.Status(Type.Mood).Add(Stats.Status(Type.Mood).MaxAmount * MoodPercentBonus);
            Stats.Status(Type.Thirst).Add(Stats.Status(Type.Thirst).MaxAmount * ThirstPercentBonus);
        }
    }

    public enum ConsumableType
    {
        Food,
        Drink
    }
}