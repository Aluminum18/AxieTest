using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoSingleton<CharacterTracker>
{
    [SerializeField]
    private GridMap _gridMap;
    [SerializeField]
    private List<CharacterProperties> _defenseTeam;

    public CharacterProperties FindNearestTarget(CharacterMovement from)
    {
        int nearestDistance = int.MaxValue;
        CharacterProperties nearestTarget = null;
        for (int i = 0; i < _defenseTeam.Count; i++)
        {
            var defender = _defenseTeam[i].Movement;
            int distance = Mathf.Abs(from.CurrentCoordinate.x - defender.CurrentCoordinate.x) + Mathf.Abs(from.CurrentCoordinate.y - defender.CurrentCoordinate.y);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = _defenseTeam[i];
            }
        }

        return nearestTarget;
    }

    public void RegisterDefender(CharacterProperties defender)
    {
        _defenseTeam.Add(defender);
    }
}
