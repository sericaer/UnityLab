using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BorderLines : MonoBehaviour
{
    public LineRenderer defalutLine;

    public void SetLines(IEnumerable<(Vector3 p1, Vector3 p2)> lines)
    {


        var lineList = new List<List<Vector3>>();

        var rawList = new LinkedList<(Vector3 p1, Vector3 p2)>(lines);
        while (rawList.Count() != 0)
        {
            var first = rawList.First;
            rawList.RemoveFirst();

            var line = new List<Vector3>() { first.Value.p1, first.Value.p2 };
            lineList.Add(line);


            while(true)
            {
                var lastPoint = line.Last();
                var FindNextNode = FindNextNodeInLine(rawList, lastPoint);
                if(FindNextNode == null)
                {
                    break;
                }

                rawList.Remove(FindNextNode);
                if (FindNextNode.Value.p1 == lastPoint)
                {
                    line.Add(FindNextNode.Value.p2);
                }
                if (FindNextNode.Value.p2 == lastPoint)
                {
                    line.Add(FindNextNode.Value.p1);
                }
            }

        }

        foreach(var line in lineList)
        {
            var newLine = Instantiate<LineRenderer>(defalutLine, defalutLine.transform.parent);
            newLine.positionCount = 0;

            foreach (var pos in line)
            {
                newLine.positionCount++;
                newLine.SetPosition(newLine.positionCount - 1, pos);
            }
 
        }
    }

    private static LinkedListNode<(Vector3 p1, Vector3 p2)> FindNextNodeInLine(LinkedList<(Vector3 p1, Vector3 p2)> rawList, Vector3 last)
    {
        var currNode = rawList.First;
        while (currNode != null)
        {
            if (currNode.Value.p1 == last || currNode.Value.p2 == last)
            {
                break;
            }

            var nextNode = currNode.Next;
            currNode = nextNode;
        }

        return currNode;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
