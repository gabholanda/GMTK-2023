using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Vector3 startingPosition;
    public int length;
    public GameObject sectionPrefab;
    LinkedList<GameObject> sections = new LinkedList<GameObject>();

    //sprites
    public Sprite head;
    public Sprite tail;
    public Sprite straight;
    public Sprite turn;


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
    void AddSection(float x, float y)
    {
        //fix position
        sections.AddLast(Instantiate(sectionPrefab, new Vector3(0,0,0), Quaternion.identity));

        //update length variable (for game over check)
        length++;
    }

    void MoveSnake(string direction)
    {
        //moving all other elements
        LinkedListNode<GameObject> node = sections.Last;
        while (node != sections.First)
        {
            node.Value.transform.position = node.Previous.Value.transform.position;
            node = node.Previous;
        }

        //moving head
        switch (direction)
        {
            case "up":
                sections.First.Value.transform.position += Vector3.up;
                break;
            case "down":
                sections.First.Value.transform.position += Vector3.down;
                break;
            case "left":
                sections.First.Value.transform.position += Vector3.left;
                break;
            case "right":
                sections.First.Value.transform.position += Vector3.right;
                break;
        }

        UpdateSprites();
    }

    void TrackFruit()
    {
        
    }

    void UpdateSprites()
    {
        LinkedListNode<GameObject> node = sections.Last;
        while (node != null)
        {
            //if node is head
            if(node == sections.First)
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
                    node.Value.GetComponent<SpriteRenderer>().sprite = turn;
                    if (node.Next.Value.transform.position == node.Value.transform.position + Vector3.right)
                    {
                        node.Value.transform.Rotate(new Vector3(0, 0, 0));
                    }
                    //next node is left of current node
                    //next node is below current node
                }
                //previous node is below current node
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.down)
                {
                    //next node is right of current node
                    //next node is left of current node
                    //next node is above current node
                }
                //previous node is right of current node
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.right)
                {
                    //next node is above current node
                    //next node is left of current node
                    //next node is below current node
                }
                //previous node is left of current node
                if (node.Previous.Value.transform.position == node.Value.transform.position + Vector3.left)
                {
                    //next node is right of current node
                    //next node is above current node
                    //next node is below current node
                }
            }
            
            node = node.Previous;
        }
    }
}
