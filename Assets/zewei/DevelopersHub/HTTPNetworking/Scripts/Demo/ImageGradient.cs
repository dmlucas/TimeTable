namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ImageGradient : BaseMeshEffect
    {

        public Color topRight = Color.white;
        public Color topLeft = Color.white;
        public Color bottomRight = Color.white;
        public Color bottomLeft = Color.white;

        public struct Matrix
        {
            public float m00, m01, m02, m10, m11, m12;
            public Matrix(float m00, float m01, float m02, float m10, float m11, float m12)
            {
                this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m10 = m10; this.m11 = m11; this.m12 = m12;
            }

            public static Vector2 operator *(Matrix m, Vector2 v)
            {
                float x = (m.m00 * v.x) - (m.m01 * v.y) + m.m02;
                float y = (m.m10 * v.x) + (m.m11 * v.y) + m.m12;
                return new Vector2(x, y);
            }
        }

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled)
            {
                float cos = 1;
                float sin = 0;
                Vector2 rectMin = graphic.rectTransform.rect.min;
                Vector2 rectSize = graphic.rectTransform.rect.size;
                float c = 0.5f;
                float ax = rectMin.x / rectSize.x + c;
                float ay = rectMin.y / rectSize.y + c;
                float m00 = cos / rectSize.x;
                float m01 = sin / rectSize.y;
                float m02 = -(ax * cos - ay * sin - c);
                float m10 = sin / rectSize.x;
                float m11 = cos / rectSize.y;
                float m12 = -(ax * sin + ay * cos - c);
                Matrix position = new Matrix(m00, m01, m02, m10, m11, m12);
                UIVertex vertex = default(UIVertex);
                for (int i = 0; i < vertexHelper.currentVertCount; i++)
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);
                    vertex.color *= Lerp(bottomLeft, bottomRight, topLeft, topRight, position * vertex.position);
                    vertexHelper.SetUIVertex(vertex, i);
                }
            }
        }

        public static Color Lerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
        {
            return Color.LerpUnclamped(Color.LerpUnclamped(a1, a2, t.x), Color.LerpUnclamped(b1, b2, t.x), t.y);
        }

    }
}