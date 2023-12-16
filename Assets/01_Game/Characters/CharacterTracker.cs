using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoSingleton<CharacterTracker>
{
    [Header("Reference - Read")]
    [SerializeField]
    private GridMap _gridMap;
    [SerializeField]
    private Vector3Variable _touchGroundPoint;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _attackTeamCount;
    [SerializeField]
    private IntegerVariable _defenseTeamCount;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onACharacterMoved;
    [SerializeField]
    private GameEvent _onACharacterDissapeared;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onAllCharacterFinishedSetUp;
    [SerializeField]
    private GameEvent _onTouchedACharacter;

    [Header("Inspec")]
    [SerializeField]
    private Vector2Int _touchCell;
    [SerializeField]
    private List<CharacterProperties> _defenseTeam;
    [SerializeField]
    private List<CharacterProperties> _attackTeam;

    private int _readyCharacterCount = 0;

    private Dictionary<Vector2Int, CharacterProperties> _characterMap = new Dictionary<Vector2Int, CharacterProperties>();

    public void CountReadyCharacter()
    {
        _readyCharacterCount++;
        if (_readyCharacterCount == _attackTeam.Count + _defenseTeam.Count)
        {
            _onAllCharacterFinishedSetUp.Raise();
        }
    }

    public CharacterProperties FindNearestDefender(CharacterMovement from)
    {
        int nearestDistance = int.MaxValue;
        CharacterProperties nearestTarget = null;
        for (int i = 0; i < _defenseTeam.Count; i++)
        {
            if (_defenseTeam[i].Defeated)
            {
                continue;
            }

            var defenderMovement = _defenseTeam[i].Movement;
            int distance = Mathf.Abs(from.CurrentCoordinate.x - defenderMovement.CurrentCoordinate.x) + Mathf.Abs(from.CurrentCoordinate.y - defenderMovement.CurrentCoordinate.y);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = _defenseTeam[i];
            }
        }

        return nearestTarget;
    }

    public CharacterProperties FindTargetAround(Vector2Int findAround, CharacterProperties.TeamId targetTeam)
    {
        Vector2Int[] candidateCoorinate = new Vector2Int[4];
        candidateCoorinate[0] = findAround + Vector2Int.up;
        candidateCoorinate[1] = findAround + Vector2Int.right;
        candidateCoorinate[2] = findAround + Vector2Int.down;
        candidateCoorinate[3] = findAround + Vector2Int.left;
        CharacterProperties candidate;
        for (int i = 0; i < candidateCoorinate.Length; i++)
        {
            _characterMap.TryGetValue(candidateCoorinate[i], out candidate);
            if (candidate != null && !candidate.Defeated && candidate.Team == targetTeam)
            {
                return candidate;
            }
        }

        return null;
    }

    public bool IsMovableCell(Vector2Int coordinate)
    {
        if (_gridMap.MapSize.x <= coordinate.x || _gridMap.MapSize.y <= coordinate.y)
        {
            return false;
        }

        _characterMap.TryGetValue(coordinate, out var character);
        return character == null || character.Defeated;
    }

    public void UpdateCoordinate(Vector2Int from, Vector2Int to, CharacterProperties mover)
    {
        _characterMap.TryGetValue(from, out var character);
        if (character == null)
        {
            return;
        }
        _characterMap.TryGetValue(to, out var toCellCharacter);
        if (toCellCharacter != null && !toCellCharacter.Defeated)
        {
            Debug.LogError($"Invalid move from [{from}] to [{to}]");
            return;
        }
        _characterMap[from] = null;
        _characterMap[to] = character;
    }

    public void UpdateCharacterDissapearance(CharacterProperties character, Vector2Int coordinate) 
    {
        _characterMap[coordinate] = null;
        if (character.Team == CharacterProperties.TeamId.Defense)
        {
            _defenseTeamCount.Value--;
            return;
        }

        _attackTeamCount.Value--;
    }

    public void RegisterDefender(CharacterProperties defender, Vector2Int coordinate)
    {
        _defenseTeam.Add(defender);
        _characterMap.Add(coordinate, defender);
        _defenseTeamCount.Value++;
    }

    public void RegisterAttacker(CharacterProperties attacker, Vector2Int coordinate)
    {
        _attackTeam.Add(attacker);
        _characterMap.Add(coordinate, attacker);
        _attackTeamCount.Value++;
    }

    private void UpdateCharacterCoordinate(object[] eventParam)
    {
        if (eventParam.Length < 3)
        {
            return;
        }
        CharacterProperties character = (CharacterProperties)eventParam[0];
        Vector2Int from = (Vector2Int)eventParam[1];
        Vector2Int to = (Vector2Int)eventParam[2];
        UpdateCoordinate(from, to, character);
    }

    private void UpdateCharacterDissapearance(object[] eventParam)
    {
        if (eventParam.Length < 2)
        {
            return;
        }
        CharacterProperties character = (CharacterProperties)eventParam[0];
        Vector2Int coordinate = (Vector2Int)eventParam[1];
        UpdateCharacterDissapearance(character, coordinate);
    }

    private void UpdateTouchedCell(Vector3 touchGroundPoint)
    {
        _characterMap.TryGetValue(_touchCell, out var previous);
 
        _touchCell = _gridMap.GetCoordinate(touchGroundPoint);
        _characterMap.TryGetValue(_touchCell, out var character);
        if (character == null || character.Defeated)
        {
            return;
        }
        if (previous != null)
        {
            previous.StopExposeProperties();
        }
        character.ExposeProperties();
        _onTouchedACharacter.Raise();
    }

    private void OnEnable()
    {
        _onACharacterMoved.Subcribe(UpdateCharacterCoordinate);
        _onACharacterDissapeared.Subcribe(UpdateCharacterDissapearance);
        _touchGroundPoint.OnValueChange += UpdateTouchedCell;
    }
    private void OnDisable()
    {
        _onACharacterMoved.Unsubcribe(UpdateCharacterCoordinate);
        _onACharacterDissapeared.Unsubcribe(UpdateCharacterDissapearance);
        _touchGroundPoint.OnValueChange -= UpdateTouchedCell;
    }

    
}
