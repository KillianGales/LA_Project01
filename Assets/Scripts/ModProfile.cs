using UnityEngine;

public class ModProfile : MonoBehaviour
{
    public EmodTypes type;
    
    public BulletType bulletType;
    //public float fireRate;
    public float lifeSpan;
    public bool dropped;
    [Range (0,1)]
    public int origin;

    public void Update()
    {
        if(dropped)
        {
            CheckLife();
        }


    }

    private void CheckLife()
    {
        if (lifeSpan > 0)
        {
            lifeSpan -= 1f * Time.deltaTime;
        }
        else
        {
            GameManager.Instance.droppedMods.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
