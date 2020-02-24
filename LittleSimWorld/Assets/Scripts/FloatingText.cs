using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 5;
    public float lifeTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Animation anim in GetComponentsInChildren<Animation>())
        {
            anim.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float _speed = Time.deltaTime * floatSpeed;
        transform.position += Vector3.up * _speed;
    }
}
