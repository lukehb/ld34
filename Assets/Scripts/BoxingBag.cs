﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BoxingBag : MonoBehaviour
{

    private SpriteRenderer _bagRenderer;

    [SerializeField]
    private BoxCollider2D _collider;

    [SerializeField]
    private GameObject _bloodParticlePrefab;

    [SerializeField] private Coach _oldCoachMsg;
    [SerializeField] private Coach _coach;

    [SerializeField]
    private GameObject _nextDialogBtn;

	// Use this for initialization
	void Start ()
	{
	    _bagRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!_nextDialogBtn.activeInHierarchy && _coach.enabled && !_coach.CanDoMoreDialog())
	    {
	        _nextDialogBtn.SetActive(true);
	    }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        _collider.enabled = false;
        _bagRenderer.enabled = false;
        _bloodParticlePrefab.SetActive(true);
        _oldCoachMsg.enabled = false;
        _coach.enabled = true;
    }
}
