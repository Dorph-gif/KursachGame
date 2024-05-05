using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Entity
{
    [SerializeField] private float speed = 4; // speed
    [SerializeField] private float lives = 1; // speed
    [SerializeField] private float lifeTime = 3; // speed
    private float spawnTime;
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
        idle
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage(3);
            Die();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        player = Hero.Instance.gameObject.transform;
        State = States.idle;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);

        if (Time.time - spawnTime > lifeTime)
        {
            Die();
        }

        if (lives < 1)
        {
            Die();
        }
    }

    public override void GetDamage(int damage)
    {
        lives -= damage;
    }

    public override void Die()
    {
        Destroy(this.gameObject);
    }
}
