using UnityEngine;

public static class InputManager
{
    public static Vector3 GetWorldMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane horizontalPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (horizontalPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
    
}
