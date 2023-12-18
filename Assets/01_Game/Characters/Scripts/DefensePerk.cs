using UnityEngine;

[CreateAssetMenu(menuName = "AxieTest/Character/DefensePerk", fileName = "DefensePerk")]
public class DefensePerk : BasePerk
{
    [SerializeField]
    private float _chance;

    public override void Consume(CharacterProperties consumer)
    {
        float roll = Random.Range(0f, 1f);
        if (roll < 1f - _chance)
        {
            return;
        }
        consumer.Behavior.Evade = true;
    }
}