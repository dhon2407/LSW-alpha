using UnityEngine;

namespace PlayerStats
{
    public class Mixing : Skill
    {
        public Mixing() { }

        public Mixing(Data existingData)
        {
            data = existingData;
        }
        
        protected override void Initialize()
        {
            name = nameof(Mixing);
            type = Type.Mixing;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Mixing;
        }
    }
}