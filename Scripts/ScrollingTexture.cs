using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0.5f, 0);
    public bool isWater = true;
    public float waterFactor = 0.1f;
    public float waterSpeed = 1.0f;

    private Renderer objectRenderer;
    private Vector2 currentOffset;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        currentOffset = objectRenderer.material.mainTextureOffset;
    }

    void Update()
    {
        currentOffset += scrollSpeed * Time.deltaTime;
        objectRenderer.material.mainTextureOffset = currentOffset;

        if (isWater)
        {
            float waterOffset = Mathf.Sin(Time.time * waterSpeed) * waterFactor;
            objectRenderer.material.SetFloat("_BumpAmt", waterOffset);
        }
    }
}