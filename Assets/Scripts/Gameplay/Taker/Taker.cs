using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Taker : MonoBehaviour
{
    public TakerType takerType;

    public List<Transform> TakerPositions = new List<Transform>();

    public List<GameObject> TakedObjects = new List<GameObject>();

    public int TakedObjectsCount;

    public int MaxTakerCount;

    // [ShowOnly]
    public ObjectTrigger ObjectTrigger;

    public Transform TakerParent;

    public UnityEvent OnTaked;
    public UnityEvent OnConverted;

    public float ConvertingSpeed = 1f;

    public bool isConverting = false;

    // public Giver giver;

    private void Start()
    {
        MaxTakerCount = TakerPositions.Count;

        ObjectTrigger = GetComponent<ObjectTrigger>();

        // giver = GetComponent<Giver>();
    }

    public void TriggerExit()
    {
        ObjectTrigger.triggeredObject.GetComponent<PlayerCollectable>().StopGiving();

        StartConverting();
    }

    public void TriggerEnter()
    {
        if (TakedObjectsCount < MaxTakerCount)
        {
            ObjectTrigger.triggeredObject.GetComponent<PlayerCollectable>().Give(this, ReturnTargetTakerPosition(), MaxTakerCount - TakedObjectsCount, takerType, TakerParent);
        }
    }

    public Vector3 ReturnTargetTakerPosition()
    {
        return TakerPositions[TakedObjectsCount].position;
    }

    public void TriggerStay()
    {
        // OnStayEvent.Invoke();
    }

    public void GetResource(GameObject _object)
    {
        TakedObjects.Add(_object);
        TakedObjectsCount = TakedObjects.Count;

        _object.transform.position = TakerPositions[TakedObjectsCount].position;

        // resource to resource action
        StartConverting();

        OnTaked.Invoke();
    }

    public void StartConverting()
    {
        if (!isConverting)
        {
            isConverting = true;
            StartCoroutine(CorConverting());
        }
    }

    public IEnumerator CorConverting()
    {
        yield return new WaitForSeconds(ConvertingSpeed);

        if (TakedObjectsCount > 0)
        {
            GameObject _convertedObj = TakedObjects[TakedObjectsCount - 1];
            TakedObjects.Remove(_convertedObj);
            Destroy(_convertedObj);
            TakedObjectsCount = TakedObjects.Count;

            OnConverted.Invoke();
        }

        if (TakedObjectsCount > 0)
        {
            StartCoroutine(CorConverting());
        }

        if (TakedObjectsCount <= 0)
            isConverting = false;
    }

    public void StopConverting()
    {
        StopCoroutine(CorConverting());
    }
}
