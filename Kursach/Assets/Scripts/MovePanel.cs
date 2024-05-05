using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanel : MonoBehaviour
{
    [SerializeField] private Transform Pos1;
    [SerializeField] private Transform Pos2;
    [SerializeField] private float speed = 3;


    private bool goTo1 = false;
    private bool goTo2 = false;

    // Start is called before the first frame update
    void Start()
    {
        goTo1 = true;
        goTo2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (goTo1)
        {
            if (Math.Abs(transform.position.x - Pos1.position.x) > 1f ||
                Math.Abs(transform.position.y - Pos1.position.y) > 1f)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, Pos1.position, speed * Time.deltaTime);
            } else
            {
                goTo1 = false;
                goTo2 = true;
            }
        }

        if (goTo2)
        {
            if (Math.Abs(transform.position.x - Pos2.position.x) > 1f ||
                Math.Abs(transform.position.y - Pos2.position.y) > 1f)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, Pos2.position, speed * Time.deltaTime);
            }
            else
            {
                goTo1 = true;
                goTo2 = false;
            }
        }
    }
}
