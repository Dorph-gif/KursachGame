using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource BossFightMusic;
    [SerializeField] private Transform player;
    private Vector3 pos;

    public Transform camTransform;

    public float shakeDuration = 0f;
    public float shakeAmount = 0.01f;
    public float decreaseFactor = 1.0f;
    private bool isBossFight;

    Vector3 originalPos;

    public static CameraController Instance { get; set; }


    private void Awake()
    {
        Instance = this;
        isBossFight = false;

        if (!player)
        {
            player = FindObjectOfType<Hero>().transform;
        }
        if (!camTransform)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        shakeDuration = 0;
    }

    public void Retry()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex >= 3)
        {
            SceneManager.LoadScene(0);
        } else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    public void Shake(float duration, float amount)
    {
        shakeDuration = duration;
        shakeAmount = amount;
    }

    public void BossFight()
    {
        isBossFight = !isBossFight;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBossFight && !BossFightMusic.isPlaying)
        {
            BossFightMusic.Play();
        }

        if (!isBossFight && BossFightMusic.isPlaying)
        {
            BossFightMusic.Stop();
        }

        if (!isBossFight && !gameMusic.isPlaying)
        {
            gameMusic.Play();
        }

        if (isBossFight && gameMusic.isPlaying)
        {
            gameMusic.Stop();
        }

        pos = player.position;
        pos.z = -10f;
        pos.y += 3f;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);

        if (shakeDuration > 0)
        {
            camTransform.localPosition = this.transform.position + UnityEngine.Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else if (shakeDuration < 0)
        {
            shakeDuration = 0f;
        }
    }
}
