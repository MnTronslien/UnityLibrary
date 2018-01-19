using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author Mattias Tronslien
/// mntronslien@gmail.com
/// 2018
/// 
/// Use this script on objects that should be pushable
/// </summary>
public class Pushable : MonoBehaviour {

    public float _mass = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// Returns true if the object can be pushed away from pushingEntity
    /// </summary>
    /// <param name="pushingEntity"> The entity who does the pushing</param>
    /// <returns></returns>
    public bool IsPushable(PlayerBehaviour pushingEntity)
    {
        Debug.DrawRay(transform.position, pushingEntity._heading * 0.5f, Color.red, 0.2f);
        Ray ray = new Ray(transform.position, pushingEntity._heading);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.5f))
        {
            if (hit.collider.tag == "PushableAreaLimiter" || hit.collider.tag == "Untagged")
            {
                return false;
            }
        }
            return true;
    }
}


