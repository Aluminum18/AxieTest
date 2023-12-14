using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ContinuousSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _referenceSlider;
    [SerializeField]
    private Slider _continuousSlider;
    [SerializeField][Tooltip("unit per second")]
    private float _changeSpeed = 0.5f;
    [SerializeField]
    private float _delay = 0.25f;

    private float _peekValue;
    private bool _isReducing = false;

    private void TweenSlider(float newValue)
    {
        if (_peekValue < newValue)
        {
            _peekValue = newValue;
        }

        float nextValue = newValue / _peekValue;
        if (_continuousSlider.value < nextValue)
        {
            _continuousSlider.value = nextValue;
            _isReducing = false;
            return;
        }
        Delay_TweenSlider().Forget();
    }

    private async UniTaskVoid Delay_TweenSlider()
    {
        if (_isReducing)
        {
            return;
        }

        _isReducing = true;
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        PerFrame_ReducePolishSlider().Forget();
    }

    private CancellationTokenSource _reduceSliderToken;
    private async UniTaskVoid PerFrame_ReducePolishSlider()
    {
        _reduceSliderToken?.Cancel();
        _reduceSliderToken = new CancellationTokenSource();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_reduceSliderToken.Token))
        {
            _continuousSlider.value -= Time.deltaTime * _changeSpeed;
            if (_continuousSlider.value <= _referenceSlider.value)
            {
                _continuousSlider.value = _referenceSlider.value;
                _reduceSliderToken.Cancel();
                _isReducing = false;
                break;
            }
        }
    }

    private void Start()
    {
        _continuousSlider.value = 1f;
    }

    private void OnEnable()
    {
        _peekValue = _referenceSlider.maxValue;
        _referenceSlider.onValueChanged.AddListener(TweenSlider);
    }

    private void OnDisable()
    {
        _referenceSlider.onValueChanged.RemoveListener(TweenSlider);
    }

}
