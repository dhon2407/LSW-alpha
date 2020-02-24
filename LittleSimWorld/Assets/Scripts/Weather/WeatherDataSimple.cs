using UnityEngine;
using UB.Simple2dWeatherEffects.Standard;

namespace Weather
{
    [CreateAssetMenu(fileName = "Weather Data", menuName = "Weather/Weather Simple System")]
    public class WeatherDataSimple : WeatherData
    {
        private D2FogsNoiseTexPE fog;
        private WeatherSystem owner;

        public override void Initialize(WeatherSystem owner)
        {
            this.owner = owner;
            fog = owner.gameObject.GetComponentInParent<D2FogsNoiseTexPE>();
        }

        public override void Cast()
        {
            if (type == WeatherType.Foggy)
                AdjustFog();
        }

        public override void Reset()
        {
            if (type == WeatherType.Foggy)
                RemoveFog();
        }

        private void RemoveFog()
        {
            owner.StartCoroutine(fog.FadeOut());
        }

        private void AdjustFog()
        {
            fog.DensityBeforeFadingAway = Random.Range(0.4f, 0.6f);
            fog.Size = Random.Range(2f, 8f);

            owner.StartCoroutine(fog.FadeIn());
        }
        public override void Pause()
        {
            throw new System.NotImplementedException();
        }
        public override void Unpause()
        {
            throw new System.NotImplementedException();
        }
       
    }
}
