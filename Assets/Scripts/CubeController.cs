using UnityEngine;
using UnityEngine.Rendering;

public class CubeController : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshFilter _slicedCube;
    static readonly float CubeHeight = 1f;
    [SerializeField] private Bounds _previousCubeBounds;
    private Vector3 _previousCubeCenter;
    private Orientation _cubeOrientation;
    private float _movingDistance;
    private Vector3 _startPosition;
    private float _startTime;
    private Spawner _spawner;

    private void Awake()
    {
        _startTime = Time.time;
    }


    public void GenerateMesh(Bounds previousBounds, Vector3 previousCenter, Orientation orientation, Spawner spawner)
    {
        _previousCubeBounds = previousBounds;
        _previousCubeCenter = previousCenter;
        _cubeOrientation = orientation;
        _spawner = spawner;
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _startPosition = transform.position;

        switch (orientation)
        {
            case Orientation.Left:
                _mesh.vertices = GenerateCubeVertices(
                    previousBounds.size.x / 2,
                    previousBounds.size.x / 2,
                    previousBounds.size.z / 2);
                _movingDistance = Mathf.Abs(transform.position.x - _previousCubeCenter.x) * 2;
                break;
            case Orientation.Right:
                _mesh.vertices = GenerateCubeVertices(
                    previousBounds.size.z / 2,
                    previousBounds.size.z / 2,
                    previousBounds.size.x / 2);
                _movingDistance = Mathf.Abs(transform.position.z - _previousCubeCenter.z) * 2;
                break;
        }

        _mesh.triangles = GenerateTriangles();
        _mesh.RecalculateNormals(MeshUpdateFlags.Default);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (_cubeOrientation)
            {
                case Orientation.Left:
                    if (transform.position.x < _previousCubeCenter.x - _previousCubeBounds.size.x ||
                        transform.position.x > _previousCubeCenter.x + _previousCubeBounds.size.x)
                    {
                        StopGame();
                        return;
                    }

                    StopLeftCube();
                    break;

                case Orientation.Right:
                    if (transform.position.z > _previousCubeCenter.z + _previousCubeBounds.size.z ||
                        transform.position.z < _previousCubeCenter.z - _previousCubeBounds.size.z)
                    {
                        StopGame();
                        return;
                    }

                    StopRightCube();
                    break;
            }

            _mesh.RecalculateBounds();
            _spawner.SpawnCube(transform.TransformPoint(_mesh.bounds.center), _meshRenderer.bounds);
            enabled = false;
            return;
        }

        MoveCube();
    }

    private void StopGame()
    {
        Destroy(gameObject);
        Debug.Log("Game Over");
    }

    private void StopLeftCube()
    {
        var distance = _previousCubeBounds.size.x / 2 - Mathf.Abs(_previousCubeCenter.x - transform.position.x);
        if (transform.position.x < _previousCubeCenter.x)
        {
            _mesh.vertices = GenerateCubeVertices(
                distance,
                _previousCubeBounds.size.x / 2,
                _previousCubeBounds.size.z / 2);

            InstantiateSlicedPart(
                transform.TransformPoint(new Vector3(_mesh.bounds.min.x, 0, 0)),
                0f,
                Mathf.Abs(_previousCubeBounds.min.x - _meshRenderer.bounds.min.x),
                _previousCubeBounds.size.x / 2);

        }
        else //if (transform.position.x > _previousCubeCenter.x)
        {
            _mesh.vertices = GenerateCubeVertices(
                _previousCubeBounds.size.x / 2,
                distance,
                _previousCubeBounds.size.z / 2);

            InstantiateSlicedPart(
                transform.TransformPoint(new Vector3(_mesh.bounds.max.x, 0, 0)),
                Mathf.Abs(_meshRenderer.bounds.max.x - _previousCubeBounds.max.x),
                0f,
                _previousCubeBounds.size.x / 2);
        }
    }

    private void StopRightCube()
    {
        var distance = _previousCubeBounds.size.z / 2 - Mathf.Abs(_previousCubeCenter.z - transform.position.z);
        if (transform.position.z > _previousCubeCenter.z)
        {
            _mesh.vertices = GenerateCubeVertices(
                distance,
                _previousCubeBounds.size.z / 2,
                _previousCubeBounds.size.x / 2);

            InstantiateSlicedPart(
                transform.TransformPoint(new Vector3(_mesh.bounds.min.x, 0, 0)),
                0f,
                Mathf.Abs(_meshRenderer.bounds.max.z - _previousCubeBounds.max.z),
                _previousCubeBounds.size.x / 2);


        }
        else //if (transform.position.z < _previousCubeCenter.z)
        {
            _mesh.vertices = GenerateCubeVertices(
                _previousCubeBounds.size.z / 2,
                distance,
                _previousCubeBounds.size.x / 2);

            InstantiateSlicedPart(
                transform.TransformPoint(new Vector3(_mesh.bounds.max.x, 0, 0)),
                Mathf.Abs(_meshRenderer.bounds.max.z - _previousCubeBounds.max.z),
                0f,
                _previousCubeBounds.size.x / 2);
        }
    }

    private void InstantiateSlicedPart(Vector3 position, float bLength, float fLength, float width)
    {
        var instance = Instantiate(_slicedCube, position,
            transform.rotation);
        var mesh = new Mesh();
        instance.mesh = mesh;
        mesh.vertices = GenerateCubeVertices(bLength, fLength, width);
        mesh.triangles = GenerateTriangles();
        mesh.RecalculateNormals();
    }

    private Vector3[] GenerateCubeVertices(float cubeXSizeBack, float cubeXSizeFace, float cubeZSize)
    {

        var vertices = new[]
        {
            new Vector3(-cubeXSizeBack, 0, -cubeZSize),
            new Vector3(-cubeXSizeBack, CubeHeight, -cubeZSize),
            new Vector3(-cubeXSizeBack, CubeHeight, cubeZSize),
            new Vector3(-cubeXSizeBack, 0, cubeZSize),
            new Vector3(cubeXSizeFace, 0, -cubeZSize),
            new Vector3(cubeXSizeFace, CubeHeight, -cubeZSize),
            new Vector3(cubeXSizeFace, CubeHeight, cubeZSize),
            new Vector3(cubeXSizeFace, 0, cubeZSize)
        };

        return vertices;
    }

    private static int[] GenerateTriangles()
    {
        var triangles = new[]
        {
            0, 3, 2, 2, 1, 0,
            1, 2, 6, 6, 5, 1,
            0, 1, 5, 5, 4, 0,
            5, 7, 4, 5, 6, 7,
            7, 3, 0, 0, 4, 7,
            2, 3, 7, 7, 6, 2
        };
        return triangles;
    }

    private void MoveCube()
    {
        if (_cubeOrientation == Orientation.Left)
        {
            transform.position = new Vector3(_startPosition.x + Mathf.PingPong((Time.time - _startTime) * 3f,
                _movingDistance), transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _startPosition.z +
                -Mathf.PingPong((Time.time - _startTime) * 3f, _movingDistance));
        }
    }
}