using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToStartWindow2 : MonoBehaviour {

    private void Start() {
        Bird_Player.GetInstance().OnStartedPlaying += WaitingToStartWindow_OnStartedPlaying;
        Bird_Player2.GetInstance().OnStartedPlaying += WaitingToStartWindow_OnStartedPlaying;
    }

    private void WaitingToStartWindow_OnStartedPlaying(object sender, System.EventArgs e) {
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}
