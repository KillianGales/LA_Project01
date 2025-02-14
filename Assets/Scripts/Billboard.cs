using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform cam; 

    void Start()
    {
        cam = GameManager.Instance.cam.transform;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
