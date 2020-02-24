using System.Collections;
using System.Collections.Generic;
using GameTime;
using Sirenix.OdinInspector;
using UnityEngine;

public class RandomCar : MonoBehaviour {

	[Header("Car controller settings")]

	[SuffixLabel("units per second")] public float CurrentSpeed = 3; // Also, starting speed.
	[SuffixLabel("units per second")] public float MaxSpeed = 5;

	[Space]

	[SuffixLabel("speed per second")] public float Acceleration = 5f;
	[SuffixLabel("speed per second")] public float Deceleration = 2f;

	[Space]

	[SuffixLabel("units")] public float BreakDistance = 3;
	[SuffixLabel("units")] public float MoveBackDistance = 0.5f;

	[Space, Header("Visual variation settings")]

	[Header("Debug")]
	public Vector2 MoveDirection = new Vector2(1, 0);
	public Collider2D CarStopZone;
	public LayerMask Mask;
	[ShowInInspector] List<Collider2D> hitColliders = new List<Collider2D>(100);
	[ShowInInspector] float distanceToClosestCollider = -1;



	enum CarState { MoveNormally, Break, MoveBack }

	System.Action<RandomCar> OnDespawn;
	System.Func<Transform, bool> ShouldDespawn;
	Collider2D col;

	public void Initialize(System.Action<RandomCar> OnDespawn, System.Func<Transform, bool> ShouldDespawn) {
		this.OnDespawn = OnDespawn;
		this.ShouldDespawn = ShouldDespawn;

		OnDespawn += (_) => {
			hitColliders.Clear();
			distanceToClosestCollider = -1;
		};

		col = GetComponent<Collider2D>();
		Physics2D.IgnoreCollision(col, CarStopZone);
	}

	/// TODO: Alter acceleration based on distance
	private void FixedUpdate()
	{
		if (Clock.Paused)
			return;

		CalculateDistanceToClosestCollider();

		var state = GetCarState();
		if (state == CarState.MoveNormally)
		{
			float acc = Acceleration * GameTime.Time.scaledFixedDeltaTime;
			CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, MaxSpeed, acc);
		}
		else if (state == CarState.Break || state == CarState.MoveBack)
		{
			//var brakeBooster = 5f;
			float dec = Deceleration * GameTime.Time.scaledFixedDeltaTime /** brakeBooster*/;
			dec /= distanceToClosestCollider;
			
			CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0, dec);
		}
		
		//REVERSING CAR IS NICE but not needed
		//float dec = 2 * Deceleration * GameTime.Time.scaledFixedDeltaTime;
		//CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, -MaxSpeed / 5, dec);

		Move();

		if (ShouldDespawn(transform))
			OnDespawn(this);
	}

	void Move() => transform.Translate(MoveDirection * CurrentSpeed * GetScaledTimeMulti(), Space.Self);

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.layer == CarStopZone.gameObject.layer) { return; }
		hitColliders.Add(collision);
	}
	void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.layer == CarStopZone.gameObject.layer) { return; }
		hitColliders.Remove(collision);
		CalculateDistanceToClosestCollider();
	}

    /*//henrique - adding a temporary fix to the npc / car collision bug
    bool CarNpcReversing;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != 31 && CarNpcReversing) return;
        StartCoroutine(TargettedCarReversing());
    }

    IEnumerator TargettedCarReversing()
    {
        CarNpcReversing = true;
        int reverseFrames = 10;
        for (int i = 0; i < reverseFrames; i++)
        {
            float dec = 2 * Deceleration * GameTime.Time.deltaTime;
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, -MaxSpeed / 25, dec);
            Move();
            yield return null;
        }
        CarNpcReversing = false;
    }
    //henrique - adding a temporary fix to the npc / car collision bug*/


    void CalculateDistanceToClosestCollider() {
		// Reset distance
		distanceToClosestCollider = 100;

		// Find minimum distance to the collider
		foreach (var _col in hitColliders) {
			var p0 = _col.ClosestPoint(transform.position);
			var p1 = col.ClosestPoint(p0);
			distanceToClosestCollider = Mathf.Min(distanceToClosestCollider, Vector2.Distance(p1, p0));
		}
	}

	CarState GetCarState()
	{
		if (distanceToClosestCollider < 0 || distanceToClosestCollider > BreakDistance)
			return CarState.MoveNormally;
		
		return CarState.Break;
	}

	float GetScaledTimeMulti() {
		const float scaleMulti = 0.2f;

		float multi = Clock.TimeMultiplier;
		if (multi <= 1) { return Clock.TimeMultiplier * GameTime.Time.fixedDeltaTime; }
		else { return (1 + (scaleMulti * (multi - 1))) * (GameTime.Time.fixedDeltaTime); }
	}



	void OnDrawGizmosSelected() {
		var _col = GetComponent<Collider2D>();
		var yDist = CarStopZone.bounds.size.y;

		// Draw area where the car will have to move back
		{
			var moveBackPoint = _col.bounds.center;
			moveBackPoint -= MoveBackDistance * transform.right;
			moveBackPoint -= _col.bounds.extents.x * transform.right;
			moveBackPoint += _col.bounds.extents.y * Vector3.up;


			var center = moveBackPoint + (MoveBackDistance) / 2 * transform.right + _col.bounds.extents.y * Vector3.down;
			var size = _col.bounds.size.y * Vector3.up + (MoveBackDistance) * Vector3.right;

			Gizmos.color = new Color(0,0,0,0.4f);
			Gizmos.DrawCube(center, size);
			size.x *= 0.99f;
			size.y *= 0.95f;
			Gizmos.color = new Color(1, 0, 0.2f, 0.3f);
			Gizmos.DrawCube(center, size);
		}

		// Draw area where the car will have to break
		{
			var breakingPoint = _col.bounds.center;
			breakingPoint -= BreakDistance * transform.right;
			breakingPoint -= _col.bounds.extents.x * transform.right;
			breakingPoint += _col.bounds.extents.y * Vector3.up;


			var center = breakingPoint + (BreakDistance-MoveBackDistance) / 2 * transform.right + _col.bounds.extents.y * Vector3.down;
			var size = _col.bounds.size.y * Vector3.up + (BreakDistance - MoveBackDistance) * Vector3.right;

			Gizmos.color = new Color(0,0,0,0.4f);
			Gizmos.DrawCube(center, size);
			size.x *= 0.99f;
			size.y *= 0.95f;
			Gizmos.color = new Color(0, 0.8f, 0.5f, 0.3f);
			Gizmos.DrawCube(center, size);
		}

        
	}

}
