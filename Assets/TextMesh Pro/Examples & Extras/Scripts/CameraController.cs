using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Coroutine zoomCoroutine;
    [SerializeField] private PixelPerfectCamera pixelPerfect;
    [SerializeField] private float baseY = 2; // starting Y pos when orthographicSize is 5
    [SerializeField] private float baseSize = 5f; // expected "normal" ortho size for your baseline

    // ✅ Adjust the camera position based on zoom
    private void LateUpdate()
    {
        float zoomFactor = (float)pixelPerfect.refResolutionY / 160f; // Assuming 180 is base resolution
        Vector3 pos = transform.position;
        pos.y = baseY + (zoomFactor - 1f) * 6f; // Adjust the '2f' as needed to shift up smoothly
        transform.position = pos;
    }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (pixelPerfect == null)
            pixelPerfect = GetComponent<PixelPerfectCamera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ZoomTo(3, 1f);
        }
    }

    public void ZoomTo(int targetSize, float duration)
    {
/*        if (pixelPerfect != null)
        {
            pixelPerfect.pixelSnapping = false;
            Debug.Log("Pixel snapping disabled");
        }*/

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(SmoothZoom(targetSize, duration));
    }

/*    private System.Collections.IEnumerator SmoothZoom(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;
        float time = 0f;
        if (pixelPerfect != null)
            pixelPerfect.pixelSnapping = false;
        while (time < duration)
        {
            Debug.Log(cam.orthographicSize);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;

        if (pixelPerfect != null)
        {
            pixelPerfect.pixelSnapping = true;
            Debug.Log("Pixel snapping re-enabled");
        }
    }*/
    private System.Collections.IEnumerator SmoothZoom(int targetResY, float duration)
    {
        int startResY = pixelPerfect.refResolutionY;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float currentResY = Mathf.Lerp(startResY, targetResY, t);
            pixelPerfect.refResolutionY = Mathf.RoundToInt(currentResY);
            time += Time.deltaTime;
            yield return null;
        }

        pixelPerfect.refResolutionY = targetResY;
    }
}
