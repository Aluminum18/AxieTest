using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _maxHp;

    [Header("Configs")]
    [SerializeField]
    private CharacterProperties _properties;

    [Header("Inspec")]
    [SerializeField]
    private int _attackRoll;
    public int AttackRoll => _attackRoll;
    [SerializeField]
    private CharacterBehavior _currentTarget;

    public void ScanTarget()
    {
        if (_currentTarget.gameObject.activeSelf)
        {
            return;
        }

        _currentTarget = CharacterTracker.Instance.FindNearestTarget(_properties.Movement).Behavior;
    }

    public void RollAttackNumber()
    {
        _attackRoll = Random.Range(0, 3);
    }

    public void Attack()
    {

    }

    public void BeAttack(int damage)
    {
        _properties.CurrentHp -= damage;
    }
}
