using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<ScoreChangedEventArgs> OnScoreChanged;
    public class ScoreChangedEventArgs : EventArgs
    {
        public int playerID;
    }

    public event EventHandler<OpponentPickedEventArgs> OnOppenentPicked;
    public class OpponentPickedEventArgs : EventArgs
    {
        public PaddleMovement.PlayerType playerType;
    }

    public event EventHandler OnGamePlaying;

    public enum AIDifficulty { Easy, Normal, Hard }

    [SerializeField] private PaddleMovement paddleMovement;
    [SerializeField] private BallMovement ballMovement;

    private bool isGamePlaying = false;
    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ChangePlayerScore(int playerID)
    {
        if (playerID == 1)
        {
            player1Score++;
        }
        else if (playerID == 2)
        {
            player2Score++;
        }
        OnScoreChanged?.Invoke(this, new ScoreChangedEventArgs { playerID = playerID });
    }

    public int GetPlayerScore(int playerID)
    {
        if (playerID == 1)
        {
            return player1Score;
        }
        else if (playerID == 2)
        {
            return player2Score;
        }
        return 0;
    }

    public bool IsGamePlaying()
    {
        return isGamePlaying;
    }

    public void OnPlayerPicked()
    {
        paddleMovement.SetPlayerType(PaddleMovement.PlayerType.Human);
        ballMovement.gameObject.GetComponent<Renderer>().enabled = true;
        isGamePlaying = true;
        OnOppenentPicked?.Invoke(this, new OpponentPickedEventArgs { playerType = PaddleMovement.PlayerType.Human });
        OnGamePlaying?.Invoke(this, EventArgs.Empty);
    }

    public void OnAiPicked()
    {
        paddleMovement.SetPlayerType(PaddleMovement.PlayerType.AI);
        OnOppenentPicked?.Invoke(this, new OpponentPickedEventArgs { playerType = PaddleMovement.PlayerType.AI });
    }

    public void OnAiEasy()
    {
        OnAiDifficultyPicked(AIDifficulty.Easy, 0.1f, 1f);
    }

    public void OnAiNormal()
    {
        OnAiDifficultyPicked(AIDifficulty.Normal, 0.3f, 0.4f);
    }

    public void OnAiHard()
    {
        OnAiDifficultyPicked(AIDifficulty.Hard, 0, 0);
    }

    private void OnAiDifficultyPicked(AIDifficulty aIDifficulty, float aiOffsetFreq, float aiOffsetMax)
    {
        paddleMovement.SetDifficultyParams(aiOffsetFreq, aiOffsetMax);
        ballMovement.gameObject.GetComponent<Renderer>().enabled = true;
        isGamePlaying = true;
        OnGamePlaying?.Invoke(this, EventArgs.Empty);
    }
}
