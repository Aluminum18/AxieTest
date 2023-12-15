using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleControl : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _timeScale;

    [Header("Configs")]
    [SerializeField]
    private Vector2 _timeScaleClamp;

    private float _beforePauseTimeScale = 1f;

    public void TogglePause()
    {
        if (Time.timeScale != 0f)
        {
            _beforePauseTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            _timeScale.SetValueWithoutNotify(0f);
            return;
        }

        Time.timeScale = _beforePauseTimeScale;
        _timeScale.SetValueWithoutNotify(_beforePauseTimeScale);
    }

    private void UpdateTimeScale(float timeScale)
    {
        if (Time.timeScale == 0f) // Prevent adjust timescale when pause
        {
            _timeScale.SetValueWithoutNotify(0f);
            return;
        }
        Time.timeScale = Mathf.Clamp(timeScale, _timeScaleClamp.x, _timeScaleClamp.y);
        _timeScale.SetValueWithoutNotify(Time.timeScale);
    }

    private void OnEnable()
    {
        _timeScale.OnValueChange += UpdateTimeScale;
    }

    private void OnDisable()
    {
        _timeScale.OnValueChange -= UpdateTimeScale;
    }
}
