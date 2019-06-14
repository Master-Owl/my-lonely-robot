using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(Animator))]
public class AIController : AbstractCharacterController {

  [SerializeField] List<Moveset> movesetList;
  int currentMoveIdx;
  int currentInputIndex;
  bool doneWithPrevMove;
  Moveset currentMoveset;
  Animator animator;

  bool isActive {
    get {
      return !gameController.playerInControl;
    }
  }

  void Start() {
    base.InitCharacter();
    animator = GetComponent<Animator>();
    doneWithPrevMove = true;
    currentMoveIdx = 0;
    currentInputIndex = 0;
    movesetList = new List<Moveset>();
    currentMoveset = new Moveset(Move.Nothing, 0, null);
  }

  void Update() {
    UpdateParent();
  }

  void FixedUpdate() {
    if (canJump && !rechargingJump) {
      animator.SetBool("Jumping", false);
    }
    if (!isActive) {
      if (animator.GetBool("Moving"))
        animator.SetBool("Moving", false);
      return;
    }

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
    
    if (input < 0) GetComponent<SpriteRenderer>().flipX = true;
    else GetComponent<SpriteRenderer>().flipX = false;

    switch (currentMoveset.move) {
      case Move.Right:
      case Move.Left:
        animator.SetBool("Moving", true);
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
        animator.SetBool("Moving", true);
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
      if (!rechargingJump) {
        animator.SetBool("Jumping", true);
        StartCoroutine(RechargeJump());
      }
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
