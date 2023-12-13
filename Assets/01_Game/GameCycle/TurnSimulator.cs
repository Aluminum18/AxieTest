using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSimulator : MonoBehaviour
{
    [SerializeField]
    private IntervalAction _interval;

    public void StartTurnSimulator()
    {
        _interval.StartIntervalAction();
    }

    public void StopSimulator()
    {
        _interval.StopIntervalAction();
    }
}
