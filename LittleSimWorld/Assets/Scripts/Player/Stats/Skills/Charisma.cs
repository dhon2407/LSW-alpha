using UnityEngine;

namespace PlayerStats
{
    public class Charisma : Skill
    {
        public Charisma() {}

        public Charisma(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Charisma);
            type = Type.Charisma;
        }

        protected override void Effect()
        {
            base.Effect();
            Stats.PriceMultiplier -= 0.02f;
            GameLibOfMethods.
                CreateFloatingText(string.Format("Items now costs only {0:P1} of their price!", Stats.PriceMultiplier), 2);
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Charisma;
        }
    }
}
