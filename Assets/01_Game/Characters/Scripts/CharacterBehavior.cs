using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private GridMap _map;

    [SerializeField]
    private GameEvent _onACharacterDefeated;
    [SerializeField]
    private UnityEvent _onHit;
    [SerializeField]
    private UnityEvent _onDisappeared;


    [Header("Configs")]
    [SerializeField]
    private CharacterProperties _properties;

    [Header("Inspec")]
    [SerializeField]
    private CharacterProperties _currentTarget;
    public CharacterProperties CurrentTarget => _currentTarget;

    private bool _finishedTurn = false;
    private Vector3 _engagePosition = GridMap.UNDEFINED_POSITON;

    public void ScanTarget()
    {
        if (DoesCurrentTargetExist())
        {
            return;
        }

        _engagePosition = GridMap.UNDEFINED_POSITON;

        var tracker = CharacterTracker.Instance;
        if (_properties.Team == CharacterProperties.TeamId.Defense)
        {
            _currentTarget = tracker.FindTargetAround(_properties.Movement.CurrentCoordinate, CharacterProperties.TeamId.Attack);
        }
        else
        {
            _currentTarget = tracker.FindNearestDefender(_properties.Movement);
        }

        if (!DoesCurrentTargetExist())
        {
            return;
        }
        _properties.Movement.LookAt(_currentTarget.Movement.CurrentCoordinate);
    }

    public void DecideAction()
    {
        _finishedTurn = false;
        if (_currentTarget == null)
        {
            return;
        }

        var movement = _properties.Movement;
        var targetMovement = _currentTarget.Movement;
        bool isTargetAdjacent = _map.IsAdjacentCell(movement.CurrentCoordinate, targetMovement.CurrentCoordinate);
        if (isTargetAdjacent)
        {
            Attack();
            _finishedTurn = true;
            return;
        }
        if (_properties.Team == CharacterProperties.TeamId.Defense)
        {
            Idle();
            return;
        }
        // Move step should be executed after attack step to prevent being attacked while moving
        NextFrame_MoveTowardToOrIdle().Forget();
    }

    public bool TryMoveTowardToCurrentTarget()
    {
        if (_finishedTurn)
        {
            return false;
        }
        var tracker = CharacterTracker.Instance;

        Vector2Int from = _properties.Movement.CurrentCoordinate;
        Vector2Int to = _currentTarget.Movement.CurrentCoordinate;
        Vector2Int[] nextPossibleCells = _map.GetNextPossibleCoordinates(from, to);
        for (int i = 0; i < nextPossibleCells.Length; i++)
        {
            if (tracker.IsMovableCell(nextPossibleCells[i], true))
            {
                _properties.Movement.MoveTo(nextPossibleCells[i]);
                _finishedTurn = true;
                return true;
            }
        }

        return false;
    }

    private async UniTaskVoid NextFrame_MoveTowardToOrIdle()
    {
        await UniTask.NextFrame();

        if (TryMoveTowardToCurrentTarget())
        {
            return;
        }

        Idle();
    }
    

    public void Attack()
    {
        if (!DoesCurrentTargetExist())
        {
            return;
        }

        int targetAttackRoll = _currentTarget.AttackRollValue;
        int damage;
        int attackInterract = (3 + _properties.AttackRollValue - targetAttackRoll) % 3;
        if (attackInterract == 0)
        {
            damage = 4;
        }
        else if (attackInterract == 1)
        {
            damage = 5;
        }
        else
        {
            damage = 3;
        }
        _properties.CurrentAttackDamage = damage;

        float animationDuration = _properties.Animator.DoAttackAnimation();
        Vector3 currentPos = _map.GetPosition(_properties.Movement.CurrentCoordinate);
        Vector3 attackPos = GetEngagePosition();
        Transform root = _properties.transform;
        var attackMoveSequence = DOTween.Sequence();
        attackMoveSequence.Append(root.DOMove(attackPos, animationDuration / 2f).SetEase(Ease.InBack));
        attackMoveSequence.Append(root.DOMove(currentPos, animationDuration / 2f).SetEase(Ease.InOutSine));
        Delay_ApplyDamage(damage, animationDuration / 2f).Forget();
    }
    private async UniTaskVoid Delay_ApplyDamage(int damage, float delay)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        _currentTarget.Behavior.BeAttacked(_properties, damage);
    }

    public void BeAttacked(CharacterProperties attacker, int damage)
    {
        if (_properties.CurrentHp <= 0)
        {
            return;
        }

        _onHit.Invoke();
        _properties.CurrentHp -= damage;
        if (_properties.CurrentHp <= 0)
        {
            Defeated();
        }
    }

    public void Defeated()
    {  
        _onACharacterDefeated.Raise(_properties, _properties.Movement.CurrentCoordinate);
        _properties.Defeated = true;
        float duration = _properties.Animator.DoDefeatedAnimation();
        Delay_Defeated(duration).Forget();
    }
    private async UniTaskVoid Delay_Defeated(float delay)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        _properties.transform.DOScale(0f, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _onDisappeared.Invoke();
            _properties.gameObject.SetActive(false);
        });
    }

    public void Idle()
    {
        _properties.Animator.DoIdleAnimation();
        _finishedTurn = true;
    }

    public Vector3 GetEngagePosition()
    {
        if (_currentTarget == null)
        {
            return GridMap.UNDEFINED_POSITON;
        }
        if (_engagePosition != GridMap.UNDEFINED_POSITON)
        {
            return _engagePosition;
        }

        Vector3 selfPosition = _map.GetPosition(_properties.Movement.CurrentCoordinate);
        bool isHeadToHeadTarget = _currentTarget.Behavior.CurrentTarget.GetInstanceID() == _properties.GetInstanceID();
        if (isHeadToHeadTarget)
        {
            Vector3 targetPosition = _map.GetPosition(_currentTarget.Movement.CurrentCoordinate);
            Vector3 centerPoint = (targetPosition + selfPosition) / 2f;
            // engage positionX: ---engage1-------center-------engage2---
            _engagePosition.x = centerPoint.x - _properties.Animator.GetFaceDirection() * (_map.CellSize.x / 2f) * 0.7f;
            _engagePosition.y = centerPoint.y;
            _engagePosition.z = 0f;
            return _engagePosition;
        }

        Vector3 targetEngagePosition = _currentTarget.Behavior.GetEngagePosition();
        Vector3 direction = targetEngagePosition - selfPosition;
        _engagePosition = selfPosition + direction * 0.7f;
        return _engagePosition;
    }

    private bool DoesCurrentTargetExist()
    {
        return _currentTarget != null && !_currentTarget.Defeated;
    }

    private void OnDrawGizmos()
    {
        if (_currentTarget == null || _properties.Team == CharacterProperties.TeamId.Attack)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
    }
}
