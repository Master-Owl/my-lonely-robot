using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Text titleText;
    public GameController gameController;

    public void OnStartPressed() {
        gameController.Spawn();
    }

    public void OnResetPressed() {
        gameController.Reset();
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
        titleText.gameObject.SetActive(false);
    }
}
