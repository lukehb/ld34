using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private string _onePlayerScene;
    [SerializeField]
    private string _twoPlayerScene;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey("z"))
	    {
	        Application.LoadLevel(_onePlayerScene);
	    }
        else if (Input.GetKey("x"))
        {
            Application.LoadLevel(_twoPlayerScene);
        }
	}
}
