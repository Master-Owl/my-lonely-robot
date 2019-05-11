using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : AbstractCharacterController {

  bool isActive {
    get {
      return gameController.playerInControl;
    }
  }

  Move currentMove;
  float timerCounter;
  List<float> inputValues;

  void Start() {
    base.InitCharacter();
    inputValues  = new List<float>();
    currentMove  = Move.Nothing;
    timerCounter = 0;
  }

  void FixedUpdate() {
    if (!isActive) return;

    Move action = Move.Nothing;
    float input = Input.GetAxis("Horizontal");
    bool ducking = Input.GetAxis("Vertical") > 0;
    bool jump = Input.GetButton("Jump");

    inputValues.Add(input);

    if (ducking) {
      action = Move.Duck;
    } else if (input != 0) {
      transform.Translate(speed * input * Time.deltaTime, 0, 0);
      action = Mathf.Sign(input) > 0 ? Move.Right : Move.Left;
    }

    if (jump) {
      action = action == Move.Right ? Move.JumpRight :
        action == Move.Left ? Move.JumpLeft :
        action == Move.Duck ? Move.JumpDuck :
        Move.Jump;
    }

    if (jump && canJump) {
      rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
      if (!rechargingJump)
        StartCoroutine(RechargeJump());
    }

    UpdateTimer();

    if (action != currentMove) {
      ChangeCurrentMove(action);
    }

    if (Input.GetButtonDown("ReleaseControl")) {
      gameController.SwapControl();
    }
  }

  void ChangeCurrentMove(Move newMove) {
    gameController.AddMovesetItem(new Moveset(currentMove, timerCounter, inputValues));

    currentMove = newMove;
    timerCounter = 0;
    inputValues.Clear();
  }

  void UpdateTimer() {
    timerCounter += Time.deltaTime;
  }
}
