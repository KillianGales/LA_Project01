using UnityEngine;

public class ModProfile : MonoBehaviour
{
    public EmodTypes type;
    
    public BulletType bulletType;
    //public float fireRate;
    public float lifeSpan;
    public bool dropped;

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
            Destroy(gameObject);
        }
    }
}
