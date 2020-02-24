using GameClock = GameTime.Clock;

namespace PlayerStats
{
    public class Hunger : Status
    {
        private float _hoursToZero = 12f;
        private float _healthPenaltyPercent = 0.01f;
        
        public Hunger() { }

        public Hunger(Data existingData)
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
            type = Type.Hunger;
            data.drainPerHour = -(data.maxAmount / _hoursToZero);
        }
    }
}
