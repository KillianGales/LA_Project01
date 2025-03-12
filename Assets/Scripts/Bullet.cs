using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType myType;
    private EBehaviour myBehaviour;
    [SerializeField] private EmodTypes mod;
    private Vector3 targetF, targetR, targetU;
    private float bulletSpeed;
    //public BulletType bullet;
    public int damages;
    private float instTime;
    private Vector3 OGPosition;
    private float expensionRate;
    public float lifeSpan;
    public delegate void behaviourFunc();
    private behaviourFunc activeBehaviour; 

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

                targetF = targetObject.forward;
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
        }



    }

    public void Update()
    {
        activeBehaviour?.Invoke();
    }

    private void BShootStraight()
    {
        transform.position += targetF * Time.deltaTime * bulletSpeed;
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
        activeBehaviour = BShootStraight;

        yield return new WaitForSeconds(1f);

        instTime = Time.time;
        OGPosition = transform.position;
        activeBehaviour = BCircleAround;
    }

}
