using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType myType;
    [SerializeField] private EmodTypes mod;
    private Vector3 target;
    private float bulletSpeed;
    //public BulletType bullet;
    public bool isActive;
    public int damages;
    private float instTime;
    private Vector3 OGPosition;
    private float angle;
    public float lifeSpan;

    public void Initialize(Vector3 target, BulletType type)
    {
        myType = type;
        GetComponent<Renderer>().material = type.m_Material;
        lifeSpan = type.m_lifeSpan;
        bulletSpeed = type.m_Speed;
        this.target = target;
        damages = type.m_damages;
        instTime = Time.time;
        OGPosition = transform.position;
        angle = type.m_angle;

        /*
        bullet = BulletTypes[bulletIndex];
        GetComponent<Renderer>().material = bullet.m_Material;
        m_bulletSpeed = bullet.m_Speed;
        m_target = target;
        damages = bullet.m_damages;*/
    }

    public void Update()
    {
        Behaviour();
    }

    private void Behaviour()
    {
        switch (myType.behaviour)
        {
            case EBehaviour.shootStraight :
                transform.position += target * Time.deltaTime * bulletSpeed;
                break;
            case EBehaviour.circleAround :
                float angle = (Time.time-instTime) * bulletSpeed;
                float radius = this.angle * (Time.time-instTime);

                transform.position = new Vector3(
                Mathf.Cos(angle) * radius + OGPosition.x,
                0,
                Mathf.Sin(angle) * radius + OGPosition.z);
                
                //transform.position += new Vector3 (transform.position.x*Time.deltaTime, 0, target.z) * Time.deltaTime * bulletSpeed;
                break;
            
        }


        
    }
}
