using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomerRequests : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public UnityEvent CurrentCustomer;
    public GameObject[] Customers = new GameObject[0];
    [SerializeField] int CustomerIndex = 0;
    [SerializeField] TMP_Text RequestCounter;
    [SerializeField] int slimeCounter = 0;
    public UnityEvent CustomerSatisfaction;
    public UnityEvent DeliveredAll;

    public void Start()
    {
        //CustomerIndex = -1;
        slimeCounter = -1;
    }
    public void NextCustomer()
    {
        // Deactivate the current customer
        Customers[CustomerIndex].SetActive(false);
        if (CustomerIndex < Customers.Length)
        {
            // Move to the next customer
            CustomerIndex++;

            // Activate the next customer
            Customers[CustomerIndex].SetActive(true);
        }
        else //triggers at end of all customers
        {
            DeliveredAll.Invoke();
        }
    }

    public void AddSlimeToShipment()
    {
        slimeCounter++;
        RequestCounter.SetText(slimeCounter.ToString()+" / 4");

        if (slimeCounter == 4)
        {
            CustomerSatisfaction.Invoke(); //Praise player for completing customer
            NextCustomer();
            slimeCounter = 0;
            RequestCounter.SetText(slimeCounter.ToString() + " / 4");
        }
    }

}
