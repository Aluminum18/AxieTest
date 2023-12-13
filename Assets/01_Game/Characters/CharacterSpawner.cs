using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField]
    private GridMap _gridMap;
    [SerializeField]
    private TeamStartFormation _teamFormation;
    [SerializeField]
    private GameObject _attackerTemplate;
    [SerializeField]
    private GameObject _defenderTemplate;

    public void SpawnStartFormation()
    {
        SpawnCharacterFollowingFormation(_teamFormation.AttackTeam, _attackerTemplate);
        SpawnCharacterFollowingFormation(_teamFormation.DefenseTeam, _defenderTemplate);
    }
    
    private void SpawnCharacterFollowingFormation(FormationDescription[] formation, GameObject characterTemplate)
    {
        CharacterTracker tracker = CharacterTracker.Instance;
        for (int i = 0; i < formation.Length; i++)
        {
            var formationPart = formation[i];
            for (int y = formationPart.start.y; y <= formationPart.end.y; y++)
            {
                for (int x = formationPart.start.x; x <= formationPart.end.x; x++)
                {
                    var character = Instantiate(characterTemplate, _gridMap.GetPosition(new Vector2Int(x, y)), Quaternion.identity);
                    if (characterTemplate.GetInstanceID() != _defenderTemplate.GetInstanceID())
                    {
                        continue;
                    }
                    tracker.RegisterDefender(character.GetComponent<CharacterProperties>());
                }
            }
        }
    }
}
