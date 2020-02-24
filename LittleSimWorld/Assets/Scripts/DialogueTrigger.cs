using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class DialogueTrigger : MonoBehaviour
//just trigger other Dialogue Script 
{
    public KeyCode DialogueInput = KeyCode.E;


    // Start is called before the first frame update
    void Start()
    {
        DialogueCanvas.enabled = false;
        //bad idea, because dialogue canvas will be just hided, not disabled
        //need remake this
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Canvas DialogueCanvas;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Input.GetKeyDown(DialogueInput) && (other.tag == "Player"))
        {
            DialogueCanvas.enabled = true;
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(DialogueInput) && (other.tag == "Player"))
        {
            DialogueCanvas.enabled = true;
            Debug.Log("shitty message");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        DialogueCanvas.enabled = false;
    }
}
