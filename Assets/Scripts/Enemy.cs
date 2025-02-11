using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public Transform closestTarget;
    public float movementSpeed;
    [SerializeField] private int life; 
    public float capsuleHeight = 2.0f;  // Height of the capsule
    public float capsuleRadius = 1.0f;  // Radius of the capsule
    public float colliderSize;
    public LayerMask collisionLayer;
    public bool move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GetTarget();

    }
//[ExecuteInEditMode]
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, capsuleRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if(move)
        {
            GoToTarget();
        }

        DetectObjects();
        
    }

    void DetectObjects()
    {
        Vector3 point1 = transform.position + Vector3.up * (capsuleHeight / 2);
        Vector3 point2 = transform.position - Vector3.up * (capsuleHeight / 2);
        
        Collider[] hitObjects = Physics.OverlapCapsule(point1, point2, capsuleRadius, collisionLayer);

        foreach (Collider hit in hitObjects)
        {
            if (!hit.GetComponent<Bullet>()) return;
            Bullet bullet = hit.GetComponent<Bullet>();
            if(life>0)
            {
                TakeDamage(bullet);
                return;
            }
            else
            {
                Die();
            }
        }
    }

    private void TakeDamage(Bullet bullet)
    {

        life -= bullet.damages;
        bullet.gameObject.SetActive(false);
        print(life);
        return;

    }
    private void Die()
    {
        gameObject.SetActive(false);
    }


    void GetTarget()
    {
        
    }

    void GoToTarget()
    {
        transform.position += transform.forward*Time.deltaTime*movementSpeed;
    }
}
