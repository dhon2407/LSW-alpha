using System;
using UnityEngine;

namespace Weather
{
    public abstract class WeatherData : ScriptableObject
    {
        public WeatherType type;
        public Sprite indicatorIcon;
        public AudioClip ambientSound;

        public abstract void Initialize(WeatherSystem owner);
        public abstract void Cast();
        public abstract void Reset();
        public abstract void Pause();
        public abstract void Unpause();

    }

    public enum WeatherType
    {
        Sunny, Cloudy, Rainy, Foggy, Snowy
    }

    [Serializable]
    public class Chance
    {
        public GameTime.Calendar.Season season;
        [Range(0, 100)]
        public int percentage;
    }

}