using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDName : MonoBehaviour
{
    public int ID;
    public enum CubeTypeEnum { Default, Rocket, Bomb, Disco}
    public CubeTypeEnum cubeType;
    public Sprite[] CubeSpriteTypes;
    public Sprite Bomb, Disco;
    public Sprite[] Rocket;
    [HideInInspector]public bool IsDiscoBall;
    [HideInInspector]public bool IsBomb;
    [HideInInspector]public bool IsHorizontalRocket, IsVerticalRocket;
    [HideInInspector]public bool IsChangeSprite;
    [HideInInspector]public bool IsSpecialCube = false;

    private void Update()
    {
        if(IsChangeSprite)
        {
            switch (cubeType)
            {
                case CubeTypeEnum.Default:
                    gameObject.GetComponent<SpriteRenderer>().sprite = CubeSpriteTypes[0];
                    break;
                case CubeTypeEnum.Rocket:
                    gameObject.GetComponent<SpriteRenderer>().sprite = CubeSpriteTypes[1];
                    break;
                case CubeTypeEnum.Bomb:
                    gameObject.GetComponent<SpriteRenderer>().sprite = CubeSpriteTypes[2];
                    break;
                case CubeTypeEnum.Disco:
                    gameObject.GetComponent<SpriteRenderer>().sprite = CubeSpriteTypes[3];
                    break;
                default:
                    break;
            }
        }       
    }
}
