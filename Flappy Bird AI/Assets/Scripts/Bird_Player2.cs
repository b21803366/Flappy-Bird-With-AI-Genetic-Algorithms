using System;
using UnityEngine;

public class Bird_Player2 : MonoBehaviour {

    private const float JUMP_AMOUNT = 90f;

    private static Bird_Player2 instance;

    public static Bird_Player2 GetInstance() {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D birdRigidbody2D;
    private State state;

    public Transform player1;
    private enum State {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake() {
        instance = this;
        Physics2D.IgnoreCollision(player1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
            Input.GetKeyDown(KeyCode.Space) ||
            Input.touchCount > 0;
    }

    private void Jump() {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log("Collide: " + collider.gameObject.name);
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }

}
