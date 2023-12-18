using UnityEngine;

[CreateAssetMenu(menuName = "AxieTest/Character/AttackPerk", fileName = "AttackPerk")]
public class AttackPerk : BasePerk
{
    [SerializeField]
    private float _chance;
    [SerializeField]
    private int _bonusAttack;

    public override void Consume(CharacterProperties consumer)
    {
        float roll = Random.Range(0f, 1f);
        if (roll < 1f - _chance)
        {
            return;
        }

        consumer.CurrentAttackDamage += _bonusAttack;
    }
}