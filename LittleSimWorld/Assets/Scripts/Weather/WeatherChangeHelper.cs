using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameTime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Weather {

	public class WeatherChangeHelper : SerializedScriptableObject {

        private WeatherSystem weatherSystem = null;

		public Dictionary<Calendar.Season, WeatherChanceCalculator> WeatherTable;

		[HideInInspector] public List<WeatherData> weatherList;

		public void InitializeCalculators() => WeatherTable.ForEach(x => x.Value.Initialize());
		public WeatherData GetRandomWeather() => GetWeatherInitialized();
		public WeatherData GetRandomWeather(Calendar.Season season) => WeatherTable[season].GetRandomWeather();

		public WeatherData GetWeather(WeatherType weatherType) => weatherList.Find(x => x.type == weatherType);

        public void SetSystem(WeatherSystem system)
        {
            weatherSystem = system;
        }

        private WeatherData GetWeatherInitialized()
        {
            var newWeather = WeatherTable[Calendar.CurrentSeason].GetRandomWeather();
            newWeather.Initialize(weatherSystem);
            return newWeather;
        }


		#region Editor Initialization
#if UNITY_EDITOR
		//[Button]
		void Initialize() {

			WeatherTable = WeatherTable.InitializeDefaultValues(false);
			weatherList = new List<WeatherData>();

			var assetGUIDs = UnityEditor.AssetDatabase.FindAssets("t:WeatherData");


			foreach (var guid in assetGUIDs) {
				var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<WeatherData>(assetPath);
				weatherList.Add(asset);
			}

			for (int i = 0; i < WeatherTable.Keys.Count; i++) {
				var element = WeatherTable.ElementAt(i);

				var newCalc = new WeatherChanceCalculator();
				newCalc.WeatherChanceTable = new List<WeatherChanceWrapper>();

				for (int j = 0; j < weatherList.Count; j++) {
					var newWrapper = new WeatherChanceWrapper();
					newWrapper.Weather = weatherList[j];
					newCalc.WeatherChanceTable.Add(newWrapper);
				}

				WeatherTable[element.Key] = newCalc;
			}

		}
#endif
		#endregion

		#region Helper Classes

		[HideReferenceObjectPicker]
		public class WeatherChanceCalculator {

			[OnInspectorGUI("CalculateItemChances")]
			public List<WeatherChanceWrapper> WeatherChanceTable = new List<WeatherChanceWrapper>(5);

			public void Initialize() => CalculateTotalRarity();

			public WeatherData GetRandomWeather(int ForceChance_0_100 = -1) {
				int RandomChance = Random.Range(0, TotalLootChance + 1);

				if (ForceChance_0_100 != -1) {
					RandomChance = Mathf.RoundToInt((ForceChance_0_100 / 100f) * TotalLootChance);
				}

				int counter = 0;

				foreach (var item in WeatherChanceTable) {
					counter += item.Occurence;
					if (counter >= RandomChance) { return item.Weather; }
				}

				Debug.Log("Wrong weather");
				return null;
			}

			// This is used internally by Odin
			void CalculateItemChances() {
				CalculateTotalRarity();
				foreach (var item in WeatherChanceTable) {
					if (TotalLootChance == 0) { item.ChanceToDrop = 0; }
					else { item.ChanceToDrop = (item.Occurence / (float) TotalLootChance) * 100; }
				}
			}

			[SerializeField, HideInInspector] int TotalLootChance = 0;

			void CalculateTotalRarity() {
				TotalLootChance = 0;

				foreach (var item in WeatherChanceTable) { TotalLootChance += item.Occurence; }
			}
		}

		[HideReferenceObjectPicker]
		public class WeatherChanceWrapper {
			[FitLabelWidth] public WeatherData Weather;

			[Space, Range(0, 100), FitLabelWidth] public int Occurence = 0;

			public float ChanceToDrop { get; set; }

			[ShowInInspector, HideLabel] string ShowString => $"Chance to drop : {ChanceToDrop:0.##}%";
		}

		#endregion
	}

}