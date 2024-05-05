using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Entity
{
    public Transform spawnPoint;
    [SerializeField] private float lives = 3;
    [SerializeField] private float lineOfSight = 3;
    [SerializeField] private float ShootDistance = 3;
    [SerializeField] private float skullInterval = 8.0f;
    [SerializeField] private float speed = 2;
    public GameObject skullPrefab;

    private Transform player;
    private float lastSkullTime;

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
        spawn,
        dieing
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
            Hero.Instance.GetDamage(1);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        lastSkullTime = Time.time;
        player = Hero.Instance.gameObject.transform;
        State = States.idle;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer < lineOfSight)
        {
            if (this.transform.position.x - player.position.x < 0)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }

            if (distanceToPlayer > ShootDistance)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
            } else if(Time.time - lastSkullTime >= skullInterval)
            {
                // UnityEngine.Debug.Log("SKUUUUUUUUUUULLLL");
                State = States.spawn;

                lastSkullTime = Time.time;
            }
        }

        if (lives < 1)
        {
            State = States.dieing;
        }
    }

    public void Idle()
    {
        State = States.idle;
    }

    public void Spawn()
    {
        Instantiate(skullPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public override void GetDamage(int damage)
    {
        lastSkullTime -= skullInterval / 2;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ShootDistance);
    }
}
