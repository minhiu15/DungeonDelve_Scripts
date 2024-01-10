using System;
using UnityEngine;
using UnityEngine.Events;

public class DetectionBase : MonoBehaviour
{
    [Space,Header("RETURN => Object Collision")]
    public UnityEvent<GameObject> CollisionEnterEvent;
    public UnityEvent<GameObject> CollisionExitEvent;

    [Space,Header("RETURN => Position Collision")]
    public UnityEvent<Vector3> PositionEnterEvent;
    public UnityEvent<Vector3> PositionExitEvent;


}
