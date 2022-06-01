using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class MainMenuWindow : MonoBehaviour {

    private void Awake() {
        transform.Find("playBtn").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene); };
        transform.Find("playBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("playBtn2").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene2); };
        transform.Find("playBtn2").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("playBtn_AI").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene_AI); };
        transform.Find("playBtn_AI").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("quitBtn").GetComponent<Button_UI>().ClickFunc = () => { Application.Quit(); };
        transform.Find("quitBtn").GetComponent<Button_UI>().AddButtonSounds();
    }

}
