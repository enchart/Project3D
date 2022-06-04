using OpenTK.Mathematics;

namespace Project3D
{
    public class Vector3dSequence
    {
        private Keyframe<Vector3d>[] _keyframes;

        private double lastTime;
        private int lastIndex;

        public Vector3dSequence(Keyframe<Vector3d>[] keyframes)
        {
            _keyframes = keyframes;
        }

        public Vector3d GetValue(double time)
        {
            if (_keyframes.Length == 1)
            {
                return _keyframes[0].Value;
            }

            if (time <= _keyframes[0].Time)
            {
                return _keyframes[0].Value;
            }

            if (time >= _keyframes[_keyframes.Length - 1].Time)
            {
                return _keyframes[_keyframes.Length - 1].Value;
            }

            FindClosestPair(time, out Keyframe<Vector3d> start, out Keyframe<Vector3d> end);

            return Vector3d.Lerp(start.Value, end.Value, (time - start.Time) / (end.Time - start.Time));
        }

        private void FindClosestPair(double time, out Keyframe<Vector3d> start, out Keyframe<Vector3d> end)
        {
            if (time >= lastTime)
            {
                while (time >= _keyframes[lastIndex + 1].Time)
                {
                    lastIndex++;
                }
                start = _keyframes[lastIndex];
                end = _keyframes[lastIndex + 1];
            }
            else
            {
                while (time < _keyframes[lastIndex].Time)
                {
                    lastIndex--;
                }
                start = _keyframes[lastIndex];
                end = _keyframes[lastIndex + 1];
            }
            lastTime = time;
        }
    }
}
