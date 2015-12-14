using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatHead : MonoBehaviour
{

    private enum Mouth
    {
        Open, 
        Closed
    }

    [SerializeField]
    private Sprite _mouthClosed;
    [SerializeField]
    private Sprite _mouthOpen;
    [SerializeField]
    private float _animationFPS = 10;

    private float _timeBetweenChanges;
    private float _timeElapsedSinceLastChange = 0;
    private Mouth mouthState = Mouth.Closed;
    private Image toChange;

	// Use this for initialization
	void Start ()
	{
	    toChange = GetComponent<Image>();
	    _timeBetweenChanges = 1f/_animationFPS;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _timeElapsedSinceLastChange += Time.deltaTime;
	    if (_timeElapsedSinceLastChange >= _timeBetweenChanges)
	    {
	        if (mouthState == Mouth.Closed)
	        {
	            mouthState = Mouth.Open;
	            toChange.sprite = _mouthOpen;
	        }
	        else
	        {
	            mouthState = Mouth.Closed;
	            toChange.sprite = _mouthClosed;
	        }
	        _timeElapsedSinceLastChange = 0;
	    }
	}
}
