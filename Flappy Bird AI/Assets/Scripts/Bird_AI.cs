using System;
using UnityEngine;

public class Bird_AI : MonoBehaviour {

    private const float JUMP_AMOUNT = 60f;

    private static Bird_AI instance;
    public static bool tester_=false;
    public static Bird_AI GetInstance() {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    
    public Rigidbody2D birdRigidbody2D;
    private State state;
    public float positionY;
    public static Vector3 currPos;
    private enum State {
        WaitingToStart,
        Playing,
        Dead
    }
    public int touchCount { get; set; }

    private void Awake() {
        instance = this;
        touchCount = 0;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }

    private void Update() {
        switch (state) {
        default:
        case State.WaitingToStart:
            if (TestInput() ) {
                // Start playing
                state = State.Playing;
                birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                Jump();
                touchCount--;
                if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
            }
            break;
        case State.Playing:
            while (TestInput()) {
                Jump();
                touchCount--;
            }

            // Rotate bird as it jumps and falls
            transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y * .15f);
            currPos=transform.position;
            break;
        case State.Dead:
            break;
        }
    }

    private bool TestInput() {
        return touchCount > 0;
    }

    public void Jump() {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }
    

    private void OnTriggerEnter2D(Collider2D collider) {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
    public float getJump(){
        return JUMP_AMOUNT;
    }
    public void Die()
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
    

}
