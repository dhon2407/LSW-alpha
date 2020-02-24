using System.Collections;
using System.Collections.Generic;
using GameSettings;
using Objects.Functionality;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Objects
{

    [RequireComponent(typeof(GameAudioSource))]
    public abstract class BaseObject : SerializedMonoBehaviour
    {
        public GameAudioSource source => _source ? _source : _source = GetComponent<GameAudioSource>();
        GameAudioSource _source;

        protected void PlaySound(AudioClip clip) => source.PlaySound(clip);

    }

    public abstract class InteractableObject : BaseObject, IInteractable
    {
        public InteractionOptions interactionOptions => _interactionOptions;
        [OdinSerialize, ShowInInspector, PropertyOrder(100)] InteractionOptions _interactionOptions = null;
        public virtual bool isValidInteractionTarget => true;

        public abstract void Interact();

        protected virtual void OnDrawGizmosSelected()
        {
            if (interactionOptions != null)
                Gizmos.DrawWireSphere(transform.position + (Vector3)interactionOptions.InteractionCenterOffset,
                    interactionOptions.InteractionRange);
        }


        #region EDITOR
#if UNITY_EDITOR
        bool shouldShowInitializeButton => interactionOptions == null || !interactionOptions.MouseOverCollider || !interactionOptions.PlayerPathfindingTarget;

        [Button, PropertyOrder(-10), ShowIf("shouldShowInitializeButton")]
        void FillInteractionReferences()
        {
            var parent = transform;
            Transform MouseOverArea = null;
            Transform PathfindingPoint = null;

            if (interactionOptions == null) { _interactionOptions = new InteractionOptions(); }
            if (transform.Find("Helpers")) { parent = transform.Find("Helpers"); }
            if (parent.Find("MouseOverArea")) { MouseOverArea = parent.Find("MouseOverArea"); }
            else
            {
                MouseOverArea = new GameObject("MouseOverArea").transform;
                MouseOverArea.SetParent(parent);
            }

            if (parent.Find("PathfindingPoint")) { PathfindingPoint = parent.Find("PathfindingPoint"); }
            else
            {
                PathfindingPoint = new GameObject("PathfindingPoint").transform;
                PathfindingPoint.SetParent(parent);
            }

            interactionOptions.PlayerPathfindingTarget = PathfindingPoint;
            interactionOptions.MouseOverCollider = MouseOverArea;

            Debug.Log("Initialized");
        }

        //void OnValidate() {
        //	for (int i = 0; i < 5; i++) {
        //		if (!UnityEditorInternal.ComponentUtility.MoveComponentUp(this)) { break; }
        //	}
        //}
#endif
        #endregion

    }

    public abstract class UseableObject : InteractableObject, IUseable
    {

        public UseableFunctionality useableFunctionality => _useableFunctionality;
        [OdinSerialize, ShowInInspector, PropertyOrder(101)] UseableFunctionality _useableFunctionality = null;
        public ObjectUseState useState { get; set; }
        public CharacterAction CurrentAction { get; set; }
        public override bool isValidInteractionTarget => useState == ObjectUseState.Unused;
        protected virtual bool shouldCancel => Input.GetKeyUp(InteractionChecker.Instance.KeyToInteract) || GameLibOfMethods.passedOut;

        // We want a delayed frame between interaction and usage, so the GetKeyUp will reset.
        bool delayedFrame;
        protected virtual void Update()
        {
            if (useState == ObjectUseState.InUse)
            {
                if (delayedFrame) { Use(); }
                else { delayedFrame = true; }

            }
            else { delayedFrame = false; }
        }

        public abstract void BeginUsing();
        public abstract void Use();
        public abstract void FinishUsing(bool cancel);

        public virtual SetActionStateResult TrySetCurrentAction(string ActionName)
        {
            var charAction = useableFunctionality.GetActionWithName(ActionName);
            if (charAction.ValidateStats())
            {
                CurrentAction = charAction;
                return (true, null);
            }

            return (false, charAction.GetErrorMessage());
        }

        /// <summary>
        /// Calls <see cref="FinishUsing(bool)"/> after the delay specified on the <see cref="useableFunctionality"/>.
        /// <para>It will be called sooner if the player presses the interact button.</para>
        /// </summary>
        protected void ExitAfterDelay(bool cancel)
        {
            bool cancelCondition() => Input.GetKeyUp(InteractionChecker.Instance.KeyToInteract);
            void delayedCall()
            {
                FinishUsing(cancel);
                if (this is IBreakable b && b.breakFunctionality.DidBreakDuringLastUse()) { b.breakFunctionality.Break(); }
            }

            MECExtensionMethods.CallDelayed(CurrentAction.DelayAfterFinish, delayedCall, CurrentAction.ApplyStatsOnUse, cancelCondition);
        }

        /// <summary>
        /// Reset the object state and terminate the action.
        /// </summary>
        /// <param name="cancel">Was the action canceled?</param>
        protected void ResetObjectState(bool cancel)
        {
            useState = ObjectUseState.Unused;
            CurrentAction.EndAction(cancel);
            CurrentAction = null;
        }

    }

    public abstract class BreakableObject : UseableObject, IBreakable
    {
        public BreakableFunctionality breakFunctionality => _breakFunctionality;
        [OdinSerialize, ShowInInspector] BreakableFunctionality _breakFunctionality = null;
    }
}