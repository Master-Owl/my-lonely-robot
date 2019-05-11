using System.Collections.Generic;

[System.Serializable]
public class Moveset {
  public float duration;
  public List<float> inputValues;
  public Move move;

  public Moveset(Move move, float duration, List<float> inputValues) {
    this.move = move;
    this.duration = duration;
    if (inputValues != null)
      this.inputValues = new List<float>(inputValues);
    else
      this.inputValues = new List<float>();
  }
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