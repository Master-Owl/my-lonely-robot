using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
  public GameController gameController;
  public Canvas canvasOverlay;
  public Text titleText;
  public Button resetButton;
  public Button nextLevelButton;


  void Start() {    
    resetButton.onClick.AddListener(OnResetPressed);
    nextLevelButton.onClick.AddListener(OnNextLevelPressed);

    DontDestroyOnLoad(canvasOverlay);
  }

  void OnResetPressed() {
    gameController.Reset();
  }

  void OnNextLevelPressed() {
    gameController.LoadNextLevel();
  } 

  public void DisplayTitleText(string text, float duration = 0) {
    titleText.text = text;
    titleText.gameObject.SetActive(true);

    if (duration > 0) {
      StartCoroutine(TitleTextTimer(duration));
    }
  }

  IEnumerator TitleTextTimer(float duration) {
    yield return new WaitForSeconds(duration);
    HideTitleText();
  }

  public void HideTitleText() {
    titleText.gameObject.SetActive(false);
  }

  public void DisplayResetButton(bool visible = true) {
    resetButton.gameObject.SetActive(visible);
  }

  public void DisplayNextLevelButton(bool visible = true) {
    nextLevelButton.gameObject.SetActive(visible);
  }
}
