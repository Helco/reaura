using System;
using System.Numerics;

namespace Aura
{
    public static class AuraMath
    {
        public static readonly Vector2 MaxAuraAngle = new Vector2(960.0f, 720.0f);

        public static Vector2 NormalizeAura(Vector2 pos)
        {
            while (pos.X < 0.0f)
                pos.X += MaxAuraAngle.X;
            while (pos.X >= MaxAuraAngle.X)
                pos.X -= MaxAuraAngle.X;
            while (pos.Y < 0.0f)
                pos.Y += MaxAuraAngle.Y;
            while (pos.Y >= MaxAuraAngle.Y)
                pos.Y -= MaxAuraAngle.Y;
            return pos;
        }

        public static Vector2 AngleToAura(Vector2 unityAngle) => new Vector2(
            unityAngle.Y / 360.0f * MaxAuraAngle.X,
            (unityAngle.X + 90.0f) / 180.0f * MaxAuraAngle.Y);

        public static Vector2 AuraToAngle(Vector2 auraAngle) => new Vector2(
            auraAngle.Y / MaxAuraAngle.Y * 180.0f - 90.0f,
            auraAngle.X / MaxAuraAngle.X * 360.0f);

        public static Vector2 AuraToAngleRadians(Vector2 auraAngle) => AuraToAngle(auraAngle) * MathF.PI / 180.0f;

        public static Vector2 AngleRadiansToAura(Vector2 radians) => AngleToAura(radians * 180.0f / MathF.PI);

        public static Vector3 AuraOnSphere(Vector2 auraAngle)
        {
            var radians = AuraToAngleRadians(auraAngle);
            return new Vector3(
                MathF.Cos(-radians.X) * MathF.Sin(radians.Y),
                MathF.Sin(-radians.X),
                MathF.Cos(-radians.X) * MathF.Cos(radians.Y));
        }

        public static Vector2 SphereToAura(Vector3 pos)
        {
            var radians = new Vector2(
                MathF.Acos(pos.Y / pos.Length()) - MathF.PI / 2.0f,
                MathF.Atan2(pos.X, pos.Z));
            if (radians.Y < 0.0f)
                radians.Y += 2 * MathF.PI;
            return AngleRadiansToAura(radians);
        }
    }
}