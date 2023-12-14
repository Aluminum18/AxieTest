using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private GridMap _gridMap;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onMoved;

    [Header("Configs")]
    [SerializeField]
    private CharacterProperties _properties;
    [SerializeField]
    private Transform _movedTransform;
    [SerializeField]
    private Ease _moveEase;
    [SerializeField]
    private float _moveDuration;
    [SerializeField]
    private Vector2Int _currentCoordinate;
    public Vector2Int CurrentCoordinate => _currentCoordinate;
    
    public void MoveTo(Vector2Int coordinate)
    {
        Vector3 destination = _gridMap.GetPosition(coordinate);
        _onMoved.Raise(_properties, _currentCoordinate, coordinate);
        _currentCoordinate = coordinate;
        _movedTransform.DOMove(destination, _moveDuration).SetEase(_moveEase);
    }

    public void LookAt(Vector2Int coordinate)
    {
        if (CurrentCoordinate.x == coordinate.x)
        {
            _properties.Animator.ScaleX(Mathf.Sign(CurrentCoordinate.y - coordinate.y));
            return;
        }

        _properties.Animator.ScaleX(Mathf.Sign(CurrentCoordinate.x - coordinate.x));
    }

    public void InitPosition(Vector2Int coordinate)
    {
        _movedTransform.position = _gridMap.GetPosition(coordinate);
        _currentCoordinate = coordinate;
    }
}
