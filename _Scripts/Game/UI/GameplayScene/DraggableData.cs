using System.Collections.Generic;
using UnityEngine;

public static class DraggableData
{
    private static readonly Dictionary<GameObject, DraggableItem> _draggableItems = new();

    
    public static void Add(GameObject _objectKey, DraggableItem _dragValue)
    {
        if(_draggableItems.ContainsKey(_objectKey))
            return;
        _draggableItems.Add(_objectKey, _dragValue);
    }
    
    public static void Remove(GameObject _objectKey)
    {
        if(!_draggableItems.ContainsKey(_objectKey)) 
            return;
        _draggableItems.Remove(_objectKey);
    }

    public static bool Get(GameObject _objectKey, out DraggableItem _draggableItem) => _draggableItems.TryGetValue(_objectKey, out _draggableItem);

}
