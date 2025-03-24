using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform cam; 

    void Start()
    {
        cam = GameManager.Instance.cam.transform;
    }/*
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }*/
    void LateUpdate()
    {
        // Get the direction from the object to the camera
        Vector3 direction = cam.transform.position - transform.position;
        direction.x = 0; // Lock X rotation
        direction.z = 0; // Lock Z rotation

        // Make the object look at the camera (only rotating on the Y-axis)
        transform.rotation = Quaternion.LookRotation(-direction);
    }

}
