using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
  public GameObject player;
  public GameObject robot;
  public Vector2 spawnPoint;


  public bool playerInControl { get; private set; }
  PlayerController pCtrl;
  AIController rCtrl;
  List<Moveset> moveset;

  void Start() {
    moveset = new List<Moveset>();
    pCtrl = player.GetComponent<PlayerController>();
    rCtrl = robot.GetComponent<AIController>();
    Reset();
  }

  public void Reset() {
    if (spawnPoint == null) spawnPoint = Vector2.zero;
    
    playerInControl = true;
    player.transform.position.Set(spawnPoint.x, spawnPoint.y + 0.5f, 0);
    robot.transform.position.Set(spawnPoint.x - (player.GetComponent<BoxCollider2D>().size.x + 0.5f), spawnPoint.y + 0.5f, 0);

    rCtrl.speed = pCtrl.speed;
    rCtrl.jumpHeight = pCtrl.jumpHeight;
    rCtrl.jumpRechargeTime = pCtrl.jumpRechargeTime;

    moveset.Clear();
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
