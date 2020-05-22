using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int maxTime = 30;
    public int currentTime;
    public bool finishRecipe = false;

    public TimeBar timeBar;

    int interval = 1;
    float nextTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = maxTime;
        timeBar.SetMaxTime(maxTime);    
    }

    // Update is called once per frame
    void Update()
    {
        if(finishRecipe)
        {
            //pauses timer and possibly delete ui object from screen
        }
        else if (Time.time >= nextTime) {
            SubtractTime(1);
            nextTime += interval;
        }
        
    }

    void SubtractTime(int time)
    {
        currentTime -= time; 
        timeBar.SetTime(currentTime);
    }
}

