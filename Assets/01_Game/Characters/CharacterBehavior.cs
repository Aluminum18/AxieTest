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
        if (_currentTarget != null && _currentTarget.isActiveAndEnabled)
        {
            return;
        }
        var tracker = CharacterTracker.Instance;

        if (_properties.Team == CharacterProperties.TeamId.Defense)
        {
            return;
        }

        _currentTarget = tracker.FindNearestDefender(_properties.Movement);
        _currentTarget.Behavior.PairTarget(_properties);
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
            return;
        }
        // Move step should be executed after attack step to prevent attacking a moving character
        NextFrame_MoveTowardTo(movement.CurrentCoordinate, targetMovement.CurrentCoordinate).Forget();
    }
    private async UniTaskVoid NextFrame_MoveTowardTo(Vector2Int from, Vector2Int to)
    {
        await UniTask.NextFrame();

        Vector2Int[] nextPossibleCells = _map.GetNextPossibleCoordinates(from, to);
        var tracker = CharacterTracker.Instance;
        for (int i = 0; i < nextPossibleCells.Length; i++)
        {
            if (tracker.IsMovableCell(nextPossibleCells[i]))
            {
                _properties.Movement.MoveTo(nextPossibleCells[i]);
                break;
            }
        }
    }

    public void Attack()
    {
        if (_currentTarget == null || !_currentTarget.isActiveAndEnabled)
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
        _currentTarget.Behavior.BeAttacked(_properties, damage);
    }

    public void BeAttacked(CharacterProperties attacker, int damage)
    {
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
}
