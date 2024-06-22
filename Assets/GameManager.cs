using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int weight;
    public int[] parks = {0, 0, 0, 0, 0, 0};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //before sim is played

        //if simulator has started
        int next_call = (int)Random.Range(0f, 5f);
    }

    public void parkChanged(string s)
    {

    }
}
