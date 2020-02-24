using System;
using UnityEngine;
using UnityEngine.UI;

namespace Weather
{
    public class WeatherIndicator : MonoBehaviour
    {
        [SerializeField]
        private Image weatherIcon = null;

        private void Start()
        {
            WeatherSystem.onChangeWeather.AddListener(ChangeWeatherIcon);

            ChangeWeatherIcon(WeatherSystem.CurrentWeatherData);
        }

        private void OnDestroy()
        {
            WeatherSystem.onChangeWeather.RemoveListener(ChangeWeatherIcon);
        }

        private void ChangeWeatherIcon(WeatherData weather)
        {
            if (weather != null)
                weatherIcon.sprite = weather.indicatorIcon;
        }
    }
}
