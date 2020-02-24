using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class DialogueSystem : MonoBehaviour
// include trigger & dialogue coroutine

{
    public KeyCode DialogueInput = KeyCode.E;
    public TextMeshProUGUI dialogueText;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public static bool done = false; // controls that the dialogue is done
    
    // CURRENTLY not in used.
    //bool collding = false; // same as OnTriggerEnter but working good


    public GameObject DialogueUI;

    void Start()
    {
        DialogueUI.SetActive(false);
        //collding = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            index = Random.Range(0, sentences.Length);
            //collding = true;
            DialogueUI.SetActive(true);
            dialogueText.text = "";
            StartCoroutine(Type());

        }
    }

   

    void OnTriggerExit2D(Collider2D other)
    {
        //collding = false;
        DialogueUI.SetActive(false);
        StopAllCoroutines();

    }

    public IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
            if (dialogueText.text == sentences[index])
            {
                yield return new WaitForSeconds(3);
                NextSentence();
            }
        }
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Type());
        }
        else
        {
            dialogueText.text = "";
            DialogueUI.SetActive(false);
        }
    }

    void Update()
    {
        /*if (collding)
        {
            if (index > sentences.Length - 1)
            {
                DialogueUI.SetActive(false);
            }
            else
            {

                DialogueUI.SetActive(true);

            }

          
            collding = false;

            if (done == false)
            {
                StartCoroutine(Type());

            }

            if (done == true)
            {
                DialogueUI.SetActive(false);
            }
        }*/
    }
}
