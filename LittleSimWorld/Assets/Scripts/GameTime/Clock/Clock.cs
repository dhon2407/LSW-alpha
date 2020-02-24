using System;
using UnityEngine;
using UnityEngine.Events;
using Weather;

namespace GameTime
{
    public class Clock : MonoBehaviour
    {
        private const int OneDayInSeconds = 86400;
        private static Clock instance = null;
        [SerializeField]
        private float dayTime = 0;

        [SerializeField]
        private float clockSpeed = 1;
        [SerializeField]
        private float timeMultiplier = 1;

        private bool _paused;

        public static UnityEvent onDayPassed;

        public static float Time => instance.dayTime;
        public static float Speed => instance.clockSpeed;
        public static float TimeMultiplier => instance.timeMultiplier;
        public static string CurrentTimeFormat => instance.GetCurrentTimeFormat();
        public static float InGameHoursSinceLastFrame => TimeMultiplier * Speed * GameTime.Time.deltaTime / (60 * 60); // 60 seconds * 60 minutes for hour conversion
        public static bool Paused => instance._paused;

        public delegate void ClockPause(bool isPausing);          // Henrique - creating a delegate for controlling the animations on pause and unpausing 
        public static event ClockPause Pausing;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            onDayPassed = new UnityEvent();
            onDayPassed.AddListener(delegate { dayTime = 0; });
        }

        public static void SetTime(float time)
        {
            instance.dayTime = time;
        }

        public static void ResetSpeed()
        {
            ChangeSpeed(1);
        }

        public static void ChangeSpeed(float multiplier)
        {
            instance.timeMultiplier = multiplier;
        }

        public static void Pause()
        {
            instance._paused = true;
            Pausing?.Invoke(true); // Henrique - creating a delegate for controlling the animations on pause and unpausing 
        }

        public static void Unpause()
        {
            instance._paused = false;
            Pausing?.Invoke(false); // Henrique - creating a delegate for controlling the animations on pause and unpausing 
        }

        private void Update()
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            dayTime += (GameTime.Time.deltaTime * clockSpeed) * timeMultiplier;

			if (dayTime >= OneDayInSeconds) { onDayPassed.Invoke(); }
        }

        private string GetCurrentTimeFormat()
        {
            var currentTimeSpan = TimeSpan.FromSeconds(dayTime);

            return string.Format("{0}:{1}",
                currentTimeSpan.Hours.ToString("00"),
                currentTimeSpan.Minutes.ToString("00"));
        }

        private void OnDestroy()
        {
            onDayPassed.RemoveAllListeners();
        }


		//TODO Coupled functions to other dependencies
		public static void ChangeSpeedToFaster() => ChangeSpeed(10);

		public static void ChangeSpeedToSupaFast() => ChangeSpeed(100);

		public static void ChangeSpeedToSleepingSpeed() => ChangeSpeed(10);

    }

    public static class Time
    {
        public static float deltaTime => Clock.Paused ? 0 : UnityEngine.Time.deltaTime;
        public static float fixedDeltaTime => Clock.Paused ? 0 : UnityEngine.Time.fixedDeltaTime;

		public static float scaledDeltaTime => Clock.TimeMultiplier * deltaTime;
		public static float scaledFixedDeltaTime => Clock.TimeMultiplier * fixedDeltaTime;
	}
}
