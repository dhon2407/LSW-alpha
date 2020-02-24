using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HighlightPlus2D
{


    [RequireComponent(typeof(HighlightEffect2D))]
    [HelpURL("http://kronnect.com/taptapgo")]
    public class HighlightManager2D : MonoBehaviour
    {

        public event OnObjectHighlightStartEvent OnObjectHighlightStart;
        public event OnObjectHighlightEndEvent OnObjectHighlightEnd;

        public HighlightOnEvent highlightEvent = HighlightOnEvent.OnOverAndClick;
        [Tooltip("Max duration for the highlight event")]
        public float highlightDuration;

        public LayerMask layerMask = -1;
        public Camera raycastCamera;
        public RayCastSource raycastSource = RayCastSource.MousePosition;

        public HighlightEffect2D baseEffect, currentEffect;
        public Transform currentObject;
        public bool menuOpen = false;

        static HighlightManager2D _instance;
        public static HighlightManager2D instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HighlightManager2D>();
                }
                return _instance;
            }
        }

        void OnEnable()
        {
            currentObject = null;
            currentEffect = null;
            if (baseEffect == null)
            {
                baseEffect = GetComponent<HighlightEffect2D>();
                if (baseEffect == null)
                {
                    baseEffect = gameObject.AddComponent<HighlightEffect2D>();
                }
            }
            raycastCamera = GetComponent<Camera>();
            if (raycastCamera == null)
            {
                raycastCamera = GetCamera();
                if (raycastCamera == null)
                {
                    Debug.LogError("Highlight Manager 2D: no camera found!");
                }
            }
        }


        void OnDisable()
        {
            SwitchesCollider(null);
        }

        

        void Update()
        {

            if (GameTime.Clock.Paused) return;
            if (!menuOpen)
            {

                if (raycastCamera == null)
                    return;
                if (highlightEvent == HighlightOnEvent.OnlyOnClick && !Input.GetMouseButtonDown(0))
                    return;

                Ray ray;
                if (raycastSource == RayCastSource.MousePosition)
                {
                    ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
                }
                else
                {
                    ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
                }

                RaycastHit2D hitInfo2D = Physics2D.GetRayIntersection(ray, 10, layerMask.value);
                if (hitInfo2D)
                {
                    Collider2D collider = hitInfo2D.collider;
                    if (collider != null)
                    {
                        var targetTransform = collider.transform;

                        // Max depth of 3 at the moment
                        for (int i = 0; i < 3; i++)
                        {
                            if (targetTransform.gameObject.layer == 10 || targetTransform.gameObject.layer == 15) { break; }
                            targetTransform = targetTransform.transform.parent;
                        }

                        if (targetTransform != currentObject)
                        {
                            SwitchesCollider(targetTransform);
                        }

                        return;
                    }
                }
            }
            // no hit
            if (highlightDuration == 0)
            {
                SwitchesCollider(null);
            }

        }


        public void SwitchesCollider(Transform newObject)
        {

            if (currentEffect != null)
            {
                if (OnObjectHighlightEnd != null)
                {
                    OnObjectHighlightEnd(currentEffect.gameObject);
                }
                currentEffect = null;
            }

            currentObject = newObject;
            if (newObject == null) { return; }

            HighlightTrigger2D ht = newObject.GetComponent<HighlightTrigger2D>();
            if (ht != null && ht.enabled) { return; }

            if (OnObjectHighlightStart != null)
            {
                bool cancelHighlight = false;
                OnObjectHighlightStart(newObject.gameObject, ref cancelHighlight);
                if (cancelHighlight) { return; }
            }

            HighlightEffect2D otherEffect = newObject.GetComponent<HighlightEffect2D>();
            currentEffect = otherEffect != null ? otherEffect : baseEffect;
            currentEffect.SetTarget(currentObject.transform);
            currentEffect.SetHighlighted(true);

            //Debug.Log("highlighting " + newObject);
            if (highlightEvent > 0)
            {
                CancelInvoke();
                Invoke("CancelHighlight", highlightDuration);
            }
        }

        public void CancelHighlight()
        {

            SwitchesCollider(null);
            Debug.Log("Canceling Highlight");


        }

        public static Camera GetCamera()
        {
            Camera raycastCamera = Camera.main;
            if (raycastCamera == null)
            {
                raycastCamera = FindObjectOfType<Camera>();
            }
            return raycastCamera;
        }


    }

}