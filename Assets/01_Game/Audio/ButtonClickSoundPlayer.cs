using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private Button _targetButton;
    [SerializeField]
    private GameEvent _soundEvent;

    private void OnEnable()
    {
        _targetButton.onClick.AddListener(RaiseSoundEvent);
    }

    private void OnDisable()
    {
        _targetButton.onClick.RemoveListener(RaiseSoundEvent);
    }

    private void RaiseSoundEvent()
    {
        _soundEvent.Raise();

    }
}
