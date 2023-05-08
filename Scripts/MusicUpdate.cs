using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicUpdate : MonoBehaviour
{
    AudioSource audSrc;

    public AudioClip Soundtrack_1_Title_Loop;
    public AudioClip Soundtrack_2_TitleAlt_Loop;
    public AudioClip Soundtrack_3_Safety_Loop;

    private int currentSong = 3;

    void Start() {
        audSrc = GetComponent<AudioSource>();
    }

    public void playSong (int songNumber){
        switch (songNumber) {
            case 1:
                audSrc.loop = true;
                audSrc.clip = Soundtrack_1_Title_Loop;
                audSrc.Play();
                break;
            case 2:
                audSrc.loop = true;
                audSrc.clip = Soundtrack_2_TitleAlt_Loop;
                audSrc.Play();
                break;
            case 3:
                audSrc.loop = true;
                audSrc.clip = Soundtrack_3_Safety_Loop;
                audSrc.Play();
                break;
        }
        currentSong = songNumber;
    }

    public int getCurrentSong () {
        return currentSong;
    }

}
