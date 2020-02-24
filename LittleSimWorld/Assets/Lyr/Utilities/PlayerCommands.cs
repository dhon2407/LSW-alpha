using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public static class PlayerCommands {
	static float DefaultJumpSpeed => InteractionChecker.Instance.JumpSpeed;

	static AnimationCurve jumpCurve => InteractionChecker.Instance.jumpCurve;

	public static Vector3 LastPositionBeforeJump;
	public static Vector3 LastPositionBeforeWalk;

	public static void MoveTo(Vector3 position, Action callback) => StartWalking(position, callback).Start();
	public static void MoveTo(Objects.IUseable useable, Action callback = null) {
		Action action = callback ?? (() => useable.BeginUsing());
		StartWalking(useable.useableFunctionality.PlayerStandPosition, action).Start();
	}

	public static void JumpTo(Vector3 position, Action callback) => StartJumping(position, callback).Start();
	public static void JumpTo(Objects.IUseable useable, Action callback = null) {
		var action = callback ?? (() => useable.BeginUsing());
		StartJumping(useable.useableFunctionality.PlayerStandPosition, action, useable, useable.useableFunctionality.CustomSpeedToPosition).Start();
	}

	public static void JumpOff(float CustomSpeed = 0, Action CustomCallBack = null) {
		if (CustomCallBack == null) { CustomCallBack = PlayerAnimationHelper.ResetPlayer; }
		else { CustomCallBack = PlayerAnimationHelper.ResetPlayer + CustomCallBack; }
		StartJumping(LastPositionBeforeJump, CustomCallBack, null, CustomSpeed).Start();
	}

	public static void WalkBackToLastPosition() => StartWalking(LastPositionBeforeWalk, PlayerAnimationHelper.ResetPlayer).Start();
	public static void WalkBackToLastPosition(Action callback) => StartWalking(LastPositionBeforeWalk, () => { PlayerAnimationHelper.ResetPlayer(); callback(); }).Start();

	public static void DelayAction(float waitTime, Action callback = null) => DelayWithAction(waitTime).AddCallback(callback).Start();
	public static void DelayAction(float waitTime, Action callback, Func<bool> cancelCondition, Action cancelCallback = null) => DelayWithAction(waitTime).AddCallback(callback).CancelIf(cancelCondition, cancelCallback).Start();

	static IEnumerator<float> StartJumping(Vector3 TargetPosition, Action callback, Objects.IUseable useable = null, float CustomSpeed = 0) {
        TargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, 1);
		var StartPosition = GameLibOfMethods.player.transform.position;

		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.doingSomething = true;

		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);

		Player.rb.velocity = Vector2.zero;
        Player.anim.SetBool("Walking", false);
        Player.anim.SetBool("Jumping", true);

		float T = 0;
		float _speed = CustomSpeed > 0 ? CustomSpeed : DefaultJumpSpeed;


		while (true) {
			T += _speed * GameTime.Time.deltaTime;

			Player.transform.position = Vector3.Lerp(StartPosition, TargetPosition, T);
			Player.transform.localScale = new Vector3(jumpCurve.Evaluate(T), jumpCurve.Evaluate(T));

			// if our value has reached the total, break out of the loop
			if (T >= 1) { break; }

			yield return 0f;
		}

		Player.anim.SetBool("Jumping", false);
		//PlayerAnimationHelper.ResetPlayer();
		callback?.Invoke();
		LastPositionBeforeJump = StartPosition;
	}

	static IEnumerator<float> StartWalking(Vector3 TargetPosition, Action callback) {
        TargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, 1);
		Vector3 StartPosition = Player.transform.position;

		PlayerAnimationHelper.ResetAnimations();

		GameLibOfMethods.doingSomething = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;

		Player.anim.SetBool("Walking", true);

		float T = 0;

		Vector3 temp = Player.transform.position;
		while (true) {

			if (!GameTime.Clock.Paused)
			{
				T += 0.04f * GameTime.Time.deltaTime / GameTime.Time.fixedDeltaTime;

				var dif = (TargetPosition - temp).normalized;
				if (Mathf.Abs(dif.x) < dif.y)
				{
					Player.anim.SetFloat("Vertical", dif.y);
				}
				else
				{
					Player.anim.SetFloat("Horizontal", dif.x);
				}

				Player.anim.SetBool("Walking", true);
				GameLibOfMethods.player.transform.position = Vector3.Lerp(temp, TargetPosition, T);

				PlayerAnimationHelper.HandlePlayerFacing();

				// if our value has reached the total, break out of the loop
				if (T >= 1)
					break;
			}

			yield return 0f;
		}

		Player.anim.SetBool("Walking", false);

		callback?.Invoke();
		LastPositionBeforeWalk = StartPosition;
	}


	static IEnumerator<float> DelayWithAction(float waitTime) {
		float t = 0;
		GameLibOfMethods.doingSomething = true;
		GameLibOfMethods.cantMove = true;
		while (true) {
			t += GameTime.Time.deltaTime;
			if (t >= waitTime) { break; }
			yield return 0f;
		}
		GameLibOfMethods.cantMove = false;
		GameLibOfMethods.doingSomething = false;
	}
}