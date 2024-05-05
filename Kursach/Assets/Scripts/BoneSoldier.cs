using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneSoldier : Entity
{
    [SerializeField] private AudioSource ghostSound;
    [SerializeField] private AudioSource ShotSound;
    [SerializeField] private float lives = 3; // speed
    [SerializeField] private float ShootInterval = 10f; // speed
    [SerializeField] private float lineOfSight = 3; // speed
    public GameObject bullet;
    public Transform firePoint;
    private bool isReady;
    private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool isFlipped = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public enum States
    {
        idle,
        dieing,
        shot
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
            isAgred = true;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        isReady = true;
        player = Hero.Instance.gameObject.transform;
        State = States.idle;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(ShootInterval);
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (this.transform.position.x - player.position.x >= 0 && !isFlipped)
        {
            isFlipped = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (this.transform.position.x - player.position.x < 0 && isFlipped)
        {
            isFlipped = false;
            transform.Rotate(0f, 180f, 0f);
        }

        if ((isAgred || distanceToPlayer < lineOfSight) && State != States.dieing && isReady)
        {
            isAgred = true;
            UnityEngine.Debug.Log(this.transform.position.x - player.position.x);

            State = States.shot;
            isReady = false;
            StartCoroutine(Reload());

        }

        if (lives < 1)
        {
            State = States.dieing;
        }
    }

    public void Shoot()
    {
        ShotSound.Play();
        Instantiate(bullet, firePoint.position, firePoint.rotation);
        State = States.idle;
    }

    public override void GetDamage(int damage)
    {
        isAgred = true;
        if (!ghostSound.isPlaying)
        {
            ghostSound.Play();
        }
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

    private void nahuy()
    {

    }
}
