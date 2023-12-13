using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _movedTransform;
    [SerializeField]
    private Ease _moveEase;
    [SerializeField]
    private float _moveDuration;
    
    public void MoveTo(Vector2Int coordinate)
    {
        Vector3 destination = GridMap.Instance.GetPosition(coordinate);
        _movedTransform.DOMove(destination, _moveDuration).SetEase(_moveEase);
    }
}
