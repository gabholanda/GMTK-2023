using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Vector3 startingPosition;
    public int startingLength;
    public GameObject sectionPrefab;
    LinkedList<GameObject> sections;


    // Start is called before the first frame update
    void Start()
    {
        //setting up initial sections
        for (int i = 0; i < startingLength; i++)
        {
            sections.AddLast(Instantiate(sectionPrefab, startingPosition + (Vector3.left * i), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //add new position
    void AddSection(float x, float y)
    {
        //fix position
        sections.AddLast(Instantiate(sectionPrefab, new Vector3(0,0,0), Quaternion.identity));
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
    }

    void TrackFruit()
    {
        //replace with reference to a list of all fruits on screen
        
        //
    }

    void AddSection()
    {

    }
}
