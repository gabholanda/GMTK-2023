using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    public GridLayout grid;
    public float spawnLength;
    public GameObject fruitPrefab;
    public int gridWidth = 16;
    public int gridHeight = 7;
    private List<Vector2Int> emptyCells = new List<Vector2Int>();

    private void Start()
    {
        StartCoroutine(SpawnFruitsCoroutine());
        InitializeEmptyCells();
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
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                emptyCells.Add(new Vector2Int(x, y));
            }
        }
    }

    private Vector3 GetCellPosition(Vector2Int cell)
    {
        Vector3 cellSize = grid.cellSize;
        Vector3 gridCenter = grid.transform.position;
        Vector3 gridOffset = new Vector3(cellSize.x * gridWidth * 0.5f, cellSize.y * gridHeight * 0.5f, 0f);
        Vector3 origin = gridCenter - gridOffset;
        return origin + new Vector3(cell.x * cellSize.x - 0.5f, cell.y * cellSize.y, 0f);
    }

    private void SpawnFruit(GameObject fruitPrefab)
    {
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("cells are full");
            return;
        }

        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector2Int randomCell = emptyCells[randomIndex];
        emptyCells.RemoveAt(randomIndex);

        Vector3 spawnPosition = GetCellPosition(randomCell);
        GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
        fruit.transform.SetParent(grid.transform);
    }
}