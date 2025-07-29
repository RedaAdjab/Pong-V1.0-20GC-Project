using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PaddleMovement : MonoBehaviour
{
    private static float yBounds = 3.25f;

    public enum PlayerType { Human, AI }

    [SerializeField] PlayerType playerType = PlayerType.Human;
    [SerializeField] private KeyCode moveUp = KeyCode.None;
    [SerializeField] private KeyCode moveDown = KeyCode.None;
    [SerializeField] private BallMovement ballMovement;

    private float moveSpeed = 7f;
    private float nextOffsetTime = 0f;
    private float aiOffsetFreq = 0.3f; //for human-like behavior for error
    private float aiOffsetMax = 0.6f;

    private void Update()
    {
        if (playerType == PlayerType.Human)
        {
            Vector3 moveDirection = new(0, 0, 0);
            if (Input.GetKey(moveUp))
            {
                moveDirection = Vector3.up;
            }
            else if (Input.GetKey(moveDown))
            {
                moveDirection = Vector3.down;
            }
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }
        else
        {
            float offset = 0f;
            if (Time.time >= nextOffsetTime)
            {
                offset = Random.Range(-aiOffsetMax, aiOffsetMax);
                nextOffsetTime = Time.time + aiOffsetFreq;
            }
            Vector3 toBallPosition = Vector3.MoveTowards(transform.position, ballMovement.GetPosition() + new Vector3(0, offset, 0), moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, toBallPosition.y, transform.position.z);
        }
        LimitMovement();
    }

    private void LimitMovement()
    {
        if (transform.position.y >= yBounds || transform.position.y <= -yBounds)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -yBounds, yBounds), transform.position.z);
        }
    }

    public void SetDifficultyParams(float aiOffsetFreq, float aiOffsetMax)
    {
        this.aiOffsetFreq = aiOffsetFreq;
        this.aiOffsetMax = aiOffsetMax;
    }

    public void SetPlayerType(PlayerType playerType)
    {
        this.playerType = playerType;
    }
}
