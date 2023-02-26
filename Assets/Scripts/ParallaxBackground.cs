using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxBackground : MonoBehaviour
{
    public float depth;
    public float scale;
    public Camera camera_;

    private Vector2[] startPositions;
    private Vector2[] scales;
    private Vector2[] bounds;
    private float cameraSize;

    void Start()
    {
        startPositions = new Vector2[transform.childCount];
        scales = new Vector2[transform.childCount];
        bounds = new Vector2[transform.childCount];
        cameraSize = camera_.orthographicSize;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

            startPositions[i] = new Vector2(child.transform.position.x, child.transform.position.y);
            scales[i] = new Vector2(child.transform.localScale.x, child.transform.localScale.y);
            bounds[i] = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y);
        }
    }

    void LateUpdate()
    {
        float scaleFactor = camera_.orthographicSize / cameraSize;
        transform.position = camera_.transform.position;
        transform.localScale = new Vector3(scaleFactor * scale, scaleFactor * scale, transform.localScale.z);
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            
            float parallaxEffect = child.transform.position.z / (depth != 0 ? depth : 1f);
            Vector2 temp = camera_.transform.position * (1 - parallaxEffect);
            Vector2 distance = camera_.transform.position * parallaxEffect;
            float scale = 1 + 1 / scaleFactor * (1 - parallaxEffect);

            Vector2 nextPosition = startPositions[i] + distance;
            child.transform.localPosition = new Vector3(nextPosition.x, nextPosition.y, child.transform.position.z) - transform.position;
            child.transform.localScale = new Vector3(scale, scale, child.transform.localScale.z);

            if (temp.x > startPositions[i].x + bounds[i].x)
                startPositions[i].x += bounds[i].x;
            else if (temp.x < startPositions[i].x - bounds[i].x)
                startPositions[i].x -= bounds[i].x;
            
            if (temp.y > startPositions[i].y + bounds[i].y)
                startPositions[i].y += bounds[i].y;
            else if (temp.y < startPositions[i].y - bounds[i].y)
                startPositions[i].y -= bounds[i].y;
        }
    }
}
