using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfVrCircle : MonoBehaviour
{
    [Header("Size Settings")]
    public float s0 = 1f;
    public float s1 = 2f;
    public float s2 = 0.5f;

    [Header("Color Settings")]
    public Color c0 = Color.red;
    public Color c1 = Color.red;   
    public Color c2 = Color.green;
    public Color c3 = Color.blue;

    [Header("Timing Settings")]
    public float t1 = 3f;
    public float t2 = 4f;
    public float t3 = 2f;

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        StartCoroutine(AnimateLoop());
    }

    private IEnumerator AnimateLoop()
    {
        while (true)
        {
            yield return AnimateTransition(s0, s1, c1, t1);
            yield return AnimateTransition(s1, s2, c2, t2);
            yield return AnimateTransition(s2, s0, c3, t3);
        }
    }

    private IEnumerator AnimateTransition(float startScale, float endScale, Color fixedColor, float duration)
    {
        _renderer.material.color = fixedColor;  // Set color immediately at start

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = new Vector3(scale, scale, scale);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure exact final scale
        transform.localScale = new Vector3(endScale, endScale, endScale);
    }
}
