using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInMultiplePoolsPool : MonoBehaviour
{
    private PoolInMultiplePool _parentPool;

    public void AssignParrentPool(PoolInMultiplePool parrent)
    {
        _parentPool = parrent;
    }

    private void OnDisable()
    {
        _parentPool.BackToPool(gameObject);
    }
}
