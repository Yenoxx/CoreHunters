using System.Collections.Generic;
using UnityEngine;
 
// Thanks NinjaISV: http://answers.unity.com/answers/1810252/view.html
public static class Collider2DExtensions 
{
    public static void TryUpdateShapeToAttachedSprite(this PolygonCollider2D collider) 
    {
        collider.UpdateShapeToSprite(collider.GetComponent<SpriteRenderer>().sprite);
    }

    public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite) 
    { 
        // Ensure both valid
        if (collider != null && sprite != null)
        {
            // Update count
            collider.pathCount = sprite.GetPhysicsShapeCount();

            // New paths variable
            List<Vector2> path = new List<Vector2>();

            // Loop path count
            for (int i = 0; i < collider.pathCount; i++) 
            {
                // Clear
                path.Clear();
                // Get shape
                sprite.GetPhysicsShape(i, path);
                // Set path
                collider.SetPath(i, path.ToArray());
            }
        }
    }
}