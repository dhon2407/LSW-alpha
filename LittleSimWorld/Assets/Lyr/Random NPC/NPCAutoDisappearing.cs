using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAutoDisappearing : MonoBehaviour
{
    private bool OutOfView, Disappearing;
    private Camera cam;
    private IEnumerator DisappearRoutine;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        IsOutOfView();
        if (OutOfView && !Disappearing)
        {
            DisappearRoutine = Disappear();
            StartCoroutine(DisappearRoutine);
        }
        else if (!OutOfView && Disappearing)
        {
            StopCoroutine(DisappearRoutine);
            Disappearing = false;
        }
    }

    private void IsOutOfView()
    {
        Vector3 camPos = GameLibOfMethods.player.transform.position;
        float sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;
        float sqrMag = Vector2.SqrMagnitude(transform.position - camPos);
        // We don't want nodes that are within camera view;
        if (sqrMag <= 3 * sqrOrthoSize || gameObject.transform.position.magnitude > 800)
            OutOfView = false;
        
        else OutOfView = true;
    }

    private IEnumerator Disappear()
    {
        Disappearing = true;
        yield return new WaitForSeconds(7);
        Disappearing = false;
        GetComponent<Characters.RandomNPC.RandomNPC>().OnCompleteAction();
        //GetComponent<Characters.RandomNPC.RandomNPC>().commandQueue.Clear();
    }
}
