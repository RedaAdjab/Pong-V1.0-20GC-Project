using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class BallMovement : MonoBehaviour
{
    private const float defaultMoveSpeed = 5f;
    private float moveSpeed = defaultMoveSpeed;
    private Vector3 moveDirection = Vector3.zero;
    private float acceleration = 1f;
    private float paddleHeight = 2f;
    private float secondsToWait = 1f;
    private bool isBallReset = true;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnGamePlaying += GameManager_OnGamePlaying;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
        if (!isBallReset)
        {
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            //bounce off top/bottom walls
            moveDirection = new Vector3(moveDirection.x, -moveDirection.y, 0f);
        }
        else if (collision.CompareTag("Player"))
        {
            //apply bounce angle based on hit point on paddle
            float distanceFromCenter = transform.position.y - collision.transform.position.y;
            float halfPaddleHeight = paddleHeight / 2f;

            float minBounceAngle = 22.5f;
            float maxBounceAngle = 75f;
            float t = Mathf.Clamp01(Mathf.Abs(distanceFromCenter / halfPaddleHeight));
            float bounceAngle = Mathf.Lerp(minBounceAngle, maxBounceAngle, t);

            //determine the new horizontal direction based on current direction
            moveDirection = moveDirection.x > 0 ? Vector3.left : Vector3.right;

            //determine the vertical bounce angle sign
            if (distanceFromCenter == 0)
            {
                //slight randomness if hit exactly at the center
                bounceAngle = Random.Range(0, 2) == 0 ? bounceAngle : -bounceAngle;
            }
            else if ((moveDirection == Vector3.left && distanceFromCenter > 0) ||
                    (moveDirection == Vector3.right && distanceFromCenter < 0))
            {
                bounceAngle = -bounceAngle;
            }

            moveDirection = Quaternion.Euler(0, 0, bounceAngle) * moveDirection;
            moveDirection.Normalize();

            //make the ball go fast the longer the game goes on
            Accelerate();
        }
        else if (collision.CompareTag("Goal"))
        {
            //change score and reset the ball
            GameManager.Instance.ChangePlayerScore(collision.transform.position.x < 0 ? 1 : 2);
            StartCoroutine(DelayResetBall(secondsToWait));
        }
    }

    private void Accelerate()
    {
        moveSpeed += acceleration;
    }

    private IEnumerator DelayResetBall(float seconds)
    {
        //randomize initial launch direction (left or right)
        isBallReset = true;
        moveSpeed = defaultMoveSpeed;
        transform.position = Vector3.back; //put the ball in the higher z position
        yield return new WaitForSeconds(seconds);
        moveDirection = Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left;
        isBallReset = false;
    }

    private void GameManager_OnGamePlaying(object sender, EventArgs e)
    {
        StartCoroutine(DelayResetBall(secondsToWait));
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsBallGoingLeft()
    {
        return moveDirection.x < 0;
    }
}
