using UnityEngine;

namespace PlayerStats
{
    public class Fitness : Skill
    {
        public Fitness() { }

        public Fitness(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Fitness);
            type = Type.Fitness;
        }

        protected override void Effect()
        {
            base.Effect();
            Stats.IncreaseMaxAmount(Status.Type.Energy, 5f);
            GameLibOfMethods.CreateFloatingText("Now you have " + Stats.MaxAmount(Status.Type.Energy) + " max energy!", 4);
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Fitness;
        }
    }
}