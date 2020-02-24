using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus2D;

namespace HighlightPlus2D_Demos {
	
	public class PotionHighlightEventSample : MonoBehaviour {


		void Start () {
			HighlightEffect2D effect = GetComponent<HighlightEffect2D> ();
			effect.OnObjectHighlightStart += ValidateHighlightObject;
		}

		void Update() {
			if (Input.GetKey (KeyCode.LeftArrow)) {
				transform.position += Vector3.left * Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				transform.position += Vector3.right * Time.deltaTime;
			}
		}

		void ValidateHighlightObject (GameObject obj, ref bool cancelHighlight) {
			// Used to fine-control if the object can be highlighted
//			cancelHighlight = true;
		}

		void HighlightStart () {
			Debug.Log ("Potion highlight started!");
		}

		void HighlightEnd () {
			Debug.Log ("Potion highlight ended!");
		}
	}

}