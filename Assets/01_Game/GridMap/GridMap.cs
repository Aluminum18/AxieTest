using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "AxieTest/SetUp/MapSettings")]
public class GridMap : ScriptableObject
{
    [SerializeField]
    private Vector3 _zeroPoint;
    [SerializeField]
    private Vector2Int _mapSize;
    public Vector2Int MapSize => _mapSize;
    [SerializeField]
    private Vector2 _cellSize;
    public Vector2 CellSize => _cellSize;
    [SerializeField]
    private Vector2 _cellCenterOffset;

    public static Vector3 UNDEFINED_POSITON = new(99f, 99f, 99f);

    public Vector3 GetPosition(Vector2Int coordinate)
    {
        Vector3 localPosition = Vector3.zero;
        localPosition.x = coordinate.x * _cellSize.x + _cellCenterOffset.x;
        localPosition.y = coordinate.y * _cellSize.y + _cellCenterOffset.y;
        return _zeroPoint + localPosition;
    }

    public Vector2Int GetCoordinate(Vector3 position)
    {
        Vector3 localPosition = position - _zeroPoint;
        Vector2Int coor = Vector2Int.zero;
        coor.x = (int)(localPosition.x / _cellSize.x);
        coor.y = (int)(localPosition.y / _cellSize.y);

        return coor;
    }

    public bool IsAdjacentCell(Vector2Int cell1, Vector2Int cell2)
    {
        return Mathf.Abs((cell1 - cell2).sqrMagnitude - 1f) < float.Epsilon;
    }

    public Vector2Int[] GetNextPossibleCoordinates(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = to - from;
        if (direction == Vector2Int.zero)
        {
            return new Vector2Int[] { from };
        }
        if (direction.x == 0)
        {
            from.y += System.Math.Sign(direction.y);
            return new Vector2Int[] { from };
        }
        if (direction.y == 0)
        {
            from.x += System.Math.Sign(direction.x);
            return new Vector2Int[] { from };
        }
        Vector2Int coord1 = from;
        coord1.x += System.Math.Sign(direction.x);
        Vector2Int coord2 = from;
        coord2.y += System.Math.Sign(direction.y);

        return new Vector2Int[] {coord1, coord2 };
    }
}
