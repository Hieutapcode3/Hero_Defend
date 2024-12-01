using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTxt : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    private TextMeshPro textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        if (textMesh != null)
        {
            Color color = textMesh.faceColor;
            Color outline = textMesh.outlineColor;
            outline.a -= fadeSpeed * Time.deltaTime;
            color.a -= fadeSpeed * Time.deltaTime;
            textMesh.faceColor = color;
            if (color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
