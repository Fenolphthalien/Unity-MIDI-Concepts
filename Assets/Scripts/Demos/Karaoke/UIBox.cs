using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBox : MaskableGraphic
{
    /** http://docs.unity3d.com/ScriptReference/UI.Graphic.html  **/
    protected override void OnFillVBO(System.Collections.Generic.List<UIVertex> vbo)
    {
        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.zero;

        corner1.x = 0f;
        corner1.y = 0f;
        corner2.x = 1f;
        corner2.y = 1f;

        corner1.x -= rectTransform.pivot.x;
        corner1.y -= rectTransform.pivot.y;
        corner2.x -= rectTransform.pivot.x;
        corner2.y -= rectTransform.pivot.y;

        corner1.x *= rectTransform.rect.width;
        corner1.y *= rectTransform.rect.height;
        corner2.x *= rectTransform.rect.width;
        corner2.y *= rectTransform.rect.height;

        vbo.Clear();

        UIVertex vert = UIVertex.simpleVert;

        vert.position = new Vector2(corner1.x, corner1.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner1.x, corner2.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner2.x, corner2.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner2.x, corner1.y);
        vert.color = color;
        vbo.Add(vert);

        //Not sure why this is here.
        //Matrix4x4 matrix = rectTransform.localToWorldMatrix;
        //Graphics.DrawMesh(mesh,matrix,material,0);
    }
}
