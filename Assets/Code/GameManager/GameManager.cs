using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Timer levelTimer;

    public static float timeBetweenOrders = 5.0f;

    List<string> currentOrders;

    private bool isActive = false;
    private float orderTimer;

    void CreateOrder()
    {
        string order = "order";
        currentOrders.Add(order);
        Debug.Log("Order Added!");
    }

    // Start is called before the first frame update
    void Start()
    {
        currentOrders = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (levelTimer.isActive)
        {
            if (!isActive)
            {
                // Timer just started
                isActive = true;
                CreateOrder();
                orderTimer = timeBetweenOrders;
            }
            else
            {
                // Timer already running
                orderTimer -= Time.deltaTime;

                if (orderTimer <= 0)
                {
                    CreateOrder();
                    orderTimer = timeBetweenOrders;
                }
            }
        }
    }
}
