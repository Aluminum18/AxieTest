using Cysharp.Threading.Tasks;
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

    public void ScanTarget()
    {
        if (DoesCurrentTargetExist())
        {
            return;
        }

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

    private bool DoesCurrentTargetExist()
    {
        return _currentTarget != null && _currentTarget.isActiveAndEnabled;
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
        float animationDuration = _properties.Animator.DoAttackAnimation();
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

        _currentTarget = attacker;
        _properties.CurrentHp -= damage;
        if (_properties.CurrentHp <= 0)
        {
            Defeated();
        }
    }

    public void Defeated()
    {  
        _onACharacterDefeated.Raise(_properties, _properties.Movement.CurrentCoordinate);
        _properties.gameObject.SetActive(false);
    }

    public void PairTarget(CharacterProperties requester)
    {
        _currentTarget = requester;
        _properties.Movement.LookAt(requester.Movement.CurrentCoordinate);
    }

    public void Idle()
    {
        _properties.Animator.DoIdleAnimation();
    }
}
