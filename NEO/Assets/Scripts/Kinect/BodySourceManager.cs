using UnityEngine;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour
{
    [Header("Kinect Sensor")]
    private KinectSensor _sensor;
    private BodyFrameReader _reader;
    private Body[] _bodies;

    public Body[] GetData() => _bodies;

    private void Awake()
    {
        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            _reader = _sensor.BodyFrameSource.OpenReader();
            if (!_sensor.IsOpen)
                _sensor.Open();
        }
    }

    private void Update()
    {
        if (_reader == null) return;

        var frame = _reader.AcquireLatestFrame();
        if (frame != null)
        {
            if (_bodies == null || _bodies.Length != _sensor.BodyFrameSource.BodyCount)
                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

            frame.GetAndRefreshBodyData(_bodies);
            frame.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }

        if (_sensor != null && _sensor.IsOpen)
        {
            _sensor.Close();
            _sensor = null;
        }
    }
}
