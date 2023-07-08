using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
    public Grid gridmap;
    private Vector3 pastPosition;
    private Vector3 cellToCheck;

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseDown()
    {
        pastPosition = gameObject.transform.position;
    }

    private void OnMouseUp()
    {

        Vector3Int cellPosition = gridmap.LocalToCell(GetMouseWorldPosition());
        Vector3 currentMousePosition = gridmap.GetCellCenterLocal(cellPosition);
        Vector3 distance = currentMousePosition - pastPosition;
        Vector3 direction = (currentMousePosition - pastPosition).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // We are going horizontal - remove vertical move.
            currentMousePosition.y = pastPosition.y;
            int quantityOfCellsToCheck = (int)Mathf.Round(Mathf.Abs(distance.x) / gridmap.cellSize.x);

            for (int i = 1; i < quantityOfCellsToCheck + 1; i++)
            {
                // Check every cell if there is anything there already
                cellToCheck = pastPosition;
                cellToCheck.x += Mathf.Sign(direction.x) * i;
                Vector3Int cellPos = gridmap.LocalToCell(cellToCheck);
                cellToCheck = gridmap.GetCellCenterLocal(cellPos);
                Collider2D something = Physics2D.OverlapBox(cellToCheck, gridmap.cellSize * .75f, 0);
                if (something != null)
                {
                    cellToCheck.x -= Mathf.Sign(direction.x);
                    transform.localPosition = cellToCheck;
                    return;
                }
            }
        }
        else
        {
            // We are going vertical - remove horizontal move
            currentMousePosition.x = pastPosition.x;
            int quantityOfCellsToCheck = (int)Mathf.Round(Mathf.Abs(distance.y) / gridmap.cellSize.y);

            // Check every cell if there is anything there already
            for (int i = 1; i < quantityOfCellsToCheck + 1; i++)
            {
                cellToCheck = pastPosition;
                cellToCheck.y += Mathf.Sign(direction.y) * i;
                Vector3Int cellPos = gridmap.LocalToCell(cellToCheck);
                cellToCheck = gridmap.GetCellCenterLocal(cellPos);
                Collider2D something = Physics2D.OverlapBox(cellToCheck, gridmap.cellSize * .75f, 0);
                if (something != null)
                {
                    cellToCheck.y -= Mathf.Sign(direction.y);
                    transform.localPosition = cellToCheck;
                    return;
                }
            }
        }

        transform.localPosition = currentMousePosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(cellToCheck, gridmap.cellSize);
    }
}
