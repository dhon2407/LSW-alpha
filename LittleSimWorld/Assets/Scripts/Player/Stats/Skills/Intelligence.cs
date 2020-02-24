using UnityEngine;

namespace PlayerStats
{
    public class Intelligence : Skill
    {
        public Intelligence() { }

        public Intelligence(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Intelligence);
            type = Type.Intelligence;
        }

        protected override void Effect()
        {
            base.Effect();
            Stats.XpMultiplier += 0.02f;
            GameLibOfMethods.
                CreateFloatingText("Now you receive " + (Stats.XpMultiplier + Stats.BonusXpMultiplier) + "% XP!", 2);
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Intelligence;
        }
    }
}
