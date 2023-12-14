using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Reference - Write")]
    [SerializeField]
    private Vector3Variable _touchGroundPoint;

    [Header("Config")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private LayerMask _interactableLayer;
    [SerializeField]
    [Tooltip("Distance of dragging to cancel a Click, unit in pixel")]
    private float _clickCancelDistance = 20f;
    [SerializeField]
    private float _maxInteractDistance = 1000f;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onDragScreen;

    private float _dragDistance = 0f;

    public void OnDrag(PointerEventData eventData)
    {
        _dragDistance += eventData.delta.magnitude;
        _onDragScreen.Raise(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var ray = _mainCamera.ScreenPointToRay(eventData.position);

        if (!Physics.Raycast(ray, out var hit, _maxInteractDistance, _interactableLayer))
        {
            return;
        }
        _touchGroundPoint.Value = hit.point;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragDistance = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_dragDistance < _clickCancelDistance)
        {
            OnPointerClick(eventData);
        }
    }
}
