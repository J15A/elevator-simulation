using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int weight;
    public int[] parks = { 0, 0, 0, 0, 0, 0 };
    int parkSettingsIndicator;
    private int elevator_position;
    private int energy = 0, waitingTime = 0;

    private float totalCost;
    public TextMeshProUGUI costCounter;

    // Start is called before the first frame update
    void Start()
    {
        costCounter.text = "Total Cost: 0";
    }

    // Update is called once per frame
    void Update()
    {

        energy++;
        //if simulator has started
        totalCost = 0;
        totalCost += Cost(energy, waitingTime, weight);
        costCounter.text = "Total Cost: " + totalCost;

        int next_call = (int)Random.Range(0f, 5f);
    }

    float Cost(int m, int n, float w)
    {
        return w * m + (1 - w) * n;
    }

    public void nextPark(string s)
    {
        parkSettingsIndicator = int.Parse(s);
    }
    public void parkChanged(string s)
    {
        int x = int.Parse(s);
        if (x <= 5 && x >= 0)
        {
            parks[parkSettingsIndicator] = x;
        }
        else
        {
            //throw error
        }
    }
}