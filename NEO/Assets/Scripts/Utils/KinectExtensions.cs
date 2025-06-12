using UnityEngine;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public static class KinectExtensions
{
    public static Vector3 ToVector3(this CameraSpacePoint point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }

    public static Vector3 ToVector3(this KinectJoint joint)
    {
        return joint.Position.ToVector3();
    }

    public static float DistanceTo(this KinectJoint joint, KinectJoint other)
    {
        return Vector3.Distance(joint.Position.ToVector3(), other.Position.ToVector3());
    }
}
