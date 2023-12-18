using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamStartFormation", menuName = "AxieTest/SetUp/TeamFormation")]
public class TeamStartFormation : ScriptableObject
{
    [SerializeField]
    private List<FormationDescription> _attackTeam;
    public FormationDescription[] AttackTeam => _attackTeam.ToArray();
    [SerializeField]
    private List<FormationDescription> _defenseTeam;
    public FormationDescription[] DefenseTeam => _defenseTeam.ToArray();
}

[System.Serializable]
public struct FormationDescription
{
    public Vector2Int start;
    public Vector2Int end;
}
