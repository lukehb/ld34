using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class KeyListener : MonoBehaviour
{

    private readonly List<string> keys;
    private readonly Dictionary<string, float> keysHeld;

    public KeyListener()
    {
        this.keys = new List<string>();
        this.keysHeld = new Dictionary<string, float>();
    }

    protected void ListenForKey(string key)
    {
        this.keys.Add(key);
    }

    protected abstract void Start();

    protected virtual void Update()
    {
        ProcessKeys();
    }

    public void ProcessKeys()
    {
        foreach (string key in keys)
        {
            //check if it is held
            if (Input.GetKey(key))
            {
                float holdTime;
                keysHeld.TryGetValue(key, out holdTime);
                keysHeld[key] = holdTime + Time.deltaTime;
            }
            //it is not held
            else
            {
                if (keysHeld.ContainsKey(key))
                {
                    float holdTime = keysHeld[key];
                    keysHeld.Remove(key);
                    KeyPressed(key, holdTime);
                }
            }
        }
    }

    public float KeyHeldFor(string key)
    {
        float timeHeld;
        keysHeld.TryGetValue(key, out timeHeld);
        return timeHeld;
    }

    protected abstract void KeyPressed(string key, float holdTime);

}
