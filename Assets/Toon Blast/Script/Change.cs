using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Change : MonoBehaviour
{
    public int X,Y; //RowNumber and ColumnNumber for CubeBG

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            collision.gameObject.name = "(" + X.ToString() + ", " + Y.ToString() + ")";
            collision.gameObject.GetComponent<PickUp>().X = X;
            collision.gameObject.GetComponent<PickUp>().Y = Y;

            if (!GameManager.Items.ContainsKey(new Tuple<int, int>(X, Y)))
                GameManager.Items.Add(new Tuple<int, int>(X, Y), collision.gameObject.GetComponent<PickUp>());
        }
    }
}
