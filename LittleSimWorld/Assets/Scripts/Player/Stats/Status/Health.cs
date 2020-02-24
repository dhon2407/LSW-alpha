using PlayerStats.Buffs;
using PlayerStats.Buffs.Core;

namespace PlayerStats
{
    public class Health : Status
    {
        private float _percentPerHour = 0.10f;
        private float _hoursADay = 24f;
        
        public Health() { }

        public Health(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            GameLibOfMethods.PassOut();
        }

        protected override void UpdateData()
        {
            type = Type.Health;
            data.drainPerHour = -(data.maxAmount * _percentPerHour) / _hoursADay;
        }
        
        public override void Drain(float timeScale)
        {
            float multiplier = (GameLibOfMethods.isSleeping) ? 0.5f : 1;
            float drainAmount = data.drainPerHour * timeScale * multiplier;

            if (PlayerBuff.HasBuff<Suppression>())
            {
                var statModifier = new StatDecreaseArgs(drainAmount);
                PlayerBuff.GetBuff<Suppression>()?.TakeEffect(statModifier);
                drainAmount = statModifier.Amount;
            }

            Add(drainAmount);
        }
    }
}
