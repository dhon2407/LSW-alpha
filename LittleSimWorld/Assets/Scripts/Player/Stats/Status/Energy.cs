namespace PlayerStats
{
    public class Energy : Status
    {
        private float _hoursToZero = 18f;
        
        public Energy() { }

        public Energy(Data existingData)
        {
            data = existingData;
        }

		public override void ZeroPenalty(float timeScale) {
			Player.anim.SetTrigger("PassOutToSleep");
		}

        protected override void UpdateData()
        {
            type = Type.Energy;
            data.drainPerHour = -(data.maxAmount / _hoursToZero);
        }
    }
}
