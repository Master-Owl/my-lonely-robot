[System.Serializable]
public class Moveset {
  public float duration;
  public Move move;
}

// This is an enum to represent which key (or lack thereof)
// should be simulated as being held down.
public enum Move {
  Right,
  Left,
  Duck,
  Jump,
  JumpRight,
  JumpLeft,
  JumpDuck,
  ReleaseControl,
  Nothing
}