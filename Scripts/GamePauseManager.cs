using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseManager : MonoBehaviour
{
    public GameObject player;
    public GameObject pauseMenu;
    public Button quitButton;
    public Button resumeButton;
    private FPSController playerController;
    private bool isGamePaused = false;

    void Start() {
        playerController = player.GetComponent<FPSController>();

		Button quitBtn = quitButton.GetComponent<Button>();
		quitBtn.onClick.AddListener(goToMainMenu);
        
		Button resumeBtn = resumeButton.GetComponent<Button>();
		resumeBtn.onClick.AddListener(TogglePause);

        pauseMenu.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    void TogglePause() {
        isGamePaused = !isGamePaused;

        if (isGamePaused) {
            Time.timeScale = 0f;
            //playerController.enabled = false;
            pauseMenu.SetActive(true);
        } else {
            Time.timeScale = 1;
            //playerController.enabled = true;
            pauseMenu.SetActive(false);
        }
    }

    public void goToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
