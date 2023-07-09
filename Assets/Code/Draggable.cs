using UnityEngine;
using UnityEngine.InputSystem;


namespace SnakeGame
{

    public class Draggable : MonoBehaviour
    {
        public Grid gridmap;
        private Vector3 pastPosition;
        private Vector3 cellToCheck;
        public FruitManager manager;

        public int indexInGrid;

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
                int horizontalIncrement = (int)Mathf.Abs(manager.startingPosition.y) + (int)Mathf.Abs(manager.finalPosition.y);
                if (direction.x > 0)
                {
                    for (int i = 1; i <= quantityOfCellsToCheck + 1; i++)
                    {
                        int k = indexInGrid + horizontalIncrement * i;
                        if (!manager.emptyCells.ContainsKey(k))
                        {
                            if (indexInGrid == k - horizontalIncrement)
                            {
                                return;
                            }
                            transform.localPosition = manager.emptyCells[k - horizontalIncrement];
                            manager.OnMoveFruit(indexInGrid, k - horizontalIncrement);
                            indexInGrid = k - horizontalIncrement;
                            return;
                        }
                    }
                    int previousIdx = indexInGrid;
                    indexInGrid += (horizontalIncrement * quantityOfCellsToCheck);
                    transform.localPosition = manager.emptyCells[indexInGrid];
                    manager.OnMoveFruit(previousIdx, indexInGrid);
                }
                else
                {
                    for (int i = 1; i <= quantityOfCellsToCheck + 1; i++)
                    {
                        int k = indexInGrid - horizontalIncrement * i;
                        if (!manager.emptyCells.ContainsKey(k))
                        {
                            if (indexInGrid == k + horizontalIncrement)
                            {
                                return;
                            }
                            transform.localPosition = manager.emptyCells[k + horizontalIncrement];
                            manager.OnMoveFruit(indexInGrid, k + horizontalIncrement);
                            indexInGrid = k + horizontalIncrement;
                            return;
                        }
                    }
                    int previousIdx = indexInGrid;
                    indexInGrid -= (horizontalIncrement * quantityOfCellsToCheck);
                    transform.localPosition = manager.emptyCells[indexInGrid];
                    manager.OnMoveFruit(previousIdx, indexInGrid);
                }

            }
            else
            {
                // We are going vertical - remove horizontal move
                currentMousePosition.x = pastPosition.x;
                int quantityOfCellsToCheck = (int)Mathf.Round(Mathf.Abs(distance.y) / gridmap.cellSize.y);
                int verticalSkippingConstraint = (int)Mathf.Abs(manager.startingPosition.y) + (int)Mathf.Abs(manager.finalPosition.y);
                int verticalIncrement = 1;


                if (direction.y > 0)
                {
                    int cachedColumnMax = Mathf.CeilToInt(indexInGrid / verticalSkippingConstraint);
                    int newIdx;
                    for (int i = 1; i <= quantityOfCellsToCheck + 1; i++)
                    {
                        int k = indexInGrid + verticalIncrement * i;
                        if (!manager.emptyCells.ContainsKey(k))
                        {
                            if (indexInGrid == k - verticalIncrement)
                            {
                                return;
                            }
                            newIdx = k - verticalIncrement;
                            if (cachedColumnMax < (newIdx) / verticalSkippingConstraint)
                            {
                                newIdx = verticalSkippingConstraint * (cachedColumnMax);
                            }

                            transform.localPosition = manager.emptyCells[newIdx];
                            manager.OnMoveFruit(indexInGrid, newIdx);
                            indexInGrid = newIdx;
                            return;
                        }
                    }
                    int previousIdx = indexInGrid;
                    newIdx = indexInGrid + (verticalIncrement * quantityOfCellsToCheck);

                    if (cachedColumnMax < (newIdx) / verticalSkippingConstraint)
                    {
                        newIdx = verticalSkippingConstraint * (cachedColumnMax) + verticalSkippingConstraint - 1;
                    }

                    if (newIdx == indexInGrid)
                    {
                        return;
                    }
                    indexInGrid = newIdx;
                    transform.localPosition = manager.emptyCells[indexInGrid];
                    manager.OnMoveFruit(previousIdx, indexInGrid);
                }
                else
                {
                    int cachedColumnMin = Mathf.FloorToInt(indexInGrid / verticalSkippingConstraint);
                    int newIdx;
                    for (int i = 1; i <= quantityOfCellsToCheck + 1; i++)
                    {
                        int k = indexInGrid - verticalIncrement * i;
                        if (k < 0)
                        {
                            break;
                        }
                        if (!manager.emptyCells.ContainsKey(k))
                        {
                            if (indexInGrid == k + verticalIncrement)
                            {
                                return;
                            }

                            newIdx = k - verticalIncrement;
                            if (cachedColumnMin > (newIdx) / verticalSkippingConstraint)
                            {
                                newIdx = verticalSkippingConstraint * (cachedColumnMin);
                            }

                            transform.localPosition = manager.emptyCells[newIdx];
                            manager.OnMoveFruit(indexInGrid, newIdx);
                            indexInGrid = newIdx;
                            return;
                        }
                    }
                    int previousIdx = indexInGrid;
                    newIdx = indexInGrid - (verticalIncrement * quantityOfCellsToCheck);

                    if (cachedColumnMin > (newIdx) / verticalSkippingConstraint)
                    {
                        newIdx = verticalSkippingConstraint * (cachedColumnMin);
                    }

                    if (newIdx < 0)
                    {
                        newIdx = 0;
                    }

                    if (newIdx == indexInGrid)
                    {
                        return;
                    }
                    indexInGrid = newIdx;
                    transform.localPosition = manager.emptyCells[indexInGrid];
                    manager.OnMoveFruit(previousIdx, indexInGrid);
                }
            }
        }
    }

}