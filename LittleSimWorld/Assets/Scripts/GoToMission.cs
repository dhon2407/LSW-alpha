using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMission : Mission
{
    Collider2D ZoneToEnter;
    public string nameOfGameObjectWithCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        GetRewardButton.gameObject.SetActive(false);
        ZoneToEnter = GameObject.Find(nameOfGameObjectWithCollider2D).GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ZoneToEnter.IsTouching(GameLibOfMethods.player.GetComponent<Collider2D>()))
        {
            Accomplish();
        }
    }

}
