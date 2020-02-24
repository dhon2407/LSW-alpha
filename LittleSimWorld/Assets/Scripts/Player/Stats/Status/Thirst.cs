using GameClock = GameTime.Clock;

namespace PlayerStats
{
    public class Thirst : Status
    {
        private float _hoursToZero = 8f;
        private float _healthPenaltyPercent = 0.01f;
        
        public Thirst() { }

        public Thirst(Data existingData)
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
            type = Type.Thirst;
            data.drainPerHour = -(data.maxAmount / _hoursToZero);
        }
    }
}
