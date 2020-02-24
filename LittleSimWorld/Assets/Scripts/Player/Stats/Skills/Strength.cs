using UnityEngine;

namespace PlayerStats
{
    public class Strength : Skill
    {
        public Strength() { }

        public Strength(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Strength);
            type = Type.Strength;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Strength;
        }
    }
}
