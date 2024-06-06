using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] defaultCubes;
    [SerializeField] private Transform cube_Parent;
    [SerializeField] private Transform cubeBG_Parent;

    private static List<GameObject> cubesToDelete = new List<GameObject>();

    public static Dictionary<Tuple<int,int>,PickUp> Items = new Dictionary<Tuple<int, int>, PickUp>();
    public int Row, Column;
    public int RandomColorCount;
    public int PaddingTop;

    private void Awake()
    {
        setupCube();
    }

    private void setupCube()
    {
        //Create Cube
        for (int x = 0; x < Row; x++) //Generate Horizontal Vector2 Axis
        {
            for(int y = 0; y < Column; y++) // Generate Vertical Vector2 Axis
            {
                var cube = Instantiate(defaultCubes[UnityEngine.Random.Range(0, RandomColorCount)],
                new Vector2 (x,y),Quaternion.identity);
                cube.transform.SetParent(cube_Parent);
                cube.AddComponent<CapsuleCollider2D>(); //Return to Collider
                cube.tag = "Cube";
                cube.AddComponent<PickUp>();
                cube.name = "(" + x.ToString() + ", " + y.ToString() + ")";
                cube.GetComponent<PickUp>().X = x;
                cube.GetComponent <PickUp>().Y = y;
                Items.Add(new Tuple<int, int>(x, y), cube.GetComponent<PickUp>());
            }
        }

        //Setup Cube BG
        for (int x = 0; x < Row; x++)
        {
            
            for (int y = 0; y < Column; y++)
            {
                var cubeBG = Instantiate(defaultCubes[UnityEngine.Random.Range(0, defaultCubes.Length)],
                new Vector2(x, y), Quaternion.identity);
                cubeBG.transform.SetParent(cubeBG_Parent);
                Destroy(cubeBG.GetComponent<SpriteRenderer>());
                cubeBG.AddComponent<BoxCollider2D>();
                cubeBG.GetComponent<BoxCollider2D>().isTrigger = true; // Return to Trigger Collision
                cubeBG.AddComponent<Change>();
                cubeBG.name = "(" + x.ToString() + ", " + y.ToString() + ")";
                cubeBG.GetComponent<Change>().X = x;
                cubeBG.GetComponent<Change>().Y = y;
                cubeBG.GetComponent<Rigidbody2D>().gravityScale = 0f;
                cubeBG.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 0.3f);
                cubeBG.GetComponent<BoxCollider2D>().enabled = false;
            }
            
            //isClicked = true;
        }

        //Setup Cube BG Pos
        Invoke("changeCubeBG_Pos", 1f);
    }

    private void changeCubeBG_Pos() //Called from setupCube Callback
    {
        for (int i = 0; i < cubeBG_Parent.childCount; i++)
        {
            
            var change = cubeBG_Parent.GetChild(i).GetComponent<Change>();
            cubeBG_Parent.GetChild(i).transform.position =
                Items[new Tuple<int, int>(change.X, change.Y)].transform.position;
            //cubeBG_Parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            
        }
    }

    private void cubeRaycastBlockLayer()
    {
        for (int i = 0; i < cube_Parent.childCount; i++)
        {
            cube_Parent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }
    private void cubeDefaultLayer()
    {
        for (int i = 0; i < cube_Parent.childCount; i++)
        {
            cube_Parent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    private void fallDown_Cube(PickUp p)
    {
        cubeRaycastBlockLayer();
        var cube = Instantiate(defaultCubes[UnityEngine.Random.Range(0, RandomColorCount)],
                new Vector2(p.X, p.Y + PaddingTop), Quaternion.identity);
        cube.transform.SetParent(cube_Parent);
        cube.AddComponent<CapsuleCollider2D>(); //Return to Collider
        cube.tag = "Cube";
        cube.AddComponent<PickUp>();
        

    }
    public static void CalculateCubeNeighbour(PickUp p, IDName i)
    {
        var top = new Tuple<int, int>(p.X, p.Y + 1);
        var bot = new Tuple<int, int>(p.X, p.Y - 1);
        var right = new Tuple<int, int>(p.X + 1, p.Y);
        var left = new Tuple<int, int>(p.X - 1, p.Y);

        if (Items.ContainsKey(top)) //Break Top
        {
            if (i.ID == Items[top].GetComponent<IDName>().ID)
            {
               if(!cubesToDelete.Contains(i.gameObject))
                    cubesToDelete.Add(i.gameObject);
                if (!cubesToDelete.Contains(Items[top].gameObject))
                {
                    cubesToDelete.Add(Items[top].gameObject);
                    Items[top].ContinueCalculate_Callback();
                }
            }
        }

        if (Items.ContainsKey(bot)) //Break Bot
        {
            if (i.ID == Items[bot].GetComponent<IDName>().ID)
            {
                if (!cubesToDelete.Contains(i.gameObject))
                    cubesToDelete.Add(i.gameObject);
                if (!cubesToDelete.Contains(Items[bot].gameObject))
                {
                    cubesToDelete.Add(Items[bot].gameObject);
                    Items[bot].ContinueCalculate_Callback();
                }
            }
        }

        if (Items.ContainsKey(left)) //Break Left
        {
            if (i.ID == Items[left].GetComponent<IDName>().ID)
            {
                if (!cubesToDelete.Contains(i.gameObject))
                    cubesToDelete.Add(i.gameObject);
                if (!cubesToDelete.Contains(Items[left].gameObject))
                {
                    cubesToDelete.Add(Items[left].gameObject);
                    Items[left].ContinueCalculate_Callback();
                }
            }
        }

        if (Items.ContainsKey(right)) //Break Right
        {
            if (i.ID == Items[right].GetComponent<IDName>().ID)
            {
                if (!cubesToDelete.Contains(i.gameObject))
                    cubesToDelete.Add(i.gameObject);
                if (!cubesToDelete.Contains(Items[right].gameObject))
                {
                    cubesToDelete.Add(Items[right].gameObject);
                    Items[right].ContinueCalculate_Callback();
                }
            }
        }

        Debug.Log("cubesToDelete.Count"+cubesToDelete.Count);
    }

    public void DeleteCubes_Callback()
    {
        Invoke("clearCubesToDelete", 0.1f);
        //Setup New Cubes
        Invoke("enable2dCollider_Callback", 1f);
    }

    private void clearCubesToDelete() //Called from CallBack
    {
        for (int i = 0; i < cubesToDelete.Count; i++)
        {
            fallDown_Cube(cubesToDelete[i].GetComponent<PickUp>());
            Destroy(cubesToDelete[i]);
        }
        cubeDefaultLayer();
        Items.Clear();
        cubesToDelete.Clear();
        
    }

    private void enable2dCollider_Callback() //Called from Callback
    {
        for(int i = 0;i < cubeBG_Parent.childCount;i++)
        {
            cubeBG_Parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
        }
        Invoke("disable2dCollider_Callback", 0.1f);
    }

    private void disable2dCollider_Callback()
    {
        for (int i = 0; i < cubeBG_Parent.childCount; i++)
        {
            cubeBG_Parent.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
