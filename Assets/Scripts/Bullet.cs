using UnityEngine;
using UnityEditor;
//using System.Numerics;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType[] m_BulletTypes;
    //[SerializeField] private Transform dummy;
    private Vector3 m_target;
    public BulletType bullet;
    public bool isActive;

    public void Initialize(int bulletIndex, Vector3 target)
    {
        bullet = m_BulletTypes[bulletIndex];
        GetComponent<Renderer>().material = bullet.m_Material;
        m_target = target; 
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, m_target, bullet.m_Speed);
    }
}
