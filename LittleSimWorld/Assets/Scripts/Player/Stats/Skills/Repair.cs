using UnityEngine;

namespace PlayerStats
{
    public class Repair : Skill
    {
        public Repair() { }

        public Repair(Data existingData)
        {
            data = existingData;
        }

        protected override void Initialize()
        {
            name = nameof(Repair);
            type = Type.Repair;
        }

        protected override void Effect()
        {
            base.Effect();
            Stats.RepairSpeed += 0.05f;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Repair;
        }
    }
}
