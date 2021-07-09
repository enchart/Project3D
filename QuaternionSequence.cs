using OpenTK.Mathematics;

namespace Project3D
{
    public class QuaternionSequence
    {
        private Keyframe<Quaternion>[] _keyframes;

        private float lastTime;
        private int lastIndex;

        public QuaternionSequence(Keyframe<Quaternion>[] keyframes)
        {
            _keyframes = keyframes;
        }

        public Quaternion GetValue(float time)
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

            FindClosestPair(time, out Keyframe<Quaternion> start, out Keyframe<Quaternion> end);

            return Quaternion.Slerp(start.Value, end.Value, (time - start.Time) / (end.Time - start.Time));
        }

        private void FindClosestPair(float time, out Keyframe<Quaternion> start, out Keyframe<Quaternion> end)
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
