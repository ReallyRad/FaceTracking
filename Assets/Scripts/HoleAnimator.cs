using UnityEngine;

public class HoleAnimator : MonoBehaviour
{
    public float maxHoleRadius;
    public float duration;
    public float holeRadius;

    private Material material;
    private float timer = 0.0f;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float currentRadius = Mathf.Lerp(0, maxHoleRadius, t);
        material.SetFloat("_HoleRadius", currentRadius);
        holeRadius = material.GetFloat("_HoleRadius");

        if (t >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
