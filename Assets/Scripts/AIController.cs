using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D))]
public class AIController : AbstractCharacterController {

  [SerializeField] List<Moveset> movesetList;
  int currentMoveIdx;
  int currentInputIndex;
  bool doneWithPrevMove;
  Moveset currentMoveset;

  bool isActive {
    get {
      return !gameController.playerInControl;
    }
  }


  void Start() {
    base.InitCharacter();
    doneWithPrevMove = true;
    currentMoveIdx = 0;
    currentInputIndex = 0;
    movesetList = new List<Moveset>();
    currentMoveset = new Moveset(Move.Nothing, 0, null);
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
    currentInputIndex = 0;
    doneWithPrevMove = false;

    currentMoveset = movesetList[currentMoveIdx];
    yield return new WaitForSeconds(currentMoveset.duration);

    currentMoveIdx++;
    doneWithPrevMove = true;
  }

  void DoAction() {
    float input = currentMoveset.inputValues[Mathf.Min(currentInputIndex++, currentMoveset.inputValues.Count - 1)];

    switch (currentMoveset.move) {
      case Move.Right:
      case Move.Left:
        transform.Translate(speed * input * Time.deltaTime, 0, 0);
        break;

      case Move.Jump:
        TryJump();
        break;

      case Move.Duck:
        // attempt duck
        break;

      case Move.JumpRight:
      case Move.JumpLeft:
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
  }

  void TryJump() {
    if (canJump) {
      rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
      if (!rechargingJump)
        StartCoroutine(RechargeJump());
    }
  }

  public void SetMovesetList(List<Moveset> list) {
    currentMoveset = new Moveset(Move.Nothing, 0, null);
    currentInputIndex = 0;
    currentMoveIdx = 0;
    movesetList.Clear();
    movesetList.AddRange(list);
  }
}
