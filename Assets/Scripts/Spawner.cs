using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private CubeController _cubePrefab;
    private Orientation _orientation;
    private CubeController _instance;
    private Transform _startPoint;
    [SerializeField] private float _spawningOffset;


    private void Start()
    { 
        _orientation = Orientation.Left; 
        SpawnCube(transform.position, GetComponent<MeshRenderer>().bounds);
    }

    public void SpawnCube(Vector3 startPosition, Bounds previousCubeBounds)
    {
        switch (_orientation)
        {
            case Orientation.Left:
                _instance = Instantiate(_cubePrefab, 
                    new Vector3(startPosition.x - _spawningOffset, startPosition.y + 0.5f, startPosition.z),
                    Quaternion.Euler(0, 0, 0));

                _instance.GenerateMesh(previousCubeBounds, startPosition, Orientation.Left, this);
                _orientation = Orientation.Right;
                break;
            case Orientation.Right:
                _instance = Instantiate(_cubePrefab, 
                    new Vector3(startPosition.x,startPosition.y + 0.5f, startPosition.z + _spawningOffset), 
                    Quaternion.Euler(0, 90, 0));

                _instance.GenerateMesh(previousCubeBounds, startPosition, Orientation.Right, this);
                _orientation = Orientation.Left;
                break;
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {

           
        }
    }
}

public enum Orientation
{
    Left,
    Right
}
