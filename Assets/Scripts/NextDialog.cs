using System;
using UnityEngine;
using System.Collections;
using Slerpy.Unity3D;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class NextDialog : MonoBehaviour
{

    [SerializeField]
    private Player _player;

    [SerializeField]
    private string _nextScene = String.Empty;

    private Text _text;

	// Use this for initialization
	void Start ()
	{
	    _text = GetComponent<Text>();
	    _text.text += " [" + _player.GetDodgeKey() + "]";
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(_player.GetDodgeKey()) && !_nextScene.Equals(String.Empty))
	    {
	        Application.LoadLevel(_nextScene);
	    }
	}
}
