// using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using MiniGames.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameSO _gameSO;
    private Text textText;

    private void Awake()
    {
        textText = gameObject.GetComponent<Text>();
    }
    void OnEnable()
    {

        InputManager.confirmEvent += OnConfirm;
    }

    void OnDisable()
    {
        InputManager.confirmEvent -= OnConfirm;
    }

    void Update()
    {
        switch (_gameSO.gameState)
        {
            case GameState.Start:
                textText.text = Constants.k_StartLabel;
                break;
            case GameState.Playing:
                textText.text = String.Format(
                        Constants.k_ScoreLabel,
                        _gameSO.timeSpent,
                        _gameSO.faceUndoneCount,
                        _gameSO.faceCurrent >= 0 ? _gameSO.faceTimeLeft[_gameSO.faceCurrent] : -1,
                        (int)_gameSO.gameState
                    );
                break;
            case GameState.Win:
                textText.text = Constants.k_WinLabel;
                break;
            case GameState.Lose:
                textText.text = String.Format(
                        Constants.k_LoseLabel,
                        _gameSO.timeSpent,
                        _gameSO.faceUndoneCount
                    );
                break;

            default:
                break;
        }

    }

    private void OnConfirm()
    {
        switch (_gameSO.gameState)
        {
            case GameState.Start:
                _gameSO.UpdateGameState(GameState.Playing);
                break;
            case GameState.Win:
            case GameState.Lose:
                _gameSO.UpdateGameState(GameState.Start);
                break;
            default:
                break;
        }

    }
}
