using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;

public class TimeDependentDoor : MonoBehaviour
{
    public bool isOpen;
    public Sprite OpenedDoor;
    public Sprite ClosedDoor;
    public float openTimeInSeconds = 21600;
    public float closeingTimeInSeconds = 79200;

    private void Update()
    {
        
         if(GameClock.Time >= openTimeInSeconds  && GameClock.Time < closeingTimeInSeconds)
        {
            OpenDoor();
        }
        else if(GameClock.Time >= openTimeInSeconds && GameClock.Time >= closeingTimeInSeconds)
        {
            CloseDoor();
        }
    }
    public void CloseDoor()
    {
            isOpen = false;
            GetComponent<Collider2D>().isTrigger = false;
            GetComponent<SpriteRenderer>().sprite = ClosedDoor;
            return;
    }
    public void OpenDoor()
    {
        isOpen = true;
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().sprite = OpenedDoor;
        return;
    }
}
