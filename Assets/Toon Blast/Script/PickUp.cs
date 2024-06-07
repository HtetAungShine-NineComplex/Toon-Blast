using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int X, Y; //RowNumber and ColumnNumber for Cube

    //Calculate #1
    private void OnMouseDown()
    {
        if (!GameManager.IsClick)
        {
            if (GetComponent<IDName>().IsSpecialCube) // True => Special Cube
            {
                //GameManager.cubeRaycastBlockLayer();
                GameManager.IsClick = true;
                var i = GetComponent<IDName>();
                if(i.IsDiscoBall && !i.IsBomb && !i.IsHorizontalRocket && !i.IsVerticalRocket)
                {
                    FindObjectOfType<GameManager>().DiscoBallEffect(i);
                }
                if (!i.IsDiscoBall && i.IsBomb && !i.IsHorizontalRocket && !i.IsVerticalRocket)
                {
                    Debug.Log("This is bomb");
                }
                if (!i.IsDiscoBall && !i.IsBomb && i.IsHorizontalRocket && !i.IsVerticalRocket)
                {
                    Debug.Log("This is Horizontal Rocket");
                }
                if (!i.IsDiscoBall && !i.IsBomb && !i.IsHorizontalRocket && i.IsVerticalRocket)
                {
                    Debug.Log("This is Vertical Rocket");
                }

            }
            else
            {
                GameManager.CalculateCubeNeighbour(GetComponent<PickUp>(), GetComponent<IDName>());
                FindObjectOfType<GameManager>().DeleteCubes_Callback();
                //GameManager.cubeRaycastBlockLayer();
                GameManager.IsClick = true;
                
                Debug.Log(this.gameObject.name);
            }
        }
                  
    }

    //Calculate #2 //Break Cube
    public void ContinueCalculate_Callback()
    {
        GameManager.CalculateCubeNeighbour(GetComponent<PickUp>(), GetComponent<IDName>());
    }

    //Calculate #3 //ChangeSprites
    public void ContinueCalculate_Callback(Tuple<int,int> id )
    {
        GameManager.CalculateCubeNeighbour(GetComponent<PickUp>(), GetComponent<IDName>(), id);
    }
}
