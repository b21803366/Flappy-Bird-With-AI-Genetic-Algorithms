﻿using System;
using UnityEngine;

public class Bird_Player : MonoBehaviour {

    private const float JUMP_AMOUNT = 90f;

    private static Bird_Player instance;

    public static Bird_Player GetInstance() {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D birdRigidbody2D;
    private State state;

    private enum State {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake() {
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Update() {
        switch (state) {
        default:
        case State.WaitingToStart:
            if (TestInput()) {
                // Start playing
                if(Bird_AI.GetInstance() != null)
                    Bird_AI.GetInstance().touchCount++;
                state = State.Playing;
                birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                Jump();
                if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
            }
            break;
        case State.Playing:
            if (TestInput()) {
                Jump();
            }

            // Rotate bird as it jumps and falls
            transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y * .15f);
            break;
        case State.Dead:
            break;
        }
    }

    private bool TestInput() {
        return
            Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0;
    }

    private void Jump() {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log(collider.gameObject.name);
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }

}
