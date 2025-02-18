using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    private TileGrid grid;

    void OnEnable()
    {
        grid = (TileGrid)target;
    }

void OnSceneGUI()
{
    Handles.color = Color.green;

    for (int x = 0; x < grid.width; x++)
    {
        for (int y = 0; y < grid.height; y++)
        {
            Vector2Int gridPos = new Vector2Int(x, y);
            Vector3 worldPos = grid.GridToWorldPosition(gridPos);

            if (Handles.Button(worldPos, grid.transform.rotation, 0.3f, 0.3f, Handles.SphereHandleCap))
            {
                Debug.Log($"Clicked on Tile {gridPos}");
                AddObjectToTile(gridPos);
            }
        }
    }
}

    void AddObjectToTile(Vector2Int gridPos)
    {
        GameObject newObject = grid.enemy;
        Instantiate(newObject, grid.enemyPool);
        newObject.transform.position = grid.GridToWorldPosition(gridPos);
        //newObject.transform.rotation = grid.GridToWorldRotation(gridPos);
        Undo.RegisterCreatedObjectUndo(newObject, "Add Tile Object");

        grid.tileObjects.Add(new TileData { position = gridPos, placedObject = newObject });
    }
}