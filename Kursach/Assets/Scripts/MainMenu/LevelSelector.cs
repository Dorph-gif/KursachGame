using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public AudioSource music;
    public Button[] levels;

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i  = 0; i < levels.Length; i++)
        {
            if (i + 2 > levelReached)
            {
                levels[i].interactable = false;
            }
        }
    }

    void Update()
    {
        if (!music.isPlaying)
        {
            music.Play();
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void Select(int numberInBuild)
    {
        SceneManager.LoadScene(numberInBuild);
    }
}
