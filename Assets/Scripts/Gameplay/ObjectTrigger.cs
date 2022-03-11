using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTrigger : MonoBehaviour
{
    [Tag]
    public string targetTag;

    public UnityEvent TriggerExit, TriggerEnter, TriggerStay;

    public GameObject triggeredObject;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == targetTag)
        {
            triggeredObject = other.gameObject;
            TriggerExit.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == targetTag)
        {
            triggeredObject = other.gameObject;
            TriggerEnter.Invoke();

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == targetTag)
        {
            triggeredObject = other.gameObject;
            TriggerStay.Invoke();
        }
    }
}
