using UnityEngine;
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

	// Use this for initialization
	void Start ()
	{
	    _bagRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
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
