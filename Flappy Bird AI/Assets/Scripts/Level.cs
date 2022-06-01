using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using System.Net;
using System.Net.Sockets;
using System.Threading;
public class Level : MonoBehaviour {
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 50f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;
    private const float GROUND_DESTROY_X_POSITION = -200f;
    private const float CLOUD_DESTROY_X_POSITION = -160f;
    private const float CLOUD_SPAWN_X_POSITION = +160f;
    private const float CLOUD_SPAWN_Y_POSITION = +30f;
    private const float BIRD_X_POSITION = 0f;
    float receivedPos = BIRD_X_POSITION;
    private static Level instance;

    public static Level GetInstance() {
        return instance;
    }
    private AutoPlay AutoPlayB0;
    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private List<Pipe> pipeList;
    private List<Line> lineList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;
    public int totalbird=0;
    public float output_pid;
    public float jp=0;
    public float red=0;
    public int clicksPerSecond=60;
    public double currFit=0;
    public System.DateTime startTime;
    public System.DateTime endTime;

    public enum Difficulty {
        Easy,
        Rare1,
        Rare2,
        Medium,
        MediumB1,
        MediumB2,
        MediumB3,
        Hard,
        Impossible,
    }

    private enum State {
        WaitingToStart,
        Playing,
        BirdDead,
    }
//C:\OutputPID.csv
    private void Awake() {
        instance = this;
        SpawnInitialGround();
        SpawnInitialClouds();
        pipeList = new List<Pipe>();
        lineList = new List<Line>();
        //pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start() {
        startTime = System.DateTime.Now;
        Bird_AI.GetInstance().OnDied += Bird_OnDied;
        Bird_AI.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
        
        AutoPlayB0=new AutoPlay(27.81909f, 57.11937f, 9.467463f);
   
    }
    private void Bird_OnStartedPlaying(object sender, System.EventArgs e) {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e) {
        state = State.BirdDead;
    }

    private void Update() {
        if (state == State.Playing) {
           /*if(receivedPos==1){
                Bird_AI.GetInstance();
           };*/
           if(Application.targetFrameRate != 60){
               Application.targetFrameRate=60;
           }
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleClouds();
            AutoPlayB0.work(lineList[0].GetYpos(), Bird_AI.currPos.y,Application.targetFrameRate,clicksPerSecond,0);
            

        }
    }

    private void SpawnInitialClouds() {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform GetCloudPrefabTransform() {
        switch (Random.Range(0, 3)) {
        default:
        case 0: return GameAssets.GetInstance().pfCloud_1;
        case 1: return GameAssets.GetInstance().pfCloud_2;
        case 2: return GameAssets.GetInstance().pfCloud_3;
        }
    }

    private void HandleClouds() {
        // Handle Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0) {
            // Time to spawn another cloud
            float cloudSpawnTimerMax = 6f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_X_POSITION, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        // Handle Cloud Moving
        for (int i=0; i<cloudList.Count; i++) {
            Transform cloudTransform = cloudList[i];
            // Move cloud by less speed than pipes for Parallax
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * .7f;

            if (cloudTransform.position.x < CLOUD_DESTROY_X_POSITION) {
                // Cloud past destroy point, destroy self
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnInitialGround() {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 192f;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGround() {
        foreach (Transform groundTransform in groundList) {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if (groundTransform.position.x < GROUND_DESTROY_X_POSITION) {
                // Ground passed the left side, relocate on right side
                // Find right most X position
                float rightMostXPosition = -100f;
                for (int i = 0; i < groundList.Count; i++) {
                    if (groundList[i].position.x > rightMostXPosition) {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }

                // Place Ground on the right most position
                float groundWidth = 192f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void HandlePipeSpawning() {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0) {
            // Time to spawn another Pipe
            pipeSpawnTimer += pipeSpawnTimerMax;
            
            float heightEdgeLimit = 14f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }

    private void HandlePipeMovement() {
        for (int i=0; i<pipeList.Count; i++) {
            Pipe pipe = pipeList[i];

            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom()) {
                // Pipe passed Bird
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }

            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION) {
                // Destroy Pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
        for (int i = 0; i < lineList.Count; i++)
        {
            Line line = lineList[i];
            line.Move();
            if (line.GetXPosition() < BIRD_X_POSITION-4.5)
            {
                // Destroy Line
                line.DestroySelf();
                lineList.Remove(line);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty) {
        System.Random rand = new System.Random();
        switch (difficulty) {
        case Difficulty.Easy:
            gapSize = 35f;
            pipeSpawnTimerMax =0.725f;//(float)(rand.NextDouble()*(0.75 - 0.70) + 0.70);
            break;
        case Difficulty.MediumB1:
            gapSize = 33f;
            pipeSpawnTimerMax = 0.705f;//(float)(rand.NextDouble()*(0.68 - 0.65) + 0.65);
            break;
        case Difficulty.MediumB2:
            gapSize = 33f;
            pipeSpawnTimerMax = 0.71f;//(float)(rand.NextDouble()*(0.63 - 0.55) + 0.55);
            break;
        case Difficulty.MediumB3:
            gapSize = 33f;
            pipeSpawnTimerMax = 0.70f;//(float)(rand.NextDouble()*(0.68 - 0.65) + 0.65);
            break;
        case Difficulty.Rare1:
            gapSize = 34f;
            pipeSpawnTimerMax = (float)(rand.NextDouble() * (0.72 - 0.71) + 0.71);
            break;
        case Difficulty.Rare2:
            gapSize = 33f;
            pipeSpawnTimerMax = (float)(rand.NextDouble() * (0.71 - 0.70) + 0.70);
            break;
        case Difficulty.Medium:
            gapSize = 32f;
            pipeSpawnTimerMax =(float)(rand.NextDouble()*(0.70 - 0.67) + 0.67);
            break;
        case Difficulty.Hard:
            gapSize = 31f;
            pipeSpawnTimerMax =(float)(rand.NextDouble()*(0.70 - 0.63) + 0.63);
            break;
        case Difficulty.Impossible:
            gapSize = 30f;
            pipeSpawnTimerMax = 0.63f;
            break;
        }
    }

    private Difficulty GetDifficulty() {
        if (pipesSpawned >= 4000) return Difficulty.Impossible;
        if (pipesSpawned >= 3900) return Difficulty.MediumB2;
        if (pipesSpawned >= 1400) return Difficulty.Hard;
        if (pipesSpawned >= 1300) return Difficulty.MediumB3;
        if (pipesSpawned >= 800) return Difficulty.Medium;
         if (pipesSpawned >= 750) return Difficulty.MediumB2;
        if (pipesSpawned >= 600) return Difficulty.Rare2;
        if (pipesSpawned >= 570) return Difficulty.MediumB3;
        if (pipesSpawned >= 400) return Difficulty.Rare2;
         if (pipesSpawned >= 375) return Difficulty.MediumB1;
        if (pipesSpawned >= 300) return Difficulty.Rare2;
        if (pipesSpawned >= 280) return Difficulty.MediumB2;
        if (pipesSpawned >= 235) return Difficulty.Rare1;
        if (pipesSpawned >= 220) return Difficulty.MediumB1;
        if (pipesSpawned >= 165) return Difficulty.Rare1;
        if (pipesSpawned >= 150) return Difficulty.MediumB2;
        if (pipesSpawned >= 70) return Difficulty.Rare1;
        if (pipesSpawned >= 50) return Difficulty.MediumB1;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition) {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        CreateLine(gapY - CAMERA_ORTHO_SIZE - gapSize*1/5, xPosition);
        pipesSpawned++;
       
  
       
            SetDifficulty(GetDifficulty());
        
    }
    //The pipe Positions
    private void CreatePipe(float height, float xPosition, bool createBottom) {
        // Set up Pipe Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom) {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        } else {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        // Set up Pipe Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom) {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        } else {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }
       private void CreateLine(float yPosition, float xPosition)
    {
        // Set up Pipe Head
        Transform lineTransform = Instantiate(GameAssets.GetInstance().pfLine);
        lineTransform.position = new Vector3(xPosition, yPosition);

        Line line = new Line(lineTransform);
        lineList.Add(line);
    }

    public int GetPipesSpawned() {
        return pipesSpawned;
    }

    public int GetPipesPassedCount() {
        return pipesPassedCount;
    }

    /*
     * Represents a single Pipe
     * */
    private class Pipe {

        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom) {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move() {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition() {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom() {
            return isBottom;
        }

        public void DestroySelf() {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }

    }
        private class Line
    {
        private Transform bodyTransform;

        public Line(Transform bodyTransform)
        {
            this.bodyTransform = bodyTransform;
        }

        public void Move()
        {
            bodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return bodyTransform.position.x;
        }
        public float GetYpos(){
            return bodyTransform.position.y;
        }

        public void DestroySelf()
        {
            Destroy(bodyTransform.gameObject);
        }

    }

    
}

