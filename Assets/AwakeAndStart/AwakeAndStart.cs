using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeAndStart : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"{name} AWAKE");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"{name} Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
