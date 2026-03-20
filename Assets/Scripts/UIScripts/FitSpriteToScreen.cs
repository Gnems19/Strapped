using UnityEngine;
using System.Collections;

/// <summary>
/// Scales a SpriteRenderer to fit within the camera viewport without overflowing.
/// Works correctly even when parented under a Canvas/Panel.
/// </summary>
public class FitSpriteToScreen : MonoBehaviour
{
    [Tooltip("How much of the screen to occupy (100 = fit to screen edges).")]
    [SerializeField, Range(10, 100)] private int screenPercent = 100;

    IEnumerator Start()
    {
        yield return null;

        var cam = Camera.main;
        var sr = GetComponent<SpriteRenderer>();
        if (cam == null || sr == null || sr.sprite == null) yield break;

        var bounds = sr.sprite.bounds.size;
        float camH = cam.orthographicSize * 2f;
        float camW = camH * cam.aspect;

        // Fit: use the smaller ratio so the sprite never overflows
        float s = Mathf.Min(camW / bounds.x, camH / bounds.y) * (screenPercent / 100f);

        var parent = transform.parent;
        if (parent != null)
        {
            var ls = parent.lossyScale;
            if (ls.x > 0f && ls.y > 0f)
            {
                transform.localScale = new Vector3(s / ls.x, s / ls.y, 1f);
                yield break;
            }
        }

        transform.localScale = new Vector3(s, s, 1f);
    }
}
