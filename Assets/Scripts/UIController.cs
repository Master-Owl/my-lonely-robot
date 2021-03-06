﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
  public GameController gameController;
  public Text titleText;
  public GameObject titleTextBackground;

  public Button resetButton;
  public Button settingsButton;
  public Button nextLevelButton;

  void Start() {
    resetButton.onClick.AddListener(OnResetPressed);
    nextLevelButton.onClick.AddListener(OnNextLevelPressed);
  }

  void OnResetPressed() {
    gameController.Reset();
  }

  void OnNextLevelPressed() {
    gameController.LoadNextLevel();
  }

  void OnCallRobotPressed() {
    if (gameController.playerInControl)
      gameController.SwapControl();
  }

  public void DisplayTitleText(string text, float duration = 0) {
    titleText.text = text;
    titleText.gameObject.SetActive(true);
    titleTextBackground.SetActive(true);

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
    titleTextBackground.SetActive(false);
  }

  public void DisplayNextLevelButton(bool visible = true) {
    nextLevelButton.gameObject.SetActive(visible);
  }
}
