using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Weather;

namespace GameTime
{
    [RequireComponent(typeof(Light2D))]
    public class DayAndNightController : MonoBehaviour
    {
        private const int OneDayInSeconds = 86400;
        private const float cloudyDayLightOffset = 0.30f;
        private const float sunndyDayLightOffset = 0.15f;

        [SerializeField]
        private Light2D light2d;
        [SerializeField]
        private AnimationCurve lightIntensityCurve = null;
        [SerializeField]
        private Gradient lightGradient = null;
        [SerializeField]
        private Color fogDay = Color.grey;
        [SerializeField]
        private Color fogNight = Color.blue;
        [SerializeField]
        private Color endColor = Color.white;
        [SerializeField]
        private Color midcolor = Color.white;
        [SerializeField]
        private float stepUpdate = 0;

        private float lightIntensity;
        private Color[] colors;
        private Color lerpedColor;
        private float colorStep;
        private int colorCounter;
		private static Keyframe[] cachedCurveKeys;
		private static int cachedUpperBound;

        public float CurrentLightIntensity => light2d.intensity;

        private void Awake()
        {
            DayAndNight.Initialize(this);
			cachedCurveKeys = lightIntensityCurve.keys;
			cachedUpperBound = cachedCurveKeys.GetUpperBound(0);
		}

        private void Start()
        {
            colors = new Color[4];
            colors[0] = Color.white;
            colors[1] = midcolor;
            colors[2] = endColor;
            colors[3] = Color.white;

            light2d = GetComponent<Light2D>();
        }

        private void Update()
        {
            var timeOfDay = Clock.Time;
            UpdateLightIntensity(timeOfDay);
            UpdateLightColor(timeOfDay);
        }

		public float GetLightIntensityUpperBoundRatio() => lightIntensity / cachedUpperBound;

		private void UpdateLightIntensity(float time)
        {
            lightIntensity = lightIntensityCurve.Evaluate(Mathf.Lerp(0, 1, time / OneDayInSeconds));
            light2d.color = lightGradient.Evaluate(Mathf.Lerp(0, 1, time / OneDayInSeconds));

            if (WeatherSystem.CurrentWeather == Weather.WeatherType.Cloudy)
                light2d.intensity = lightIntensity - cloudyDayLightOffset;
            if (WeatherSystem.CurrentWeather == Weather.WeatherType.Sunny)
                light2d.intensity = lightIntensity + sunndyDayLightOffset;
            else
                light2d.intensity = lightIntensity;

            RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, lightIntensity * lightIntensity);
        }

        //TODO this functions seems to overwrite settings base lightGradient on function UpdateLightIntensity
        private void UpdateLightColor(float timeOfDay)
        {
            if (timeOfDay >= OneDayInSeconds)
            {
                if (colorStep < stepUpdate) //As long as the step is less than "every stepUpdate"
                {
                    lerpedColor = Color.Lerp(colors[colorCounter], colors[colorCounter + 1], colorStep);
                    light2d.color = lerpedColor;
                    colorStep += 0.025f;  //The lower this is, the smoother the transition
                }
                else
                { //Once the step equals the time we want to wait for the color, increment to lerp to the next color

                    colorStep = 0;

                    if (colorCounter < (colors.Length - 2)) //Keep incrementing until i + 1 equals the Lengh
                        colorCounter++;
                    else //and then reset to zero
                        colorCounter = 0;
                }
            }
            else //if (time <= 38000)
            {
                light2d.color = Color.white;
            }
        }

        private void Reset()
        {
            light2d = GetComponent<Light2D>();
        }
    }

    public static class DayAndNight
    {
        private static DayAndNightController controller;
        public static float LightIntensity => controller.CurrentLightIntensity;

        public static void Initialize(DayAndNightController dayAndNightController)
        {
            controller = dayAndNightController;
        }
    }
}
