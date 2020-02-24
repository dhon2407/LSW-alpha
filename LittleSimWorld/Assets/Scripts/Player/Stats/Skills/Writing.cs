using UnityEngine;

namespace PlayerStats
{
    public class Writing : Skill
    {
        public Writing() { }

        public Writing(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Writing);
            type = Type.Writing;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Writing;
        }
    }
}
