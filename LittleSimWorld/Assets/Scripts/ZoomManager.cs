using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ZoomManager : MonoBehaviour
{
    public Camera cam;
    public float maxCameraZoom;
    public float minCameraZoom;
    [SerializeField] float ZoomStepPercentage = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        if (!cam)
        {
            cam = GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0 || !Application.isFocused) { return; }
        if (!CameraFollow.Instance || EventSystem.current.IsPointerOverGameObject()) { return; }

        float zoomAdj = -Input.GetAxis("Mouse ScrollWheel") * ((maxCameraZoom - minCameraZoom) * ZoomStepPercentage);
        ForceUpdateCamera(cam.orthographicSize + zoomAdj, true);
    }
    void ForceUpdateCamera(float targetSize, bool forceRecalculation = false)
    {
        if (CameraFollow.Instance != null)
        {
            var currentSize = cam.orthographicSize;
            if (forceRecalculation || targetSize != currentSize)
            {
                var recalculatedTargetCameraSize = Mathf.Clamp(targetSize, minCameraZoom, maxCameraZoom);
                if (recalculatedTargetCameraSize != currentSize) { cam.orthographicSize = recalculatedTargetCameraSize; }
            }
            CameraFollow.Instance.SetLimits();
            CameraFollow.Instance.UpdateCamera();
        }
    }
}
