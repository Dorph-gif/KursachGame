using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Ghost : Entity
{
    [SerializeField] private AudioSource ghostSound;
    [SerializeField] private float speed = 3; // speed
    [SerializeField] private float lives = 3; // speed
    [SerializeField] private float lineOfSight = 3; // speed
    private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public enum States
    {
        idle,
        dieing
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && State != States.dieing)
        {
            Hero.Instance.GetDamage(1);
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        player = Hero.Instance.gameObject.transform;
        State = States.idle;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if ((isAgred || distanceToPlayer < lineOfSight) && State != States.dieing)
        {
            isAgred = true;
            if(this.transform.position.x - player.position.x < 0) {
                sprite.flipX = true;
            }else
            {
                sprite.flipX = false;
            }
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);

            Vector3 targetPosition = new Vector3(transform.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 0.5f * speed * Time.deltaTime);
        }

        if (lives < 1)
        {
            State = States.dieing;
        }
    }

    private IEnumerator BackImpulse()
    {
        int dir = sprite.flipX == false ? 1 : -1;
        rb.AddForce(transform.right * 2f * dir, ForceMode2D.Impulse);
        float spd = speed;
        speed = 0;
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(transform.right * (-2f) * dir, ForceMode2D.Impulse);
        speed = spd / 2;
        yield return new WaitForSeconds(0.1f);
        speed = spd;
    }

    public override void GetDamage(int damage)
    {
        if (!ghostSound.isPlaying)
        {
            ghostSound.Play();
        }
        StartCoroutine(BackImpulse());
        lives -= damage;
    }

    public override void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
