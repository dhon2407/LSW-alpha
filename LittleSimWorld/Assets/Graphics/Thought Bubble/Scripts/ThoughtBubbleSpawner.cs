using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ThoughtBubbleSpawner : MonoBehaviour
{
    public static ThoughtBubbleSpawner Instance;
    public GameObject ThoughtBubble;
    public float Cooldown = 10;
    public bool LastBubbleCD;
    private List<GameObject> Bubbles = new List<GameObject>();
    [SerializeField]
    public Dictionary<object, float> Cooldowns = new Dictionary<object, float>();
    public Sprite[] StatSprites;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SpawnBubble(Transform whereToGravitateParent, Vector3 positionToSpawnFrom, object Object, Sprite bubbleSprite = null, int statSprite = -1)
    {
        StartCoroutine(Spawn(whereToGravitateParent, positionToSpawnFrom, Object, bubbleSprite, statSprite));
    }

    IEnumerator Spawn(Transform whereToGravitateParent, Vector3 positionToSpawnFrom, object Object, Sprite bubbleSprite = null, int statSprite = -1)
    {
        LastBubbleCD = true;
        var bubblePos = new Vector2(whereToGravitateParent.position.x + .42f, whereToGravitateParent.position.y + 1.3f);
        var bubble = Instantiate(ThoughtBubble, bubblePos, Quaternion.Euler(Vector3.zero), whereToGravitateParent);
        var bubbleChildren = bubble.GetComponentsInChildren<Transform>();
        foreach (var child in bubbleChildren)
            if (child.name.Contains("Sprite"))
            {
                if (statSprite >= 0) child.GetComponent<SpriteRenderer>().sprite = StatSprites[statSprite];
                else child.GetComponent<SpriteRenderer>().sprite = bubbleSprite;
            }
        Bubbles.Add(bubble);
        yield return new WaitForSeconds(Cooldown);
        LastBubbleCD = false;
    }

    public void DestroyBubble()
    {
        Destroy(Bubbles.First());
        Bubbles.RemoveAt(0);
    }

    //public void ToggleText()
    //{
    //    Bubbles.Last().GetComponentInChildren<>
    //    if (Bubbles.GetComponentInChildren<TextMeshPro>().text == BubblesText)
    //        Bubbles.GetComponentInChildren<TextMeshPro>().text = "";
    //    else Bubbles.GetComponentInChildren<TextMeshPro>().text = ImmediateBubbleText;
    //}
}
