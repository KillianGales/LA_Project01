using UnityEngine;

[CreateAssetMenu(fileName = "BulletType", menuName = "Scriptable Objects/BulletType")]
public class BulletType : ScriptableObject
{
    [SerializeField] public float m_Speed;
    //[SerializeField] public GameObject m_Mesh;
    [SerializeField] public Material m_Material;
    [SerializeField] public int m_damages;
    [SerializeField] public float m_expensionRate;
    [SerializeField] public float m_lifeSpan;
    public float fireRate;
    [SerializeField] public EBehaviour behaviour;
    [Header("UI Setup")]
    [SerializeField] public Color32 colorOverride;
    public Sprite imageVisual;


}
