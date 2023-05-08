using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPlay : MonoBehaviour
{
	public Button playButton;

	void Start () {
		Button btn = playButton.GetComponent<Button>();
		btn.onClick.AddListener(loadGameScene);
	}

    public void loadGameScene () {
        SceneManager.LoadScene("CMSC425Game");
    }
}
