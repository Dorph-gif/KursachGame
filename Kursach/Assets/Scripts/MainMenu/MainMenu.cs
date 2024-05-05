using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource music;

    void Update()
    {
        if (!music.isPlaying)
        {
            music.Play();
        }
    }

    public void PlayCurrentLevel()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 2);
        SceneManager.LoadScene(levelReached);
    }

    // Update is called once per frame
    public void OpenLevelList()
    {
        SceneManager.LoadScene(1);
    }
}
