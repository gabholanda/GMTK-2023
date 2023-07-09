using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

public class FruitManager : MonoBehaviour
{
    public Grid grid;
    public float spawnLength;
    public GameObject fruitPrefab;
    public Vector2 startingPosition;
    public Vector2 finalPosition;
    public Dictionary<int, Vector2> emptyCells = new Dictionary<int, Vector2>();
    public List<Vector2> allCells = new List<Vector2>();
    public List<GameObject> spawnedFruits = new List<GameObject>();
    public Snake snake;
    public int maxFruit = 30;

    private void Awake()
    {
        InitializeEmptyCells();
        StartCoroutine(SpawnFruitsCoroutine());
    }

    private IEnumerator SpawnFruitsCoroutine()
    {
        while (true)
        {
            if (spawnedFruits.Count <= maxFruit)
            {
                SpawnFruit(fruitPrefab);
            }
                yield return new WaitForSeconds(spawnLength);
        }
    }

    private void InitializeEmptyCells()
    {
        int i = 0;
        for (int x = (int)startingPosition.x; x < (int)finalPosition.x; x++)
        {
            for (int y = (int)startingPosition.y; y < (int)finalPosition.y; y++)
            {
                Vector3Int cellPosition = grid.LocalToCell(new Vector2(x, y));
                emptyCells.Add(i, grid.GetCellCenterLocal(cellPosition));
                allCells.Add(grid.GetCellCenterLocal(cellPosition));
                i++;
            }
        }
        Debug.Log(emptyCells.Count);
        Debug.Log(allCells.Count);
    }

    private void SpawnFruit(GameObject fruitPrefab)
    {
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("cells are full");
            return;
        }
        //int headIndex = 0;
        //if (snake.sections.First != null)
        //{
        //    headIndex = snake.sections.First.Value.GetComponent<Snake>().keys.First.Value;
        //}
        //Dictionary<int, Vector2> validEmptyCells = new Dictionary<int, Vector2>(emptyCells); 

        //// Remove keys within 3 cells of the snake head in all four directions
        //for (int x = -3; x <= 3; x++)
        //{
        //    for (int y = -3; y <= 3; y++)
        //    {
        //        int cellIndex = headIndex + x + y * snake.gridWidth;
        //        validEmptyCells.Remove(cellIndex);
        //    }
        //}

        //if (validEmptyCells.Count == 0)
        //{
        //    Debug.LogWarning("No valid empty cells available");
        //    return;
        //}

        List<int> keys = emptyCells.Keys.ToList<int>();
        int randomIndex = Random.Range(0, keys.Count);
        Vector3 randomCell = allCells[keys[randomIndex]];
        emptyCells.Remove(keys[randomIndex]);
        Vector3Int cellPosition = grid.LocalToCell(randomCell);
        Vector3 spawnPosition = grid.GetCellCenterLocal(cellPosition);
        GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);

        fruit.transform.SetParent(grid.transform);
        fruit.GetComponent<SnakeGame.Draggable>().gridmap = grid;
        fruit.GetComponent<SnakeGame.Draggable>().manager = this;
        fruit.GetComponent<SnakeGame.Draggable>().indexInGrid = keys[randomIndex];
        spawnedFruits.Add(fruit);
    }

    public void OnMoveFruit(int previous, int current)
    {
        emptyCells.Remove(current);
        emptyCells.Add(previous, allCells[previous]);
    }

    private void OnDrawGizmos()
    {
        // This only appears in the Scene tab - It is purely for debugging
        foreach (KeyValuePair<int, Vector2> kv in emptyCells)
        {
            Gizmos.DrawCube(kv.Value, new Vector3(1, 1));
        }
    }
}