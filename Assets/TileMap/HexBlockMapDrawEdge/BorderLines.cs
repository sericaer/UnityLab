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

        defalutLine.positionCount++;
        defalutLine.SetPosition(defalutLine.positionCount - 1, lines.First().p1);

        defalutLine.positionCount++;
        defalutLine.SetPosition(defalutLine.positionCount - 1, lines.Last().p2);
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
