using System;
using TMPro;
using UnityEngine;

namespace GameTime
{
    public class ClockUI : MonoBehaviour
    {
        [SerializeField]
        private Clock gameClock = null;

        [SerializeField] private TextMeshProUGUI hours10 = null;
        [SerializeField] private TextMeshProUGUI hours01 = null;
        [SerializeField] private TextMeshProUGUI mins10 = null;
        [SerializeField] private TextMeshProUGUI mins01 = null;
        
        private TimeSpan currentTime;
        private TimeSpan lastTimeUpdate;
		private int currentHourValue;
		private int currentMinutesValue;

        private void Start()
        {
            if (gameClock == null)
                throw new UnityException("Game clock UI: Game clock not referenced properly.");

            Refresh();
            Clock.onDayPassed.AddListener(Refresh);
        }

        private void Update()
        {
            if (!Clock.Paused)
                Refresh();
        }


		private void UpdateTimeDisplay()
        {
			int currentHour = currentTime.Hours;
			if (currentHourValue != currentHour) {
				hours10.text = (currentHour / 10).ToString("0");
				hours01.text = (currentHour % 10).ToString("0");
				
				currentHourValue = currentHour;
			}

			int currentMinutes = currentTime.Minutes;
			if (currentMinutesValue != currentMinutes) {
				mins10.text = (currentMinutes / 10).ToString("0");
				mins01.text = (currentMinutes % 10).ToString("0");
				currentMinutesValue = currentMinutes;
			}

            lastTimeUpdate = currentTime;
        }

        private void Refresh()
        {
            currentTime = TimeSpan.FromSeconds(Clock.Time);
			bool shouldUpdate = currentTime.TotalMinutes != lastTimeUpdate.TotalMinutes;
			if (shouldUpdate) { UpdateTimeDisplay(); }
        }
    }
}
