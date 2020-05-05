using UnityEngine;

namespace Aura
{
    public static class AuraMath
    {
        public static readonly Vector2 MaxAuraAngle = new Vector2(960.0f, 720.0f);

        public static Vector2 AngleToAura(Vector2 unityAngle) => new Vector2(
            unityAngle.y / 360.0f * MaxAuraAngle.x,
            (unityAngle.x + 90.0f) / 180.0f * MaxAuraAngle.y);

        public static Vector2 AngleToUnity(Vector2 auraAngle) => new Vector2(
            auraAngle.y / MaxAuraAngle.y * 180.0f - 90.0f,
            auraAngle.x / MaxAuraAngle.x * 360.0f);

        public static Vector2 AngleToUnityRadians(Vector2 auraAngle) => AngleToUnity(auraAngle) * Mathf.Deg2Rad;

        public static Vector2 AngleRadiansToAura(Vector2 unityRadians) => AngleToAura(unityRadians * Mathf.Rad2Deg);

        public static Vector3 AuraOnSphere(Vector2 auraAngle)
        {
            var radians = AngleToUnityRadians(auraAngle);
            return new Vector3(
                Mathf.Cos(-radians.x) * Mathf.Sin(radians.y),
                Mathf.Sin(-radians.x),
                Mathf.Cos(-radians.x) * Mathf.Cos(radians.y));
        }

        public static Vector2 SphereToAura(Vector3 pos)
        {
            var radians = new Vector2(
                Mathf.Acos(pos.y / pos.magnitude) - Mathf.PI / 2.0f,
                Mathf.Atan2(pos.x, pos.z));
            if (radians.y < 0.0f)
                radians.y += 2 * Mathf.PI;
            return AngleRadiansToAura(radians);
        }
    }
}