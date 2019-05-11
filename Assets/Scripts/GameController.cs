using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  public UIController uiController;
  public GameObject playerPrefab;
  public GameObject robotPrefab;

  // Instances
  Transform playerSpawn;
  Transform playerGoal;
  Transform robotSpawn;
  Transform robotGoal;
  GameObject player;
  GameObject robot;
  PlayerController pCtrl;
  AIController rCtrl;

  int curScene;

  public bool playerInControl { get; private set; }
  List<Moveset> moveset;

  void Start() {
    moveset = new List<Moveset>();
    DontDestroyOnLoad(this);
    DontDestroyOnLoad(uiController);
  }

  void OnEnable() {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnDisable() {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    Debug.Log("Scene Loaded: " + scene.name);
    DestroyCharacters();
    curScene = scene.buildIndex;
    if (curScene == 0)       
      return;
    LevelStart();
  }

  public void LevelStart() {
    try {
      playerSpawn = GameObject.FindWithTag("PlayerSpawn").transform;
      playerGoal = GameObject.FindWithTag("PlayerGoal").transform;
      robotSpawn = GameObject.FindWithTag("RobotSpawn").transform;
      robotGoal = GameObject.FindWithTag("RobotGoal").transform;
    } catch (UnityException e) {
      Debug.LogError(e.Message);
      return;
    }

    player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
    robot = Instantiate(robotPrefab, robotSpawn.position, robotSpawn.rotation);

    pCtrl = player.GetComponent<PlayerController>();
    rCtrl = robot.GetComponent<AIController>();

    player.name = "Player";
    robot.name = "Robot";
    pCtrl.gameController = rCtrl.gameController = this;

    rCtrl.speed = pCtrl.speed;
    rCtrl.jumpHeight = pCtrl.jumpHeight;
    rCtrl.jumpRechargeTime = pCtrl.jumpRechargeTime;

    Vector3 scale = player.transform.localScale;
    Quaternion rotation = player.transform.rotation;
    robot.transform.localScale.Set(scale.x, scale.y, scale.z);
    robot.transform.rotation.Set(rotation.x, rotation.y, rotation.z, rotation.w);

    playerInControl = true;

    uiController.DisplayTitleText("Begin Level", 3.5f);
    uiController.DisplayLevelButtons();
    uiController.DisplayNextLevelButton(false);
  }

  public void Reset() {
    uiController.HideTitleText();
    playerInControl = true;

    player.transform.position = playerSpawn.position;
    player.transform.rotation = playerSpawn.rotation;
    robot.transform.position = robotSpawn.position;
    robot.transform.rotation = robotSpawn.rotation;

    moveset.Clear();
  }

  public void DestroyCharacters() {
    if (player != null) Destroy(player);
    if (robot != null) Destroy(robot);
    player = null;
    robot = null;
  }

  public void AddMovesetItem(Moveset item) {
    // Don't add the first move if it's the player doing nothing
    if (moveset.Count == 0 && item.move == Move.Nothing) return;

    moveset.Add(item);
  }

  public void SwapControl() {
    playerInControl = !playerInControl;

    if (!playerInControl) {
      // Remove the last move if it's just the player doing nothing
      if (moveset.Count > 0 && moveset[moveset.Count - 1].move == Move.Nothing) {
        moveset.RemoveAt(moveset.Count - 1);
      }

      rCtrl.SetMovesetList(moveset);
      moveset.Clear();
    }

    // Check for level complete after robot finishes movement
    if (playerInControl)
      CheckForLevelComplete();
  }

  public void LoadNextLevel() {
    SceneManager.LoadScene(curScene + 1);
  }

  void CheckForLevelComplete() {
    float pDistToGround = player.GetComponent<BoxCollider2D>().bounds.extents.y;
    float rDistToGround = robot.GetComponent<BoxCollider2D>().bounds.extents.y;
    Vector2 pOrigin = new Vector2(player.transform.position.x, player.transform.position.y - pDistToGround);
    Vector2 rOrigin = new Vector2(robot.transform.position.x, robot.transform.position.y - rDistToGround);
    LayerMask pGoal = LayerMask.GetMask("PlayerGoal");
    LayerMask rGoal = LayerMask.GetMask("RobotGoal");

    // Middle of base is touching goal
    if (Physics2D.Raycast(pOrigin, -Vector2.up, 0.1f, pGoal) &&
      Physics2D.Raycast(rOrigin, -Vector2.up, 0.1f, rGoal)) {
      
      if (curScene <= SceneManager.sceneCountInBuildSettings) {
        uiController.DisplayTitleText("Level Complete!");
        uiController.DisplayNextLevelButton();
      } else {
        uiController.DisplayTitleText("Congrats on Finishing The Game!");
      }
    }
  }
}
