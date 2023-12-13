using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "AxieTest/SetUp/MapSettings")]
public class GridMap : ScriptableObject
{
    [SerializeField]
    private Transform _zeroPoint;
    [SerializeField]
    private Vector2 _mapSize;
    [SerializeField]
    private Vector2 _cellSize;
    [SerializeField]
    private Vector2 _cellCenterOffset;

    public Vector3 GetPosition(Vector2Int coordinate)
    {
        Vector3 localPosition = Vector3.zero;
        localPosition.x = coordinate.x * _cellSize.x + _cellCenterOffset.x;
        localPosition.y = coordinate.y * _cellSize.y + _cellCenterOffset.y;
        return _zeroPoint.position + localPosition;
    }

    public Vector2Int GetCoordinate(Vector3 position)
    {
        Vector3 localPosition = position - _zeroPoint.position;
        Vector2Int coor = Vector2Int.zero;
        coor.x = (int)(localPosition.x / _cellSize.x);
        coor.y = (int)(localPosition.y / _cellSize.y);

        return coor;
    }
}
