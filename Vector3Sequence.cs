using OpenTK.Mathematics;

namespace Project3D
{
    public class Vector3Sequence
    {
        private Keyframe<Vector3>[] _keyframes;

        private float lastTime;
        private int lastIndex;

        public Vector3Sequence(Keyframe<Vector3>[] keyframes)
        {
            _keyframes = keyframes;
        }

        public Vector3 GetValue(float time)
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

            FindClosestPair(time, out Keyframe<Vector3> start, out Keyframe<Vector3> end);

            return Vector3.Lerp(start.Value, end.Value, (time - start.Time) / (end.Time - start.Time));
        }

        private void FindClosestPair(float time, out Keyframe<Vector3> start, out Keyframe<Vector3> end)
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
