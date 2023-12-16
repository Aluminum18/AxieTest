using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _hpShownOnPanel;
    [SerializeField]
    private IntegerVariable _attackRollShownOnPanel;

    [field: Header("Configs")]
    [field: SerializeField]
    public TeamId Team { get; private set; }
    [field: SerializeField]
    public IntegerVariable MaxHp { get; private set; }
    [SerializeField]
    private IntegerVariableToSlider _hpSlider;
    [field: SerializeField]
    public CharacterMovement Movement { get; private set; }
    [field: SerializeField]
    public CharacterBehavior Behavior { get; private set; }
    [field: SerializeField]
    public CharacterAnimator Animator { get; private set; }

    [field: Header("Runtime Setup")]
    [SerializeField]
    private IntegerVariable _currentHp;
    public int CurrentHp
    {
        get
        {
            if (_currentHp == null)
            {
                _currentHp = ScriptableObject.CreateInstance<IntegerVariable>();
                _currentHp.Value = MaxHp.Value;
            }
            return _currentHp.Value;
        }
        set
        {
            if (_currentHp == null)
            {
                _currentHp = ScriptableObject.CreateInstance<IntegerVariable>();
                _currentHp.Value = MaxHp.Value;
            }
            _currentHp.Value = value;
        }
    }

    [field: SerializeField]
    public int AttackRollValue { get; private set; }
    [field: SerializeField]
    public bool Defeated { get; set; }

    public enum TeamId
    {
        Attack = 0,
        Defense = 1
    }

    public void ExposeProperties()
    {

    }

    private void OnEnable()
    {
        CurrentHp = MaxHp.Value;
        Defeated = false;
        AttackRollValue = Random.Range(0, 3);
        _hpSlider.Init(_currentHp);
    }
}
