using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

  public UIController uiController;

  public GameObject playerPrefab;
  public GameObject robotPrefab;

  public Transform playerSpawn;
  public Transform playerGoal;
  public Transform robotSpawn;
  public Transform robotGoal;

  // Instances
  GameObject player;
  GameObject robot;

  public bool playerInControl { get; private set; }
  PlayerController pCtrl;
  AIController rCtrl;
  List<Moveset> moveset;

  void Start() {
    moveset = new List<Moveset>();
    Spawn();
  }

  public void Spawn() {
    DestroyCharacters();

    player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
    robot = Instantiate(robotPrefab, robotSpawn.position, robotSpawn.rotation);

    pCtrl = player.GetComponent<PlayerController>();
    rCtrl = robot.GetComponent<AIController>();

    player.name = "Player";
    robot.name = "Robot";
    pCtrl.gameController = rCtrl.gameController = this;

    playerInControl = true;

    uiController.DisplayTitleText("Begin Level", 3.5f);
  }

  public void Reset() {
    uiController.HideTitleText();
    playerInControl = true;

    player.transform.position = playerSpawn.position;
    player.transform.rotation = playerSpawn.rotation;
    robot.transform.position = robotSpawn.position;
    robot.transform.rotation = robotSpawn.rotation;

    rCtrl.speed = pCtrl.speed;
    rCtrl.jumpHeight = pCtrl.jumpHeight;
    rCtrl.jumpRechargeTime = pCtrl.jumpRechargeTime;

    Vector3 scale = player.transform.localScale;
    Quaternion rotation = player.transform.rotation;
    robot.transform.localScale.Set(scale.x, scale.y, scale.z);
    robot.transform.rotation.Set(rotation.x, rotation.y, rotation.z, rotation.w);

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

      rCtrl.SetMoveset(moveset);
      moveset.Clear();
    }

    // Check for level complete after robot finishes movement
    if (playerInControl)
      CheckForLevelComplete();
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
      uiController.DisplayTitleText("Level Complete!");
    }
  }
}
