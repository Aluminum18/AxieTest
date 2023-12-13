using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _dontDestroyOnLoad= true;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).ToString());
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    protected virtual void DoOnAwake()
    {

    }

    protected virtual void DoOnStart()
    {

    }

    protected virtual void DoOnDestroy()
    {

    }

    protected virtual void DoOnEnable()
    {

    }

    protected virtual void DoOnDisable()
    {

    }

    private void Awake()
    {
        T[] singletons = FindObjectsOfType<T>();
        if (singletons.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        DoOnAwake();
    }

    private void Start()
    {
        DoOnStart();
    }

    private void OnDestroy()
    {
        DoOnDestroy();
    }

    private void OnEnable()
    {
        DoOnEnable();
    }

    private void OnDisable()
    {
        DoOnDisable();
    }
}
