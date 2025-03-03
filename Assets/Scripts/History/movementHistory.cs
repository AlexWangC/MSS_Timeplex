using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.ComponentModel;
using System.Diagnostics;

public class movementHistory : MonoBehaviour
{
    public Dictionary<int,GridObject.Data> history = new Dictionary<int,GridObject.Data>();
    GridObject gridobject;


    void Awake()
    {
        gridobject = GetComponent<GridObject>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void recordMovement(int turnValue)
    {
        GridObject.Data data = gridobject.GetData();
        history.Add(turnValue,data);
        UnityEngine.Debug.Log(data.position);
    }

}