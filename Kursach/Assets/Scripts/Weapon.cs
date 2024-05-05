using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private AudioSource ShotSound;
    [SerializeField] public int heroDamage = 1;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.enabled = false;
    }

    private void Shot()
    {
        StartCoroutine(Shoot());
        CameraController.Instance.Shake(0.2f, 0.1f);
    }

    IEnumerator Shoot()
    {
        // UnityEngine.Debug.Log("Pew");
        // Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            // UnityEngine.Debug.Log(hitInfo.transform.name);

            Entity enemy = hitInfo.transform.GetComponent<Entity>();
            if (enemy != null)
            {
                enemy.GetDamage(heroDamage);
                enemy.isAgred = true;
            }

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
        }

        lineRenderer.enabled = true;

        ShotSound.Play();
        yield return new WaitForSeconds(0.02f);

        lineRenderer.enabled = false;
    }
}
