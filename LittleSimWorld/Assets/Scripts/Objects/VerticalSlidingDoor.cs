using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;

public class VerticalSlidingDoor : SlidingDoor
{
    protected override void Start()
    {
        startLeftPosition = leftDoor.transform.position;
        startRightPosition = rightDoor.transform.position;

        endLeftPosition = new Vector3(leftDoor.position.x , leftDoor.position.y - maxDoorPosition);
        endRightPosition = new Vector3(rightDoor.position.x , rightDoor.position.y + maxDoorPosition);

        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
        transform.GetChild(1).GetComponent<Collider2D>().enabled = false;

    }

	protected override void OpenDoor()
    {
        leftDoor.position = Vector3.Lerp(leftDoor.position, endLeftPosition, doorSpeed);
        rightDoor.position = Vector3.Lerp(rightDoor.position, endRightPosition, doorSpeed);
    }

    protected override void CloseDoor()
    {
        leftDoor.position = Vector3.Lerp(leftDoor.position, startLeftPosition, doorSpeed);
        rightDoor.position = Vector3.Lerp(rightDoor.position, startRightPosition, doorSpeed);
    }

}
