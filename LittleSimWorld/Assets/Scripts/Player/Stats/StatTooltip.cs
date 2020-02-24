using System;
using LSW.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerStats
{
    [AddComponentMenu(menuName:"Tooltip/Stats Tooltip")]
    public class StatTooltip : Tooltip<StatTooltip.Data>
    {
        [Space, Header("Labels")]
        [SerializeField] private TextMeshProUGUI statName = null;
        [SerializeField] private TextMeshProUGUI xpInfo = null;
        [SerializeField] private Image fillImage = null;

        private Skill.Type _currentSkill;
        
        public override void SetData(Data data)
        {
            _currentSkill = data.statType;
            statName.text = GetStatName(data.statType);
            fillImage.fillAmount = (data.xpCurrent / data.xpMax);
            
            fillImage.transform.parent.gameObject.SetActive(data.statType != Skill.Type.Unknown);

            if (data.statType != Skill.Type.Unknown)
                xpInfo.text = $"XP: {data.xpCurrent:0}";
        }
        
        public struct Data
        {
            public Skill.Type statType;
            public float xpMax;
            public float xpCurrent;
        }
        
        private string GetStatName(Skill.Type statType)
        {
            if (statType == Skill.Type.Unknown)
                return "Skill Locked";
            else
                return statType.ToString();
        }

        private void Update()
        {
            if (IsVisible && (_currentSkill != Skill.Type.Unknown))
            {
                var currentXP = Stats.Skill(_currentSkill).GetData().xp;
                var requiredXP = Stats.Skill(_currentSkill).GetData().RequiredXP;
                fillImage.transform.parent.gameObject.SetActive(true);
                fillImage.fillAmount = ( currentXP/ requiredXP);
                
                xpInfo.text = $"XP: {currentXP:0}";
            }
        }
    }
}