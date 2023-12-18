using UnityEngine;

public interface IPerk
{
    void Consume(CharacterProperties consumer);
}

[CreateAssetMenu(menuName = "AxieText/Character/AttackPerk", fileName = "AttackPerk")]
public class AttackPerk : ScriptableObject, IPerk
{
    [SerializeField]
    private float _chance;
    [SerializeField]
    private int _bonusAttack;

    public void Consume(CharacterProperties consumer)
    {
        float roll = Random.Range(0f, 1f);
        if (1f - _chance < roll)
        {
            return;
        }

        consumer.CurrentAttackDamage += _bonusAttack;
    }
}

[CreateAssetMenu(menuName = "AxieText/Character/DefensePerk", fileName = "AttackDefensePerk")]
public class DefensePerk : ScriptableObject, IPerk
{
    [SerializeField]
    private float _chance;
    [SerializeField]
    private int _bonusAttack;

    public void Consume(CharacterProperties consumer)
    {
        throw new System.NotImplementedException();
    }
}