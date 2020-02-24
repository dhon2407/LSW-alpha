using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
public class PlayerChatLog : MonoBehaviour {
	public static PlayerChatLog Instance;
	public Scrollbar Scroll;
    public GameObject ChatMessege;
    public Transform ChatContent;
    public GuiFadeAnim fadeAnim;

    private ScrollRect _scrollRect;

    // Start is called before the first frame update
    private void Awake() {
		if (Instance == null) {
			Instance = this;
			_scrollRect = GetComponent<ScrollRect>();
		}
		else {
			Destroy(this);
		}
	}
	void Start() {

	}

	public bool MouseOver { get; set; } = false;

	public IEnumerator Reset()
    {
	    _scrollRect.enabled = true;
        yield return new WaitForFixedUpdate();
        Canvas.ForceUpdateCanvases();
        Scroll.value = 0f;
        yield return null;
        Debug.Log("Chat resetted");
        
        if (!MouseOver)
			_scrollRect.enabled = false;
    }
	// Update is called once per frame
	void Update() {
	}
    public void AddChatMessege(string text)
    {
        GameObject chatMessege = Instantiate(ChatMessege, ChatContent);
        chatMessege.GetComponentInChildren<TextMeshProUGUI>().text = "[" + GameTime.Clock.CurrentTimeFormat + "]" + text;
        StartCoroutine(Reset());
        fadeAnim.FadeIn();
    }

}
