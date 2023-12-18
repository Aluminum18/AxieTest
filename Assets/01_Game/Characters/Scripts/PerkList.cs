using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AxieTest/Character/PerkList", fileName = "PerkList")]
public class PerkList : ScriptableObject
{
    [SerializeField]
    private BasePerk[] _perks;

    public void TryConsumePerks(CharacterProperties consumer)
    {
        for (int i = 0; i < _perks.Length; i++)
        {
            _perks[i].Consume(consumer);
        }
    }
}
