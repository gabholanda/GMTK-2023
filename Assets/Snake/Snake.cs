using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public enum SnakeHeadDirection
{
    Up,
    Down,
    Left,
    Right
}

public class Snake : MonoBehaviour
{
    Vector3 startingPosition = new Vector3(0.5f, 0.5f, 0);
    public int length;
    public GameObject sectionPrefab;
    public LinkedList<GameObject> sections = new LinkedList<GameObject>();
    public LinkedList<int> keys = new LinkedList<int>();
    public FruitManager fruitScript;
    public float secondsBetweenMoves;

    //emptyCells stuff
    public int gridWidth;
    int gridHeight;

    //sprites
    public Sprite head;
    public Sprite tail;
    public Sprite straight;
    public Sprite turn;

    [SerializeField]
    private SnakeHeadDirection cachedDirection;

    // Start is called before the first frame update
    void Awake()
    {
        Invoke("FiveMoreMinutesMom", 0.1f);
    }

    void FiveMoreMinutesMom()
    {
        gridHeight = (int)Mathf.Abs(fruitScript.startingPosition.y) + (int)Mathf.Abs(fruitScript.finalPosition.y);
        gridWidth = (int)Mathf.Abs(fruitScript.startingPosition.x) + (int)Mathf.Abs(fruitScript.finalPosition.x);

        //setting up initial sections
        for (int i = 0; i < length; i++)
        {
            sections.AddLast(Instantiate(sectionPrefab, startingPosition + (Vector3.left * i), Quaternion.identity));
            int key = ((gridHeight / 2)) + gridHeight * ((gridWidth / 2) - i);
            keys.AddLast(key);

            //Debug.Log(key);
        }

        Debug.Log("from Snake Awake: " + fruitScript.emptyCells.Count);
        Debug.Log("from Snake Awake: " + fruitScript.allCells.Count);

        foreach (int k in keys)
        {
            fruitScript.emptyCells.Remove(k);
        }
        Debug.Log("from Snake Awake: " + fruitScript.emptyCells.Count);
        Debug.Log("from Snake Awake: " + fruitScript.allCells.Count);

        UpdateSprites();
        StartCoroutine(TrackFruitCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
    }

    //add new position
    void AddSection()
    {

        Vector3 newSectionPosition = new Vector3();
        int newTailKey = keys.Last.Value;

        //if tail is pointing to the right
        if(sections.Last.Previous.Value.transform.position.x < sections.Last.Value.transform.position.x)
        {
            newSectionPosition = sections.Last.Value.transform.position + Vector3.right;
            newTailKey = keys.Last.Previous.Value + gridHeight;
            //keys.AddLast(keys.Last.Previous.Value + gridHeight);
        }
        //if tail is pointing to the left
        if (sections.Last.Previous.Value.transform.position.x > sections.Last.Value.transform.position.x)
        {
            newSectionPosition = sections.Last.Value.transform.position + Vector3.left;
            newTailKey = keys.Last.Previous.Value - gridHeight;
            //keys.AddLast(keys.Last.Previous.Value - gridHeight);
        }
        //if tail is pointing down
        if (sections.Last.Previous.Value.transform.position.y < sections.Last.Value.transform.position.y)
        {
            newSectionPosition = sections.Last.Value.transform.position + Vector3.down;
            newTailKey = keys.Last.Previous.Value - 1;
            //keys.AddLast(keys.Last.Previous.Value - 1);
        }
        //if tail is pointing up
        if (sections.Last.Previous.Value.transform.position.y > sections.Last.Value.transform.position.y)
        {
            newSectionPosition = sections.Last.Value.transform.position + Vector3.up;
            newTailKey = keys.Last.Previous.Value + 1;
            //keys.AddLast(keys.Last.Previous.Value + 1);
        }

        sections.AddLast(Instantiate(sectionPrefab, newSectionPosition, Quaternion.identity));
        //keys.AddLast(newTailKey);
        //fruitScript.emptyCells.Remove(newTailKey);




        // TODO: Test this
        //Vector3 newSectionPosition = sections.First.Value.transform.position;
        //if (cachedDirection == SnakeHeadDirection.Up)
        //{
        //    newSectionPosition.y += 1;
        //}
        //else if (cachedDirection == SnakeHeadDirection.Down)
        //{
        //    newSectionPosition.y -= 1;
        //}
        //else if (cachedDirection == SnakeHeadDirection.Left)
        //{
        //    newSectionPosition.x -= 1;
        //}
        //else
        //{
        //    newSectionPosition.x += 1;
        //}

        //// We are adding to the head here, so I am pushing it one tile ahead and updating sprites, so player wont see anything funny happening
        //sections.AddBefore(sections.First, Instantiate(sectionPrefab, newSectionPosition, Quaternion.identity));




        // Update sprites is doing all the work of setting the right sprite and its necessary rotation, thanks Sabrina.
        UpdateSprites();
        //update length variable (for game over check)
        length++;
    }

    void MoveSnake(SnakeHeadDirection direction)
    {
        int oldTailKey = keys.Last.Value;
        int newHeadKey = keys.First.Value;

        //moving all other elements
        LinkedListNode<GameObject> node = sections.Last;
        while (node != sections.First)
        {
            node.Value.transform.position = node.Previous.Value.transform.position;
            node = node.Previous;
        }

        //moving head and caching the last direction it went towards to
        switch (direction)
        {
            case SnakeHeadDirection.Up:
                cachedDirection = SnakeHeadDirection.Up;
                sections.First.Value.transform.position += Vector3.up;
                newHeadKey = keys.First.Value + 1;
                break;
            case SnakeHeadDirection.Down:
                cachedDirection = SnakeHeadDirection.Down;
                sections.First.Value.transform.position += Vector3.down;
                newHeadKey = keys.First.Value - 1;
                break;
            case SnakeHeadDirection.Left:
                cachedDirection = SnakeHeadDirection.Left;
                sections.First.Value.transform.position += Vector3.left;
                newHeadKey = keys.First.Value - gridHeight;
                break;
            case SnakeHeadDirection.Right:
                cachedDirection = SnakeHeadDirection.Right;
                sections.First.Value.transform.position += Vector3.right;
                newHeadKey = keys.First.Value + gridHeight;
                break;
        }

        keys.AddFirst(newHeadKey);
        keys.RemoveLast();


        fruitScript.OnMoveFruit(oldTailKey, newHeadKey);

        UpdateSprites();
    }
    IEnumerator TrackFruitCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenMoves);
            TrackFruit();
        }
    }
    void TrackFruit()
    {
        //find closest fruit
        GameObject closestFruit = null;
        for (int i = 0; i < fruitScript.spawnedFruits.Count; i++)
        {
            if (closestFruit == null)
            {
                closestFruit = fruitScript.spawnedFruits[i];
            }
            else if (Vector3.Distance(sections.First.Value.transform.position, fruitScript.spawnedFruits[i].transform.position) < Vector3.Distance(sections.First.Value.transform.position, closestFruit.transform.position))
            {
                closestFruit = fruitScript.spawnedFruits[i];
            }
        }



        if (closestFruit != null)
        {
            //move towards closest fruit
            float yDistance = closestFruit.transform.position.y - sections.First.Value.transform.position.y;
            float xDistance = closestFruit.transform.position.x - sections.First.Value.transform.position.x;

            //if not on fruit, move
            if (!(xDistance == 0 && yDistance == 0))
            {
                //if y distance == 0 -> move x
                //if x < y -> move x
                //if x = y -> move x
                if ((yDistance == 0) || (Mathf.Abs(xDistance) < Mathf.Abs(yDistance)) || (Mathf.Abs(xDistance) == Mathf.Abs(yDistance)))
                {
                    //move in x direction
                    if (xDistance < 0)
                    {
                        //check if moving back on itself
                        if ((sections.First.Value.transform.position + Vector3.left) != sections.First.Next.Value.transform.position)
                        {
                            MoveSnake(SnakeHeadDirection.Left);
                        }
                        else
                        {
                            //move in y direction
                            if (yDistance < 0)
                            {
                                MoveSnake(SnakeHeadDirection.Down);
                            }
                            else
                            {
                                MoveSnake(SnakeHeadDirection.Up);
                            }
                        }
                    }
                    else if (xDistance > 0)
                    {
                        //check if moving back on itself
                        if ((sections.First.Value.transform.position + Vector3.right) != sections.First.Next.Value.transform.position)
                        {
                            MoveSnake(SnakeHeadDirection.Right);
                        }
                        else
                        {
                            //move in y direction
                            if (yDistance < 0)
                            {
                                MoveSnake(SnakeHeadDirection.Down);
                            }
                            else
                            {
                                MoveSnake(SnakeHeadDirection.Up);
                            }
                        }
                    }
                }
                //if x distance == 0 -> move y
                //if y < x -> move y
                if ((xDistance == 0) || (Mathf.Abs(yDistance) < Mathf.Abs(xDistance)))
                {
                    //move in y direction
                    if (yDistance < 0)
                    {
                        //check if moving back on itself
                        if ((sections.First.Value.transform.position + Vector3.down) != sections.First.Next.Value.transform.position)
                        {
                            MoveSnake(SnakeHeadDirection.Down);
                        }
                        else
                        {
                            //move in x direction
                            if (xDistance < 0)
                            {
                                MoveSnake(SnakeHeadDirection.Left);
                            }
                            else
                            {
                                MoveSnake(SnakeHeadDirection.Right);
                            }
                        }
                    }
                    else if (yDistance > 0)
                    {
                        //check if moving back on itself
                        if ((sections.First.Value.transform.position + Vector3.up) != sections.First.Next.Value.transform.position)
                        {
                            MoveSnake(SnakeHeadDirection.Up);
                        }
                        else
                        {
                            //move in x direction
                            if (xDistance < 0)
                            {
                                MoveSnake(SnakeHeadDirection.Left);
                            }
                            else
                            {
                                MoveSnake(SnakeHeadDirection.Right);
                            }
                        }
                    }
                }
            }


            //update distance and check if head is on fruit
            yDistance = closestFruit.transform.position.y - sections.First.Value.transform.position.y;
            xDistance = closestFruit.transform.position.x - sections.First.Value.transform.position.x;
            if (xDistance == 0 && yDistance == 0)
            {
                AddSection();
                fruitScript.spawnedFruits.Remove(closestFruit);
                Destroy(closestFruit);
            }
        }
        closestFruit = null;
    }

    void UpdateSprites()
    {
        LinkedListNode<GameObject> node = sections.Last;
        while (node != null)
        {
            //if node is head
            if (node == sections.First)
            {
                node.Value.GetComponent<SpriteRenderer>().sprite = head;
                //if piece after is above
                if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.up)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                //if piece after is below
                if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.down)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                //if piece after is to right
                if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.right)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                //if piece after is to left
                if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.left)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
            }

            //if node is tail
            else if (node == sections.Last)
            {
                node.Value.GetComponent<SpriteRenderer>().sprite = tail;
                //if piece before is above
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.up)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                //if piece before is below
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.down)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                //if piece before is to right
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.right)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                //if piece before is to left
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.left)
                {
                    node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }

            //section is middle piece
            else
            {
                //previous node is above current node
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.up)
                {
                    //next node is right of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.right)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    //next node is left of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.left)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    //next node is below current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.down)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = straight;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                //previous node is below current node
                else if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.down)
                {
                    //next node is right of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.right)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
                    }
                    //next node is left of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.left)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    //next node is above current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.up)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = straight;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                //previous node is right of current node
                else if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.right)
                {
                    //next node is above current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.up)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    //next node is left of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.left)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = straight;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    //next node is below current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.down)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
                    }
                }
                //previous node is left of current node
                else if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.left)
                {
                    //next node is right of current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.right)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = straight;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    //next node is above current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.up)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    //next node is below current node
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.down)
                    {
                        node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                }
            }

            node = node.Previous;
        }
    }
}
