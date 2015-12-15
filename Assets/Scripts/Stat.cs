using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{

    [SerializeField]
    private Image _statBar;
    [SerializeField]
    private int _maxStat;
    [SerializeField]
    private float _regenPerSecond = 0.33f;

    private float _currentStat;

	// Use this for initialization
	void Start ()
	{
	    _currentStat = _statBar.fillAmount * _maxStat;
        UpdateStatBar();
	}
	
	// Update is called once per frame
	void Update () {

	    if (_regenPerSecond > 0 && _currentStat < _maxStat)
	    {
	        float amountToRegen = Time.deltaTime/1f * _regenPerSecond;
            IncrementStat(amountToRegen);
	    }
	    
	}

    void UpdateStatBar()
    {
        _statBar.fillAmount = _currentStat/_maxStat;
    }

    internal void DecrementStat(int damage)
    {
        _currentStat -= damage;
        if (_currentStat < 0)
        {
            _currentStat = 0;
        }
        UpdateStatBar();
    }

    internal void IncrementStat(float increment)
    {
        _currentStat = Mathf.Min(_maxStat, _currentStat + increment);
        UpdateStatBar();
    }

    internal float GetStat()
    {
        return _currentStat;
    }

    internal bool IsFull()
    {
        return _currentStat >= _maxStat;
    }

    internal void SetRegenPerSecond(float regen)
    {
        _regenPerSecond = regen;
    }

}
