using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnAreas = new List<GameObject>();

    [SerializeField] private GameObject node;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            Spawn();
    }

    public void Spawn()
    {
        foreach (var spawnArea in spawnAreas)
        {
            if(spawnArea.transform.childCount > 0) continue;
            var spawnedNode = Instantiate(node, Vector3.zero, Quaternion.identity);
            spawnedNode.transform.SetParent(spawnArea.transform);
            spawnedNode.GetComponent<Node>().number = 2;
            break;
        }
    }
}
