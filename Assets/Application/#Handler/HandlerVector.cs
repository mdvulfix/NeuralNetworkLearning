using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP
{
    public static class HandlerVector
    {
        public static float X { get; set; }
        public static float Y { get; set; }
        public static float Z { get; set; }
        
    
        public static Vector3 GetRandomVector(float min, float max)
        {
            X = URandom.Range(min, max);
            Y = URandom.Range(min, max);
            Z = URandom.Range(min, max);

            return new Vector3(X, Y, Z);
        }
    }
}