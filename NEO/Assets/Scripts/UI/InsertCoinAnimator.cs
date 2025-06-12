using UnityEngine;
using TMPro; // Si tu utilises TextMeshPro

public class InsertCoinAnimator : MonoBehaviour
{
    public float blinkSpeed = 1.2f;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        textMesh.alpha = alpha;
    }
}
