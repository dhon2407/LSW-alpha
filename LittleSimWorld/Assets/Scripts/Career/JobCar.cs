using System.Collections;
using System.Collections.Generic;
using Objects;
using Objects.Functionality;
using UnityEngine;
using Sirenix.OdinInspector;
using GameClock = GameTime.Clock;

public class JobCar : MonoBehaviour, IInteractable
{
    public static JobCar Instance;
    public Animator anim;
    private GameObject car;

    public bool CarReadyToInteract = false;

    public float interactionRange = 2;
    public float customSpeedToPosition = 0.1f;
    public float InteractionRange => interactionRange;
    public float CustomSpeedToPosition => customSpeedToPosition;
    public Transform playerStandPosition;
    public Vector2 PlayerStandPosition => playerStandPosition.transform.position;
    public float BreakDistance = 3;
    public bool IgnorePlayer;

    [Header("Debug")]
    public Vector2 MoveDirection = new Vector2(1, 0);
    public Collider2D CarStopZone;
    public LayerMask Mask;
    [ShowInInspector] List<Collider2D> hitColliders = new List<Collider2D>(100);
    [ShowInInspector] float distanceToClosestCollider = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        car = this.gameObject;
        Initialize();

    }

    private void FixedUpdate()
    {
        if (GameClock.Paused)
            return;

        CalculateDistanceToClosestCollider();

        var state = GetCarState();

        if (state == CarState.MoveNormally)
        {
            anim.speed = Mathf.Lerp(anim.speed, .7f, .1f);
        }
        else if (state == CarState.Break || state == CarState.MoveBack)
        {
            anim.speed = Mathf.Lerp(anim.speed, 0, .15f);
        }

        //Move();

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == CarStopZone.gameObject.layer) { return; }
        hitColliders.Add(collision);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == CarStopZone.gameObject.layer) { return; }
        hitColliders.Remove(collision);
        CalculateDistanceToClosestCollider();
    }

    void CalculateDistanceToClosestCollider()
    {
        // Reset distance
        distanceToClosestCollider = 100;

        // Find minimum distance to the collider
        foreach (var _col in hitColliders)
        {
            //print(_col.gameObject.name);
            if (IgnorePlayer && _col.gameObject.tag == "Player")
                return;
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

    enum CarState { MoveNormally, Break, MoveBack }

    System.Action<RandomCar> OnDespawn;
    System.Func<Transform, bool> ShouldDespawn;
    Collider2D col;

    public void Initialize()
    {
        hitColliders.Clear();
        distanceToClosestCollider = -1;
        col = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(col, CarStopZone);
    }


    public InteractionOptions interactionOptions => _interactionOptions;

    public bool isValidInteractionTarget => true;

    [SerializeField] InteractionOptions _interactionOptions = null;

    public void Interact()
    {
        if (CarReadyToInteract)
        {
            GoInCar();
        }
       
    }
    public void CarToPlayerHouse()
    {
        anim.SetTrigger("CarToPlayerHouse");
    }
    public void CarWaitForPlayer()
    {
        anim.SetTrigger("CarWaitForPlayer");
    }
    public void CarDriveFromHouseToLeft()
    {
        anim.SetTrigger("CarDriveFromHouseToLeft");

    }
    public void GoInCar()
    {
		PlayerCommands.MoveTo(PlayerStandPosition, delegate {
			CarDriveFromHouseToLeft();
			Player.anim.SetBool("HidePlayer", true);
		});

        anim.SetBool("PlayerIsInCar", true);
        JobManager.Instance.isWorking = true;
        IgnorePlayer = true;
    }
	public void UnloadFromCar() {
		if (!anim.GetBool("PlayerIsInCar")) { return; }

		PlayerAnimationHelper.ResetPlayer();
		PlayerCommands.MoveTo(PlayerCommands.LastPositionBeforeWalk,  delegate { PlayerAnimationHelper.ResetPlayer();
            Player.col.enabled = true;});
		anim.SetBool("PlayerIsInCar", false);
		JobManager.Instance.isWorking = false;
		CarDriveFromHouseToLeft();
        Player.col.enabled = true;
        IgnorePlayer = false;
    }
   
    public IEnumerator<float> DoingJob()
    {
        //PlayerAnimationHelper.ResetPlayer();
        Debug.Log("StartedWork");
        Player.col.enabled = false;
        JobManager.Instance.CurrentJob.ShowUpAtWork();
        Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
       
        GameLibOfMethods.cantMove = true;
        
        GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GameClock.ChangeSpeedToSleepingSpeed();

        float TimeToFinishWork = ((float)System.TimeSpan.FromHours(JobManager.Instance.CurrentJob.WorkingTimeInHours).TotalSeconds - (GameClock.Time - (float)System.TimeSpan.FromHours(JobManager.Instance.CurrentJob.JobStartingTimeInHours).TotalSeconds));
        float GetJobProgress() => JobManager.Instance.CurrentWorkingTime / TimeToFinishWork;

        int progressBarID = ProgressBar.StartTracking("Working", GetJobProgress);

        while (JobManager.Instance.CurrentJob != null && JobManager.Instance.CurrentWorkingTime <= TimeToFinishWork && !Input.GetKeyDown(KeyCode.P))
        {
            JobManager.Instance.CurrentWorkingTime += (Time.deltaTime * GameClock.TimeMultiplier) * GameClock.Speed;
           // Debug.Log("Current job progress is " + GameLibOfMethods.progress + ". Working time in seconds: " + JobManager.Instance.CurrentWorkingTime + ". And required work time is " + JobManager.Instance.CurrentJob.WorkingTimeInSeconds);
            yield return 0f;
        }

		ProgressBar.HideUI(progressBarID);

        if (JobManager.Instance.CurrentJob != null)
        {
            JobManager.Instance.CurrentJob.Finish();
            JobManager.Instance.CurrentWorkingTime = 0;
            
        }
        
        Debug.Log("Called car back from work");
        CarToPlayerHouse();
       
        yield return 0f;

        GameClock.ResetSpeed();
    }
}
