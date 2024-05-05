using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfFire : MonoBehaviour
{
    [SerializeField] private Transform dir;
    [SerializeField] private float lifeTime = 4; // speed
    [SerializeField] private float speed = 4; // speed
    private float spawnTime;

    private SpriteRenderer sprite;
    private Animator anim;

    public enum States
    {
        start,
        idle
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage(1);
        }
    }

    public void Fire()
    {
        speed = 4;
        State = States.idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        State = States.start;
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, dir.position, speed * Time.deltaTime);

        if (Time.time - spawnTime > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
