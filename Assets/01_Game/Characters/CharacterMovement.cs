using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private GridMap _gridMap;

    [Header("Configs")]
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
        _movedTransform.DOMove(destination, _moveDuration).SetEase(_moveEase).OnComplete(() =>
        {
            _currentCoordinate = coordinate;
        });
    }
}
