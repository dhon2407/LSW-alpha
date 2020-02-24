using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbubblesReceiver : MonoBehaviour
{
    public Animation animationToPlayOnCollision;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnParticleTrigger()
    {
        //Debug.Log("playing");
       // if (!animationToPlayOnCollision.isPlaying)
            animationToPlayOnCollision.Play();
    }
    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("playing");
        //if(!animationToPlayOnCollision.isPlaying)
        animationToPlayOnCollision.Play();
    }
}
