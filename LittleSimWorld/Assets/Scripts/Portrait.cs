using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : MonoBehaviour
{
    private static Portrait instance;
    public Camera PortraitCamera;
    public float DoPhotoAfterThisAmountOfFrames = 120;
    int FramesAfterStart = 0;
    bool alreadyInitialized = false;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Render()
    {
        PortraitCamera.Render();
    }
    private void Update()
    {
        FramesAfterStart += 1;
        if(!alreadyInitialized && FramesAfterStart > DoPhotoAfterThisAmountOfFrames)
        {
            alreadyInitialized = true;
            SpriteControler.Instance.FaceDOWN();
            Render();
        }
    }

    public static void TakePortraitNextFrame()
    {
        instance.StartCoroutine(instance.RendeBeforeNextFrame());
    }

    private IEnumerator RendeBeforeNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Render();
    }
}
