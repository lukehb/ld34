using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Coach : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField]
    private string[] _dialogStack;
    [SerializeField]
    private float _readOutCharsPerSecond = 10f;
    [SerializeField]
    private Player _player;

    private bool _readTextOut = true;
    private float _timePerChar;
    private float _timeSinceLastChar = 0f;
    private int _dialogIndex = -1;
    private int _charIndex = -1;
    private string _dialog = String.Empty;

    // Use this for initialization
    void Start()
    {
        _text.text = "";
        _timePerChar = 1.0f / _readOutCharsPerSecond;
        _player.SetInputEnabled(false);
        DoNextDialog();
    }

    // Update is called once per frame
    void Update()
    {
        if (_readTextOut && !_dialog.Equals(String.Empty))
        {
            _timeSinceLastChar += Time.deltaTime;
            if (_timeSinceLastChar >= _timePerChar)
            {
                if (CanDoNextChar())
                {
                    _charIndex++;
                    _text.text += _dialog[_charIndex];
                    _timeSinceLastChar = 0;
                }
                else
                {
                    _text.text += " [Press " + _player.GetAttackKey() + "]";
                    _readTextOut = false;
                }
            }
        }
        else if (!_readTextOut)
        {
            //listen for input
            if (Input.GetKey(_player.GetAttackKey()))
            {
                DoNextDialog();
            }
        }
    }

    void DoNextDialog()
    {
        if (CanDoMoreDialog())
        {
            _dialogIndex++;
            _dialog = _dialogStack[_dialogIndex];
            _charIndex = -1;
            _text.text = "";
            _readTextOut = true;
        }
        else
        {
            _player.SetInputEnabled(true);
        }
    }

    bool CanDoNextChar()
    {
        return _dialog.Length > 0 && _charIndex + 1 < _dialog.Length;
    }

    bool CanDoMoreDialog()
    {
        return _dialogStack.Length > 0 && _dialogIndex + 1 < _dialogStack.Length;
    }

}

