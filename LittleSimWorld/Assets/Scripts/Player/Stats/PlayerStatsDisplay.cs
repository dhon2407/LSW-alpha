using System.Collections;
using TMPro;
using UnityEngine;

namespace PlayerStats
{
    using Type = Skill.Type;

    public class PlayerStatsDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI PhysicsLVLText = null;
        [SerializeField]
        private TextMeshProUGUI StrengthLVLText = null;
        [SerializeField]
        private TextMeshProUGUI CharismaLVLText = null;
        [SerializeField]
        private TextMeshProUGUI FitnessLVLText = null;
        [SerializeField]
        private TextMeshProUGUI CookingLVLText = null;
        [SerializeField]
        private TextMeshProUGUI RepairLVLText = null;
        [SerializeField]
        private TextMeshProUGUI WritingLVLText = null;
        [SerializeField]
        private TextMeshProUGUI MixingLVLText = null;

        private void Start()
        {
            StartCoroutine(RegisterSkillChanges());
        }

        private IEnumerator RegisterSkillChanges()
        {
            while (!Stats.Ready || !Stats.Initialized)
                yield return null;

            Stats.OnSkillUpdate.AddListener(UpdateSkillDisplay);

            Stats.UpdateSkillsData();
        }

        private void UpdateSkillDisplay(Type type)
        {
            TextMeshProUGUI lvlText = null;

            switch (type)
            {
                case Type.Strength:
                    lvlText = StrengthLVLText;
                    break;
                case Type.Fitness:
                    lvlText = FitnessLVLText;
                    break;
                case Type.Intelligence:
                    lvlText = PhysicsLVLText;
                    break;
                case Type.Cooking:
                    lvlText = CookingLVLText;
                    break;
                case Type.Charisma:
                    lvlText = CharismaLVLText;
                    break;
                case Type.Repair:
                    lvlText = RepairLVLText;
                    break;
                case Type.Writing:
                    lvlText = WritingLVLText;
                    break;
                case Type.Mixing:
                    lvlText = MixingLVLText;
                    break;
                
                default:
                    Debug.LogWarning($"No dedicated text display for skill {type}");
                    break;
            }

            if (lvlText != null)
                lvlText.text = (Stats.SkillLevel(type) == 0) ? "-" : Stats.SkillLevel(type).ToString("0");
        }
    }
}
