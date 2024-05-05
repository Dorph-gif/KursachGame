using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform followingTarget;
    [SerializeField, Range(0f, 1f)] float parallaxStrength;
    [SerializeField] bool verticalParallax;
    Vector3 targetPrevPosition;

    // Start is called before the first frame update
    void Start()
    {
       if (!followingTarget)
       {
            followingTarget = Camera.main.transform;
       }

       targetPrevPosition = followingTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        var delta = followingTarget.position - targetPrevPosition;

        if (verticalParallax)
        {
            delta.y = 0f;
        }

        targetPrevPosition = followingTarget.position;

        transform.position += delta * parallaxStrength;
    }
}
