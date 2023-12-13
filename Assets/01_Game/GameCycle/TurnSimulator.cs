using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSimulator : MonoBehaviour
{
    [SerializeField]
    private IntervalAction _interval;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onStartedATurn;
    [SerializeField]
    private GameEvent _onMiddleTurn;
    [SerializeField]
    private GameEvent _onEndedATurn;

    public void StartTurnSimulator()
    {
        _interval.StartIntervalAction();
    }

    public void StopSimulator()
    {
        _interval.StopIntervalAction();
    }

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
