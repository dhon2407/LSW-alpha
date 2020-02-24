using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArrow : MonoBehaviour
{
    public Animator anim;
    public Image image;
    float CurrentFill;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float PreviousFill = CurrentFill;
        CurrentFill = image.fillAmount;

        
            anim.SetFloat("Delta", CurrentFill - PreviousFill);
            
        
        
        



    }
}
