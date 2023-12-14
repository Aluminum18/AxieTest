using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSimulator : MonoBehaviour
{
    [Header("Events out")]
    [SerializeField]
    private GameEvent _onStartedATurn;
    [SerializeField]
    private GameEvent _onMiddleTurn;
    [SerializeField]
    private GameEvent _onEndedATurn;

    public void StartATurn()
    {
        _onStartedATurn.Raise();
    }

    public void StartMiddleTurn()
    {
        _onMiddleTurn.Raise();
    }

    public void EndTurn()
    {
        _onEndedATurn.Raise();
    }
}
