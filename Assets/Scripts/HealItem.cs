using UnityEngine;

public class HealItem : MonoBehaviour
{
    public float healAmount;
    public float lifeSpan;

    public void Update()
    {
        CheckLife();
    }

    private void CheckLife()
    {
        if (lifeSpan > 0)
        {
            lifeSpan -= 1f * Time.deltaTime;
        }
        else
        {
            //GameManager.Instance.droppedMods.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
