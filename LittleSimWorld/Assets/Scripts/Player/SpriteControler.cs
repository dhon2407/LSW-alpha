using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using CharacterData;
using GameTime;
using UI.CharacterCreation;
using UnityEngine.Rendering;
public class SpriteControler : SerializedMonoBehaviour {

	[Header("Visuals"), HideReferenceObjectPicker]
	public CharacterData.CharacterInfo visuals;

	[Header("In-game References for Visuals")]
	public Dictionary<CharacterPart, SpriteRenderer> References;
    public SortingGroup PlayerSortingGroup;

	public SpriteRenderer Hand_L;
	public SpriteRenderer Hand_R;

	[Space,Header("Stuff to be moved away from here")]
	public GameObject Censor;
	public AudioSource WalkingSource;
	public List<AudioClip> WalkingSounds;
	public AudioClip BumpingSound;

	public static SpriteControler Instance;
	static Animator anim;

    public CharacterOrientation currentOrientation;

	private void Awake() {
		if (CharacterCreationManager.CurrentCharacterInfo != null) {
			visuals = CharacterCreationManager.CurrentCharacterInfo;
			CharacterCreationManager.CurrentCharacterInfo = null;
		}
		Instance = this;
	}

	void Start() {
		anim = Player.anim;
		CheckForNullValues();
		FaceDOWN();
	}

	private void Update() {
		if (!GameLibOfMethods.cantMove && !GameLibOfMethods.doingSomething) {
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");

            anim.SetBool("Running", Input.GetButton("Run"));

            anim.SetFloat("Vertical", y);
			anim.SetFloat("Horizontal", x);

            anim.SetFloat("Vertical", y);

            anim.SetFloat("Horizontal", x);

            if (x != 0 || y != 0) { GameTime.Clock.ResetSpeed(); }
		}
    
	}

	void LateUpdate() {
		if (Clock.Paused) { return; }
		if (GameLibOfMethods.cantMove || GameLibOfMethods.doingSomething) { return; }

		Vector2 MoveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (MoveDir.y < 0) { FaceDOWN(); }
		if (MoveDir.y > 0) { FaceUP(); }
		if (MoveDir.x < 0) { FaceLEFT(); }
		if (MoveDir.x > 0) { FaceRIGHT(); }
	}

	public void ChangeSortingOrder(int targetSortingOrder) {
		
			PlayerSortingGroup.sortingOrder = targetSortingOrder;
		
	}

	public void SetClothesState(bool active) {
		References[CharacterPart.Top].enabled = active;
		References[CharacterPart.Bottom].enabled = active;
	}

	public void FaceUP() {
		UpdateCharacterOrientationInternal(CharacterOrientation.Top);

		anim.SetFloat("FaceX", 0f);
		anim.SetFloat("FaceY", 1f);
	}
	public void FaceDOWN() {
		UpdateCharacterOrientationInternal(CharacterOrientation.Bot);

		anim.SetFloat("FaceX", 0f);
		anim.SetFloat("FaceY", -1f);
	}
	public void FaceRIGHT() {
		UpdateCharacterOrientationInternal(CharacterOrientation.Right);

		anim.SetFloat("FaceX", 1f);
		anim.SetFloat("FaceY", 0f);
	}
	public void FaceLEFT() {
		UpdateCharacterOrientationInternal(CharacterOrientation.Left);

		anim.SetFloat("FaceX", -1f);
		anim.SetFloat("FaceY", 0f);
	}



	public void UpdateCharacterOrientation(CharacterOrientation orientation) {

		if (orientation == CharacterOrientation.Bot) { FaceDOWN(); }
		else if (orientation == CharacterOrientation.Top) { FaceUP(); }
		else if (orientation == CharacterOrientation.Right) { FaceRIGHT(); }
		else if (orientation == CharacterOrientation.Left) { FaceLEFT(); }

	}

    public Vector3 FacingVectorNorm()
    {
        switch (_currentCharacterOrientation)
        {
            case CharacterOrientation.Bot:
                return new Vector3(0, -1, 0);
            case CharacterOrientation.Top:
                return new Vector3(0, 1, 0);
            case CharacterOrientation.Right:
                return new Vector3(1, 0, 0);
            case CharacterOrientation.Left:
                return new Vector3(-1, 0, 0);
            default:
                return new Vector3(0,0,0);
        }
    }

	CharacterOrientation _currentCharacterOrientation = (CharacterOrientation) (-1);

	void UpdateCharacterOrientationInternal(CharacterOrientation orientation) {
		if ((int) orientation == (int) _currentCharacterOrientation) { return; }

		foreach (var key in References.Keys) {
			References[key].sprite = visuals.SpriteSets[key].Get(orientation);
		}

		var handSprite = visuals.SpriteSets[CharacterPart.Hands].Get(orientation);
		Hand_L.sprite = handSprite;
		Hand_R.sprite = handSprite;

		_currentCharacterOrientation = orientation;
	}

	void CheckForNullValues() {
		if (visuals == null) {
			visuals = new CharacterData.CharacterInfo();
			visuals.Name = "Bob";
			visuals.Initialize();
		}
	}

	public void PickRandomSound() {
		WalkingSource.clip = WalkingSounds[Random.Range(0, WalkingSounds.Count - 1)];
		WalkingSource.Play();
	}

}