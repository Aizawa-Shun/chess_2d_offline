using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private Color _markerColor;
    [SerializeField] private SpriteRenderer _renderer;

    GameObject clikedSquare;
    Color _previousColor;

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void Update ()
    {
        if (Input.GetMouseButtonUp(1)) 
        {
            int layerMask = 1 << 7;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, layerMask);

            if (hit2d)
                clikedSquare = hit2d.transform.gameObject;
            else
                clikedSquare = null;

            if (clikedSquare == this.gameObject && clikedSquare.GetComponent<Renderer>().material.color != _markerColor)
            {
                _previousColor = clikedSquare.GetComponent<Renderer>().material.color;
                clikedSquare.GetComponent<Renderer>().material.color = _markerColor;
            }
            else if (clikedSquare == this.gameObject && clikedSquare.GetComponent<Renderer>().material.color == _markerColor)
                clikedSquare.GetComponent<Renderer>().material.color = _previousColor;
        }
    }
}