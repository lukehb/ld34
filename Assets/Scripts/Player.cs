using System;
using UnityEngine;

public class Player : KeyListener
{
    //player states
    public enum Moves
    {
        Dazed = -3,
        Idle = -2,
        MovingForward = -1,
        MovingBackward = 0,
        Jabbing = 1,
        OneTwo = 2,
        Uppercut = 3,
        Dodge = 4,
        Finisher = 10
    }

    //parameters to pass to animator
    private static readonly string Anim_Bool_Walking_Forward = "WalkingForward";
    private static readonly string Anim_Bool_Walking_Backward = "WalkingBackward";
    private static readonly string Anim_Trigger_Jabbing = "Jabbing";
    private static readonly string Anim_Trigger_OneTwoing = "OneTwoing";
    private static readonly string Anim_Trigger_Dodging = "Dodging";
    private static readonly string Anim_Trigger_Uppercut = "Uppercut";
    private static readonly string Anim_Trigger_Dazed = "Dazed";
    private static readonly string Anim_Trigger_Finisher = "Finisher";
    //names of animations, used to query the animation for state
    private static readonly string Anim_Idle = "idle";
    private static readonly string Anim_Jab = "jab";
    private static readonly string Anim_Walk_Backward = "walkbackward";
    private static readonly string Anim_Walk_Forward = "walkforward";
    private static readonly string Anim_One_Two = "onetwo";
    private static readonly string Anim_Dodge = "dodge";
    private static readonly string Anim_Uppercut = "uppercut";
    private static readonly string Anim_Dazed = "dazed";
    private static readonly string Anim_Finisher = "finisher";
    //CONSTANTS
    /// <summary>
    /// The time that must be exceeded to trigger a long press (or hold).
    /// </summary>
    private const float KeyLongPressTime = 0.137f;
    /// <summary>
    /// To do a double tap on a key both presses must occur within this time.
    /// </summary>
    private const float KeyDoubleTapTime = 0.3f;
    /// <summary>
    /// The players movement speed, increase to move faster
    /// </summary>
    private const float MovementSpeed = 2f;

    [SerializeField]
    private string _attackKey;
    [SerializeField]
    private string _dodgeKey;
    [SerializeField]
    private bool _inputEnabled = true;
    [SerializeField]
    private Stat _hp;
    [SerializeField]
    private Stat _guard;
    [SerializeField]
    private Stat _sp;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private GameObject _bloodParticlePrefab;
    [SerializeField]
    private GameObject _onDeathGameObject;
    [SerializeField]
    private GameObject _playerStateGameObject;

    [SerializeField] private Moves[] _preprogrammedMoves;

    private float _timeSinceLastMove = 0f;
    private int _preprogrammedMoveIdx = 0;
    private Animator _anim;
    private Rigidbody2D _rb;
    private int doubleTapCount = 0;
    private float doubleTapStarted;

	protected override void Start ()
	{
	    _anim = GetComponent<Animator>();
	    _rb = GetComponent<Rigidbody2D>();
        ListenForKey(_attackKey);
        ListenForKey(_dodgeKey);
	}
	
	protected override void Update () {
	    if (_inputEnabled)
	    {
	        base.Update();
	        DetectKeyHolding();
	    }
	    else
	    {
	        if (_preprogrammedMoves != null && _preprogrammedMoves.Length > 0)
	        {
	            if (_timeSinceLastMove >= 0.7f)
	            {
	                _preprogrammedMoveIdx++;
	                _preprogrammedMoveIdx %= _preprogrammedMoves.Length - 1;
                    Debug.Log(_preprogrammedMoveIdx);
                    DoMove(_preprogrammedMoves[_preprogrammedMoveIdx]);
	                _timeSinceLastMove = 0f;
	            }
	            _timeSinceLastMove += Time.deltaTime;
	        }
	    }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hp != null)
        {
            //check if we aren't dodging
            if (!IsCurrentMove(Moves.Dodge))
            {
                if (other is CircleCollider2D)
                {
                    Player otherPlayer = other.attachedRigidbody.gameObject.GetComponent<Player>();
                    Moves move = otherPlayer.GetCurrentMove();
                    int damage = (int) move;
                    if (damage > 0 && move != Moves.Dodge)
                    {
                        //if we aren't blocking
                        if (!IsCurrentMove(Moves.Idle) && !IsCurrentMove(Moves.MovingBackward) &&
                            !IsCurrentMove(Moves.MovingForward))
                        {
                            _hp.DecrementStat(damage);
                        }
                        else
                        {
                            if (_guard.GetStat() > 0)
                            {
                                _guard.DecrementStat(damage);
                                if (_guard.GetStat() <= 0)
                                {
                                    DoMove(Moves.Dazed);
                                }
                            }
                            else
                            {
                                _hp.DecrementStat(damage);
                            }
                        }

                    }

                    if (_hp.GetStat() <= 0)
                    {
                        DoDeath();
                    }

                }
            }
            else
            {
                //current move is dodging
                if (_sp != null)
                {
                    _sp.IncrementStat(1);
                }
            }
        }
        
    }

    void DoDeath()
    {
        _spriteRenderer.enabled = false;
        _hp.DecrementStat(10);
        _guard.DecrementStat(10);
        _guard.SetRegenPerSecond(0);
        _bloodParticlePrefab.SetActive(true);
        _onDeathGameObject.SetActive(true);
        _playerStateGameObject.SetActive(false);
        _preprogrammedMoves = new Moves[0];
    }

    protected override void KeyPressed(string key, float holdTime)
    {
        //attacks
        if (key.Equals(_attackKey))
        {
            if (holdTime < KeyLongPressTime)
            {

                bool doJab = true;

                if (_sp != null && _sp.IsFull())
                {
                    _sp.DecrementStat(3);
                    DoMove(Moves.Finisher);
                    doJab = false;
                }

                if (holdTime < KeyDoubleTapTime)
                {
                    //uppercut
                    if (IsCurrentMove(Moves.Dodge))
                    {
                        doJab = false;
                        DoMove(Moves.Uppercut);
                    }
                    else
                    {
                        //one two
                        if (DidDoubleTap())
                        {
                            DoMove(Moves.OneTwo);
                            doJab = false;
                        }
                    }
                }
                //jab
                if (doJab)
                {
                    DoMove(Moves.Jabbing);
                }
            }
            else
            {
                DoMove(Moves.Idle);
            }
        }
        //dodges
        else if (key.Equals(_dodgeKey))
        {
            if (holdTime < KeyLongPressTime)
            {
                DoMove(Moves.Dodge);
            }
            else
            {
                DoMove(Moves.Idle);
            }
        }
    }

    bool DidDoubleTap()
    {
        if (doubleTapCount == 0)
        {
            doubleTapStarted = Time.timeSinceLevelLoad;
        }
        doubleTapCount++;
        if (doubleTapCount >= 2)
        {
            if (Time.timeSinceLevelLoad - doubleTapStarted < KeyDoubleTapTime)
            {
                doubleTapStarted = 0;
                doubleTapCount = 0;
                return true;
            }
            doubleTapStarted = 0;
            doubleTapCount = 0;
        }
        return false;
    }

    void Walk(bool right)
    {

        float newVelocity = MovementSpeed;
        newVelocity *= ((right) ? 1 : -1) * Math.Sign(gameObject.transform.localScale.x);
        _rb.velocity = new Vector2(newVelocity, 0);
    }

    void DoMove(Moves move)
    {
        switch (move)
        {
            case Moves.Dazed:
                _anim.SetTrigger(Anim_Trigger_Dazed);
                break;
            case Moves.MovingForward:
                _anim.SetBool(Anim_Bool_Walking_Forward, true);
                Walk(true);
                break;
            case Moves.MovingBackward:
                _anim.SetBool(Anim_Bool_Walking_Backward, true);
                Walk(false);
                break;
            case Moves.Jabbing:
                _anim.SetTrigger(Anim_Trigger_Jabbing);
                break;
            case Moves.OneTwo:
                _anim.SetTrigger(Anim_Trigger_OneTwoing);
                break;
            case Moves.Dodge:
                _anim.SetTrigger(Anim_Trigger_Dodging);
                break;
            case Moves.Uppercut:
                _anim.SetTrigger(Anim_Trigger_Uppercut);
                break;
            case Moves.Finisher:
                _anim.SetTrigger(Anim_Trigger_Finisher);
                break;
            case Moves.Idle:
                //reset everything
                _anim.SetBool(Anim_Bool_Walking_Backward, false);
                _anim.SetBool(Anim_Bool_Walking_Forward, false);
                _rb.velocity = Vector2.zero;
                break;
        }

        //cancel any double tap counter, unless we pressed jab
        if (move != Moves.Jabbing && move != Moves.Idle)
        {
            doubleTapStarted = 0;
            doubleTapCount = 0;
        }

    }

    bool IsCurrentMove(Moves move)
    {
        switch (move)
        {
            case Moves.Dazed:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Dazed);
            case Moves.Idle:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Idle);
            case Moves.Jabbing:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Jab);
            case Moves.OneTwo:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_One_Two);
            case Moves.MovingForward:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Walk_Forward);
            case Moves.MovingBackward:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Walk_Backward);
            case Moves.Dodge:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Dodge);
            case Moves.Uppercut:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Uppercut);
            case Moves.Finisher:
                return _anim.GetCurrentAnimatorStateInfo(0).IsName(Anim_Finisher);
            default:
                return false;
        }
    }

    void DetectKeyHolding()
    {
        //move forward
        if (KeyHeldFor(_attackKey) > KeyLongPressTime)
        {
            DoMove(Moves.MovingForward);
        }
        //move backward
        else if (KeyHeldFor(_dodgeKey) > KeyLongPressTime)
        {
            DoMove(Moves.MovingBackward);
        }
    }

    internal Moves GetCurrentMove()
    {
        foreach (Moves move in Enum.GetValues(typeof(Moves)))
        {
            if (IsCurrentMove(move))
            {
                return move;
            }
        }
        return Moves.Idle;
    }

    internal string GetDodgeKey()
    {
        return _dodgeKey;
    }

    internal string GetAttackKey()
    {
        return _attackKey;
    }

    internal void SetInputEnabled(bool on)
    {
        this._inputEnabled = on;
    }

    

}
