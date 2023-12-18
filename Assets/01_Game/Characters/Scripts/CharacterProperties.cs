using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterProperties : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _hpShownOnPanel;
    [SerializeField]
    private IntegerVariable _maxHpShownOnPanel;
    [SerializeField]
    private IntegerVariable _attackRollShownOnPanel;
    [SerializeField]
    private IntegerVariable _attackDamageShownOnPanel;
    [SerializeField]
    private StringVariable _characterIdShownOnPanel;

    [SerializeField]
    private UnityEvent _onAppeared;

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
            _currentHp.Value = Mathf.Max(0, value);
            if (_isExposeProperties)
            {
                _hpShownOnPanel.Value = _currentHp.Value;
            }
        }
    }

    [field: SerializeField]
    public int AttackRollValue { get; private set; }
    [SerializeField]
    private int _currentAttackDamage;
    public int CurrentAttackDamage
    {
        get
        {
            return _currentAttackDamage;
        }
        set
        {
            _currentAttackDamage = value;
            if (_isExposeProperties)
            {
                _attackDamageShownOnPanel.Value = _currentAttackDamage;
            }
        }
    }
    [field: SerializeField]
    public bool Defeated { get; set; }

    private bool _isExposeProperties = false;

    public enum TeamId
    {
        Attack = 0,
        Defense = 1
    }

    public void ShowVisual()
    {
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
        _onAppeared.Invoke();
    }

    public void ExposeProperties()
    {
        _isExposeProperties = true;

        _characterIdShownOnPanel.Value = Animator.AxieId;
        _hpShownOnPanel.Value = CurrentHp;
        _maxHpShownOnPanel.Value = MaxHp.Value;

        _attackRollShownOnPanel.Value = AttackRollValue;
        _attackDamageShownOnPanel.Value = CurrentAttackDamage;
    }

    public void StopExposeProperties()
    {
        _isExposeProperties = false;
    }

    private void OnEnable()
    {
        CurrentHp = MaxHp.Value;
        Defeated = false;
        AttackRollValue = Random.Range(0, 3);
        _hpSlider.Init(_currentHp);
    }
}
