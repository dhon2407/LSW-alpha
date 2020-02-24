using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeSoundHandler : MonoBehaviour
{
    [SerializeField] private AudioClip open = null, close = null;

    [SerializeField] private AudioSource source = null;

    public void PlayOpenCloseSound()
    {
        if (source.clip == open)
        {
            source.clip = close; 
            source.Play();
        }
        else
        {
            source.clip = open; 
            source.Play();
        }
    }
}
