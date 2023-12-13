using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour
{
    [field: SerializeField]
    public TeamId Team { get; private set; }
    [field: SerializeField]
    public IntegerVariable MaxHp { get; private set; }
    [SerializeField]
    private IntegerVariable _currentHp;
    [SerializeField]
    public int CurrentHp
    {
        get
        {
            return _currentHp.Value;
        }
        set
        {
            _currentHp.Value = value;
        }
    }
    [SerializeField]
    private IntegerVariable _attackRollValue;
    public int AttackRollValue
    {
        get
        {
            if (_attackRollValue == null)
            {
                _attackRollValue.Value = Random.Range(0, 3);
            }
            return _attackRollValue.Value;
        }
    }
    [field: SerializeField]
    public CharacterMovement Movement { get; private set; }
    [field: SerializeField]
    public CharacterBehavior Behavior { get; private set; }

    public enum TeamId
    {
        Attack = 0,
        Defense = 1
    }

    private void OnEnable()
    {
        CurrentHp = MaxHp.Value;
    }
}
