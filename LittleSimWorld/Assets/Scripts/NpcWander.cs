using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This makes an object move randomly in a set of directions, with some random time delay in between each decision
/// </summary>
public class NpcWander : MonoBehaviour
{
    internal Transform thisTransform;
    public float moveSpeed = 0.2f;

    public Vector2 decisionTime = new Vector2(1, 5);

    public SpriteRenderer Body;
    public Sprite[] body;
    internal float decisionTimeCount = 0.2f;

    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero, Vector3.zero };
    public int currentMoveDirection;

    // Use this for initialization
    void Start()
    {
        thisTransform = this.transform;
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

        ChooseMoveDirection();
    }

    void FixedUpdate()
    {
        
        thisTransform.position += moveDirections[currentMoveDirection] * GameTime.Time.deltaTime * moveSpeed;

        if (decisionTimeCount > 0) decisionTimeCount -= GameTime.Time.deltaTime;
        else
        {
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
            
            ChooseMoveDirection();
        }
    }

    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));

        if(currentMoveDirection < 4)
            Body.sprite = body[currentMoveDirection];
    }
}
