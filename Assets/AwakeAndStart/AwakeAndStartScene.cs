using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AwakeAndStartScene : MonoBehaviour
{
    public GameObject prefabs;

    private GameObject curr;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnTimer());
    }

    public void OnChangeScene()
    {
        SceneManager.LoadScene(nameof(TriggerScene));
    }

    private IEnumerator OnTimer()
    {
        curr = Instantiate(prefabs, this.transform);

        yield return new WaitForSeconds(5);

        Destroy(curr);

        yield return new WaitForSeconds(5);

        StartCoroutine(OnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
