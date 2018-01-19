using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    public Transform _target;
    private Vector3 _Offset;


	// Use this for initialization
	void Start () {
        _Offset = _target.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = _target.position - _Offset;
	}
}
