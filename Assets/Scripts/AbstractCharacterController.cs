using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharacterController : MonoBehaviour {

  public GameController gameController;

  protected bool canJump {
    get {
      float distToGround = boxCollider.bounds.extents.y;
      float halfWidth = boxCollider.bounds.extents.x;
      Vector2 origin = new Vector2(transform.position.x, transform.position.y - distToGround - .01f);
      LayerMask floor = LayerMask.GetMask("Floor", "Character");

      // Middle of base is touching floor
      if (Physics2D.Raycast(origin, -Vector2.up, 0.1f, floor)) return true;

      // Fixes super high jump bug when next to wall... kinda
      if (Physics2D.Raycast(origin, -Vector2.right, halfWidth + 0.1f, floor) ||
          Physics2D.Raycast(origin, Vector2.right, halfWidth + 0.1f, floor)) return false;

      // Edge of base is touching floor
      if (Physics2D.Raycast(new Vector2(origin.x - halfWidth, origin.y), -Vector2.up, 0.1f, floor)) return true;
      if (Physics2D.Raycast(new Vector2(origin.x + halfWidth, origin.y), -Vector2.up, 0.1f, floor)) return true;
      return false;
    }
  }

  public float jumpHeight = 5f;
  public float jumpRechargeTime = 1f;
  public float speed = 5f;

  protected bool rechargingJump = false;
  protected Rigidbody2D rb;
  protected BoxCollider2D boxCollider;

  protected void InitCharacter() {
    rb = GetComponent<Rigidbody2D>();
    boxCollider = GetComponent<BoxCollider2D>();
  }

  protected IEnumerator RechargeJump() {
    float tmp = jumpHeight;
    jumpHeight = 0;

    rechargingJump = true;
    yield return new WaitForSeconds(jumpRechargeTime);
    rechargingJump = false;

    jumpHeight = tmp;
  }
}
