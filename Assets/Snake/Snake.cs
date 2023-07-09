using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SnakeHeadDirection
{
    Up,
    Down,
    Left,
    Right
}

public class Snake : MonoBehaviour
{
    public Vector3 startingPosition;
    public int length;
    public GameObject sectionPrefab;
    public LinkedList<GameObject> sections = new LinkedList<GameObject>();

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
        //setting up initial sections
        for (int i = 0; i < length; i++)
        {
            sections.AddLast(Instantiate(sectionPrefab, startingPosition + (Vector3.left * i), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprites();
    }

    //add new position
    void AddSection()
    {

        // TODO: Test this
        Vector3 newSectionPosition = sections.Last.Value.transform.position;
        if (cachedDirection == SnakeHeadDirection.Up)
        {
            newSectionPosition.y += 1;
        }
        else if (cachedDirection == SnakeHeadDirection.Down)
        {
            newSectionPosition.y -= 1;
        }
        else if (cachedDirection == SnakeHeadDirection.Left)
        {
            newSectionPosition.x -= 1;
        }
        else
        {
            newSectionPosition.x += 1;
        }
        // We are adding to the head here, so I am pushing it one tile ahead and updating sprites, so player wont see anything funny happening
        sections.AddBefore(sections.Last, Instantiate(sectionPrefab, newSectionPosition, Quaternion.identity));

        // Update sprites is doing all the work of setting the right sprite and its necessary rotation, thanks Sabrina.
        UpdateSprites();
        //update length variable (for game over check)
        length++;
    }

    void MoveSnake(SnakeHeadDirection direction)
    {
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
                break;
            case SnakeHeadDirection.Down:
                cachedDirection = SnakeHeadDirection.Down;
                sections.First.Value.transform.position += Vector3.down;
                break;
            case SnakeHeadDirection.Left:
                cachedDirection = SnakeHeadDirection.Left;
                sections.First.Value.transform.position += Vector3.left;
                break;
            case SnakeHeadDirection.Right:
                cachedDirection = SnakeHeadDirection.Right;
                sections.First.Value.transform.position += Vector3.right;
                break;
        }

        UpdateSprites();
    }

    void TrackFruit()
    {
        //get access to list of fruits
        //store first fruit as closest fruit
        //if closer fruit is found, replace closest fruit
        //once closest fruit is found, move in a direction towards it
        //start all over, checking if there is a closer fruit
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
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
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
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
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
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 90);
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
                        node.Value.transform.rotation = Quaternion.Euler(0, 0, 270);
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
