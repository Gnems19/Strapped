using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LockCameraAspect : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f;

    private Camera _camera;
    private int _lastWidth = -1;
    private int _lastHeight = -1;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        ApplyViewport();
    }

    private void OnEnable()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

        ApplyViewport();
    }

    private void LateUpdate()
    {
        if (Screen.width == _lastWidth && Screen.height == _lastHeight)
            return;

        ApplyViewport();
    }

    private void ApplyViewport()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        if (_camera == null || _lastHeight <= 0 || targetAspect <= 0f)
            return;

        float windowAspect = (float)_lastWidth / _lastHeight;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            var rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
            _camera.rect = rect;
            return;
        }

        float scaleWidth = 1f / scaleHeight;
        _camera.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
    }
}
