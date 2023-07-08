using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    public Grid grid;
    public float spawnLength;
    public GameObject fruitPrefab;
    public Vector2 startingPosition;
    public Vector2 finalPosition;
    [SerializeField]
    private List<Vector2> emptyCells = new List<Vector2>();

    private void Awake()
    {
        InitializeEmptyCells();
        StartCoroutine(SpawnFruitsCoroutine());
    }

    private IEnumerator SpawnFruitsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnLength);
            SpawnFruit(fruitPrefab);
        }
    }

    private void InitializeEmptyCells()
    {
        for (int x = (int)startingPosition.x; x < (int)finalPosition.x; x++)
        {
            for (int y = (int)startingPosition.y; y < (int)finalPosition.y; y++)
            {
                Vector3Int cellPosition = grid.LocalToCell(new Vector2(x, y));
                emptyCells.Add(grid.GetCellCenterLocal(cellPosition));
            }
        }
    }

    private void SpawnFruit(GameObject fruitPrefab)
    {
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("cells are full");
            return;
        }

        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector3 randomCell = emptyCells[randomIndex];
        emptyCells.RemoveAt(randomIndex);
        Vector3Int cellPosition = grid.LocalToCell(randomCell);
        Vector3 spawnPosition = grid.GetCellCenterLocal(cellPosition);
        GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
        fruit.transform.SetParent(grid.transform);
    }

    private void OnDrawGizmos()
    {
        // This only appears in the Scene tab - It is purely for debugging
        for (int i = 0; i < emptyCells.Count; i++)
        {
            Gizmos.DrawCube(emptyCells[i], new Vector3(1, 1));
        }
    }
}