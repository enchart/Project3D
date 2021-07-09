namespace Project3D
{
    public struct Keyframe<T> where T : struct
    {
        public float Time;
        public T Value;
    }
}
