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
    [field: SerializeField]
    public CharacterMovement Movement { get; private set; }
    [field: SerializeField]
    public CharacterBehavior Behavior { get; private set; }
    [field: SerializeField]
    public CharacterAnimator Animator { get; private set; }

    [field: Header("Runtime Setup")]
    [field: SerializeField]
    public int CurrentHp { get; set; }
    [field: SerializeField]
    public int AttackRollValue { get; private set; }

    public enum TeamId
    {
        Attack = 0,
        Defense = 1
    }

    private void OnEnable()
    {
        CurrentHp = MaxHp.Value;
        AttackRollValue = Random.Range(0, 3);
    }
}
