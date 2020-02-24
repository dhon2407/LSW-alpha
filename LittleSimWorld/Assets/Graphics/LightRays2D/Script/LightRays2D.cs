using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weather;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class LightRays2D:LightRays2DAbstract{

    public float LightIntensityMultiplyer = 0.5f;
    [SerializeField]
    private GameTime.DayAndNightController dayAndNightController = null;

	private MeshRenderer mr;

	//For sorting layers
	[HideInInspector]
	public int sortingLayer=0;
	private int _sortingLayer;
	[HideInInspector]
	public int orderInLayer=0;
	private int _orderInLayer;

	protected override void GetReferences(){
		mr=GetComponent<MeshRenderer>();
	}

	protected override Material GetMaterial(){
		return mr.sharedMaterial;
	}

	protected override void Update(){
		base.Update();
		if (sortingLayer!=_sortingLayer || orderInLayer!=_orderInLayer){
			mr.sortingLayerID=sortingLayer;
			mr.sortingOrder=orderInLayer;
			_sortingLayer=sortingLayer;
			_orderInLayer=orderInLayer;
		}

        if (WeatherSystem.CurrentWeather == WeatherType.Sunny)
        {
            float intensity = dayAndNightController.GetLightIntensityUpperBoundRatio();
            color1.a = intensity * LightIntensityMultiplyer;
            color2.a = intensity * LightIntensityMultiplyer;
        }
        else
        {
            color1.a = 0;
            color2.a = 0;
        }

    }
	
	protected override void ApplyMaterial(Material material){
		
	}
	
}
