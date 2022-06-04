namespace Project3D
{
    public struct Keyframe<T> where T : struct
    {
        public double Time;
        public T Value;
    }
}
