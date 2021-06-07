
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.ComponentModel;

[CreateAssetMenu(fileName = "GameSO", menuName = "SO/GameSO", order = 0)]
public class GameSO : ScriptableObject
{
    public event UnityAction<GameState, GameState> onGameStateChange = delegate { };

    private GameState _gameState;
    public GameState gameState { get { return _gameState; } }
    public DateTime timeStart;
    public float timeSpent;
    public const float timeFail = 6000; // s

    // public int faceDone = 0; /* 已经煎完的面 */
    public const float eachFaceDoneTime = 3; // s
    public List<float> faceTimeLeft;
    public List<int> faceUndone;// = new List<int>{ 0, 1, 2, 3, 4, 5 };
    public int faceUndoneCount = 6;
    public int faceCurrent = 0; // debug

    // actually onCreate; so not called every launch
    // private void Awake() { }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CookBeefScene")
        {
            Debug.Log("[GameSO]CookBeefScene Loaded");
            ResetGameState(GameState.Start);
        }
    }

    public void ResetGameState([DefaultValue("GameState.Start")] GameState state = 0)
    {
        Debug.Log("[GameSO]ResetGameState:" + state);
        _gameState = state;
        // timeStart ;
        timeSpent = -1;
        // timeFail = 60; // s
        // faceDone = 0;
        // eachFaceDoneTime = 5000; // ms
        // faceTimeLeft = 0;
        faceUndone = new List<int> { 0, 1, 2, 3, 4, 5 };
        faceUndoneCount = 6;
        faceCurrent = 0;
        faceTimeLeft = new List<float> { eachFaceDoneTime, eachFaceDoneTime, eachFaceDoneTime, eachFaceDoneTime, eachFaceDoneTime, eachFaceDoneTime };
    }

    public void UpdateGameState(GameState state)
    {
        if (_gameState != state)
        {
            if (state == GameState.Start)
            {
                ResetGameState();
            }
            onGameStateChange.Invoke(_gameState, state);
        }
        Debug.Log("[GameSO]UpdateGameState:" + state);
        _gameState = state;
    }

    public bool CheckIsTimeOver()
    {
        if (gameState == GameState.Playing)
        {
            timeSpent = (float)DateTime.Now.Subtract(timeStart).TotalSeconds;
            if (timeSpent > timeFail)
            {
                UpdateGameState(GameState.Lose);
                return true;
            }
        }
        return false;
    }

    public bool CheckIsFaceAllDone()
    {
        if (gameState == GameState.Playing)
        {
            if (faceUndone.Count <= 0)
            {
                UpdateGameState(GameState.Win);
                return true;
            }
        }
        return false;
    }

    public bool CheckIsFaceRaw(int faceIndex)
    {
        return faceTimeLeft[faceIndex] > 0;
    }

    public float SetFaceTimeLeft(int faceIndex, float delta)
    {
        if (faceTimeLeft[faceIndex] > 0)
        {
            faceTimeLeft[faceIndex] += delta;
            if (faceTimeLeft[faceIndex] <= 0)
            {
                faceTimeLeft[faceIndex] = 0;
                --faceUndoneCount;
            }
        }
        return faceTimeLeft[faceIndex];

    }

}

public enum GameState
{
    Start,
    Playing,
    Win,
    Lose

}
