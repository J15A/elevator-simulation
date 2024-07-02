using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int weight;
    public int[] parks = { 0, 0, 0, 0, 0, 0 };
    public int elevator_position;
    public int nextCall;
    private int energy = 0, waitingTime = 0;

    public int globalElevatorPos;
    private int oldElevatorPos;

    public bool simPlaying;
    public bool simHistory;

    public TMP_InputField totalCalls;
    public int numOfCalls;

    private float totalCost;
    public TextMeshProUGUI costCounter;

    public Slider slider;
    public TMP_InputField[] parkInputs;
    public Button historyButton;

    public int errorType;
    public float errorWaitTime;

    public Transform elevator;
    public float elevatorSpeed;
    public float elevatorWaitTime;
    public float[] yFloorPositions;
    private int currentElevatorPosition, newElevatorPosition;
    private int elevatorUpdatePhase;

    public GameObject[] errorTypes;
    public GameObject[] floorMarkers;
    public GameObject[] parkMarkers;

    private bool programSleeping = false;

    public TextMeshProUGUI callHistory;
    private int callHistoryLines;

    public int numOfDecimals;
    private int callCounter;

    // Start is called before the first frame update
    void Start()
    {
        programSleeping = false;

        elevatorUpdatePhase = 0;

        callCounter = 0;

        errorType = 0;
        simPlaying = false;
        simHistory = false;

        historyButton.interactable = true;

        energy = 0;
        waitingTime = 0;

        totalCost = 0;
        costCounter.text = "Cost per Call: ";

        callHistory.text = "";
        callHistoryLines = 0;
        
        globalElevatorPos = 0;
        currentElevatorPosition = 0;
        newElevatorPosition = 0;
        elevator.transform.position = new Vector3(elevator.transform.position.x, yFloorPositions[0]);

        for (int i = 0; i < errorTypes.Length; i++)
        {
            errorTypes[i].SetActive(false);
        }
        for (int i = 0; i < floorMarkers.Length; i++)
        {
            floorMarkers[i].SetActive(false);
        }
        for (int i = 0; i < parkMarkers.Length; i++)
        {
            parkMarkers[i].SetActive(false);
        }

        weight = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (simPlaying)
        {
            if (simHistory == false)
            {
                callHistory.text = "";
                for (int i = 0; i < numOfCalls; i++)
                {
                    SimulateNextCall(globalElevatorPos);

                    totalCost += Cost(energy, waitingTime, weight);
                    costCounter.text = "Cost per Call: " + Mathf.Round(totalCost/(i+1) * Mathf.Pow(10, numOfDecimals)) / Mathf.Pow(10, numOfDecimals);

                    energy = 0;
                    waitingTime = 0;
                }

                errorType = 0;
                simPlaying = false;
                simHistory = false;
                totalCost = 0;
                globalElevatorPos = 0;
                historyButton.interactable = true;
            }
            else
            {
                if (programSleeping == false)
                {
                    if (numOfCalls > 0)
                    {
                        floorMarkers[currentElevatorPosition].SetActive(false);
                        parkMarkers[currentElevatorPosition].SetActive(false);

                        //move elevator
                        if (currentElevatorPosition > newElevatorPosition)
                        {
                            if (elevator.transform.position.y > yFloorPositions[newElevatorPosition])
                            {
                                elevator.transform.Translate(new Vector3(0, -1 * elevatorSpeed * Time.deltaTime));
                            }
                            else StartCoroutine(StopElevator(elevatorWaitTime));
                        }
                        else if (currentElevatorPosition < newElevatorPosition)
                        {
                            if (elevator.transform.position.y < yFloorPositions[newElevatorPosition])
                            {
                                elevator.transform.Translate(new Vector3(0, elevatorSpeed * Time.deltaTime));
                            }
                            else StartCoroutine(StopElevator(elevatorWaitTime));
                        }
                        else
                        {
                            if (elevatorUpdatePhase == 0)
                            {
                                //elevator is called to a passenger
                                int probability = Random.Range(0, 2);
                                if (probability == 0)
                                {
                                    nextCall = 0;
                                }
                                else
                                {
                                    nextCall = Random.Range(1, 6);
                                }

                                if (globalElevatorPos == nextCall)
                                {
                                    parkMarkers[globalElevatorPos].SetActive(false);
                                    floorMarkers[globalElevatorPos].SetActive(true);
                                    StartCoroutine(StopElevator(elevatorWaitTime));
                                }
                                else floorMarkers[nextCall].SetActive(true);

                                //move elevator to call position                               
                                waitingTime += Mathf.Abs(nextCall - globalElevatorPos);
                                energy += Mathf.Abs(nextCall - globalElevatorPos);
                                globalElevatorPos = nextCall;
                                callHistory.text += "\nCall from " + nextCall;

                                elevatorUpdatePhase = 1;
                            }
                            else if (elevatorUpdatePhase == 1)
                            {
                                //elevator goes to its destination
                                //if nextCall 1 - 5: move elevator to 0, add to energy
                                if (nextCall >= 1)
                                {
                                    globalElevatorPos = 0;
                                    energy += Mathf.Abs(nextCall - globalElevatorPos);
                                }
                                //else move elevator to Random.Range(1, 5), add to energy
                                else
                                {
                                    globalElevatorPos = Random.Range(1, 6);
                                    energy += Mathf.Abs(nextCall - globalElevatorPos);
                                }
                                oldElevatorPos = globalElevatorPos;
                                floorMarkers[globalElevatorPos].SetActive(true);

                                callHistory.text += " to " + oldElevatorPos + ". ";

                                elevatorUpdatePhase = 2;
                            }
                            else if (elevatorUpdatePhase == 2)
                            {
                                //park elevator based on final position and add to energy
                                globalElevatorPos = parks[oldElevatorPos];
                                energy += Mathf.Abs(globalElevatorPos - oldElevatorPos);

                                if (globalElevatorPos == oldElevatorPos)
                                {
                                    floorMarkers[oldElevatorPos].SetActive(false);
                                    parkMarkers[oldElevatorPos].SetActive(true);
                                    StartCoroutine(StopElevator(elevatorWaitTime));
                                }
                                else parkMarkers[globalElevatorPos].SetActive(true);

                                callHistory.text += "Parked at " + globalElevatorPos + ". ";

                                elevatorUpdatePhase = 3;
                            }
                            else if (elevatorUpdatePhase == 3)
                            {
                                //call is complete
                                callCounter++;

                                float x = Cost(energy, waitingTime, weight);
                                totalCost += x;
                                costCounter.text = "Cost per Call: " + Mathf.Round(totalCost / callCounter * Mathf.Pow(10, numOfDecimals)) / Mathf.Pow(10, numOfDecimals);

                                callHistory.text += "Cost: " + Mathf.Round(x);

                                energy = 0;
                                waitingTime = 0;
                                elevatorUpdatePhase = 0;

                                callHistoryLines++;
                                numOfCalls--;
                            }
                            newElevatorPosition = globalElevatorPos;
                        }
                        if (callHistoryLines == 10)
                        {
                            StartCoroutine(StopElevator(1f));
                            callHistory.text = "";
                            callHistoryLines = 0;
                        }
                    }
                    else
                    {
                        elevator.transform.position = new Vector3(elevator.transform.position.x, yFloorPositions[0]);
                        errorType = 0;
                        simPlaying = false;
                        simHistory = false;
                        historyButton.interactable = true;
                        totalCost = 0;
                        globalElevatorPos = 0;
                        for (int i = 0; i < floorMarkers.Length; i++)
                        {
                            floorMarkers[i].SetActive(false);
                        }
                        for (int i = 0; i < parkMarkers.Length; i++)
                        {
                            parkMarkers[i].SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            slider.onValueChanged.AddListener((v) =>
            {
                weight = (int) v;
            });
        }
    }

    IEnumerator StopElevator(float time)
    {
        programSleeping = true;
        yield return new WaitForSeconds(time);
        currentElevatorPosition = newElevatorPosition;
        programSleeping = false;
    }

    private void SimulateNextCall(int elevatorPos)
    {
        int probability = Random.Range(0, 2);
        if (probability == 0)
        {
            nextCall = 0;
        }
        else
        {
            nextCall = Random.Range(1, 6);
        }

        //move elevator to call position
        globalElevatorPos = nextCall;
        waitingTime += Mathf.Abs(nextCall - elevatorPos);
        energy += Mathf.Abs(nextCall - elevatorPos);

        //if nextCall 1 - 5: move elevator to 0, add to energy
        if (nextCall >= 1)
        {
            globalElevatorPos = 0;
            energy += Mathf.Abs(nextCall - globalElevatorPos);
        }
        //else move elevator to Random.Range(1, 5), add to energy
        else
        {
            globalElevatorPos = Random.Range(1, 6);
            energy += Mathf.Abs(nextCall - globalElevatorPos);
        }
        //park elevator based on final position and add to energy
        int oldElevatorPos = globalElevatorPos;
        globalElevatorPos = parks[oldElevatorPos];
        energy += Mathf.Abs(globalElevatorPos - oldElevatorPos);
    }

    public void GoButtonPressed()
    {
        errorType = 0;
        //check parking input fields
        for (int i = 0; i < 6; i++)
        {
            if (parkInputs[i].text != "")
            {
                int x = int.Parse(parkInputs[i].text);
                if (x <= 5 && x >= 0) parks[i] = x;
                else
                {
                    errorType = 1;
                }
            }
            else errorType = 4;
        }
        //check number of calls field
        if (totalCalls.text != "")
        {
            int n = int.Parse(totalCalls.text);
            if (n > 10000000)
            {
                errorType = 2;
            }
            else if (n < 0)
            {
                errorType = 3;
            }
            else numOfCalls = n;
        }
        else errorType = 4;

        if (errorType == 0 && simPlaying == false) 
        { 
            simPlaying = true;
            callHistory.text = "";
            callHistoryLines = 0;
        }
        else
        {
            errorTypes[errorType].SetActive(true);
            StartCoroutine(errorReset(errorWaitTime, errorType));
        }
    }

    IEnumerator errorReset(float time, int errorType)
    {
        yield return new WaitForSeconds(time);
        errorTypes[errorType].SetActive(false);
    }

    public void recordHistory()
    {
        if (simPlaying == false) { 
            simHistory = true;
            historyButton.interactable = false;
        }
    }

    public void cancelSim()
    {
        programSleeping = false;

        callCounter = 0;

        elevatorUpdatePhase = 0;

        errorType = 0;
        simPlaying = false;
        simHistory = false;

        historyButton.interactable = true;

        energy = 0;
        waitingTime = 0;

        totalCost = 0;
        costCounter.text = "Cost per Call: ";

        callHistory.text = "";
        callHistoryLines = 0;

        globalElevatorPos = 0;
        currentElevatorPosition = 0;
        newElevatorPosition = 0;
        elevator.transform.position = new Vector3(elevator.transform.position.x, yFloorPositions[0]);

        for (int i = 0; i < errorTypes.Length; i++)
        {
            errorTypes[i].SetActive(false);
        }
        for (int i = 0; i < floorMarkers.Length; i++)
        {
            floorMarkers[i].SetActive(false);
        }
        for (int i = 0; i < parkMarkers.Length; i++)
        {
            parkMarkers[i].SetActive(false);
        }
    }

    private float Cost(float m, float n, int w)
    {
        print (m + " " + n + " " + w);
        return m + w*n;
    }

    public void plusButton(int n)
    {
        if (n < 6)
        {
            int x = int.Parse(parkInputs[n].text);
            x++;
            if (x > 5) x = 0;
            parkInputs[n].text = x.ToString();
        }
        else
        {
            float x = int.Parse(totalCalls.text);
            int y = Mathf.RoundToInt(x * 10f);
            if (y > 1000000) y = 1;
            totalCalls.text = y.ToString();
        }
    }

    public void minusButton(int n)
    {
        if (n < 6)
        {
            int x = int.Parse(parkInputs[n].text);
            x--;
            if (x < 0) x = 5;
            parkInputs[n].text = x.ToString();
        }
        else
        {
            float x = int.Parse(totalCalls.text);
            int y = Mathf.RoundToInt(x/10f);
            if (y < 1) y = 1000000;
            totalCalls.text = y.ToString();
        }
    }

}