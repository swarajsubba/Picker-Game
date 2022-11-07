using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
    [Serializable]
    public class GradientSet
    {
        public enum GradientType
        {
            Background = 0,
            Other = 1
        }

        public GradientType GradientFor;
        public Color colorA = Color.white;
        public Color colorB = Color.white;

        [Range(-180f, 180f)] public float angle = 0f;
        public bool ignoreRatio = true;
    }

    public GradientSet gradientSet;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (enabled)
        {
            Rect rect = graphic.rectTransform.rect;
            Vector2 dir = UIGradientUtils.RotationDir(gradientSet.angle);

            if (!gradientSet.ignoreRatio)
                dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

            UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                Vector2 localPosition = localPositionMatrix * vertex.position;
                vertex.color *= Color.Lerp(gradientSet.colorA, gradientSet.colorB, localPosition.y);
                vh.SetUIVertex(vertex, i);
            }

            Debug.Log("Modified Mesh!");
        }
    }
}
