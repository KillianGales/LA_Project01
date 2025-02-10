using UnityEngine;

[CreateAssetMenu(fileName = "BulletType", menuName = "Scriptable Objects/BulletType")]
public class BulletType : ScriptableObject
{
    [SerializeField] public float m_Speed;
    //[SerializeField] public GameObject m_Mesh;
    [SerializeField] public Material m_Material;
}
