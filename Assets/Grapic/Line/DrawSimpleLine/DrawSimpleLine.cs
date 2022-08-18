using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSimpleLine : MonoBehaviour
{
    public LineRenderer line;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f));

            line.positionCount++;
            line.SetPosition(line.positionCount-1, pos);

            Debug.Log(pos);
        }
    }
}
