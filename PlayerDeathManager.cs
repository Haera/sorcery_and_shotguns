using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeathManager : MonoBehaviour
{
    public GameObject player;
    public GameObject deathScreen;
    public Button continueButton;
    public Button quitButton;
    private FPSController playerController;

    void Start() {
        playerController = player.GetComponent<FPSController>();

		Button quitBtn = quitButton.GetComponent<Button>();
		quitBtn.onClick.AddListener(goToMainMenu);
        
		Button continueBtn = continueButton.GetComponent<Button>();
		continueBtn.onClick.AddListener(reloadCheckpoint);

        deathScreen.SetActive(false);
    }

    void Update() {
        if(playerController.getHealth() <= 0){
            Time.timeScale = 0f;
            deathScreen.SetActive(true);
            Cursor.visible = true;
        }
    }

    void reloadCheckpoint() {
        //reload checkpoint here
        //playerController.enabled = true;
        deathScreen.SetActive(false);
        SceneManager.LoadScene("CMSC425Game");
        Time.timeScale = 1f;
    }

    public void goToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
