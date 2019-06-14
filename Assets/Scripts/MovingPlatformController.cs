using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovingPlatformDirection { Horizontal, Vertical }
public enum ForwardBackward { Forward, Backward }

/// This script should be attached
/// to a parent game object with a single
/// child, the child being the actual platform.
public class MovingPlatformController : MonoBehaviour {
  
  public MovingPlatformDirection movementDirection = MovingPlatformDirection.Horizontal;
  public ForwardBackward initialDirection = ForwardBackward.Forward;
  public bool active = true;

  [Range(0,20)]
  public float range = 3;
  [Range(1,5)]
  public float speed = 3;
  [Range(0,100)]
  public float positionPercentage = 0;

  Vector2 direction;
  Vector2 startingPos;
  Vector2 endingPos;
  Transform platform;

  void Start() {
    if (platform == null) platform = transform.GetChild(0);
    SetStartEndPositions();
  }

  void Update() {
    if (!active) return;
    
    platform.Translate(direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, 0);

    if (platform.position.x >= endingPos.x && platform.position.y >= endingPos.y) {
      direction = movementDirection == MovingPlatformDirection.Horizontal ?
        new Vector2(-1,0):
        new Vector2(0,-1);
    } else if (platform.position.x <= startingPos.x && platform.position.y <= startingPos.y) {
      direction = movementDirection == MovingPlatformDirection.Horizontal ?
        new Vector2(1,0):
        new Vector2(0,1);
    }
  }

  void SetStartEndPositions() {
    float halfRange = range/2;

    if (movementDirection == MovingPlatformDirection.Horizontal) {
      startingPos = new Vector2(-halfRange, transform.position.y);
      endingPos = new Vector2(halfRange, transform.position.y);
      direction = initialDirection == ForwardBackward.Forward ?
        new Vector2(1,0) :
        new Vector2(-1,0);
    } else {
      startingPos = new Vector2(transform.position.x, -halfRange);
      endingPos = new Vector2(transform.position.x, halfRange);
      direction = initialDirection == ForwardBackward.Forward ?
        new Vector2(0,1) :
        new Vector2(0,-1);
    }
  }


  #region Editor
  void OnDrawGizmos() {
    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(startingPos, endingPos);
  }

  void OnValidate() {
    if (platform == null) platform = transform.GetChild(0);
    SetStartEndPositions();
    
    float halfRange = range/2;
    Vector2 pos = transform.position;
    if (movementDirection == MovingPlatformDirection.Horizontal) {
      platform.position = new Vector2(Mathf.Lerp(pos.x - halfRange, pos.x + halfRange, positionPercentage/100), transform.position.y);
    } else {
      platform.position = new Vector2(transform.position.x, Mathf.Lerp(pos.y - halfRange, pos.y + halfRange, positionPercentage/100));
    }    
  }
  #endregion
}
