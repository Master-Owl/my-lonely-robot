using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour {
  public GameObject startOptions;
  public GameObject levelSelect;
  public GameObject buttonPrefab;

  int levelButtonsLoaded;

  void Start() {
    startOptions.SetActive(true);
    levelSelect.SetActive(false);
    levelButtonsLoaded = 0;
  }

  public void OnPlayGamePressed() {
    startOptions.SetActive(false);
    levelSelect.SetActive(true);

    if (levelButtonsLoaded == 0)
      SpawnLevelButtons();
  }

  public void OnBackPressed() {
    startOptions.SetActive(true);
    levelSelect.SetActive(false);
  }

  void SpawnLevelButtons() {
    // # of scenes minus 1 for start screen
    int levelCount = SceneManager.sceneCountInBuildSettings - 1;
    GameObject panel = GameObject.FindWithTag("UIPanel");

    for (int i = 1; i <= levelCount; ++i) {
      int lvl = i; // Necessary b/c lambda function doesn't keep static scope of loop var  
      GameObject buttonGO = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
      buttonGO.transform.SetParent(panel.transform);
      buttonGO.transform.localScale = Vector3.one;
      buttonGO.GetComponentInChildren<Text>().text = lvl.ToString();
      buttonGO.GetComponent<Button>().onClick.AddListener(() => {
        SceneManager.LoadScene(lvl);
      });
      levelButtonsLoaded++;
    };   
  }
}
