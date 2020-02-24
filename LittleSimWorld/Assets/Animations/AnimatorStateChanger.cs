using System.Collections;
using System.Collections.Generic;
using GameTime;
using UnityEngine;
using Stats = PlayerStats.Stats;

public class AnimatorStateChanger : MonoBehaviour
{
    public Animator anim;
    public float walkingSpeed;
	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();	
	}

    private void Start()
    {
        Clock.Pausing += PauseAnimation;        // Henrique - creating a delegate for controlling the animations on pause and unpausing 
    }

    void FixedUpdate()
    {
		if (!GameLibOfMethods.cantMove && !Clock.Paused) {
			float speedMulti = walkingSpeed * Stats.MoveSpeed;
			Vector2 temp = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			if (temp.sqrMagnitude >= 1) { temp.Normalize(); }
			temp *= speedMulti;

			rb.velocity = temp;
			if (temp.sqrMagnitude != 0) { anim.SetBool("Walking", true); }
			else { anim.SetBool("Walking", false); }
		}
		else if (!GameLibOfMethods.doingSomething) { anim.SetBool("Walking", false); }

    }
    
    private void PauseAnimation(bool isPausing) // Henrique - creating a delegate for controlling the animations on pause and unpausing
    {
	    anim.speed = isPausing ? 0 : 1;
    }
}
