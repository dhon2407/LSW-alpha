using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class GameLibOfMethods : MonoBehaviour

{
    [SerializeField]
    public static Transform HospitalRespawnPoint;
    public static int HospitalFee = 100;

    public static bool canInteract = true;
    public static bool cantMove = false;
    public static bool doingSomething = false;
	public static bool isSleeping = false;
	public static bool passedOut = false;

    public GameObject Player;
    public static GameObject player;
    public static Image blackScreen;
    public static GameObject FloatingText;
    public static GUI_TimersList timers;
    public static GameObject ChatMessege;

    public static Transform ChatContent;
    public static AudioSource PlayerAudioSource;
    public static GameLibOfMethods Instance;


	public LayerMask InteractablesLayer = 1 << 2 | 1 << 16;

	[SerializeField] private Transform chatContent = null;
	private static bool _passingOut;

	public static UnityEvent OnPassOut { get; private set; }

	private void Awake() {
		Instance = this;

		timers = FindObjectOfType<GUI_TimersList>();

		HospitalRespawnPoint = GameObject.Find("Hospital Respawn Point").transform;
		FloatingText = Resources.Load<GameObject>("Core/AIS/FloatingText");
        ChatMessege = Resources.Load<GameObject>("Core/AIS/ChatMessege");
        player = Player;

        blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
        Vector4 temp = new Vector4(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1);
        blackScreen.color = temp;
        blackScreen.CrossFadeAlpha(0, 2, false);

        ChatContent = chatContent;
        PlayerAudioSource = player.GetComponent<AudioSource>();
        doingSomething = false;
        OnPassOut = new UnityEvent();
	}

/*
	private void OnDrawGizmos() {
		if (player && facingDir != null) {
			Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
			Gizmos.DrawSphere(player.transform.position + (facingDir * 0.3f), 0.5f);
			int layerMask = 1 << 10 | 1 << 11;

			List<Collider2D> colliders = new List<Collider2D>();
			ContactFilter2D contactFilter = new ContactFilter2D();
			contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
			Physics2D.OverlapCircle(player.transform.position + (facingDir * 0.3f), 0.5f, contactFilter, colliders);
			//colliders = colliders.OrderBy(x => x.Distance(player.GetComponent<Collider2D>())).ToList();

			colliders.Sort(delegate (Collider2D a, Collider2D b) {

				return Vector2.Distance(player.transform.position, a.transform.position)
				.CompareTo(
				  Vector2.Distance(player.transform.position, b.transform.position));
			});
			foreach (Collider2D collider in colliders) {	
			}
			RaycastHit2D hit = new RaycastHit2D();

			foreach (Collider2D collider in colliders) {
				hit = Physics2D.Raycast(checkRaycastOrigin.transform.position, (collider.bounds.ClosestPoint(player.transform.position) - player.transform.position).normalized, 1000, layerMask);
				if (hit.collider == collider) {


					Gizmos.color = Color.red;
					Gizmos.DrawLine(player.transform.position, hit.point);

				}	
			}
		}
	}
	*/


	public static void PassOut() {
		if (!doingSomething && canInteract && !cantMove && !isSleeping && !_passingOut)
		{
			_passingOut = true;
			GameTime.Clock.ResetSpeed();
			global::Player.anim.SetBool("PassOut", true);
			OnPassOut.Invoke();
			Debug.Log("Passing Out");
		}
	}

    public static void WakeUpAtHospital()
    {
        GameTime.Calendar.NextDay();
        WakeUpHospital().Start();
    }
    public static void CreateFloatingText(string text, float FadeDuration)	
    {
        /*var pos = player.transform.position + 1.5f * Vector3.up;

		GameObject floatingText = Instantiate(FloatingText, pos, Quaternion.Euler(Vector3.zero));
        floatingText.GetComponent<TextMeshPro>().text = text;
        Destroy(floatingText, FadeDuration);*/
        PlayerChatLog.Instance.AddChatMessege(text);
        Debug.Log("Floating messages are removed coz of Terry's request.");
    }
    public static void AddChatMessage(string text)
    {
        GameObject chatMessege = Instantiate(ChatMessege, ChatContent);
        chatMessege.GetComponentInChildren<TextMeshProUGUI>().text = "[" + GameTime.Clock.CurrentTimeFormat+ "]" + text;
        PlayerChatLog.Instance.Reset();
    }
	
	static IEnumerator<float> WakeUpHospital() {	
		GameTime.Clock.ResetSpeed();
		yield return MEC.Timing.WaitForSeconds(2);
		player.GetComponent<Animator>().enabled = true;
		Stats.Status(Type.Hunger).Set(float.MaxValue);	
		Stats.Status(Type.Energy).Set(float.MaxValue);
		Stats.Status(Type.Health).Set(float.MaxValue);
		Stats.Status(Type.Mood).Set(float.MaxValue);
		Stats.Status(Type.Bladder).Set(100);
		Stats.Status(Type.Hunger).Set(100);
		Stats.Status(Type.Hygiene).Set(100);

		Vector3 tempRotation = new Vector3(0, 0, 0);
		player.transform.rotation = Quaternion.Euler(tempRotation);

		Stats.RemoveMoney(HospitalFee);	
		player.transform.position = new Vector2(HospitalRespawnPoint.position.x, HospitalRespawnPoint.position.y);	
		blackScreen.CrossFadeAlpha(0, 2, false);
		cantMove = false;
		CameraFollow.Instance.ResetCamera();

		player.GetComponent<Animator>().SetBool("PassOut", false);
		_passingOut = false;
	}

	public void SpawnFX(AudioClip clip, Vector2 position) {
		if (clip == null) { return; }
		GameObject newFX = Instantiate(Resources.Load<GameObject>("Core/AIS/_FX"), position, Quaternion.Euler(Vector2.zero));
		newFX.GetComponent<AudioSource>().clip = clip;
		newFX.GetComponent<AudioSource>().Play();
		Destroy(newFX, clip.length);
	}

}

	