using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimationHelper {
	static Animator anim => Player.anim;

	#region Initialization 
	static PlayerAnimationHelper() {
#if UNITY_EDITOR




#else




#endif
	}
	#endregion


	public static void ResetAnimations() {
		var _anim = Player.anim;
		_anim.SetBool("Lifting", false);
		_anim.SetBool("Use Toilet", false);
		_anim.SetBool("Use Shower", false);
		_anim.SetBool("Cooking", false);
		_anim.SetBool("Mixing", false);
		_anim.SetBool("Sleeping", false);
		_anim.SetBool("Jumping", false);
		_anim.SetBool("Eating", false);
		_anim.SetBool("Learning", false);
		_anim.SetBool("Drinking", false);
		_anim.SetBool("Fixing", false);
		_anim.ResetTrigger("PassOutToSleep");
        _anim.SetBool("HidePlayer", false);
    }

	public static void ResetPlayer() {
		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, false);

		GameLibOfMethods.isSleeping = false;
		GameLibOfMethods.cantMove = false;
		GameLibOfMethods.canInteract = true;
		GameLibOfMethods.doingSomething = false;

		GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector2.zero);
		anim.enabled = true;

        GameTime.Clock.ResetSpeed();

        JobManager.Instance.isWorking = false;

        ResetAnimations();
	}

	public static void StopPlayer() {
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.doingSomething = true;
	}

	public static void HandlePlayerFacing() {
		if (anim.GetFloat("Vertical") < 0) { SpriteControler.Instance.FaceDOWN(); }
		if (anim.GetFloat("Vertical") > 0) { SpriteControler.Instance.FaceUP(); }
		if (anim.GetFloat("Horizontal") < 0) { SpriteControler.Instance.FaceLEFT(); }
		if (anim.GetFloat("Horizontal") > 0) { SpriteControler.Instance.FaceRIGHT(); }
	}


}
