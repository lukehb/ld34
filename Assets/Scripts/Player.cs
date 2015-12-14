using UnityEngine;

public class Player : KeyListener
{
    //player states
    private enum Moves
    {
        Idle = 0,
        MovingForward = 1,
        MovingBackward = 2,
        Jabbing = 3,
        OneTwo = 4,
        Uppercut = 5,
        Dodge = 6,
    }

    //parameters to pass to animator
    private static readonly string Anim_Bool_Walking_Forward = "WalkingForward";
    private static readonly string Anim_Bool_Walking_Backward = "WalkingBackward";
    private static readonly string Anim_Trigger_Jabbing = "Jabbing";
    private static readonly string Anim_Trigger_OneTwoing = "OneTwoing";
    private static readonly string Anim_Trigger_Dodging = "Dodging";
    private static readonly string Anim_Trigger_Uppercut = "Uppercut";
    //names of animations, used to query the animation for state
    private static readonly string Anim_Idle = "idle";
    private static readonly string Anim_Jab = "jab";
    private static readonly string Anim_Walk_Backward = "walkbackward";
    private static readonly string Anim_Walk_Forward = "walkforward";
    private static readonly string Anim_One_Two = "onetwo";
    private static readonly string Anim_Dodge = "dodge";
    private static readonly string Anim_Uppercut = "uppercut";
    //CONSTANTS
    /// <summary>
    /// The time that must be exceeded to trigger a long press (or hold).
    /// </summary>
    private const float KeyLongPressTime = 0.137f;
    /// <summary>
    /// To do a double tap on a key both presses must occur within this time.
    /// </summary>
    private const float KeyDoubleTapTime = 0.3f;

    [SerializeField]
    private string _attackKey;
    [SerializeField]
    private string _dodgeKey;

    private Animator _anim;
    private int doubleTapCount = 0;
    private float doubleTapStarted;

	protected override void Start ()
	{
	    _anim = GetComponent<Animator>();
        ListenForKey(_attackKey);
        ListenForKey(_dodgeKey);
	}
	
	protected override void Update () {
        base.Update();
        HandleInputs();
	}

    protected override void KeyPressed(string key, float holdTime)
    {
        //attacks
        if (key.Equals(_attackKey))
        {
            if (holdTime < KeyLongPressTime)
            {
                bool doJab = true;
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

    void DoMove(Moves move)
    {
        switch (move)
        {
            case Moves.MovingForward:
                _anim.SetBool(Anim_Bool_Walking_Forward, true);
                break;
            case Moves.MovingBackward:
                _anim.SetBool(Anim_Bool_Walking_Backward, true);
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
            case Moves.Idle:
                //reset everything
                _anim.SetBool(Anim_Bool_Walking_Backward, false);
                _anim.SetBool(Anim_Bool_Walking_Forward, false);
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
            default:
                return false;
        }
    }

    void HandleInputs()
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

    

}
