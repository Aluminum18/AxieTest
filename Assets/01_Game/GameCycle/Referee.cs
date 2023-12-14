using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _attackerCount;
    [SerializeField]
    private IntegerVariable _defenderCount;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onDecidedStartNextTurn;
    [SerializeField]
    private GameEvent _onAttackerWon;
    [SerializeField]
    private GameEvent _onDefenderWon;

    public void CheckTeamAppearance()
    {
        if (_attackerCount.Value == 0)
        {
            _onDefenderWon.Raise();
            return;
        }
        if ( _defenderCount.Value == 0)
        {
            _onAttackerWon.Raise();
            return;
        }

        _onDecidedStartNextTurn.Raise();
    }
}
