using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOODType
{
    FISH,
    APPLE,
    SAUSAGE,
    DISH01,
    DISHFISH
}
public enum FOODStatus
{
    ORIGINAL,
    SLICING,
    SLICED,
    COOKING,
    COOKED
}
public class Food: MonoBehaviour
{
    public FOODType foodType;
    public FOODStatus foodStatus;

    public void slicing()
    {
        foodStatus = FOODStatus.SLICING;
        GetComponent<Animator>().SetBool("isSlicing", true);
        GetComponent<Animator>().SetTrigger("SlicingTrigger");
        foodStatus = FOODStatus.SLICED;
    }

    public void sliceInit()
    {
        GetComponent<Animator>().SetBool("isSlicing", false);
        foodStatus = FOODStatus.ORIGINAL;
    }
    public void cooked()
    {
        foodStatus = FOODStatus.COOKED;
        //GetComponent<Animator>().SetBool("isCooked", true);
        GetComponent<Animator>().SetTrigger("CookedTrigger");
    }
}
