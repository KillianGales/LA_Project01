using UnityEngine;
using System.Collections;
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
    [SerializeField] Image healthFill;
    [SerializeField] float lifeAnimSpeed;
    [SerializeField] TMP_Text lifeText;
    [SerializeField] float atkRange;
    [SerializeField] int hitStrength;
    [SerializeField] float hitRate;
    private TurretBehaviour turretBe;
    public ObjectPool pool;
    //public GameObject lifeUI;
    public EnemyType myType;
    private HashSet<Bullet> processedBullets = new HashSet<Bullet>();
    private bool hasDied;

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
        if(myType != null)
        {
            healthFill.color = myType.s_lifeUIColor;
            baselife = myType.s_life;
        }

        hasDied = false;
        Vector3 directionToOrigin = Vector3.zero - transform.position;
        life = Mathf.Clamp(baselife, 0, baselife);
        healthBar.maxValue = life;
        healthBar.value = healthBar.maxValue;
        lifeText.SetText(life.ToString());
        
        if (directionToOrigin != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToOrigin);
        }

        move = true;
    }

    /* void OnDrawGizmosSelected()
       {

        Gizmos.DrawSphere(transform.position, 7);
        /*   Vector3 point1 = transform.position + Vector3.up * (capsuleHeight / 2);
           Vector3 point2 = transform.position - Vector3.up * (capsuleHeight / 2);
           CapsuleCollider coll = transform.GetComponent<CapsuleCollider>();
           coll.radius = capsuleRadius;
           coll.height = capsuleHeight;
           Gizmos.DrawSphere(transform.position, capsuleRadius);
       }
*/
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
            
            //if (!hit.GetComponent<Bullet>()) return;
            Bullet bullet = hit.GetComponent<Bullet>();

            if (bullet == null || processedBullets.Contains(bullet)) 
                continue;
            
            processedBullets.Add(bullet);

            switch(bullet.myBehaviour)
            {
                case EBehaviour.shootStraight:
                    bullet.gameObject.SetActive(false);
                    EvaluateDamage(bullet);
                    break;
                case EBehaviour.circleAround:
                    bullet.gameObject.SetActive(false);
                    EvaluateDamage(bullet);
                    break;
                case EBehaviour.Disperse:
                    bullet.getNextTarget(this);
                    EvaluateDamage(bullet);
                    break;
            }
            
        }
    }

    void EvaluateDamage(Bullet bullet)
    {
        if (hasDied) return;
        //Debug.Break();
        ParticleSystem ps = bullet.myType.endParticleSystem;
        if(ps !=null)
            FXPoolManager.Instance.PlayParticle(bullet.myType.name, ps.name, transform.position);

        if(life > 0)
        {
            TakeDamage(bullet);
            lifeText.SetText(life.ToString());

            StartCoroutine(StopInTrack(bullet.stunTime));

            if(life <= 0)
            {
                Die();
            }
                
            return;
        }
/*
            if(life <= 0)
            {
                Die();
                return;
            }*/
    }

    private void TakeDamage(Bullet bullet)
    {

        life -= bullet.damages;
        healthBar.value = life;
        return;

    }
    private void Die()
    {
        hasDied = true;
        SpawnerManager.instance.EnemyDefeated();
        GameManager.Instance.CheckForDrop(transform);
        pool.ReturnObject(gameObject);
        return;
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

    IEnumerator StopInTrack(float stunTime)
    {
        move = false;
        yield return new WaitForSeconds(stunTime);
        move = true;
    }


}
