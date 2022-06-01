﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow2 : MonoBehaviour {

    private Text scoreText;
    private Text highscoreText;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
        
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene2); };
        transform.Find("retryBtn").GetComponent<Button_UI>().AddButtonSounds();
        
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.MainMenu); };
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Start() {
        Bird_Player.GetInstance().OnDied += Bird_OnDied;
        Bird_Player2.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Update() {
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            // Retry
            Loader.Load(Loader.Scene.GameScene2);
        }*/
    }

    private void Bird_OnDied(object sender, System.EventArgs e) {
        scoreText.text = Level2.GetInstance().GetPipesPassedCount().ToString();

        if (Level2.GetInstance().GetPipesPassedCount() >= Score.GetHighscore()) {
            // New Highscore!
            highscoreText.text = "NEW HIGHSCORE";
        } else {
            highscoreText.text = "HIGHSCORE: " + Score.GetHighscore();
        }

        Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}