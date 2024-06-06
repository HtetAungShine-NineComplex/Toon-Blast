using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int X, Y; //RowNumber and ColumnNumber for Cube

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GameManager.CalculateCubeNeighbour_CallBack(GetComponent<PickUp>(), GetComponent<IDName>());
    //        FindObjectOfType<GameManager>().DeleteCubes_Callback();
    //    }
    //}
    private void OnMouseDown()
    {
        GameManager.CalculateCubeNeighbour(GetComponent<PickUp>(), GetComponent<IDName>());
        FindObjectOfType<GameManager>().DeleteCubes_Callback();
        Debug.Log(this.gameObject.name);       
    }

    public void ContinueCalculate_Callback()
    {
        GameManager.CalculateCubeNeighbour(GetComponent<PickUp>(), GetComponent<IDName>());
    }
}
