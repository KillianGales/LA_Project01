using UnityEditor;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    private Transform Canon;
    Vector3 direction, WorldMousePos;
    Quaternion targetRotation;
    [SerializeField] private float rotationSpeed;

    void Awake()
    {
        Canon = this.transform.Find("Canon");
    }

    void Update()
    {
        CanonLookat();
    }


    private void CanonLookat()
    {
        WorldMousePos = InputManager.GetWorldMousePosition();

        direction = WorldMousePos - Canon.position;
        direction.y = 0;

        targetRotation = Quaternion.LookRotation(direction);
        Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
