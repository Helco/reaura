using System;
using System.Numerics;

namespace Aura
{
    public enum CubeFace
    {

        Front = 0,
        Right = 1,
        Back = 2,
        Left = 3,
        Down = 4,
        Up = 5
    }

    public static class CubeFaceExtensions
    {
        public static Vector3 AsVector(this CubeFace face)
        {
            return face switch
            {
                CubeFace.Front => Vector3.UnitZ,
                CubeFace.Back => Vector3.UnitZ * -1,
                CubeFace.Right => Vector3.UnitX,
                CubeFace.Left => Vector3.UnitX * -1,
                CubeFace.Up => Vector3.UnitY,
                CubeFace.Down => Vector3.UnitY * -1,
                _ => throw new InvalidProgramException("Unknown cubeface")
            };
        }
    }
}
