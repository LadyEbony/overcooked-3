using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOODType
{
    FISH,
    TOMATO,
    DISH01
}
public enum FOODStatus
{
    ORIGINAL,
    SLICED,
    COOKED
}
public class Food: MonoBehaviour
{
    public FOODType foodType;
    public FOODStatus foodStatus;

    public void slicing()
    {
        GetComponent<Animator>().SetBool("isSlicing", true);
        foodStatus = FOODStatus.SLICED;
    }

    public void sliceInit()
    {
        GetComponent<Animator>().SetBool("isSlicing", false);
        foodStatus = FOODStatus.ORIGINAL;
    }
}
