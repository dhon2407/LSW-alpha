using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UB.Simple2dWeatherEffects.Standard;
using UnityEngine.UI;
public class WeatherSystem_Old : MonoBehaviour
{

    public List<ParticleSystemRenderer> WeatherRenderers;
    public static WeatherSystem_Old Instance;
    public static bool wasWeatherDecidedToday = false;

    public Image WeatherIcon;
    public Sprite NormalWeatherSprite, RainWeatherSprite, SnowWeatherSprite, CloudyWeatherSprite, SunnyWeatherSprite;

    public float SoundChangeSpeed = 0.01f;
    public AudioSource WeatherSounds;
    public AudioClip RainSound, CloudySound, SnowSound, SunnySound, FogSound;
    private float startingVolume;


    public float snowChance = 3;
    public float fogChance = 8;
    public float rainChance = 28;
    public float sunnyChance = 48;
    public float cloudyChance = 78;
    
    public int renderQueue = 3000;


    public D2FogsNoiseTexPE FogScript;



    public bool cloudyToday = false;
    public bool sunnyToday = false;
    public bool fogToday = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if(!Instance)
        Instance = this;
        startingVolume = WeatherSounds.volume;
    }

	// Update is called once per frame
	void Update() {

		if (!FogScript.isVisible && fogToday) { StartCoroutine(FogScript.FadeIn()); }

		foreach (ParticleSystemRenderer weather in WeatherRenderers) {
			weather.sortingOrder = 11;
			weather.material.renderQueue = 3000;
			renderQueue = 3000;
		}

	}

    public IEnumerator TurnOffSound()
    {
        while (WeatherSounds.volume > 0)
        {
            WeatherSounds.volume -= SoundChangeSpeed;
            yield return new WaitForFixedUpdate();
        }
        WeatherSounds.enabled = false;
        yield return null;
    }
    public IEnumerator ChangeSound(AudioClip ToThis)
    {
        WeatherSounds.enabled = true;
        while (WeatherSounds.volume > 0)
        {
            WeatherSounds.volume -= SoundChangeSpeed;
            yield return new WaitForFixedUpdate();
        }
        
        WeatherSounds.clip = ToThis;
        WeatherSounds.Play();

        while (WeatherSounds.volume < startingVolume)
        {
            WeatherSounds.volume += SoundChangeSpeed;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public IEnumerator TurnOnSound()
    {
        WeatherSounds.Play();
        WeatherSounds.enabled = true;
        while (WeatherSounds.volume < startingVolume)
        {
            WeatherSounds.volume += SoundChangeSpeed;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    public void ChooseWeatherForTheDay()
    {
        ResetWeather();
        float temp = Random.Range(0, 100);
        Debug.Log(temp);
        WeatherIcon.sprite = NormalWeatherSprite;
        if (temp < snowChance)
        {
            foreach (ParticleSystemRenderer weather in WeatherRenderers)
            {
                if (weather.gameObject.name == "Snow")
                {
                    StartCoroutine(ChangeSound(SnowSound));
                    var tempWeather = weather.GetComponent<ParticleSystem>();
                    var tempEmission = tempWeather.emission;
                    tempEmission.enabled = true;
                    WeatherIcon.sprite = SnowWeatherSprite;

                }
            }
            PlayerChatLog.Instance.AddChatMessege("Snow weather choosed");
            return;


        }
        if (temp < fogChance)
        {
            StartCoroutine(ChangeSound(FogSound));
            FogScript.DensityBeforeFadingAway = Random.Range(0.4f, 0.6f);
            FogScript.Size = Random.Range(2f, 8f);
            fogToday = true;
            PlayerChatLog.Instance.AddChatMessege("Fog weather choosed");
            return;


        }
        if (temp <= rainChance)
        {
            foreach (ParticleSystemRenderer weather in WeatherRenderers)
            {
                if (weather.gameObject.name == "Rain")
                {
                    StartCoroutine(ChangeSound(RainSound));
                    var tempWeather = weather.GetComponent<ParticleSystem>();
                    var tempMain = tempWeather.main;
                    var tempEmission = tempWeather.emission;
                    tempEmission.rateOverTime =  Random.Range(500, 3000);
                    tempMain.startSize = Random.Range(0.3f, 0.7f);
                    tempEmission.enabled = true;
                    WeatherIcon.sprite = RainWeatherSprite;
                }
            }
            PlayerChatLog.Instance.AddChatMessege("Today is raining.");
            return;

        }
        if (temp < sunnyChance)
        {
            StartCoroutine(ChangeSound(SunnySound));
            sunnyToday = true;
            PlayerChatLog.Instance.AddChatMessege("Today is sunny weather");
            WeatherIcon.sprite = SunnyWeatherSprite;
            return;
        }
        if (temp < cloudyChance)
        {
            foreach (ParticleSystemRenderer weather in WeatherRenderers)
            {
                if (weather.gameObject.name == "Clouds")
                {
                    StartCoroutine(ChangeSound(CloudySound));
                    var tempWeather = weather.GetComponent<ParticleSystem>();
                    var tempEmission = tempWeather.emission;
                    tempEmission.enabled = true;
                    WeatherIcon.sprite = CloudyWeatherSprite;
                }
            }
            cloudyToday = true;
            PlayerChatLog.Instance.AddChatMessege("Cloudy weather today.");
            return;
        }
        
        
        
       


        foreach (ParticleSystemRenderer weather in WeatherRenderers)
            {

                var tempWeather = weather.GetComponent<ParticleSystem>();
                var tempEmission = tempWeather.emission;
                tempEmission.enabled = false;

            }
        StartCoroutine(TurnOffSound());

    }
    public void ResetWeather()
    {
        wasWeatherDecidedToday = false;
        sunnyToday = false;
        cloudyToday = false;
        fogToday = false;
        StartCoroutine(FogScript.FadeOut());
        foreach (ParticleSystemRenderer weather in WeatherRenderers)
        {

            var tempWeather = weather.GetComponent<ParticleSystem>();
            var tempEmission = tempWeather.emission;
            tempEmission.enabled = false;

        }
    }
}
