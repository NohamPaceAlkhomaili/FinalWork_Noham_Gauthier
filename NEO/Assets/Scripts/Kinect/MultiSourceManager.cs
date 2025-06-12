using UnityEngine;
using Windows.Kinect;

public class MultiSourceManager : MonoBehaviour
{
    [Header("Kinect")]
    private KinectSensor _sensor;
    private MultiSourceFrameReader _reader;

    private Texture2D _colorTexture;
    private byte[] _colorData;
    private int _colorWidth;
    private int _colorHeight;

    private ushort[] _depthData;
    private Texture2D _depthTexture;
    private int _depthWidth;
    private int _depthHeight;

    private Body[] _bodies;

    public Texture2D GetColorTexture() => _colorTexture;
    public Body[] GetBodies() => _bodies;
    public ushort[] GetDepthData() => _depthData;
    public int ColorWidth() => _colorWidth;
    public int ColorHeight() => _colorHeight;

    private void Awake()
    {
        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            _reader = _sensor.OpenMultiSourceFrameReader(
                FrameSourceTypes.Color | 
                FrameSourceTypes.Body | 
                FrameSourceTypes.Depth
            );
            
            if (!_sensor.IsOpen)
                _sensor.Open();
        }
    }

    private void Update()
    {
        if (_reader == null) return;

        var frame = _reader.AcquireLatestFrame();
        if (frame == null) return;

        ProcessColorFrame(frame.ColorFrameReference.AcquireFrame());
        ProcessDepthFrame(frame.DepthFrameReference.AcquireFrame());
        ProcessBodyFrame(frame.BodyFrameReference.AcquireFrame());
    }

    private void ProcessColorFrame(ColorFrame colorFrame)
    {
        if (colorFrame == null) return;

        FrameDescription colorDesc = colorFrame.FrameDescription;
        _colorWidth = colorDesc.Width;
        _colorHeight = colorDesc.Height;

        if (_colorTexture == null)
        {
            _colorTexture = new Texture2D(_colorWidth, _colorHeight, TextureFormat.BGRA32, false);
            _colorData = new byte[_colorWidth * _colorHeight * 4];
        }

        colorFrame.CopyConvertedFrameDataToArray(_colorData, ColorImageFormat.Bgra);
        _colorTexture.LoadRawTextureData(_colorData);
        _colorTexture.Apply();
        colorFrame.Dispose();
    }

    private void ProcessDepthFrame(DepthFrame depthFrame)
    {
        if (depthFrame == null) return;

        FrameDescription depthDesc = depthFrame.FrameDescription;
        _depthWidth = depthDesc.Width;
        _depthHeight = depthDesc.Height;

        if (_depthData == null)
            _depthData = new ushort[_depthWidth * _depthHeight];

        depthFrame.CopyFrameDataToArray(_depthData);
        depthFrame.Dispose();
    }

    private void ProcessBodyFrame(BodyFrame bodyFrame)
    {
        if (bodyFrame == null) return;

        if (_bodies == null || _bodies.Length != _sensor.BodyFrameSource.BodyCount)
            _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

        bodyFrame.GetAndRefreshBodyData(_bodies);
        bodyFrame.Dispose();
    }

    private void OnApplicationQuit()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
                _sensor.Close();
            
            _sensor = null;
        }
    }
}
