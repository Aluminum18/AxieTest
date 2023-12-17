using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class FloatVariableToText : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _floatVariable;
    [SerializeField]
    private TMP_Text _textMesh;
    [SerializeField]
    private bool _continuousChange = false;
    [SerializeField]
    private float _unitChangePerSec = 5f;
    [SerializeField]
    private float _continuousChangeCapTime = 1f;
    [SerializeField]
    private string _format = "0.0";
    [SerializeField]
    private string _additionFormat = "{0}";
    [SerializeField]
    private bool _convertToDateTime;
    [SerializeField]
    private string _dateTimeFormat = "hh\\:mm\\:ss\\:fff";

    private CancellationTokenSource _updateToken;

    private float _lastValue;

    private void Start()
    {
        SetValueText(_floatVariable.Value);
        _floatVariable.OnValueChange += UpdateValue;
    }

    private void OnDestroy()
    {
        _floatVariable.OnValueChange -= UpdateValue;
    }

    private void SetValueText(float value)
    {
        if (_convertToDateTime)
        {
            var ts = System.TimeSpan.FromSeconds(value);
            _textMesh.text = string.Format(_additionFormat, ts.ToString(@_dateTimeFormat));
            return;
        }
        _textMesh.text = string.Format(_additionFormat, value.ToString(_format));
    }

    private void UpdateValue(float newValue)
    {
        if (_continuousChange)
        {
            ContinuousUpdateValue(newValue).Forget();
            return;
        }

        SetValueText(_floatVariable.Value);
    }

    private async UniTaskVoid ContinuousUpdateValue(float newValue)
    {
        float diff = newValue - _lastValue;

        float expectedTime = Mathf.Abs(diff) / _unitChangePerSec;

        float changePerSec = _unitChangePerSec * Mathf.Sign(diff);
        if (expectedTime > _continuousChangeCapTime)
        {
            changePerSec = diff / _continuousChangeCapTime;
            expectedTime = _continuousChangeCapTime;
        }

        float bufferValue = _lastValue;
        _lastValue = newValue;

        if (_updateToken != null)
        {
            _updateToken.Cancel();
        }
        _updateToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            bufferValue += changePerSec * Time.deltaTime;
            if (expectedTime < 0f)
            {
                SetValueText(_floatVariable.Value);
                return;
            }
            SetValueText(bufferValue);
            expectedTime -= Time.deltaTime;
        }
    }
}
