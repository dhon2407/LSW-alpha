using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiFadeAnim : MonoBehaviour
{
    [Header("Canvas Group")]
    public CanvasGroup cg;

    [Header("Fading Time")]
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Fade To Value")]
    public float fadeInTo;
    public float fadeOutTo;

    [Header("Setting Delays")]
    public bool delay;
    public float delayTime;

    
    [Header("For Repetion of Animation")]
    public bool repeat;
    public int repetitionTurns;

    [Header("For Looping Animation")]
    public bool loop;


    private void Start()
    {

        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;

        

        if (!repeat)
        {
            FadeIn();
        }
        else
        {
            FadeIn(repetitionTurns);
        }
       
    }

    //0
    public void FadeIn()
    {
        if (loop)
        {
            LeanTween.alphaCanvas(cg, fadeInTo, fadeInTime).setLoopPingPong();
        }
        else
        {
            LeanTween.alphaCanvas(cg, fadeInTo, fadeInTime);
        }
    }

    public void FadeIn(int repeatTime)
    {
       
            LeanTween.alphaCanvas(cg, fadeInTo, fadeInTime).setLoopPingPong().setRepeat(repeatTime);
      
       
    }


    public void FadeIn(float delayTime)
    {
        if (loop)
        {
            LeanTween.alphaCanvas(cg, fadeInTo, fadeInTime).setDelay(this.delayTime);
        }
        else
        {
            LeanTween.alphaCanvas(cg, fadeInTo, fadeInTime).setDelay(this.delayTime).setLoopPingPong();
        }
    }


   

    //1
    public void FadeOut()
    {
        LeanTween.alphaCanvas(cg, fadeOutTo, fadeOutTime);
    }

    //2
    public void FadeOutWithDisable(GameObject go)
    {
        LeanTween.alphaCanvas(cg, fadeOutTo, fadeOutTime);
        StartCoroutine(WaitRoutine(2, go));
    }
    //3
    public void FadeOutWithDestroy(GameObject go)
    {
        LeanTween.alphaCanvas(cg, fadeOutTo, fadeOutTime);
    }
    
    private IEnumerator WaitRoutine(int fNum, GameObject go)
    {
        yield return new WaitForSeconds(delayTime);
        switch (fNum)
        { 

            case 2:
                {
                    go.SetActive(false);
                    break;
                }
            case 3:
                {
                    Destroy(go);
                    break;
                }
        }
    }
    

}
