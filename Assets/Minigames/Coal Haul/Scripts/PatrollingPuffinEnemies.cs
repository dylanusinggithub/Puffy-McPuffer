using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PatrollingPuffinEnemies : MonoBehaviour
{
    public float speed; //Puffin enemy speed
    public float patrolDistance; //Puffin enemy patrolling distance
    [SerializeField] private bool moveHorizontally; //Boolean determines whether Puffin enemy will move horizontally (left to right) or vertically (up and down)

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;


    void Start()
    {
        startPosition = transform.position;

        //If moving vertically, change default direction movement
        if (moveHorizontally)
        {
            targetPosition = startPosition + Vector3.right * patrolDistance;
        }

        else
        {
            targetPosition = startPosition + Vector3.up * patrolDistance;
        }
    }


    void Update()
    {
        //Move Puffin enemy in set direction
        transform.position = Vector3.MoveTowards(transform.position,
            movingToTarget ? targetPosition : startPosition,
            speed * Time.deltaTime);

        //If Puffin enemy has moved too far from starting position
        if (Vector3.Distance(transform.position, (movingToTarget ? targetPosition : startPosition)) <= 0.1f)
        {
            movingToTarget = !movingToTarget;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //If Puffin enemy collides with Wall boundary
        if (collision.gameObject.CompareTag("Wall"))
        {
            movingToTarget = !movingToTarget;
        }
    }
}
