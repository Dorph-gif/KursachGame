using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }


}
