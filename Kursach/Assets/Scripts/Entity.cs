using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool isAgred = false;

    public virtual void GetDamage(int damage)
    {

    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
