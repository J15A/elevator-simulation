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

    public bool simPlaying;
    public bool simSkipped;

    public TMP_InputField totalCalls;
    public int numOfCalls;

    private float totalCost;
    public TextMeshProUGUI costCounter;

    public Slider slider;
    public TMP_InputField[] parkInputs;

    public int errorType;

    public RectTransform elevator;
    public float[] yFloorPositions = { -143, -86, -35, 16, 66, 119 };

    public GameObject[] errorTypes;

    // Start is called before the first frame update
    void Start()
    {
        errorType = 0;
        simPlaying = false;
        simSkipped = false;

        energy = 0;
        waitingTime = 0;

        totalCost = 0;
        costCounter.text = "Total Cost: 0";
        globalElevatorPos = 0;
        elevator.localPosition = new Vector3(elevator.localPosition.x, yFloorPositions[0]);

        for (int i = 0; i < errorTypes.Length; i++)
        {
            errorTypes[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (simPlaying)
        {
            for (int i = 0; i < numOfCalls; i++)
            {
                SimulateNextCall(globalElevatorPos);

                totalCost += Cost(waitingTime, energy, weight);
                costCounter.text = "Total Cost: " + (int) totalCost;

                energy = 0;
                waitingTime = 0;
            }

            errorType = 0;
            simPlaying = false;
            simSkipped = false;
            totalCost = 0;
            globalElevatorPos = 0;
        }
        else
        {
            slider.onValueChanged.AddListener((v) =>
            {
                weight = (int) v;
            });
        }
    }

    IEnumerator errorReset(float time, int errorType)
    {
        yield return new WaitForSeconds(time);
        errorTypes[errorType].SetActive(false);
    }

    private void SimulateNextCall(int elevatorPos)
    {
        int probability = Random.Range(0, 1);
        if (probability == 0)
        {
            nextCall = 0;
        }
        else
        {
            nextCall = Random.Range(1, 5);
        }
        //indicate call with arrow
        //move elevator to call position
        print("about to move");
        StartCoroutine(UpdateElevatorPosition(elevatorPos, globalElevatorPos));
        globalElevatorPos = nextCall;
        waitingTime += Mathf.Abs(nextCall - parks[elevatorPos]);
        energy += Mathf.Abs(nextCall - parks[elevatorPos]);

        //if nextCall 1 - 5: move elevator to 0, add to energy
        if (nextCall >= 1)
        {
            globalElevatorPos = 0;
            energy += Mathf.Abs(nextCall - globalElevatorPos);
        }
        //else move elevator to Random.Range(1, 5), add to energy
        else
        {
            globalElevatorPos = Random.Range(1, 5);
            energy += Mathf.Abs(nextCall - globalElevatorPos);
        }
        //park elevator based on final position and add to energy
        int oldElevatorPos = globalElevatorPos;
        globalElevatorPos = parks[oldElevatorPos];
        energy += Mathf.Abs(globalElevatorPos - oldElevatorPos);
    }


    IEnumerator UpdateElevatorPosition(int oldPosition, int newPosition)
    {
        print("moving");
        /*
        float translation = yFloorPositions[newPosition] - yFloorPositions[oldPosition];
        for (int i = 0; i < Mathf.Abs(translation); i++)
        {
            elevator.localPosition = new Vector3(elevator.localPosition.x, elevator.localPosition.y + 100 + (translation / Mathf.Abs(translation)));
            yield return new WaitForEndOfFrame();
        }
        */
        yield return new WaitForSeconds(0f);
    }

    public void PlayButtonPressed()
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
            if (n > 1000000)
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

        if (errorType == 0) simPlaying = true;
        else
        {
            errorTypes[errorType].SetActive(true);
            StartCoroutine(errorReset(3f, errorType));
        }
    }
    
    public void skipSim()
    {
        if (simPlaying) simSkipped = true;
    }

    private float Cost(float m, float n, float w)
    {
        return (w/5 * m) + ((10 - w)/5 * n);
    }
}