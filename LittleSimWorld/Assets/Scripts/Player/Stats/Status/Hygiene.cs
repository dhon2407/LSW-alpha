using UnityEngine;
using GameClock = GameTime.Clock;

namespace PlayerStats
{
    public class Hygiene : Status
    {
        private float _hoursToZero = 18f;
        private float _healthPenaltyPercent = 0.01f;

        public Hygiene() { }

        public Hygiene(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            var healthPunishmentAmount = Stats.MaxAmount(Type.Health) * _healthPenaltyPercent;
            Stats.Remove(Type.Health, healthPunishmentAmount * (timeScale * GameClock.Speed) /
                                      GameClock.TimeMultiplier);
        }

        protected override void UpdateData()
        {
            type = Type.Hygiene;
            data.drainPerHour = -(data.maxAmount / _hoursToZero);
        }
    }
}
