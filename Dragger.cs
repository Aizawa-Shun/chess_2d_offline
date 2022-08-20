using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    public GameObject movePlate;

    private Vector3 _dragOffset; 

    private Vector2 prevPos;

    public bool setPiece;
    public bool set;

    void OnMouseDown()
    {
        prevPos = this.transform.position;

        _dragOffset = this.transform.position - GetMousePos();
    }

    void OnMouseDrag() 
    {
        this.transform.position = GetMousePos() + _dragOffset;
    }

    Vector3 GetMousePos()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    void OnMouseUp()
    {
        if (setPiece == false)
        {
            this.transform.position = prevPos;
            set = false;
        }
        else
            set = true;
    }
}