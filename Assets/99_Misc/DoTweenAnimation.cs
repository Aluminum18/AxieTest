using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class DoTweenAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _doTweenOnEnable = false;
    [SerializeField]
    private bool _allowSeftInteruptTween = false;
    [SerializeField]
    private bool _backToOriginWhenFinishTween = false;
    [SerializeField]
    private GameObject _go;
    [SerializeField]
    private AnimType _animType;
    [SerializeField]
    private Ease _easeType;
    [SerializeField]
    private float _duration;
    public float Duration
    {
        set
        {
            _duration = value;
        }
    }
    [SerializeField]
    private bool _useCurrentAsFrom = false;
    [SerializeField]
    private Vector3 _from;
    public Vector3 From
    {
        set
        {
            _from = value;
        }
    }
    [SerializeField]
    private Vector3 _to;
    public Vector3 To
    {
        set
        {
            _to = value;
        }
    }

    [SerializeField]
    private UnityEvent _onStartTween;
    [SerializeField]
    private UnityEvent _onFinishTween;

    [Header("Inspec")]
    [SerializeField]
    private bool _isTweening = false;
    private Tweener _tweener;

    public enum AnimType
    {
        None = 0,
        Move,
        Zoom,
        FadeSprite,
        FadeUI,
        MoveLocal,
        RotateLocal,
        ShakeMove,
        ShakeScale,
    }

    public async UniTaskVoid DoTweenWithDelay(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        DoTween();
    }

    public void DoTween()
    {
        if (_go == null)
        {
            _go = gameObject;
        }

        if (!_allowSeftInteruptTween && _isTweening)
        {
            return;
        }

        _tweener?.Kill();
        _onStartTween.Invoke();
        _isTweening = true;

        switch (_animType)
        {
            case AnimType.Move:
                {
                    if (!_useCurrentAsFrom)
                    {
                        _go.transform.position = _from;
                    }

                    _tweener = _go.transform.DOMove(_to, _duration).SetEase(_easeType).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.position = _from;
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.MoveLocal:
                {
                    if (!_useCurrentAsFrom)
                    {
                        _go.transform.localPosition = _from;
                    }
                    _tweener = _go.transform.DOLocalMove(_to, _duration).SetEase(_easeType).OnComplete(() => {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.localPosition = _to;
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.Zoom:
                {
                    if (!_useCurrentAsFrom)
                    {
                        _go.transform.localScale = _from;
                    }
                    _tweener = _go.transform.DOScale(_to, _duration).SetEase(_easeType).OnComplete( () => {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.localScale = _from;
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.FadeSprite:
                {
                    var spriteRenderer = _go.GetComponent<SpriteRenderer>();

                    if (spriteRenderer == null)
                    {
                        return;
                    }
                    if (!_useCurrentAsFrom)
                    {
                        Color bufferColor = spriteRenderer.color;
                        bufferColor.a = _from.x;
                        spriteRenderer.color = bufferColor;
                    }

                    _tweener = spriteRenderer.DOFade(_to.x, _duration).SetEase(_easeType).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, _from.x);
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.FadeUI:
                {
                    var canvasGroup = _go.GetComponent<CanvasGroup>();

                    if (canvasGroup == null)
                    {
                        Debug.LogWarning("To use FadeUI, add CanvasGroup component to the target");
                        return;
                    }

                    if (!_useCurrentAsFrom)
                    {
                        canvasGroup.alpha = _from.x;
                    }

                    _tweener = canvasGroup.DOFade(_to.x, _duration).SetEase(_easeType).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            canvasGroup.alpha = _from.x;
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.RotateLocal:
                {
                    if (!_useCurrentAsFrom)
                    {
                        _go.transform.localRotation = Quaternion.Euler(_from);
                    }

                    _tweener = _go.transform.DOLocalRotate(_to, _duration).SetEase(_easeType).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.localRotation = Quaternion.Euler(_from);
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.ShakeMove:
                {
                    _tweener = _go.transform.DOShakePosition(_duration, _to).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.localRotation = Quaternion.Euler(_from);
                        }
                        OnFinishTween();
                    });
                    break;
                }
            case AnimType.ShakeScale:
                {
                    _tweener = _go.transform.DOShakeScale(_duration, _to).OnComplete(() =>
                    {
                        if (_backToOriginWhenFinishTween)
                        {
                            _go.transform.localRotation = Quaternion.Euler(_from);
                        }
                        OnFinishTween();
                    });
                    break;
                }
            default:
                break;
        }
    }

    private void OnFinishTween()
    {
        _isTweening = false;
        _onFinishTween.Invoke();
    }

    private void OnEnable()
    {
        if (_doTweenOnEnable)
        {
            DoTween();
        }
    }
}