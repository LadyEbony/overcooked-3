using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cabinetPot : MonoBehaviour
{
    public Food dishFish;  // the dish01 prefab

    public Food food;
    public ParticleSystem cookingParticle;

    // Start is called before the first frame update
    void Start()
    {
        food = null;
    }

    public void Cooking()
    {
        if (cookingParticle) cookingParticle.Play();
        StartCoroutine(FinishCooking());
    }
    IEnumerator FinishCooking()
    {
        yield return new WaitForSeconds(5.0f);
        if (food != null)
        {
            if(food.foodType == FOODType.FISH)
            {
                Destroy(food.gameObject);
                food = null;
                Transform parent = GetComponent<Transform>().GetChild(1);
                //food = GameObject.Instantiate(dishFish, parent.position, parent.rotation, parent);
                food = GameObject.Instantiate(dishFish);
                food.transform.parent = parent;
                food.transform.position = parent.position;
                food.transform.rotation = parent.rotation;
                food.foodType = FOODType.DISHFISH;
                food.foodStatus = FOODStatus.COOKED;

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetPot = this;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetPot = null;
        }
        
    }

}
