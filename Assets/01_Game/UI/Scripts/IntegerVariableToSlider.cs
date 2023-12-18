using UnityEngine;
using UnityEngine.UI;

public class IntegerVariableToSlider : MonoBehaviour
{
    public IntegerVariable _floatVariable;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private bool _normalizedValue = false;
    [SerializeField]
    private bool _invert;

    private float _peekValue = 1f;

    public void Init(IntegerVariable refVariable, float maxValue = 100f)
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

    private void UpdateSliderValue(int newValue)
    {
        if (_normalizedValue)
        {
            NormalizeValue(newValue);
            return;

        }

        _slider.value = _invert ? _slider.maxValue - newValue : newValue;
    }

    private void NormalizeValue(int newValue)
    {
        _slider.maxValue = 1f;
        if (_peekValue < newValue)
        {
            _peekValue = newValue;
        }

        float normalizedValue = newValue / _peekValue;
        _slider.value = _invert ? _slider.maxValue - normalizedValue : normalizedValue;
    }
}
