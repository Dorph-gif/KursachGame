using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

    [SerializeField] private BoxCollider2D collider;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool isClosed = false;
    private bool FirstEnter = true;

    public enum States
    {
        closed,
        opened,
        open,
        close
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void Close()
    {
        State = States.close;
        closeSound.Play();
        collider.enabled = true;
    }

    private void Closed()
    {
        isClosed = true;
        State = States.closed;
    }

    public void Open()
    {
        openSound.Play();
        State = States.open;
        collider.enabled = false;
    }

    private void Opened()
    {
        isClosed = false;
        State = States.opened;
    }

    public void TerminalSignal(bool signal)
    {
        FirstEnter = false;
        if (signal)
        {
            Open();
        }else
        {
            Close();
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo != null && FirstEnter && State != States.closed && collisionInfo.gameObject.tag == "Player")
        {
            FirstEnter = false;
            Close();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FirstEnter = true;
        isClosed = false;
        Open();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
