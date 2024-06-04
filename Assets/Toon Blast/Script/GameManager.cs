using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] defaultCubes;
    [SerializeField] private Transform cube_Parent;
    [SerializeField] private Transform cubeBG_Parent;

    public static Dictionary<Tuple<int,int>,PickUp> Item = new Dictionary<Tuple<int, int>, PickUp>();
    public int Row, Column;

    private void Awake()
    {
        setupCube();
    }

    private void Start()
    {
        
    }

    private void setupCube()
    {
        //Create Cube
        for (int x = 0; x < Row; x++)
        {
            for(int y = 0; y < Column; y++)
            {
                GameObject cube = Instantiate(defaultCubes[UnityEngine.Random.Range(0, defaultCubes.Length)],
                new Vector2 (x,y),Quaternion.identity);
                cube.transform.parent = cube_Parent;
                cube.AddComponent<BoxCollider2D>(); //Return to Collider
                cube.tag = "Cube";
                cube.AddComponent<PickUp>();
                cube.name = "(" + x.ToString() + ", " + y.ToString() + ")";
                cube.GetComponent<PickUp>().X = x;
                cube.GetComponent <PickUp>().Y = y;
                Item.Add(new Tuple<int, int>(x, y), cube.GetComponent<PickUp>());
            }
        }

        //Setup Cube BG
        for (int x = 0; x < Row; x++)
        {
            for (int y = 0; y < Column; y++)
            {
                GameObject cubeBG = Instantiate(defaultCubes[UnityEngine.Random.Range(0, defaultCubes.Length)],
                new Vector2(x, y), Quaternion.identity);
                cubeBG.transform.parent = cubeBG_Parent;
                Destroy(cubeBG.gameObject.GetComponent<SpriteRenderer>());
                cubeBG.AddComponent<BoxCollider2D>();
                cubeBG.GetComponent<BoxCollider2D>().isTrigger = true; // Return to Trigger Collision
                cubeBG.AddComponent<Change>();
                cubeBG.name = "(" + x.ToString() + ", " + y.ToString() + ")";
                cubeBG.GetComponent<Change>().X = x;
                cubeBG.GetComponent<Change>().Y = y;
                cubeBG.GetComponent<Rigidbody2D>().gravityScale = 0f;
                cubeBG.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 0.3f);
            }
        }

        //Setup Cube BG Pos
        Invoke("changeCubeBG_Pos", 1f);
    }

    private void changeCubeBG_Pos()
    {
        for(int i = 0; i < cubeBG_Parent.childCount; i++)
        {
            var change = cubeBG_Parent.GetChild(i).GetComponent<Change>();
            cubeBG_Parent.GetChild(i).transform.position =
                Item[new Tuple<int, int>(change.X,change.Y)].transform.position;
            cubeBG_Parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

        }
    }
}
