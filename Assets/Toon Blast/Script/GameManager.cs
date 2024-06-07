using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] defaultCubes;
    [SerializeField] private Transform cube_Parent;
    [SerializeField] private Transform cubeBG_Parent;
    [SerializeField] private float clickTimeInterval;
    private float clickTimeIntervalSet;

    private static List<GameObject> cubesToDelete = new List<GameObject>();

    public static Dictionary<Tuple<int,int>,PickUp> Items = new Dictionary<Tuple<int, int>, PickUp>();
    public static Dictionary<Tuple<int, int>, List<GameObject>> Squares = new Dictionary<Tuple<int, int>, List<GameObject>>();
    public int Row, Column;
    public int RandomColorCount;
    public int PaddingTop;
    public static bool IsClick;
    public AudioClip BreakCubeSound, DiscoBallSound, RocketSound, BombSound;
    public AudioSource InGameAudioSource;

    private void Awake()
    {
        cube_Parent = GameObject.FindGameObjectWithTag("CubeParent").transform;
        clickTimeIntervalSet = clickTimeInterval;
        setupCube();
        StartCoroutine(Iwait(0.1f, () => { startCreateSquares(); }));
    }
    private void Update()
    {
        if (IsClick)
        {
            clickTimeInterval -=Time.deltaTime;
            if(clickTimeInterval <= 0)
            {
                clickTimeInterval = clickTimeIntervalSet;
                IsClick = false;
                //cubeDefaultLayer();
            }
        }
    }
    private void startCreateSquares()
    {
        foreach(var item in Items.Values)
        {
            if (!item.GetComponent<IDName>().IsSpecialCube)
            {
                Squares.Add(new Tuple<int, int>(item.X, item.Y), new List<GameObject>());
                CalculateCubeNeighbour(Items[new Tuple<int, int>(item.X, item.Y)],
                                        Items[new Tuple<int, int>(item.X, item.Y)].GetComponent<IDName>(),
                                        new Tuple<int, int>(item.X, item.Y));
            }
            
           
        } 
        foreach (var item in Squares.Values) //Calculation for changing sprites
        {
            if(item.Count == 1 || item.Count == 2 || item.Count == 3 || item.Count == 4)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].GetComponent<IDName>().cubeType = IDName.CubeTypeEnum.Default;
                }
            }
            else if (item.Count == 5 || item.Count == 6 || item.Count == 7 )
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].GetComponent<IDName>().cubeType = IDName.CubeTypeEnum.Rocket;
                }
            }
            else if (item.Count == 8 || item.Count == 9 || item.Count == 10)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].GetComponent<IDName>().cubeType = IDName.CubeTypeEnum.Bomb;
                }
            }
            else if (item.Count > 10)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].GetComponent<IDName>().cubeType = IDName.CubeTypeEnum.Disco;
                }
            }
        } //Calculating for changing sprites
        Squares.Clear();
    }
    private IEnumerator Iwait(float time, Action Call)
    {
        yield return new WaitForSeconds(time);
        if(Call != null)
        {
            Call.Invoke();
        }
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
                cube.GetComponent<IDName>().IsChangeSprite = true;
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
    //public static void cubeRaycastBlockLayer()
    //{
    //    for (int i = 0; i < cube_Parent.childCount; i++)
    //    {
    //        cube_Parent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    //    }
    //}
    //public static void cubeDefaultLayer()
    //{
    //    for (int i = 0; i < cube_Parent.childCount; i++)
    //    {
    //        cube_Parent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Default");
    //    }
    //}
    private void fallDown_Cube(PickUp p)
    {
        //cubeRaycastBlockLayer();
        var cube = Instantiate(defaultCubes[UnityEngine.Random.Range(0, RandomColorCount)],
                new Vector2(p.X, p.Y + PaddingTop), Quaternion.identity);
        cube.transform.SetParent(cube_Parent);
        cube.AddComponent<CapsuleCollider2D>(); //Return to Collider
        cube.tag = "Cube";
        cube.AddComponent<PickUp>();
        cube.GetComponent<IDName>().IsChangeSprite = true; 
    }
    public static void CalculateCubeNeighbour(PickUp p, IDName i, Tuple<int, int> id)
    {
        var top = new Tuple<int, int>(p.X, p.Y + 1);
        var bot = new Tuple<int, int>(p.X, p.Y - 1);
        var right = new Tuple<int, int>(p.X + 1, p.Y);
        var left = new Tuple<int, int>(p.X - 1, p.Y);

        if (Items.ContainsKey(top)) //Break Top
        {
            if (i.ID == Items[top].GetComponent<IDName>().ID)
            {
                if (!Squares[id].Contains(i.gameObject) && !i.IsSpecialCube)
                    Squares[id].Add(i.gameObject);

                if (!Squares[id].Contains(Items[top].gameObject) && !Items[top].GetComponent<IDName>().IsSpecialCube)
                {
                    Squares[id].Add(Items[top].gameObject);
                    Items[top].ContinueCalculate_Callback(id);
                }
            }
        }

        if (Items.ContainsKey(bot)) //Break Bot
        {
            if (i.ID == Items[bot].GetComponent<IDName>().ID && !i.IsSpecialCube)
            {
                if (!Squares[id].Contains(i.gameObject))
                    Squares[id].Add(i.gameObject);

                if (!Squares[id].Contains(Items[bot].gameObject) && !Items[bot].GetComponent<IDName>().IsSpecialCube)
                {
                    Squares[id].Add(Items[bot].gameObject);
                    Items[bot].ContinueCalculate_Callback(id);
                }
            }
        }

        if (Items.ContainsKey(left)) //Break Left
        {
            if (i.ID == Items[left].GetComponent<IDName>().ID && !i.IsSpecialCube)
            {
                if (!Squares[id].Contains(i.gameObject))
                    Squares[id].Add(i.gameObject);

                if (!Squares[id].Contains(Items[left].gameObject) && !Items[left].GetComponent<IDName>().IsSpecialCube)
                {
                    Squares[id].Add(Items[left].gameObject);
                    Items[left].ContinueCalculate_Callback(id);
                }
            }
        }

        if (Items.ContainsKey(right)) //Break Right
        {
            if (i.ID == Items[right].GetComponent<IDName>().ID && !i.IsSpecialCube)
            {
                if (!Squares[id].Contains(i.gameObject))
                    Squares[id].Add(i.gameObject);

                if (!Squares[id].Contains(Items[right].gameObject) && !Items[right].GetComponent<IDName>().IsSpecialCube)
                {
                    Squares[id].Add(Items[right].gameObject);
                    Items[right].ContinueCalculate_Callback(id);
                }
            }
        }

        //Debug.Log("cubesToDelete.Count" + cubesToDelete.Count);
    }
    public static void CalculateCubeNeighbour(PickUp p, IDName i)
    {
        var top = new Tuple<int, int>(p.X, p.Y + 1);
        var bot = new Tuple<int, int>(p.X, p.Y - 1);
        var right = new Tuple<int, int>(p.X + 1, p.Y);
        var left = new Tuple<int, int>(p.X - 1, p.Y);

        if (Items.ContainsKey(top)) //Break Top
        {
            if (!Items[top].GetComponent<IDName>().IsSpecialCube)

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
            if (!Items[bot].GetComponent<IDName>().IsSpecialCube)

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
            if (!Items[left].GetComponent<IDName>().IsSpecialCube)

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
            if (!Items[right].GetComponent<IDName>().IsSpecialCube)

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
    public static bool CalculateCubeNeighbour(PickUp p)
    {
        var top = new Tuple<int, int>(p.X, p.Y + 1);
        var bot = new Tuple<int, int>(p.X, p.Y - 1);
        var right = new Tuple<int, int>(p.X + 1, p.Y);
        var left = new Tuple<int, int>(p.X - 1, p.Y);

        if (Items.ContainsKey(top)) //Break Top
        {
            if (!Items[top].GetComponent<IDName>().IsSpecialCube)

                if (p.GetComponent<IDName>().ID == Items[top].GetComponent<IDName>().ID)
                {
                    return true;
                }
        }

        if (Items.ContainsKey(bot)) //Break Bot
        {
            if (!Items[bot].GetComponent<IDName>().IsSpecialCube)

                if (p.GetComponent<IDName>().ID == Items[bot].GetComponent<IDName>().ID)
                {
                    return true;
                }
        }

        if (Items.ContainsKey(left)) //Break Left
        {
            if (!Items[left].GetComponent<IDName>().IsSpecialCube)

                if (p.GetComponent<IDName>().ID == Items[left].GetComponent<IDName>().ID)
                {
                    return true;
                }
        }

        if (Items.ContainsKey(right)) //Break Right
        {
            if (!Items[right].GetComponent<IDName>().IsSpecialCube)

                if (p.GetComponent<IDName>().ID == Items[right].GetComponent<IDName>().ID)
                {
                    return true;
                }
        }
        return false;

        //Debug.Log("cubesToDelete.Count" + cubesToDelete.Count);
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
            if (i == 0) //First Element Position that we cliked first
            {
                if (cubesToDelete[0].GetComponent<IDName>().cubeType == IDName.CubeTypeEnum.Disco) //Working for Disco ball spawning
                {
                    cubesToDelete[0].GetComponent<IDName>().IsChangeSprite = false;
                    cubesToDelete[0].GetComponent<SpriteRenderer>().sprite =
                        cubesToDelete[0].GetComponent<IDName>().Disco;
                    cubesToDelete[0].GetComponent<IDName>().IsSpecialCube = true;
                    cubesToDelete[0].GetComponent<IDName>().IsDiscoBall = true;

                    //Sound Effect
                    InGameAudioSource.clip = DiscoBallSound;
                    InGameAudioSource.Play();
                }

                else if (cubesToDelete[0].GetComponent<IDName>().cubeType == IDName.CubeTypeEnum.Rocket) //Working for Rocket spawning
                {
                    cubesToDelete[0].GetComponent<IDName>().IsChangeSprite = false;
                    int randomRocket = UnityEngine.Random.Range(0, cubesToDelete[0].GetComponent<IDName>().Rocket.Length);
                    if(randomRocket == 0)
                    {
                        cubesToDelete[0].GetComponent <IDName>().IsHorizontalRocket = true;
                    }
                    if (randomRocket == 1)
                    {
                        cubesToDelete[0].GetComponent<IDName>().IsVerticalRocket = true;
                    }
                    cubesToDelete[0].GetComponent<SpriteRenderer>().sprite =
                        cubesToDelete[0].GetComponent<IDName>().Rocket[randomRocket];
                    cubesToDelete[0].GetComponent<IDName>().IsSpecialCube = true;

                    //Sound Effect
                    InGameAudioSource.clip = RocketSound;
                    InGameAudioSource.Play();
                }

                else if (cubesToDelete[0].GetComponent<IDName>().cubeType == IDName.CubeTypeEnum.Bomb) //Working for Bomb spawning
                {
                    cubesToDelete[0].GetComponent<IDName>().IsChangeSprite = false;
                    cubesToDelete[0].GetComponent<SpriteRenderer>().sprite =
                        cubesToDelete[0].GetComponent<IDName>().Bomb;
                    cubesToDelete[0].GetComponent<IDName>().IsSpecialCube = true;
                    cubesToDelete[0].GetComponent<IDName>().IsBomb = true;

                    //Sound Effect
                    InGameAudioSource.clip = BombSound;
                    InGameAudioSource.Play();
                }

                else
                {
                    fallDown_Cube(cubesToDelete[i].GetComponent<PickUp>());
                    Destroy(cubesToDelete[i]);

                    //Sound Effect
                    InGameAudioSource.clip = BreakCubeSound;
                    InGameAudioSource.Play();
                }
            }
            else 
            {
                fallDown_Cube(cubesToDelete[i].GetComponent<PickUp>());
                Destroy(cubesToDelete[i]);
            }
            
        }
        
        Items.Clear();
        cubesToDelete.Clear();
        //cubeDefaultLayer();

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
        StartCoroutine(Iwait(0.1f, () => 
        { 
            startCreateSquares();
            foreach(var i in Items.Values)
            {
                if (!CalculateCubeNeighbour(i))
                {
                    i.GetComponent<IDName>().cubeType = IDName.CubeTypeEnum.Default;
                }
            }
        }));
    }

    public void DiscoBallEffect(IDName i)
    {
        foreach(var item in Items.Values)
        {
            if (item.GetComponent<IDName>().ID == i.ID)
            {
                cubesToDelete.Add(item.gameObject);
            }
            DeleteCubes_Callback();
        }
    }
}
