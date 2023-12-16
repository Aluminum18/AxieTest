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

    [Header("Configs")]
    [SerializeField]
    private CharacterProperties _properties;

    [Header("Inspec")]
    [SerializeField]
    private CharacterProperties _currentTarget;
    public CharacterProperties CurrentTarget => _currentTarget;

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
            return;
        }
        if (_properties.Team == CharacterProperties.TeamId.Defense)
        {
            Idle();
            return;
        }
        // Move step should be executed after attack step to prevent attacking a moving character
        NextFrame_MoveTowardToOrIdle(movement.CurrentCoordinate, targetMovement.CurrentCoordinate).Forget();
    }
    private async UniTaskVoid NextFrame_MoveTowardToOrIdle(Vector2Int from, Vector2Int to)
    {
        await UniTask.NextFrame();

        Vector2Int[] nextPossibleCells = _map.GetNextPossibleCoordinates(from, to);
        var tracker = CharacterTracker.Instance;
        for (int i = 0; i < nextPossibleCells.Length; i++)
        {
            if (tracker.IsMovableCell(nextPossibleCells[i]))
            {
                _properties.Movement.MoveTo(nextPossibleCells[i]);
                return;
            }
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
        _properties.transform.DOScale(0f, 0.3f).SetEase(Ease.InOutSine).OnComplete(() => _properties.gameObject.SetActive(false));
    }

    public void Idle()
    {
        _properties.Animator.DoIdleAnimation();
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
}
