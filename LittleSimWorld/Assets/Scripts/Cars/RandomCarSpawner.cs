using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCarSpawner : MonoBehaviour
{

	public float MinSpawnTime = 5;
    public float MaxSpawnTime = 60;

	[Space]
	public int MaxActiveCarsOnLane = 5;
    public AnimationCurve Probs;

	public List<RandomCar> carVariations;

    float TimeWithoutSpawning = 0;
    float CurrentUpdateTime;
    float UpdatePeriod;

	List<RandomCar> cars = new List<RandomCar>(100);
	int activeCarAmountOnLane;

	void Start() {
		UpdatePeriod = MaxSpawnTime / MinSpawnTime;

		var spawnPointX = transform.position.x;
		bool ShouldObjectDespawn(Transform tr) => Mathf.Abs(tr.position.x - spawnPointX) > 90;
		void OnObjectDespawn(RandomCar car) {
			car.gameObject.SetActive(false);
			cars.Add(car);
			activeCarAmountOnLane--;
		}

		for (int i = 0; i < 10; i++) {
			foreach (var carPrefab in carVariations) {
				var newCar = Instantiate(carPrefab, transform.position, transform.rotation, transform);
				newCar.Initialize(OnObjectDespawn, ShouldObjectDespawn);
				newCar.gameObject.SetActive(false);
				cars.Add(newCar);
			}
		}

	}

	void Update() {
		if (activeCarAmountOnLane >= MaxActiveCarsOnLane) { return; }

		CurrentUpdateTime += Time.deltaTime;
		TimeWithoutSpawning += Time.deltaTime;

		if (CurrentUpdateTime > UpdatePeriod) {
			CurrentUpdateTime = 0;

			float chance = Probs.Evaluate(Mathf.Clamp(TimeWithoutSpawning, MinSpawnTime, MaxSpawnTime));
			if (chance <= Random.Range(1, 100) || TimeWithoutSpawning > MaxSpawnTime) {
				TimeWithoutSpawning = 0;
				SpawnRandomCar();
			}
		}
	}

	public void SpawnRandomCar() {
		var index = Random.Range(0, cars.Count);
		var car = cars[index];
		car.transform.position = transform.position;
		car.gameObject.SetActive(true);
		cars.RemoveAt(index);
		activeCarAmountOnLane++;
	}
}
