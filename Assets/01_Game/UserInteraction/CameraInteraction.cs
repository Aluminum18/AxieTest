using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraInteraction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private Transform _cameraHolder;
    [SerializeField]
    private Camera _refererenceCamera;
    [SerializeField]
    private float _dragSensitivity = 1f;
    [SerializeField]
    private float _zoomSpeed = 1f;
    [SerializeField]
    private Vector2 _camSizeClamp = new Vector2(5f, 10f);
    [SerializeField]
    private Vector2 _camXPositionClamp;
    [SerializeField]
    private Vector2 _camYPositionClamp;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onDragSceen;

    [Header("Inspect")]
    [SerializeField]
    private float _meterPerPixel;
    [SerializeField]
    private float _cameraAngle;
    [SerializeField]
    private float _cameraSinAngle;

    private float _ratioCamSize;

    private void DragCamera(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        PointerEventData eventData = (PointerEventData)args[0];
        Vector2 dragDelta = eventData.delta;

        float height = _refererenceCamera.pixelHeight;
        _meterPerPixel = _refererenceCamera.orthographicSize * 2f / height;

        Vector3 worldDragDelta = _meterPerPixel * dragDelta.x * _cameraHolder.right
                                + _meterPerPixel * dragDelta.y * _cameraHolder.up / _cameraSinAngle;

        Vector3 nextCamPosition = _cameraHolder.position - worldDragDelta * _dragSensitivity;
        nextCamPosition.x = Mathf.Clamp(nextCamPosition.x, _camXPositionClamp.x, _camXPositionClamp.y);
        nextCamPosition.y = Mathf.Clamp(nextCamPosition.y, _camYPositionClamp.x, _camYPositionClamp.y);
        _cameraHolder.position = nextCamPosition;
    }

    private void AdjustZoom(Vector3 scrollDelta)
    {
        if (scrollDelta.y == 0f)
        {
            return;
        }

        float currentSize = _refererenceCamera.orthographicSize;
        float targetSize = Mathf.Clamp(currentSize - scrollDelta.y * _zoomSpeed * Time.deltaTime, _camSizeClamp.x, _camSizeClamp.y);
        _refererenceCamera.orthographicSize = targetSize;
    }

    private void OnEnable()
    {
        _onDragSceen.Subcribe(DragCamera);

        _cameraAngle = Vector3.Angle(_refererenceCamera.transform.forward, _cameraHolder.forward);
        _cameraSinAngle = Mathf.Abs(Mathf.Sin(_cameraAngle * Mathf.Deg2Rad));
        if (_cameraSinAngle == 0f)
        {
            _cameraSinAngle = 1f;
        }
    }

    private void Update()
    {
        AdjustZoom(Input.mouseScrollDelta);
    }

    private void OnDisable()
    {
        _onDragSceen.Unsubcribe(DragCamera);
    }
}
