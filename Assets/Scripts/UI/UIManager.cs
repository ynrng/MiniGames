// using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using MiniGames.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameSO _gameSO;
    // attention to use the right Class (NOT TextMeshPro)
    // private TMP_Text scoreLabelTMP;
    private Text textText;
    // Start is called before the first frame update
    private void Awake()
    {
        textText = gameObject.GetComponent<Text>();
        // scoreLabelTMP = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_gameSO.gameState)
        {
            case GameState.Start:
                // scoreLabelTMP.SetText(Constants.k_StartLabel);
                textText.text = Constants.k_StartLabel;
                break;
            case GameState.Playing:
                textText.text = String.Format(
                        Constants.k_ScoreLabel,
                        _gameSO.timeSpent,
                        String.Join(",", _gameSO.faceUndone),
                        _gameSO.faceCurrent,
                        _gameSO.faceTimeLeft[_gameSO.faceCurrent],
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
                        String.Join(",", _gameSO.faceUndone)
                    );
                break;

            default:
                break;
        }

    }
}
