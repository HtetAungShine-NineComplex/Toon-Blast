using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] defaultCubes;
    [SerializeField] private Transform cubeParent_TF;

    public int Row, Column;

    private void Awake()
    {
        SpawnGrid();
    }

    private void Start()
    {
        
    }

    private void SpawnGrid()
    {
        //Create Cube
        for (int x = 0; x < Row; x++)
        {
            for(int y = 0; y < Column; y++)
            {
                GameObject cube = Instantiate(defaultCubes[Random.Range(0, defaultCubes.Length)],
                new Vector2 (x,y),Quaternion.identity);
                cube.transform.parent = cubeParent_TF;
            }
        }
    }
}
