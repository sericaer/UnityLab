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
        defalutLine.positionCount = 0;

        var lineList = new List<List<Vector3>>();

        var rawList = new LinkedList<(Vector3 p1, Vector3 p2)>(lines);
        while (rawList.Count() != 0)
        {
            var first = rawList.First;
            rawList.RemoveFirst();

            var line = new List<Vector3>() { first.Value.p1, first.Value.p2 };
            lineList.Add(line);

            var currNode = rawList.First;
            while (currNode!= null)
            {
                var nextNode = currNode.Next;
                if (currNode.Value.p1 == line.Last())
                {
                    line.Add(currNode.Value.p2);
                    rawList.Remove(currNode);
                }
                else if (currNode.Value.p2 == line.Last())
                {
                    line.Add(currNode.Value.p1);
                    rawList.Remove(currNode);
                }
                currNode = nextNode;
            }
        }

        var test = lineList.First();

        foreach(var p in test)
        {
            defalutLine.positionCount++;
            defalutLine.SetPosition(defalutLine.positionCount - 1, p);
        }
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
