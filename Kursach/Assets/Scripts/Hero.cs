using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource smokeSound;
    [SerializeField] private AudioSource kickSound;
    [SerializeField] private AudioSource missSound;
    [SerializeField] private AudioSource runSound;
    [SerializeField] private float speed = 3f; // speed
    [SerializeField] private int lives = 5; // Lives
    [SerializeField] private float jumpForce = 15f; // jump Force
    [SerializeField] public float checkGroundOffsetY = -1.8f;
    [SerializeField] public float checkGroundOffsetX = -1.8f;
    [SerializeField] public float checkGroundRadius = 0.3f;
    public GameObject winPanel;
    public GameObject losePanel;
    private bool levelEnd = false;

    public bool isRecharged = true;
    public bool isAttacking = false;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    private bool isGrounded = true;
    private bool isFliped = false;
    private int flip = 1;
    private int jumpCounter = 0;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    public static Hero Instance { get; set; }

    public enum States
    {
        idle,
        run,
        jump,
        shot,
        dead,
        reload,
        mark,
        dieing,
        jumpShot,
        attack,
        endLevel,
        runShot
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private IEnumerator MarkTime()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int levelReached = PlayerPrefs.GetInt("levelReached", 2);
        UnityEngine.Debug.Log(levelReached);
        UnityEngine.Debug.Log(currentSceneIndex);

        if (currentSceneIndex <= 3 && (currentSceneIndex + 1 > levelReached))
        {
            PlayerPrefs.SetInt("levelReached", currentSceneIndex + 1);
            UnityEngine.Debug.Log(PlayerPrefs.GetInt("levelReached", 2));
        }

        smokeSound.Play();
        yield return new WaitForSeconds(0.6f);
        State = States.endLevel;
        winPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        smokeSound.Stop();
        winSound.Play();
    }

    private void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo != null && collisionInfo.gameObject.tag == "LevelEnd")
        {
            levelEnd = true;
            State = States.mark;
            StartCoroutine(MarkTime());
        }
    }

    private void Awake()
    {
        isRecharged = true;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        isRecharged = true;
    }

    private void Attack()
    {
        if (isRecharged)
        {
            State = States.attack;
            isRecharged = false;
            isAttacking = true;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCooldown());
        }
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        if (colliders.Length > 0)
        {
            kickSound.Play();
        }else
        {
            missSound.Play();
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage(1);
        }
    }

    private void Run()
    {
        if (!isGrounded)
        {
            runSound.Stop();
        }
        if (!runSound.isPlaying)
        {
            runSound.Play();
        }
        if (isGrounded) State = States.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal") * flip;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        bool Flipped = false;
        if (Input.GetAxis("Horizontal") < 0) Flipped = true;

        if (isFliped != Flipped)
        {
            isFliped = Flipped;
            flip *= -1;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        jumpCounter--;
    }

    private void Jump()
    {
        jumpCounter++;
        StartCoroutine(JumpCooldown());
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        jumpSound.Play();
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll
            (new Vector2(transform.position.x + checkGroundOffsetX, transform.position.y + checkGroundOffsetY), checkGroundRadius);

        if (colliders.Length > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (!isGrounded && !isAttacking) State = States.jump;
    }

    public override void GetDamage(int damage)
    {
        if (!levelEnd)
        {
            lives -= damage;
            if (lives < 1)
            {
                Die();
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    // Start is called before the first frame update
    void Start()
    {
        levelEnd = false;
        jumpCounter = 0;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelEnd)
        {

            if (isGrounded && !isAttacking) State = States.idle;

            if (Input.GetButton("Horizontal"))
            {
                Run();
            }
            else if ((isGrounded == false || !Input.GetButton("Horizontal")) && runSound.isPlaying)
            {
                runSound.Stop();
            }

            if (isGrounded == true && Input.GetKeyDown(KeyCode.W) && jumpCounter < 2)
            {
                Jump();
            }

            if (Input.GetAxis("Horizontal") == 0 && Input.GetButton("Fire1") && isGrounded == true)
            {
                State = States.shot;
            }
            else if (Input.GetButton("Fire1") && isGrounded == false)
            {
                State = States.jumpShot;
            }else if(Input.GetAxis("Horizontal") != 0 && Input.GetButton("Fire1") && isGrounded == true)
            {
                State = States.runShot;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Attack();
            }
        }
    }

    public void Dead()
    {
        State = States.dead;
        losePanel.SetActive(true);
    }

    public override void Die()
    {
        losePanel.SetActive(true);
        levelEnd = true;
        State = States.dieing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere
            (new Vector2(transform.position.x + checkGroundOffsetX, transform.position.y + checkGroundOffsetY), checkGroundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
