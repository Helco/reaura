using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aura
{
    public class SphericalArea : MonoBehaviour
    {
        private static readonly float gizmoSectionSize = AuraMath.MaxAuraAngle.x / 60.0f;
        private static readonly float gizmoMeshSectionSize = gizmoSectionSize * 2.0f;

        public Vector2 upperLeft;
        public Vector2 size;

        public bool IsPointInside(Vector2 auraPos) =>
            auraPos.x >= upperLeft.x && auraPos.x < upperLeft.x + size.x &&
            auraPos.y >= upperLeft.y && auraPos.y < upperLeft.y + size.y;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (size == Vector2.zero)
                return;

            void drawSphericalSection(Vector2 auraFrom, Vector2 auraTo)
            {
                Vector3 from = AuraMath.AuraOnSphere(auraFrom);
                Vector3 to = AuraMath.AuraOnSphere(auraTo);
                Gizmos.DrawLine(from, to);
            }

            void drawSphericalLine(Vector2 auraStart, Vector2 auraEnd)
            {
                float length = Vector2.Distance(auraStart, auraEnd);
                int sections = Mathf.CeilToInt(length / gizmoSectionSize);
                Vector2 section = (auraEnd - auraStart) / sections;
                for (int i = 0; i < sections; i++)
                    drawSphericalSection(auraStart + i * section, auraStart + (i + 1) * section);
            }

            Gizmos.color = Color.yellow;
            Vector2 right = new Vector2(size.x, 0.0f);
            Vector2 down = new Vector2(0.0f, size.y);
            drawSphericalLine(upperLeft, upperLeft + right);
            drawSphericalLine(upperLeft, upperLeft + down);
            drawSphericalLine(upperLeft + right, upperLeft + right + down);
            drawSphericalLine(upperLeft + down, upperLeft + right + down);
        }


        private Mesh gizmoMesh;
        private Vector2 gizmoPos, gizmoSize;
        private void OnDrawGizmosSelected()
        {
            if (size == Vector2.zero)
                return;

            var color = Color.yellow;
            color.a = 0.5f;
            Gizmos.color = color;

            if (gizmoMesh == null || gizmoPos != upperLeft || gizmoSize != size)
            {
                Vector2Int sections = new Vector2Int(
                    Mathf.CeilToInt(size.x / gizmoMeshSectionSize),
                    Mathf.CeilToInt(size.y / gizmoMeshSectionSize));
                Vector2 right = new Vector2(size.x / sections.x, 0.0f);
                Vector2 down = new Vector2(0.0f, size.y / sections.y);

                gizmoMesh = new Mesh();
                gizmoMesh.vertices = Enumerable
                    .Range(0, sections.y + 1)
                    .SelectMany(y => Enumerable
                        .Range(0, sections.x + 1)
                        .Select(x => upperLeft + x * right + y * down)
                    ).Select(AuraMath.AuraOnSphere)
                    .ToArray();

                var line = sections.x + 1;
                var indices = Enumerable
                    .Range(0, sections.y)
                    .SelectMany(y => Enumerable
                        .Range(0, sections.x)
                        .Select(x => y * line + x)
                        .SelectMany(baseI => new int[] {
                        0, 1, line,
                        1, line + 1, line
                        }.Select(i => i + baseI))
                    ).ToArray();

                gizmoMesh.SetTriangles(indices, 0);
                gizmoMesh.RecalculateNormals();
                gizmoMesh.Optimize();
                gizmoPos = upperLeft;
                gizmoSize = size;
            }
            Gizmos.DrawMesh(gizmoMesh);
        }
#endif
    }
}