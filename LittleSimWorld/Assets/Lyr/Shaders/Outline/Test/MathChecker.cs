using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MathChecker : MonoBehaviour
{
	public float Size1 = 1000;
	public float Size2 = 256;

	public float uv;

	//[Header("INFO")]
	[ShowInInspector] public float Result => (uv - minUV) * ratio;
	[ShowInInspector] public float ratio => Size1 / Size2;

	[ShowInInspector] public float minUV => (Size1 - Size2) / Size1 / 2;
	[ShowInInspector] public float maxUV => 1 - minUV;


}
