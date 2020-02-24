using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI NameText;
    // Start is called before the first frame update
    void Start()
    {
        NameText = GetComponent<TMPro.TextMeshProUGUI>() != null ? NameText = GetComponent<TMPro.TextMeshProUGUI>() : NameText = null;
        if (NameText)
        {
            NameText.text = GameFile.Data.Filename;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
