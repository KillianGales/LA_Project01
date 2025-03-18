using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [SerializeField] public BulletType myType;
    public EBehaviour myBehaviour;
    [SerializeField] private EmodTypes mod;
    private Vector3 targetF, targetR, targetU, targetDir;
    private float bulletSpeed;
    //public BulletType bullet;
    public int damages;
    private float instTime;
    private Vector3 OGPosition;
    private float expensionRate;
    public float lifeSpan;
    public delegate void behaviourFunc();
    private behaviourFunc activeBehaviour;
    private float myPropagationRadius;
    private LayerMask enemyLayer;
    public int maxTargets;
    private Enemy directedTarget;
    private HashSet<Enemy> processedEnemy = new HashSet<Enemy>();
    public Collider[] nearEnemyColliders;
    public float stunTime;

    public void Initialize(Transform targetObject, BulletType type)
    {
        //Universal init
        myType = type;
        myBehaviour = myType.behaviour;
        GetComponent<Renderer>().material = type.m_Material;
        lifeSpan = myType.m_lifeSpan;
        bulletSpeed = myType.m_Speed;
        damages = type.m_damages;

        //Behaviour specific init
        switch (myBehaviour)
        {
            case EBehaviour.shootStraight :

                targetDir = targetObject.forward;
                activeBehaviour = BShootStraight;

                break;

            case EBehaviour.circleAround :

                instTime = Time.time;
                OGPosition = transform.position;
                expensionRate = type.m_expensionRate;
                targetF = targetObject.forward;
                targetU = targetObject.up;
                targetR = targetObject.right;

                StartCoroutine(CircleAroundInit());
                //activeBehaviour = BCircleAround;

                break;

            case EBehaviour.Disperse : 

                myPropagationRadius = type.propagationRadius;
                enemyLayer = type.enemyLayer;
                maxTargets = type.maxTargets;
                lifeSpan = 1000;
                stunTime = type.stunTime;

                targetDir = targetObject.forward;
                activeBehaviour = BShootStraight;

            break;
        }



    }

    public void Update()
    {
        activeBehaviour?.Invoke();
    }

    private void BShootStraight()
    {
        transform.position += targetDir * Time.deltaTime * bulletSpeed;
    }

    private void BCircleAround()
    {
        float angle = (Time.time-instTime) * bulletSpeed;
        float radius = expensionRate * (Time.time-instTime);

        Vector3 offset = (Mathf.Cos(angle) * radius * targetR) + (Mathf.Sin(angle) * radius * targetU);
        transform.position = OGPosition + offset;

    }

    private IEnumerator CircleAroundInit()
    {
        targetDir = targetU;
        activeBehaviour = BShootStraight;

        yield return new WaitForSeconds(0.1f);

        instTime = Time.time;
        OGPosition = transform.position;
        activeBehaviour = BCircleAround;
    }


    public void getNextTarget(Enemy currentEnemy)
    {
        processedEnemy.Add(currentEnemy);
        Collider[] colliders = Physics.OverlapSphere(transform.position, myPropagationRadius, enemyLayer);

        Enemy closestTarget = null;
        float minSqrDist = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            Enemy target = col.GetComponent<Enemy>();

            if (target == null || processedEnemy.Contains(target) || target == currentEnemy)
               continue;

            float sqrDist = (target.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closestTarget = target;
            }
        }

        if (maxTargets <= 0 || closestTarget == null)
        {
            ResetBullet();
            return;
        }

        if (closestTarget != null)
        {
            processedEnemy.Add(closestTarget);
            directedTarget = closestTarget;
            maxTargets--;
            activeBehaviour = goToDirectedTarget;
        }
    }

    public void goToDirectedTarget()
    {
        transform.position = Vector3.Lerp(transform.position, directedTarget.transform.position, Time.deltaTime*bulletSpeed);
    }

    public void ResetBullet()
    {
        activeBehaviour = null;
        gameObject.SetActive(false);
        lifeSpan = 0;
    }

}
