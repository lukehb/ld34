using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //player states
    private enum Moves
    {
        Idle = 0,
        MovingForward = 1,
        MovingBackward = 2
    }

    private string Anim_WalkingForward = "WalkingForward";
    private string Anim_WalkingBackward = "WalkingBackward";

    private const float HoldingKeyTime = 0.5f;

    [SerializeField]
    private String _attackKey;
    [SerializeField]
    private String _dodgeKey;

    private Animator _anim;
    private float _timeholdingAttackKey;
    private float _timeholdingDodgeKey;
    private Moves _currentMove = Moves.Idle;

	void Start ()
	{
	    _anim = GetComponent<Animator>();
	}
	
	void Update () {
	    ScanInputs();
        HandleInputs();
	}

    void DoMove(Moves move)
    {
        switch (move)
        {
            case Moves.MovingForward:
                _anim.SetBool(Anim_WalkingForward, true);
                break;
            case Moves.MovingBackward:
                _anim.SetBool(Anim_WalkingBackward, true);
                break;
            case Moves.Idle:
                //reset everything
                _anim.SetBool(Anim_WalkingBackward, false);
                _anim.SetBool(Anim_WalkingForward, false);
                break;
        }
        _currentMove = move;
    }

    void HandleInputs()
    {
        //move forward
        if (_currentMove != Moves.MovingForward 
            && _timeholdingAttackKey > 0 && _timeholdingAttackKey > HoldingKeyTime)
        {
            DoMove(Moves.MovingForward);
        }
        //move backward
        else if (_currentMove != Moves.MovingBackward && 
            _timeholdingDodgeKey > 0 && _timeholdingDodgeKey > HoldingKeyTime)
        {
            DoMove(Moves.MovingBackward);
        }
        else if (_currentMove != Moves.Idle && !Input.anyKey)
        {
            DoMove(Moves.Idle);
        }

    }

    void ScanInputs()
    {
        //detect holding
        if (Input.GetKey(_attackKey))
        {
            _timeholdingAttackKey += Time.deltaTime;
        }
        else
        {
            _timeholdingAttackKey = 0;
        }


        if (Input.GetKey(_dodgeKey))
        {
            _timeholdingDodgeKey += Time.deltaTime;
        }
        else
        {
            _timeholdingDodgeKey = 0;
        }
        
    }

}
