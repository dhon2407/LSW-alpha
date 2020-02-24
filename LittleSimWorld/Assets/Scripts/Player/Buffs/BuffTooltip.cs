using System;
using LSW.Tooltip;
using PlayerStats.Buffs.Core;
using TMPro;
using UnityEngine;
using Time = GameTime.Time;

namespace PlayerStats.Buffs
{
    [AddComponentMenu(menuName:"Tooltip/Buff Tooltip")]
    public class BuffTooltip : Tooltip<IBuff>
    {
        [SerializeField] private TextMeshProUGUI buffName = null;
        [SerializeField] private TextMeshProUGUI description = null;
        [SerializeField] private GameObject timeIndicator = null;
        [SerializeField] private TextMeshProUGUI time = null;
        [SerializeField] private Color buffTextColor = Color.black;
        [SerializeField] private Color deBuffTextColor = Color.red;
        
        private IBuff _currentBuff;

        public override void SetData(IBuff currentBuff)
        {
            _currentBuff = currentBuff;
            
            timeIndicator.SetActive(!_currentBuff.IsPermanent);
            buffName.text = _currentBuff.Name;
            description.text = _currentBuff.Description;

            buffName.color = _currentBuff.IsDebuff ? deBuffTextColor : buffTextColor;
            
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            var remainingTime = _currentBuff.Remaining;
            int hoursLeft = TimeSpan.FromSeconds(remainingTime).Hours;
            int minutesLeft = TimeSpan.FromSeconds(remainingTime - TimeSpan.FromHours(hoursLeft).Seconds).Minutes;
            int secondsLeft = TimeSpan
                .FromSeconds(remainingTime -
                             (TimeSpan.FromHours(hoursLeft).Seconds + TimeSpan.FromMinutes(minutesLeft).Seconds))
                .Seconds;

            var hours = hoursLeft > 0 ? $"{hoursLeft:00}h " : string.Empty;
            var mins = minutesLeft > 0 ? $"{minutesLeft:00}m " : string.Empty;
            
            time.text = $"{hours}{mins}{secondsLeft:00}s";

        }

        private void Update()
        {
            if (IsVisible)
                UpdateTimeDisplay();
        }
        

    }
}