using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GlowEffect : MonoBehaviour
{
    

    [Header("Glow effect")]
    public float approachSpeed = 0.02f;
    public float growthBound = 2f;
    public float shrinkBound = 0.5f;
    public float currentRatio = 1;

    public bool keepGoing = false;
    public Color GlowingColor;

    private Color StartingColor;
    private Image GlowingImage;

    void Start()
    {
        GlowingImage = gameObject.GetComponent<Image>();
		GlowOfOutline().Start(MEC.Segment.FixedUpdate);
        StartingColor = GlowingImage.color;
    }

    public IEnumerator<float> GlowOfOutline()
    {
		// Run this indefinitely

		while (true) {
			if (keepGoing) {
				// Get bigger for a few seconds
				while (currentRatio != growthBound) {
					// Determine the new ratio to use
					currentRatio = Mathf.MoveTowards(currentRatio, growthBound, approachSpeed);

					// Update our text element
					GlowingImage.color = Color.Lerp(StartingColor, GlowingColor, currentRatio);

					//Debug.Log("growing");
					yield return 0f;
				}

				// Shrink for a few seconds
				while (currentRatio != shrinkBound) {
					// Determine the new ratio to use
					currentRatio = Mathf.MoveTowards(currentRatio, shrinkBound, approachSpeed);

					// Update our text element
					GlowingImage.color = Color.Lerp(StartingColor, GlowingColor, currentRatio);
					//Debug.Log("shrinking");
					yield return 0f;
				}
			}
			else {
				GlowingImage.color = StartingColor;
				yield return 0f;
			}
			//yield return new WaitForFixedUpdate();
		}
    }
}
