using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToSlider : MonoBehaviour
{
    public FloatVariable _floatVariable;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private bool _normalizedValue = false;
    [SerializeField]
    private bool _continuousUpdate = false;

    private float _peekValue = 1f;
    private Tweener _valueTweener;

    public void Init(FloatVariable refVariable, float maxValue = 100f)
    {
        _slider.maxValue = maxValue;
        _floatVariable = refVariable;
        _floatVariable.OnValueChange += UpdateSliderValue;
        UpdateSliderValue(_floatVariable.Value);
    }

    private void Start()
    {
        if (_floatVariable == null)
        {
            return;
        }

        _floatVariable.OnValueChange += UpdateSliderValue;
        UpdateSliderValue(_floatVariable.Value);
    }

    private void OnDestroy()
    {
        if (_floatVariable == null)
        {
            return;
        }

        _floatVariable.OnValueChange -= UpdateSliderValue;
    }

    private void UpdateSliderValue(float newValue)
    {
        float nextValue = newValue;
        if (_normalizedValue)
        {
            nextValue = NormalizeValue(newValue);
        }
        if (_continuousUpdate)
        {
            _valueTweener?.Kill();
            float currentValue = _slider.value;
            _valueTweener = DOTween.To(() => currentValue, val => currentValue = val, nextValue, 0.5f).OnUpdate(() =>
            {
                _slider.value = currentValue;
            });

            return;
        }

        _slider.value = nextValue;
    }

    private float NormalizeValue(float newValue)
    {
        _slider.maxValue = 1f;
        if (_peekValue < newValue)
        {
            _peekValue = newValue;
        }

        return newValue / _peekValue;
    }
}