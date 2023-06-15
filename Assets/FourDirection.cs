using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourDirection : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float distance;
    public float getDistance()
    {
        distance = Vector2.Distance(transform.position, target.position);
        return distance;
    }
    public string getDirectionName()
    {
        return transform.gameObject.ToString();
    }
}
