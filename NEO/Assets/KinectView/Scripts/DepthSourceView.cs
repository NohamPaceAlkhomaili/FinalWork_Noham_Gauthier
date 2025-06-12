using UnityEngine;
using System.Collections;
using Windows.Kinect;

public enum DepthViewMode
{
    SeparateSourceReaders,
    MultiSourceReader,
}

public class DepthSourceView : MonoBehaviour
{
    public DepthViewMode ViewMode = DepthViewMode.SeparateSourceReaders;

    [Header("Managers")]
    public GameObject ColorSourceManager;
    public GameObject DepthSourceManager;
    public GameObject MultiSourceManager;

    private KinectSensor _Sensor;
    private CoordinateMapper _Mapper;
    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    private const int _DownsampleSize = 4;
    private const double _DepthScale = 0.1f;
    private const int _Speed = 50;

    private MultiSourceManager _MultiManager;
    private ColorSourceManager _ColorManager;
    private DepthSourceManager _DepthManager;
    private bool _isInitialized;
    private FrameDescription _depthFrameDesc;

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Mapper = _Sensor.CoordinateMapper;
            _depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
            StartCoroutine(InitializeAfterSensorReady());
        }
    }

    private IEnumerator InitializeAfterSensorReady()
    {
        while (!_Sensor.IsAvailable)
            yield return null;

        CreateMesh(_depthFrameDesc.Width / _DownsampleSize, _depthFrameDesc.Height / _DownsampleSize);
        _isInitialized = true;
    }

    void CreateMesh(int width, int height)
    {
        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;

        _Vertices = new Vector3[width * height];
        _UV = new Vector2[width * height];
        _Triangles = new int[6 * ((width - 1) * (height - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

                _Vertices[index] = new Vector3(x, -y, 0);
                _UV[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));

                if (x < width - 1 && y < height - 1)
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
        _Mesh.RecalculateBounds();
    }

    void Update()
    {
        if (!_isInitialized || _Sensor == null) return;

        HandleInput();
        UpdateVisualization();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ViewMode = (ViewMode == DepthViewMode.MultiSourceReader) ?
                DepthViewMode.SeparateSourceReaders :
                DepthViewMode.MultiSourceReader;
        }

        float yVal = Input.GetAxis("Horizontal");
        float xVal = -Input.GetAxis("Vertical");
        transform.Rotate(
            (xVal * Time.deltaTime * _Speed),
            (yVal * Time.deltaTime * _Speed),
            0,
            Space.Self);
    }

    private void UpdateVisualization()
    {
        if (ViewMode == DepthViewMode.SeparateSourceReaders)
        {
            UpdateSeparateSourceMode();
        }
        else
        {
            UpdateMultiSourceMode();
        }
    }

    private void UpdateSeparateSourceMode()
    {
        if (ColorSourceManager == null || DepthSourceManager == null)
            return;

        _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
        _DepthManager = DepthSourceManager.GetComponent<DepthSourceManager>();

        if (_ColorManager == null || _DepthManager == null)
            return;

        GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();
        RefreshData(_DepthManager.GetData());
    }

    private void UpdateMultiSourceMode()
    {
        if (MultiSourceManager == null)
            return;

        _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
        if (_MultiManager == null)
            return;

        GetComponent<Renderer>().material.mainTexture = _MultiManager.GetColorTexture();
        RefreshData(_MultiManager.GetDepthData());
    }

    private void RefreshData(ushort[] depthData)
    {
        if (depthData == null || depthData.Length == 0) return;

        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        _Mapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        for (int y = 0; y < _depthFrameDesc.Height; y += _DownsampleSize)
        {
            for (int x = 0; x < _depthFrameDesc.Width; x += _DownsampleSize)
            {
                int indexX = x / _DownsampleSize;
                int indexY = y / _DownsampleSize;
                int smallIndex = (indexY * (_depthFrameDesc.Width / _DownsampleSize)) + indexX;

                double avg = GetAvg(depthData, x, y);
                _Vertices[smallIndex].z = (float)(avg * _DepthScale);

                var colorSpacePoint = colorSpace[(y * _depthFrameDesc.Width) + x];
                _UV[smallIndex] = new Vector2(
                    Mathf.Clamp01(colorSpacePoint.X / _depthFrameDesc.Width),
                    Mathf.Clamp01(colorSpacePoint.Y / _depthFrameDesc.Height)
                );
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.RecalculateNormals();
    }

    private double GetAvg(ushort[] depthData, int x, int y)
    {
        double sum = 0.0;
        int count = 0;

        for (int y1 = y; y1 < Mathf.Min(y + 4, _depthFrameDesc.Height); y1++)
        {
            for (int x1 = x; x1 < Mathf.Min(x + 4, _depthFrameDesc.Width); x1++)
            {
                int fullIndex = (y1 * _depthFrameDesc.Width) + x1;
                if (fullIndex >= depthData.Length) continue;

                sum += (depthData[fullIndex] == 0) ? 4500 : depthData[fullIndex];
                count++;
            }
        }

        return (count == 0) ? 0 : sum / count;
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        GUI.TextField(new Rect(Screen.width - 250, 10, 250, 20), "DepthMode: " + ViewMode.ToString());
        GUI.EndGroup();
    }

    void OnApplicationQuit()
    {
        if (_Mapper != null)
        {
            _Mapper = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            _Sensor = null;
        }
    }
}
