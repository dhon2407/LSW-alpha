using GameSettings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;

namespace GameCamera
{

    [RequireComponent(typeof(PixelPerfectCamera))]
    public class PixelPerfectZoomManager : MonoBehaviour
    {
		new PixelPerfectCamera camera;

		const int defaultMinPPU = 105;
		const int defaultMaxPPU = 170;
		const float defaultScreenHeight = 1080f;
		const int defaultZoomSpeed = 60;


		[SerializeField] float ZoomSpeed = 60;

		[Space]

		[SerializeField] int minPPU = 105;
		[SerializeField] int maxPPU = 170;

		void Awake() {
			camera = GetComponent<PixelPerfectCamera>();
		}
		void Start() {
			UpdateValues();
			Settings.Display.onChangeResolution.AddListener(delegate { UpdateValues(); });
		}
		void LateUpdate() {
			// Check for mousewheel first for optimization
			if (Input.GetAxis("Mouse ScrollWheel") == 0 || !Application.isFocused) { return; }
			if (!CameraFollow.Instance || EventSystem.current.IsPointerOverGameObject()) { return; }

			float zoomAdj = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
			ForceUpdateCamera(camera.assetsPPU + Mathf.RoundToInt(zoomAdj));
		}

		void ForceUpdateCamera(int targetPPU, bool forceRecalculation = false) {
            if (CameraFollow.Instance != null) {
				var currentPPU = camera.assetsPPU;
				if (forceRecalculation || targetPPU != currentPPU) {
					var recalculatedTargetPPU = Mathf.Clamp(targetPPU, minPPU, maxPPU);
					if (recalculatedTargetPPU != currentPPU) { camera.assetsPPU = recalculatedTargetPPU; }
				}
                CameraFollow.Instance.SetLimits();
                CameraFollow.Instance.UpdateCamera();
            }
		}

		private void UpdateValues()
        {
#if !UNITY_EDITOR
            camera.refResolutionX = Settings.Display.CurrentGameResolution.width;
            camera.refResolutionY = Settings.Display.CurrentGameResolution.height;

			float ratio = Settings.Display.CurrentGameResolution.height / defaultScreenHeight;

			float previousZoomPercentage = (camera.assetsPPU - minPPU) / (float)(maxPPU - minPPU);

			// Update value range
			minPPU = (int) (defaultMinPPU * ratio);
			maxPPU = (int) (defaultMaxPPU * ratio);
			
			ZoomSpeed = defaultZoomSpeed * ratio;

			// // Set current zoom to be the middle of the zoom range allowed
			// int newZoom = minPPU + (maxPPU - minPPU) / 2;
			
			// Set zoom to match previous' zoom amount
			float newZoom = minPPU + (maxPPU - minPPU) * previousZoomPercentage;

			ForceUpdateCamera((int)newZoom, true);
#endif
		}
    }
}