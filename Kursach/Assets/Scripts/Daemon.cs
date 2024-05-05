using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daemon : Entity
{
    [SerializeField] private AudioSource ghostSound;
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private AudioSource raurSound;
    [SerializeField] private AudioSource deadSound;
    [SerializeField] private float speed = 3; // speed
    [SerializeField] private float reloadTime = 3; // speed
    [SerializeField] private float lives = 3; // speed
    [SerializeField] private float lineOfSight = 3; // speed
    public LineRenderer HPLine;
    public GameObject skullPrefab;
    public GameObject wallPrefab;
    public GameObject firePrefab;
    private Transform player;
    public Transform HPBegin;
    public Transform HPEnd;
    public Transform deadPosition;
    public Transform upPosition;
    public Transform upPosition2;
    public Transform spawnPoint;
    public Transform[] WallsPosition;
    private bool isReady = true;
    private bool isOnStart = false;
    private bool isAttacking = false;
    private bool isUpPos1 = false;
    private bool isUpPos2 = false;
    private bool isSeeking = false;
    private bool attackEnd = false;
    private int attackType;
    private int last_attack;
    private float health;

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
        attack,
        fire,
        dead,
        firing
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage(1);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        health = lives;
        HPLine.enabled = false;
        last_attack = -1;
        isReady = true;
        player = Hero.Instance.gameObject.transform;
        State = States.idle;
    }

    private IEnumerator AttackReload()
    {
        yield return new WaitForSeconds(reloadTime);
        isReady = true;
    }

    private IEnumerator Seek()
    {
        yield return new WaitForSeconds(8);
        isSeeking = false;
    }

    private IEnumerator SkullSpawn()
    {
        State = States.attack;
        yield return new WaitForSeconds(1f);
        Instantiate(skullPrefab, this.transform.position, this.transform.rotation);
        yield return new WaitForSeconds(1f);
        Instantiate(skullPrefab, this.transform.position, this.transform.rotation);
        yield return new WaitForSeconds(1f);
        Instantiate(skullPrefab, this.transform.position, this.transform.rotation);
        State = States.idle;
    }

    public void SpawnFire()
    {
        Instantiate(firePrefab, this.transform.position, this.transform.rotation);
    }

    private void Attack()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (State == States.dead)
        {
            if (Math.Abs(transform.position.x - deadPosition.position.x) > 1f ||
                                Math.Abs(transform.position.y - deadPosition.position.y) > 1f)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, deadPosition.position, speed * Time.deltaTime);
            } else
            {
                Die();
            }
        }

        float distToPlayer = Vector2.Distance(player.position, transform.position);
        if (!isAgred && distToPlayer < 15f)
        {
            raurSound.Play();
            CameraController.Instance.Shake(1.5f, 0.5f);
            CameraController.Instance.BossFight();
            isAgred = true;
            
        }

        if (isAgred)
        {
            HPLine.enabled = true;
            Transform HPNow = HPEnd;

            Vector3 newPosition = HPEnd.position;
            newPosition.x = HPBegin.position.x + 10f * lives / health;

            HPNow.position = newPosition;

            HPLine.SetPosition(0, HPBegin.position);
            HPLine.SetPosition(1, HPNow.position);
            if (this.transform.position.x - player.position.x < 0)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
            if (isReady)
            {
                attackEnd = false;
                isReady = false;
                isAttacking = true;
                isOnStart = false;
                attackType = UnityEngine.Random.Range(0, 4);

                if (attackType == last_attack)
                {
                    attackType = UnityEngine.Random.Range(0, 4);
                }

                if (attackType == last_attack)
                {
                    attackType = UnityEngine.Random.Range(0, 4);
                }

                if (attackType == last_attack)
                {
                    attackType = UnityEngine.Random.Range(0, 4);
                }

                last_attack = attackType;

                if (attackType == 2)
                {
                    isUpPos1 = false;
                    isUpPos2 = false;
                }
                if (attackType == 3)
                {
                    isSeeking = true;
                    StartCoroutine(Seek());
                }
            }

            if (isAttacking)
            {
                if (!attackEnd)
                {
                    if (attackType == 0)
                    {
                        StartCoroutine(SkullSpawn());
                        attackEnd = true;
                    }
                    else if (attackType == 1)
                    {
                        if (!isUpPos1)
                        {
                            UnityEngine.Debug.Log(transform.position.x - upPosition.position.x);
                            UnityEngine.Debug.Log(transform.position.y - upPosition.position.y);
                            if (Math.Abs(transform.position.x - upPosition.position.x) > 1f ||
                                Math.Abs(transform.position.y - upPosition.position.y) > 1f)
                            {
                                transform.position = Vector2.MoveTowards(this.transform.position, upPosition.position, speed * Time.deltaTime);
                            }
                            else
                            {
                                isUpPos1 = true;
                            }
                        }
                        else
                        {
                            UnityEngine.Debug.Log("upPos1");
                            State = States.firing;
                            if (!fireSound.isPlaying)
                            {
                                fireSound.Play();
                            }

                            if (!isUpPos2)
                            {
                                if (Math.Abs(transform.position.x - upPosition2.position.x) > 1f ||
                                    Math.Abs(transform.position.y - upPosition2.position.y) > 1f)
                                {
                                    transform.position = Vector2.MoveTowards(this.transform.position, upPosition2.position, speed * Time.deltaTime);
                                }
                                else
                                {
                                    isUpPos2 = true;
                                }
                            }
                            else
                            {
                                UnityEngine.Debug.Log("upPos2");

                                State = States.idle;

                                attackEnd = true;
                            }
                        }
                    }
                    else if (attackType == 2)
                    {
                        raurSound.Play();
                        Instantiate(wallPrefab, WallsPosition[0].transform.position, WallsPosition[0].transform.rotation);
                        Instantiate(wallPrefab, WallsPosition[1].transform.position, WallsPosition[1].transform.rotation);
                        Instantiate(wallPrefab, WallsPosition[2].transform.position, WallsPosition[2].transform.rotation);
                        Instantiate(wallPrefab, WallsPosition[3].transform.position, WallsPosition[3].transform.rotation);

                        attackEnd = true;
                    }
                    else if (attackType == 3)
                    {
                        if (isSeeking)
                        {
                            transform.position = Vector2.MoveTowards(this.transform.position, player.position, 0.5f * speed * Time.deltaTime);
                        }
                        else
                        {
                            attackEnd = true;
                        }
                    }
                }
                else if (!isOnStart)
                {
                    float distanceToPlayer = Vector2.Distance(player.position, transform.position);

                    if (Math.Abs(transform.position.x - spawnPoint.position.x) > 1f ||
                        Math.Abs(transform.position.y - spawnPoint.position.y) > 1f)
                    {
                        transform.position = Vector2.MoveTowards(this.transform.position, spawnPoint.position, speed * Time.deltaTime);
                    }
                    else
                    {
                        isOnStart = true;
                    }
                }
                else
                {
                    StartCoroutine(AttackReload());
                    isAttacking = false;
                }
            }

        }

        if (lives < 1)
        {
            deadSound.Play();
            CameraController.Instance.Shake(1.5f, 0.5f);
            State = States.dead;
        }
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
        CameraController.Instance.BossFight();
        State = States.dead;
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
