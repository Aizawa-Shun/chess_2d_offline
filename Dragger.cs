using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    [SerializeField] private GameObject movePlate;

    private Vector3 _dragOffset;
    private Vector2 prevPos;

    public bool setPiece = false;
    public bool set = false;

    private void OnMouseDown()
    {
        prevPos = this.transform.position;
        _dragOffset = this.transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        this.transform.position = GetMousePos() + _dragOffset;
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    private void OnMouseUp()
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
