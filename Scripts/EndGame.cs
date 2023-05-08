using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject gameCompleteScreen;
    public Button quitButton;
    public GameObject player;

    void Start () {
		Button quitBtn = quitButton.GetComponent<Button>();
		quitBtn.onClick.AddListener(goToMainMenu);

        gameCompleteScreen.SetActive(false);
    }

    public void goToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(player.tag)){
            Time.timeScale = 0f;
            gameCompleteScreen.SetActive(true);
            Cursor.visible = true;
        }
    }
}