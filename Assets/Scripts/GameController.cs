using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
  public GameObject playerPrefab;
  public GameObject robotPrefab;
  public Transform playerSpawn;
  public Transform robotSpawn;

  // Instances
  GameObject player;
  GameObject robot;

  public bool playerInControl { get; private set; }
  PlayerController pCtrl;
  AIController rCtrl;
  List<Moveset> moveset;

  void Start() {
    moveset = new List<Moveset>();
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
  }

  public void Reset() {
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
  }
}
