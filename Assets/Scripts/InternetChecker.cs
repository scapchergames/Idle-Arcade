using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
    public GameObject CheckInternetConnectionPanel;

    void Start()
    {
        CheckInternetConnection();
    }

    private void Awake()
    {
        CheckInternetConnection();
    }

    public void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            CheckInternetConnectionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Successful internet Connection!");
            CheckInternetConnectionPanel.SetActive(false);
        }
    }
}
