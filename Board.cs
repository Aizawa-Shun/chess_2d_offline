using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _file, _rank;
    [SerializeField] private Square _squarePrefab;
    [SerializeField] private Transform _cam;

    string[] file = {"a", "b", "c", "d", "e", "f", "g", "h"};

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _file; x++) {
            for (int y = 0; y < _rank; y++) {
                var spawnedSquare = Instantiate(_squarePrefab, new Vector3(x, y, 10), Quaternion.identity);
                spawnedSquare.name = $"{file[x]} {y + 1}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedSquare.Init(isOffset);
            }
        }

        _cam.transform.position = new Vector3((float)_file / 2 - 0.5f, (float)_rank / 2 - 0.5f, -10);
    }

}
