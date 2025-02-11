using UnityEngine;
using UnityEditor;
//using System.Numerics;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType[] m_BulletTypes;
    //[SerializeField] private Transform dummy;
    private Vector3 m_target;
    private float m_bulletSpeed;
    public BulletType bullet;
    public bool isActive;
    public int damages;

    public void Initialize(int bulletIndex, Vector3 target)
    {
        bullet = m_BulletTypes[bulletIndex];
        GetComponent<Renderer>().material = bullet.m_Material;
        m_bulletSpeed = bullet.m_Speed;
        m_target = target;
        damages = bullet.m_damages;
    }

    public void Update()
    {
        transform.position += m_target*Time.deltaTime*m_bulletSpeed;//Vector3.Lerp(transform.position, m_target, bullet.m_Speed);
    }
}
