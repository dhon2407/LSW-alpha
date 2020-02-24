using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class NpcSpawner : MonoBehaviour
{
    public List<Sprite> WhiteHeads = new List<Sprite>();
    public List<Sprite> WhiteBodies = new List<Sprite>();
    public List<Sprite> WhiteHands = new List<Sprite>();
    public Dictionary<string, Sprite> Sprites;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnRandomNpc()
    { 
         Sprite HeadDown = WhiteBodies.Where(obj => obj.name == "HeadDown").FirstOrDefault();
    }
    private void LoadWhiteHeads()
    {
        Sprite[] SpritesData = Resources.LoadAll<Sprite>("WhiteHeads");
        Sprites = new Dictionary<string, Sprite>();

        for (int i = 0; i < SpritesData.Length; i++)
        {
            Sprites.Add(SpritesData[i].name, SpritesData[i]);
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        if (Sprites.ContainsKey(name))
            return Sprites[name];
        else
            return null;
    }
}
