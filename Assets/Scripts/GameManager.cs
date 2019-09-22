﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject cameraOrbit;
    public GameObject aimGuideMesh;
    public GameObject ball;

    public GameObject MainMenuUI;
    public GameObject GamePlayUI;
    public GameObject LevelCompleteUI;
    public GameObject LevelFailUI;
    public GameObject GameCompleteUI;
    

    public GameObject BallGroup;
    //public GameObject AimGuide;  //may not be needed

    public GameObject levelManager;
    public GameObject uIManager;

    private BallController _ballController;
    private LevelManager _levelManager;
    private UIManager _uIManager;

    private int shotsLeft = 99;

    private GameObject LevelInfo;
    private LevelInfo _levelInfo;

    private float stateChangeTimeStamp;
    private float checkTime;
    private float delay;

    public float BallStopCheckTime;
    public float BallStopDelayTime;

    public GameObject startPosition;

    public enum GameState { MainMenu, Aim, Rolling, LoseCheck, LevelComplete }

    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        _ballController = ball.GetComponent<BallController>();
        _levelManager = levelManager.GetComponent<LevelManager>();
        _uIManager = uIManager.GetComponent<UIManager>();
        startPosition = GameObject.FindWithTag("StartPos");
        cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameState);

        SceneManager.sceneLoaded += OnSceneLoaded;

        //Debug.Log(_ballController.ballSpeed);


        switch (gameState)
        {
            case GameState.MainMenu:
                MainMenuUI.SetActive(true);
                GamePlayUI.SetActive(false);
                LevelCompleteUI.SetActive(false);
                GameCompleteUI.SetActive(false);
                


                break;

            // *** AIM *** ,this mode lets you aim your shot and fire with SPACE

            case GameState.Aim:
                cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = true;
                MainMenuUI.SetActive(false);
                GamePlayUI.SetActive(true);
                LevelCompleteUI.SetActive(false);
                GameCompleteUI.SetActive(false);
                LevelFailUI.SetActive(false);

                _uIManager.modeText.text = "Aim with MOUSE \n & \n Shoot with SPACE";

                aimGuideMesh.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    aimGuideMesh.SetActive(false);
                    _ballController.ballShoot();
                    shotsLeft -= 1;
                    _uIManager.UpdateShotsleft(shotsLeft);
                    gameState = GameState.Rolling;
                    TimeDelay(0.1f);
                }
                break;




            // *** ROLLING *** ,after Shooting when the ball is rolling

            case GameState.Rolling:

                

                cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = true;

                _uIManager.modeText.text = "Wait for ball to stop";
                
                // once ball is ALMOST not moving, stop the ball outright.
                if (Time.time > checkTime) // this adds a slight delay before checking, prevents it from thinking the ball stopped before it even started moving
                {
                    if (_ballController.ballSpeed < 0.05f)
                    {     
                            _ballController.StopBall();
                            gameState = GameState.LoseCheck;
                    }
                }
                break;



            // *** LOSECHECK ***, after each shot is done rolling, check to see if you used up all your shots (if so you lose and level resets)

            case GameState.LoseCheck:
                

                if (shotsLeft <= 0)
                {
                    cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = false;
                    aimGuideMesh.SetActive(false);

                    GamePlayUI.SetActive(false);
                    LevelFailUI.SetActive(true);
                }
                else
                { gameState = GameState.Aim; }

                                                              
                break;


            // *** LEVEL COMPLETE *** if you manage to hit the target goal, celebrate and switch to next level

            case GameState.LevelComplete:

                cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = false;
                MainMenuUI.SetActive(false);
                GamePlayUI.SetActive(false);
                //play Fireworks

                int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

                Debug.Log(nextScene);

                if (nextScene > SceneManager.sceneCountInBuildSettings -1) 
                {
                    Debug.Log("last Scene");
                    GameCompleteUI.SetActive(true);
                }
                else
                {
                    LevelCompleteUI.SetActive(true);
                }                    
                break;
        }

    }

    public void StartGame()
    {
        _levelManager.LoadNextlevel();        
        BallGroup.SetActive(true);
        gameState = GameState.Aim;
    }

    public void ResetBallPos()
    {
        ball.transform.position = startPosition.transform.position;
        _ballController.StopBall();
        cameraOrbit.GetComponent<MouseOrbitImproved>().ResetCamera();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelInfo = GameObject.FindWithTag("LevelInfo");
        _levelInfo = LevelInfo.GetComponent<LevelInfo>();
        shotsLeft = _levelInfo.ShotsToComplete;

        startPosition = GameObject.FindWithTag("StartPos");
        ResetBallPos();
        _uIManager.UpdateShotsleft(shotsLeft);
    }

    public void GoalReached()
    {
        gameState = GameState.LevelComplete;        
    }

    public void loadNextLevel()
    {
        LevelCompleteUI.SetActive(false);
        _ballController.StopBall();
        _levelManager.LoadNextlevel();
        gameState = GameState.Aim;
        cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = true;
    }

    public void ReloadLevel()
    {
        LevelCompleteUI.SetActive(false);
        _ballController.StopBall();
        _levelManager.ReloadCurrentScene();
        gameState = GameState.Aim;
        cameraOrbit.GetComponent<MouseOrbitImproved>().enabled = true;
    }

    public void returnToMainMenu()
    {
        _levelManager.LoadMainMenu();
        gameState = GameState.MainMenu;
        ResetBallPos();
        Camera.main.transform.SetPositionAndRotation(new Vector3(-0.7f,10f,11f), Quaternion.Euler(new Vector3(45, -180, 0)));

    }
         
    void TimeDelay(float Delay)
    {
        stateChangeTimeStamp = Time.time;
        checkTime = Time.time + Delay; 
    }

    void BallStopDelay()
    {
        BallStopCheckTime = Time.time;
        BallStopDelayTime = Time.time + 0.5f;
    }



}
