using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class IntegerVariableToText : MonoBehaviour
{
    [SerializeField]
    private IntegerVariable _intVariable;
    [SerializeField]
    private TMP_Text _textMesh;
    [SerializeField]
    private bool _continueUpdate = true;
    [SerializeField]
    private bool _continuousChange = false;
    [SerializeField]
    private int _unitChangePerSec = 5;
    [SerializeField]
    private float _continuousChangeCapTime = 1f;
    [SerializeField]
    private string _format = "{0}";

    private CancellationTokenSource _updateToken;

    private int _lastValue = 0;
    private void OnEnable()
    {
        UpdateValue(_intVariable.Value);
        
        if (_continueUpdate)
        {
            _intVariable.OnValueChange += UpdateValue;
        }
    }

    private void OnDisable()
    {
        if (_continueUpdate)
        {
            _intVariable.OnValueChange -= UpdateValue;
        }
    }

    private void UpdateValue(int newValue)
    {
        if (_continuousChange)
        {
            ContinuousUpdateValue(newValue).Forget();
            return;
        }
        _textMesh.text = string.Format(_format, newValue.ToString());
    }

    private async UniTaskVoid ContinuousUpdateValue(int newValue)
    {
        int diff = newValue - _lastValue;

        float expectedTime = (float)(Mathf.Abs(diff)) / _unitChangePerSec;

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
                _textMesh.text = string.Format(_format, newValue.ToString());
                return;
            }
            _textMesh.text = string.Format(_format, ((int)bufferValue).ToString());
            expectedTime -= Time.deltaTime;
        }
    }
}
