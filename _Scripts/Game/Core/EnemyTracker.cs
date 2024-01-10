using System.Collections.Generic;
using UnityEngine;

public static class EnemyTracker
{
    private static readonly List<Transform> EnemiesTracker = new();
    public static bool DetectEnemy => EnemiesTracker.Count != 0;
    
    public static void Add(Transform _transform)
    {
        if(EnemiesTracker.Contains(_transform)) return;
        EnemiesTracker.Add(_transform);
    }
    public static void Remove(Transform _transform)
    {
        if(!EnemiesTracker.Contains(_transform)) return;
        EnemiesTracker.Remove(_transform);
    }
    public static void Clear() => EnemiesTracker.Clear();

    public static bool FindClosestEnemy(Transform _transfLocal, out Vector3 _target)
    {
        _target = Vector3.zero;
        EnemiesTracker.RemoveAll(trans => trans == null);
        if (!DetectEnemy) 
            return false;
        EnemiesTracker.Sort((a, b) => Vector3.Distance(a.transform.position, _transfLocal.position)
                                                                .CompareTo(Vector3.Distance(b.transform.position, _transfLocal.position)));
        _target = EnemiesTracker[0].transform.position;
        return true;
    }
    
}
