using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D))]
public class AIController : AbstractCharacterController {

  [SerializeField] List<Moveset> movesetList;
  int currentMoveIdx;
  Moveset currentMoveset;
  bool isActive {
    get {
      return !gameController.playerInControl;
    }
  }

  const float ACCEL_TIME = 0.32f; // time it takes to go from 0 to 1/-1, mimmicing keyboard input
  float accelerationTimer; // how much time has passed since beginning of input
  float decelerationTimer;
  bool doneWithPrevMove;
  bool shouldDecelerate;

  void Start() {
    base.InitCharacter();
    doneWithPrevMove = true;
    shouldDecelerate = false;
    currentMoveIdx = 0;
    movesetList = new List<Moveset>();
    currentMoveset = new Moveset { move = Move.Nothing };
  }

  void FixedUpdate() {
    if (!isActive) return;

    if (doneWithPrevMove) {
      if (CheckIfFinished()) return;
      StartCoroutine(ChangeMove());
      if (CheckIfFinished()) return;
    }

    DoAction();
  }

  bool CheckIfFinished() {
    if (currentMoveIdx >= movesetList.Count) {
      gameController.SwapControl();
      return true;
    }
    return false;
  }

  IEnumerator ChangeMove() {
    accelerationTimer = 0;
    decelerationTimer = 0;
    doneWithPrevMove = false;
    shouldDecelerate = currentMoveIdx == movesetList.Count - 1 ||
      movesetList[currentMoveIdx + 1].move == Move.Nothing;

    currentMoveset = movesetList[currentMoveIdx];
    yield return new WaitForSeconds(currentMoveset.duration);
    currentMoveIdx++;
    doneWithPrevMove = true;
  }

  void DoAction() {
    float input = 0;
    bool decelerating = shouldDecelerate && (accelerationTimer > currentMoveset.duration - ACCEL_TIME) && accelerationTimer >= ACCEL_TIME;

    switch (currentMoveset.move) {
      case Move.Right:
        input = MimicInput(0, 1, decelerating);
        transform.Translate(speed * input * Time.deltaTime, 0, 0);
        break;

      case Move.Left:        
        input = MimicInput(0, -1, decelerating);
        transform.Translate(speed * input * Time.deltaTime, 0, 0);
        break;

      case Move.Jump:
        TryJump();
        break;

      case Move.Duck:
        // attempt duck
        break;

      case Move.JumpRight:
        input = MimicInput(0, 1, decelerating);
        transform.Translate(speed * input * Time.deltaTime, 0, 0);
        TryJump();
        break;

      case Move.JumpLeft:
        input = MimicInput(0, -1, decelerating);
        transform.Translate(speed * input * Time.deltaTime, 0, 0);
        TryJump();
        break;

      case Move.JumpDuck:
        // attempt duck
        TryJump();
        break;

      case Move.ReleaseControl:
        gameController.SwapControl();
        break;

      case Move.Nothing:
      default:
        break;
    }

    if (decelerating) {
      decelerationTimer += Time.deltaTime;
    }
    accelerationTimer += Time.deltaTime;
  }

  float MimicInput(float a, float b, bool decelerating) {
    if (decelerating)
      return Mathf.Lerp(b, a, Mathf.Min(decelerationTimer, ACCEL_TIME) / ACCEL_TIME);
    return Mathf.Lerp(a, b, Mathf.Min(accelerationTimer, ACCEL_TIME) / ACCEL_TIME);
  }

  void TryJump() {
    if (canJump) {
      rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
      if (!rechargingJump)
        StartCoroutine(RechargeJump());
    }
  }

  public void SetMoveset(List<Moveset> list) {
    accelerationTimer = 0;
    decelerationTimer = 0;
    shouldDecelerate = false;
    currentMoveset = new Moveset { move = Move.Nothing };
    currentMoveIdx = 0;
    movesetList.Clear();
    movesetList.AddRange(list);
  }
}
