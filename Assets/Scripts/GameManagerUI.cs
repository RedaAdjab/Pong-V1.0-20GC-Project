using System;
using TMPro;
using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject opponentChoiceUI;
    [SerializeField] private GameObject aiDifficultyChoiceUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnScoreChanged += GameManager_OnScoreChanged;
        gameManager.OnGamePlaying += GameManager_OnGamePlaying;
        gameManager.OnOppenentPicked += GameManager_OnOppenentPicked;

        opponentChoiceUI.SetActive(true);
    }

    private void GameManager_OnScoreChanged(object sender, GameManager.ScoreChangedEventArgs e)
    {
        if (e.playerID == 1)
        {
            player1ScoreText.text = gameManager.GetPlayerScore(e.playerID).ToString();
        }
        else if (e.playerID == 2)
        {
            player2ScoreText.text = gameManager.GetPlayerScore(e.playerID).ToString();
        }
    }

    private void GameManager_OnGamePlaying(object sender, EventArgs e)
    {
        aiDifficultyChoiceUI.SetActive(false);
        gameUI.SetActive(true);
    }

    private void GameManager_OnOppenentPicked(object sender, GameManager.OpponentPickedEventArgs e)
    {
        opponentChoiceUI.SetActive(false);
        if (e.playerType == PaddleMovement.PlayerType.AI)
        {
            aiDifficultyChoiceUI.SetActive(true);
        }
    }
}
