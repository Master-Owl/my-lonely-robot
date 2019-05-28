using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovingPlatformDirection { Horizontal, Vertical }

public class MovingPlatformController : MonoBehaviour {
  
  // Platform will start at left or bottom respectively
  public MovingPlatformDirection movementDirection = MovingPlatformDirection.Horizontal;
  public bool canEnterFromBelow = false;

  [Range(0,20)]
  public float range = 3;
  [Range(1,5)]
  public float speed = 3;

  Vector2 direction;
  Vector2 startingPos;
  Vector2 endingPos;

  void Start() {
    startingPos = gameObject.transform.position;
    if (movementDirection == MovingPlatformDirection.Horizontal) {
      endingPos = new Vector2(startingPos.x + range, startingPos.y);
      direction = new Vector2(1,0);
    } else {
      endingPos = new Vector2(startingPos.x, startingPos.y + range);
      direction = new Vector2(0,1);
    }
  }

  void Update() {
    transform.Translate(direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, 0);

    if (transform.position.x >= endingPos.x && transform.position.y >= endingPos.y) {
      direction = movementDirection == MovingPlatformDirection.Horizontal ?
        new Vector2(-1,0):
        new Vector2(0,-1);
    } else if (transform.position.x <= startingPos.x && transform.position.y <= startingPos.y) {
      direction = movementDirection == MovingPlatformDirection.Horizontal ?
        new Vector2(1,0):
        new Vector2(0,1);
    }
  }


  // Debugging purposes
  void OnDrawGizmos() {
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(startingPos, endingPos);
  }
}
