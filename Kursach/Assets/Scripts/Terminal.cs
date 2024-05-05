using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Entity
{
    [SerializeField] Door[] door;
    private int lives = 0;
    private bool signal = true;

    public override void GetDamage(int damage)
    {
        lives--;
        if(lives < 1)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        for (int i = 0; i < door.Length; i++)
        {
            if (door[i] != null)
            {
                door[i].TerminalSignal(signal);
            }
        }
        signal = !signal;
    }
}
