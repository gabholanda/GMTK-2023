using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

public class Fruits : MonoBehaviour
{
    public Grid grid;
    public float spawnLength;
    public GameObject fruitPrefab;
    public Vector2 startingPosition;
    public Vector2 finalPosition;
    [SerializeField]
    private List<Vector2> emptyCells = new List<Vector2>();
    public List<GameObject> spawnedFruits = new List<GameObject>();
    public Snake snake;

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
            Debug.LogWarning("no free cells");
            return;
        }

        List<Vector2> validEmptyCells = new List<   Vector2>(emptyCells);

        Vector3 snakeHeadPosition = snake.sections.First.Value.transform.position;
        validEmptyCells.RemoveAll(cell =>
        {
            Vector3Int cellPosition = grid.LocalToCell(cell);
            Vector3 cellCenter = grid.GetCellCenterLocal(cellPosition);
            float distance = Vector3.Distance(cellCenter, snakeHeadPosition);
            return distance <= 3f;
        });

        if (validEmptyCells.Count == 0)
        {
            Debug.LogWarning("no free cells pt2");
            return;
        }

        int randomIndex = Random.Range(0, validEmptyCells.Count);
        Vector3 randomCell = validEmptyCells[randomIndex];

        emptyCells.Remove(randomCell);

        Vector3Int cellPosition = grid.LocalToCell(randomCell);
        Vector3 spawnPosition = grid.GetCellCenterLocal(cellPosition);

        GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
        fruit.transform.SetParent(grid.transform);

        spawnedFruits.Add(fruit);
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