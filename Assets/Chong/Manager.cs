using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

  public Button[] playButtons;
  public Button quitButton;

  public string[] playLevels;
  public AudioClip button;

  // Start is called before the first frame update
  void Start() {
    for(var i = 0; i < playButtons.Length && i < playLevels.Length; ++i){
      playButtons[i].onClick.RemoveAllListeners();

      var level = playLevels[i];

      playButtons[i].onClick.AddListener(() => {
        SceneManager.LoadScene(level);
        AudioSource.PlayClipAtPoint(button, Camera.main.transform.position);
        }
        );
    }

    quitButton.onClick.RemoveAllListeners();
    quitButton.onClick.AddListener(() => Application.Quit());
  }

}
