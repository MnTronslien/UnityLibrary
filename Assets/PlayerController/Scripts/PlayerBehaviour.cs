using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//Author Mattias Tronslien, 2018
//mntronslien@gmail.com

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]

public class PlayerBehaviour : MonoBehaviour
{
    //state machine variables
    public enum State
    {
        Pushing, Attacking, Idle, Dead, Jumping
    }
    private State _state = State.Idle;
    private State _lastState = State.Idle;
    private bool _isEnteringState = false;

    //component refs
    private NavMeshAgent _NMAgent;
    private AudioSource _Speaker;

    //Sounds
    [Header("Sounds")]
    //public AudioClip _phaseloop;
    [Header("Player Stats")] //if you are making a more complex this might me moved into a manager
    public float _Strenght = 0;

    //other global variables
    [NonSerialized]
    public Vector3 _heading; //current direction of travel
    [NonSerialized]
    public Vector3 _lastHeading; //direction of travel in last update
    private float _Pushcount = 0; //TODO: Remove this
    [NonSerialized]
    private GameObject _pushableObject; //can this be made more elegant?

    //Setting up component references, Awake() is called before Start()
    private void Awake()
    {
        _NMAgent = GetComponent<NavMeshAgent>();
        _Speaker = GetComponent<AudioSource>();
        _NMAgent.destination = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        _lastHeading = _heading;
        _heading = Vector3.zero;
        _heading.x += Mathf.Round(Input.GetAxisRaw("Horizontal")); //Offset from current position based on input to get heading
        _heading.z += Mathf.Round(Input.GetAxisRaw("Vertical"));
        gameObject.name = "Player " + _state; //For debugging

        if (_state != _lastState) //detect state transition
        {
            _isEnteringState = true;
            _lastState = _state;
        }

        //State Machine -------------------------------------------------------------------------------------------
        //If you do not know what a state machine is, the basic concept is 
        //that you organize the behaviour into certain states that decribe what the player (or enmy or whatever) may do.
        //The clue here is that each state is *inclusive* and not exlusive in the options of behaviour.
        //An example of this is that "jumping" is possible both while idle and moving. 
        //Instead of making specialized bahaviour for jumping and idle, we just allow both states to call the same method and transition into jumping state.

        //Idle state
        if (_state == State.Idle)
        {
            //Use _isEnteringState to declare bahaviour that should only be run the first frame while in this state
            //Usefull for for setting variables that is needed for this state, but only needs to be run once
            if (_isEnteringState)
            {
                _NMAgent.enabled = true;
            }
            Navigate(); //This metod handles navigation, wich the player can do while in idle.

            // --- exit to Jumping
            if (Input.GetButton("Jump"))
            {
                _state = State.Jumping;
                //TODO: Animation
                //TODO: Sound
            }

            //pushing is the player pushing agains a pushable object? 
            //TODO: Add "rake-cast" for better accuracy
            Debug.DrawRay(transform.position, _heading * 0.5f, Color.green, 0.2f);
            Ray ray = new Ray(transform.position, _heading);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                if (hit.collider.GetComponent<Pushable>() != null)
                {
                    print("There is something pushablle in front of the object!");
                    _Pushcount = _Pushcount + 1 * Time.deltaTime;
                    //Debug.Log(_Pushcount);
                    transform.position = hit.collider.transform.position - _heading;
                    _NMAgent.destination = transform.position; //don't move closer to object
                }
            }
            //how long should player strain before starting to push?
            else _Pushcount = 0;
            if (_Pushcount > 1 / _Strenght)
            {
                _state = State.Pushing;
                _Pushcount = 0;
                _pushableObject = hit.collider.gameObject;
            }
        }

        //Some object can be pushed by attaching a the script "pushable" to them
        if (_state == State.Pushing)
        {
            _NMAgent.enabled = false;

            if (_pushableObject.GetComponent<Pushable>().IsPushable(this))
            {
                Vector3 objectOffset = transform.position - _pushableObject.transform.position;
                float speed = _NMAgent.speed / _pushableObject.GetComponent<Pushable>()._mass;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + _heading, speed * Time.deltaTime);
                _pushableObject.transform.position = transform.position - objectOffset;

            }
            else _state = State.Idle;
            if (_heading != _lastHeading)
            {

                _state = State.Idle;
            }
        }
        _isEnteringState = false; //reset entering state to false

    } //End Update

    /// <summary>
    /// The player is allowed to move using normal controlls
    /// </summary>
    private void Navigate()
    {
        //Sets a destination for the nav mesh agent to navigate to, 
        //how the agent actually gets from point a to point b is handled by the agent

        Vector3 destination = transform.position; //destination is the same as current position.
        destination.x += Input.GetAxisRaw("Horizontal"); //Offset from current position based on input
        destination.z += Input.GetAxisRaw("Vertical");
        _NMAgent.destination = destination;
        //Debug.Log("Player tries to navigate");

    }

    public State GetState() { return _state; }
}
