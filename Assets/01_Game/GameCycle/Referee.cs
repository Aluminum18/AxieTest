using Cysharp.Threading.Tasks;
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

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _attackerRatio;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onPregame;
    [SerializeField]
    private GameEvent _onDecidedStartNextTurn;
    [SerializeField]
    private GameEvent _onAttackerWon;
    [SerializeField]
    private GameEvent _onDefenderWon;

    public void StartFirstTurn()
    {
        CalculateAttackerRatio();
        Delay_StartFirstTurn().Forget();
    }

    private async UniTaskVoid Delay_StartFirstTurn()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        _onDecidedStartNextTurn.Raise();
    }

    public void CheckTeamAppearance()
    {
        CalculateAttackerRatio();
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

    public void SetupPregame()
    {
        _onPregame.Raise();
    }

    private void CalculateAttackerRatio()
    {
        _attackerRatio.Value = (float)_attackerCount.Value / (_attackerCount.Value + _defenderCount.Value);
    }
}
