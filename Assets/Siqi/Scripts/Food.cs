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
    COOKED,
    BURNT
}
public class Food: MonoBehaviour
{
    public FOODType foodType;
    public FOODStatus foodStatus;

    //public AudioSource[] sources;
    public AudioSource source1;
    //public AudioSource source2;
    public AudioClip cutting;
    //public GameObject stove;
    //public AudioClip cooking;

    private void Awake()
    {
        source1 = GetComponent<AudioSource>();
        //stove = GameObject.Find("Stove");
        //source2 = stove.GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (GetComponent<Animator>().GetBool("finishedCutting"))
        {
            source1.Stop();
        }
        /*
        if (GetComponent<Animator>().GetBool("finishedCooking"))
        {
            source2.Stop();
        }*/
    }

    public void slicing()
    {
        if(!(foodStatus == FOODStatus.SLICED))
        {
            foodStatus = FOODStatus.SLICING;
            GetComponent<Animator>().SetBool("isSlicing", true);
            GetComponent<Animator>().SetTrigger("SlicingTrigger");
            //foodStatus = FOODStatus.SLICED;

            source1.clip = cutting;
            source1.Play();
        }
        /*
        foodStatus = FOODStatus.SLICING;
        GetComponent<Animator>().SetBool("isSlicing", true);
        GetComponent<Animator>().SetTrigger("SlicingTrigger");
        foodStatus = FOODStatus.SLICED;

        source1.clip = cutting;
        source1.Play();
        */
    }

    public void sliceInit()
    {
        GetComponent<Animator>().SetBool("isSlicing", false);
        foodStatus = FOODStatus.ORIGINAL;
    }
    public void cooked()
    {
        //if (!(foodStatus == FOODStatus.COOKED))
        //{
            foodStatus = FOODStatus.COOKED;
            //GetComponent<Animator>().SetBool("isCooked", true);
            GetComponent<Animator>().SetTrigger("CookedTrigger");
        //}
        /*
        foodStatus = FOODStatus.COOKED;
        //GetComponent<Animator>().SetBool("isCooked", true);
        GetComponent<Animator>().SetTrigger("CookedTrigger");
        */
    }

    public void burnt()
    {
        foodStatus = FOODStatus.BURNT;
        GetComponent<Animator>().SetBool("isBurnt", true);

    }
}
