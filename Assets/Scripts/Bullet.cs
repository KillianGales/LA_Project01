using UnityEngine;
using UnityEditor;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType myType;
    [SerializeField] private EmodTypes mod;
    private Vector3 target;
    private float bulletSpeed;
    //public BulletType bullet;
    public bool isActive;
    public int damages;

    public void Initialize(Vector3 target, BulletType type)
    {
        myType = type;
        GetComponent<Renderer>().material = type.m_Material;
        bulletSpeed = type.m_Speed;
        this.target = target;
        damages = type.m_damages;
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
                transform.position += target * Time.deltaTime * bulletSpeed;
                break;
            
        }


        
    }
}
