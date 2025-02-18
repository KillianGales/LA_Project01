using UnityEngine;
using UnityEditor;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType[] m_BulletTypes;
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
        transform.position += m_target*Time.deltaTime*m_bulletSpeed;
    }
}
