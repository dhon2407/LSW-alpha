namespace PlayerStats
{
    public class Bladder : Status
    {
        private float _hoursToZero = 12f;

        public Bladder() { }

        public Bladder(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            Player.anim.SetTrigger("PissingInPants");
        }

        protected override void UpdateData()
        {
            type = Type.Bladder;
            data.drainPerHour = -(data.maxAmount / _hoursToZero);
        }
    }
}
