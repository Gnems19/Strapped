using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Coroutine zoomCoroutine;
    [SerializeField] private PixelPerfectCamera pixelPerfect;
    public Parallax[] parallaxLayers;
    [SerializeField] private float baseY = 2; // starting Y pos when orthographicSize is 5
    [SerializeField] private float baseSize = 5f; // expected "normal" ortho size for your baseline

    // ✅ Adjust the camera position based on zoom
    private void LateUpdate()
    {
        float zoomFactor = (float)pixelPerfect.refResolutionY / 160f; // Assuming 160 is base resolution
        Vector3 pos = transform.position;
        pos.y = baseY + (zoomFactor - 1f) * 5f; // Adjust the '2f' as needed to shift up smoothly
        transform.position = pos;
    }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (pixelPerfect == null)
            pixelPerfect = GetComponent<PixelPerfectCamera>();
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
        foreach(Parallax parallax in parallaxLayers) 
        {
            parallax.SetZooming(true);
        }
        int startResY = pixelPerfect.refResolutionY;
        int startResX = pixelPerfect.refResolutionX;
        float targetResX = targetResY * (1920f / 1080f);
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float currentResY = Mathf.Lerp(startResY, targetResY, t);
            float currentResX = Mathf.Lerp(startResX, targetResY * (cam.aspect), t);
            pixelPerfect.refResolutionX = Mathf.RoundToInt(currentResX);
            pixelPerfect.refResolutionY = Mathf.RoundToInt(currentResY);
            time += Time.deltaTime;
            yield return null;
        }
        pixelPerfect.refResolutionX = Mathf.RoundToInt(targetResX);
        pixelPerfect.refResolutionY = targetResY;

        foreach(Parallax parallax in parallaxLayers) 
        {
            parallax.SetZooming(false);
        }
    }
}
