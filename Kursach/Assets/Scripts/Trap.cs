using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    [SerializeField] private AudioSource ElectricitySound;
    [SerializeField] private float SoundDistance = 5f;
    private bool isActive = true;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage(1);
        }
    }

    public enum States
    {
        trap
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Hero.Instance.gameObject.transform;
        State = States.trap;
    }

    private IEnumerator Sound()
    {
        isActive = false;
        ElectricitySound.Play();
        yield return new WaitForSeconds(2f);
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < SoundDistance && isActive)
        {
            StartCoroutine(Sound());
        } else if(Vector3.Distance(transform.position, player.transform.position) >= SoundDistance)
        {
            isActive = true;
            ElectricitySound.Stop();
        }
    }
}
