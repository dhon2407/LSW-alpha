using UnityEngine;
using GameTime;
namespace Weather
{
    [CreateAssetMenu(fileName = "Weather Data", menuName = "Weather/Weather Particle System")]
    public class WeatherDataParticle : WeatherData
    {
        [SerializeField][Header("Particle System")]
        public ParticleSystem particleSystem;
        public override void Initialize(WeatherSystem owner)
        {
            particleSystem = owner.GetParticleSystem(type);
            Clock.Pausing += delegate { if (Clock.Paused) Pause(); else Unpause(); };
        }

        public override void Cast()
        {
            var weatherEmission = particleSystem.emission;
            weatherEmission.enabled = true;
        }

        public override void Reset()
        {
            var weatherEmission = particleSystem.emission;
            weatherEmission.enabled = false;
        }
        public override void Pause()
        {
            var main = particleSystem.main;
            main.simulationSpeed = 0;
        }
        public override void Unpause()
        {
            
            var main = particleSystem.main;
            main.simulationSpeed = 1;
        }
    }
}
