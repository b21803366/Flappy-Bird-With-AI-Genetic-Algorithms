using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score {

    public static void Start() {
        //ResetHighscore();
        if(Bird_AI.GetInstance() != null)
            Bird_AI.GetInstance().OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied(object sender, System.EventArgs e) {
        if(Level.GetInstance() != null)
            TrySetNewHighscore(Level.GetInstance().GetPipesPassedCount());
        else if(Level2.GetInstance() != null)
            TrySetNewHighscore(Level2.GetInstance().GetPipesPassedCount());
        else
            TrySetNewHighscore(Level_AI.GetInstance().GetPipesPassedCount());
    }

    public static int GetHighscore() {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighscore(int score) {
        int currentHighscore = GetHighscore();
        if (score > currentHighscore) {
            // New Highscore
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        } else {
            return false;
        }
    }

    public static void ResetHighscore() {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
