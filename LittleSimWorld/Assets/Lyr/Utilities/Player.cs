using UnityEngine;

/// <summary>
/// Class for providing easy and quick accessibility to the player.
/// </summary>
public static class Player {

	public static GameObject gameObject;
	public static Transform transform;
	public static Rigidbody2D rb;
	public static Animator anim;
	public static Collider2D col;

	public static Vector2 position {
		get => transform.position;
		set => transform.position = value;
	}

	static Player() {
		Initialize();
		UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (a, b) => Initialize();
	}


	static void Initialize() {
		try {
			gameObject = GameObject.Find("Player").transform.Find("Body").gameObject;
			transform = gameObject.transform;

			rb = gameObject.GetComponent<Rigidbody2D>();
			anim = gameObject.GetComponent<Animator>();
			col = gameObject.GetComponent<Collider2D>();
		}
		catch {
			Debug.LogError("Could not initialize Player class.");
		}
	}

}
