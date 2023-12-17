using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _objectInPool;
    [SerializeField]
    private List<GameObject> _randomObjList;
    [SerializeField]
    private Transform _parrentTransform;
    [SerializeField]
    private Transform _spawnPos;
    public Transform SpawnPos => _spawnPos;
    [SerializeField]
    private Vector3 _randomRangePos;
    [SerializeField]
    private int _poolSize;

    private Stack<GameObject> _available = new Stack<GameObject>();

    public void Spawn()
    {
        var go = SpawnAndReturnObject();
        SetupObjectTransform(go);
        go.SetActive(true);
    }

    public void Spawn(Vector3 position)
    {
        var go = SpawnAndReturnObject();
        go.transform.position = position;
        go.SetActive(true);
    }

    public void SpawnRandomObject()
    {
        SpawnRandomAndReturnObject();
    }

    public GameObject SpawnRandomAndReturnObject()
    {
        if (_randomObjList.Count == 0)
        {
            return null;
        }

        var targetPrefab = _randomObjList[Random.Range(0, _randomObjList.Count)];

        var go = SpawnAndReturnObject();
        SetupObjectTransform(go);

        return go;
    }

    public GameObject SpawnAndReturnObject()
    {
        return MultiplePools.Instance.SpawnGameObject(_objectInPool, _poolSize, false);
    }

    public void ReturnToPool(GameObject go)
    {
        if (_available.Count >= _poolSize)
        {
            Destroy(go);
            return;
        }

        go.SetActive(false);
        _available.Push(go);
    }

    Vector3 _bufferVector = Vector3.zero;
    public void SpawnWithRandomRange()
    {
        _bufferVector.x = Random.Range(-_randomRangePos.x, _randomRangePos.x);
        _bufferVector.y = Random.Range(-_randomRangePos.y, _randomRangePos.y);
        _bufferVector.z = Random.Range(-_randomRangePos.z, _randomRangePos.z);

        var go = SpawnAndReturnObject();
        if (_parrentTransform != null)
        {
            go.transform.parent = _parrentTransform;
        }

        if (_spawnPos == null)
        {
            go.transform.position += _bufferVector;
            return;
        }

        _bufferVector += _spawnPos.position;

        go.transform.position = _bufferVector;

        go.SetActive(true);

    }

    private void SetupObjectTransform(GameObject go)
    {
        go.transform.SetParent(_parrentTransform);
        go.transform.localScale = _objectInPool.transform.localScale;

        if (_spawnPos == null)
        {
            go.transform.SetPositionAndRotation(_spawnPos.position, _spawnPos.rotation);
            return;
        }
        go.transform.position = _spawnPos.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (_spawnPos == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_spawnPos.position, _randomRangePos * 2f);
    }
}
