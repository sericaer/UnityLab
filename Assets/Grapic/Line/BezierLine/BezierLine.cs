using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierLine : MonoBehaviour
{
    public LineRenderer line;

    public List<Vector3> vectors;

    public int numberOfPoints = 20;

    public GameObject point;

    // Start is called before the first frame update
    void Start()
    {

        var points = new Queue<GameObject>(Enumerable.Range(0, vectors.Count() - 1)
            .Select(_ => Instantiate(point, point.transform.parent))
            .Append(point));

        foreach (var p in vectors)
        {
            var point = points.Dequeue();
            point.transform.position = p;
        }

        //foreach (var p in vectors)
        //{
        //    line.positionCount++;
        //    line.SetPosition(line.positionCount - 1, p);
        //}

        line.positionCount = numberOfPoints;

        Vector3 p0 = vectors[0];
        Vector3 p1 = vectors[1];
        Vector3 p2 = vectors[2];
        for (int i = 0; i < numberOfPoints; i++)
        {
            var t = i / (numberOfPoints - 1.0f);
            var position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
            line.SetPosition(i, position);
        }
    }

}
