using UnityEngine;
using System.Collections;
using System.Net.WebSockets;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public Transform closestTarget;
    public float movementSpeed;
    [SerializeField] private int life, baselife; 
    public float capsuleHeight = 2.0f;  // Height of the capsule
    public float capsuleRadius = 1.0f;  // Radius of the capsule
    public float colliderSize;
    public LayerMask collisionLayer;
    public bool move;
    private float minDist;
    private int closestTargetIndex;
    [SerializeField] List<Transform> allTurrets; 
    [SerializeField] float rotationSpeed;
    [SerializeField] Slider healthBar;
    [SerializeField] float lifeAnimSpeed;
    [SerializeField] TMP_Text lifeText;
    [SerializeField] float atkRange;
    [SerializeField] int hitStrength;
    [SerializeField] float hitRate;
    private TurretBehaviour turretBe;
    public ObjectPool pool; 

    void Start()
    {
        Init();
    }
    void OnEnable()
    {
        Init();
    }

    void Init()
    {
        Vector3 directionToOrigin = Vector3.zero - transform.position;
        life = baselife;
        healthBar.maxValue = life;
        healthBar.value = healthBar.maxValue;
        lifeText.SetText(life.ToString());
        
        if (directionToOrigin != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToOrigin);
        }

        move = true;
    }

    /*  void OnDrawGizmosSelected()
       {
           Vector3 point1 = transform.position + Vector3.up * (capsuleHeight / 2);
           Vector3 point2 = transform.position - Vector3.up * (capsuleHeight / 2);
           CapsuleCollider coll = transform.GetComponent<CapsuleCollider>();
           coll.radius = capsuleRadius;
           coll.height = capsuleHeight;
           Gizmos.DrawSphere(transform.position, capsuleRadius);
       }*/

    // Update is called once per frame
    void Update()
    {

        if(move)
        {
           GoToTarget();
        }

        if(closestTarget == null)
        {
            GetTarget();
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


            if(life > 0)
            {
                TakeDamage(bullet);
                lifeText.SetText(life.ToString());
            }

            if(life <= 0)
            {
                Die();
            }
        }
    }

    private void TakeDamage(Bullet bullet)
    {

        life -= bullet.damages;
        bullet.gameObject.SetActive(false);
        healthBar.value = life;//Mathf.Lerp(healthBar.value, life, lifeAnimSpeed*Time.deltaTime);
        return;

    }
    private void Die()
    {
        GameManager.Instance.CheckForDrop(transform);
        pool.ReturnObject(gameObject);
    }

    private IEnumerator InflictDamage()
    {
        while(!move)
        {
            turretBe.life -= hitStrength;
            yield return new WaitForSeconds(hitRate);
        }

    }


    void GetTarget()
    {
        allTurrets = GameManager.Instance.allTurrets;

        for(int i = 0;i < allTurrets.Count; i++ )
        {
            float dist = Vector3.Distance(transform.position, allTurrets[i].position);

            if(dist < minDist)
            {
                closestTargetIndex = i;
            }
        }

        closestTarget = allTurrets[closestTargetIndex];
        if(closestTarget != null)
        {
            turretBe = closestTarget.GetComponent<TurretBehaviour>();
            move = true;
        }

        
    }

    void GoToTarget()
    {
        if(closestTarget ==null) return;
        Vector3 direction = closestTarget.position - transform.position;
        direction.y = 0;

        Quaternion newTar = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTar, rotationSpeed * Time.deltaTime);

        if(atkRange*atkRange <= direction.sqrMagnitude)
        {
            transform.position += transform.forward*Time.deltaTime*movementSpeed;
        }
        else
        {
            move = false;
            StartCoroutine(InflictDamage());

            return;
        }



    }
}
