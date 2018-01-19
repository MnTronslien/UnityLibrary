using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public GameObject _Player;
    private NavMeshAgent _NMAgent;
   //public enum State
   //{
   //    Pushing, Attacking, Idle, Dead, EnterSymetry, SymetryPhasing
   //}
   //private State _state = State.Idle;

    // Use this for initialization
    void Start () {
        if (_Player == null)
        {
            _Player = GameObject.FindWithTag("Player");
        }
        _NMAgent = GetComponent<NavMeshAgent>();

    }
	
	// Update is called once per frame
	void Update () {
        NavMeshHit NavHit;
        NavMesh.Raycast(transform.position, _Player.transform.position, out NavHit, 0);
        if (NavHit.hit == false)
        {
            Debug.Log("Player Reachable by spectre!");
            _NMAgent.destination =_Player.transform.position;
        }
		
	}
}
