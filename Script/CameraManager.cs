using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public float _cameraSpeed = 6f;

    private float _scroll;

    public GameObject _camera;

    private GameObject came = null;
    private Camera cs;

    private bool up = false;
    
    private bool mv = false;

    public GameObject display;


    void Start() 
    {
        _camera = Camera.main.gameObject;
    }

    void Update() 
    {
        if (mv == true)
        {
            var nowCameraPos = _camera.transform.position;

            _scroll = Input.GetAxis("Mouse ScrollWheel");

            nowCameraPos.y = Mathf.Clamp(nowCameraPos.y, 4.0f, 11.0f); 

            nowCameraPos.y += _scroll * _cameraSpeed;

            _camera.transform.position = nowCameraPos;
        }

        if (Input.GetKeyDown(KeyCode.Space) && up == false)
        {
            came = GameObject.Find ("Main Camera");
            cs = came.GetComponent<Camera>();
            cs.orthographicSize = 4.5f;
            up = true;
            mv = true;
            display.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && up == true)
        {
            came = GameObject.Find ("Main Camera");
            cs = came.GetComponent<Camera>();
            cs.orthographicSize = 8.5f;
            cs.transform.position = new Vector3(3.5f, 7.5f, -10);
            up = false;
            mv = false;
            display.gameObject.SetActive(true);
        }
    }
}  
