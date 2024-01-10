using System;
using UnityEngine;

public class DrawDistance : MonoBehaviour
{
    public Transform target;
    private float _currentDistance;

    public bool debugValue;
    
    private void Update()
    {
        _currentDistance = Vector3.Distance(transform.position, target.position);
        Debug.DrawLine(transform.position, target.position, Color.black, .2f);
       
        if(debugValue)
            Debug.Log("Distance = " + _currentDistance.ToString("F1"));
    }
    
}
