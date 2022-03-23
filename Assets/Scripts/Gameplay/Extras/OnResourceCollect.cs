using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class OnResourceCollect : MonoBehaviour
{
    public float duration;
    public float delay;
    public int maxAmount;

    Vector3 startPosition;
    Transform targetPosition;

    [Space(20)]
    public GameObject ResourceParent;
    public GameObject resourcePrefab;
    public List<GameObject> resourceList = new List<GameObject>();

    [Header("Resource")]
    [Space(20)]
    public Transform[] resourceTargetPositions;
    public Sprite resourceSprite;

    [Header("Coin")]
    [Space(20)]
    public Transform coinTargetPosition;
    public Sprite coinSprite;

    void Start()
    {
        PrepareResources();
    }

    public void PrepareResources()
    {
        ResourceParent.SetActive(false);

        for (int i = 0; i < maxAmount; i++)
        {
            GameObject newResource = Instantiate(resourcePrefab, ResourceParent.transform);
            newResource.SetActive(false);

            resourceList.Add(newResource);
        }

        startPosition = resourceList[0].transform.position;
    }

    public void SetAsResource()
    {
        // targetPosition = resourceTargetPositions[0];
        for (int i = 0; i < maxAmount; i++)
        {
            resourceList[i].GetComponent<Image>().sprite = resourceSprite;
        }
    }

    public void SetAsCoin()
    {
        targetPosition = coinTargetPosition;
        for (int i = 0; i < maxAmount; i++)
        {
            resourceList[i].GetComponent<Image>().sprite = coinSprite;
        }
    }

    public void OnCollect(int _amount, int _whichOne)
    {
        if (_whichOne == 0)
        {
            SetAsCoin();
        }
        else if (_whichOne > 0)
        {
            targetPosition = resourceTargetPositions[_whichOne - 1];
            SetAsResource();
        }

        ResourceParent.SetActive(true);
        StartCoroutine(DelayedActivate(_amount));
    }

    public IEnumerator DelayedActivate(int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            yield return new WaitForSeconds(delay);

            resourceList[i].transform.position = startPosition;

            resourceList[i].SetActive(true);

            resourceList[i].transform.DOMove(targetPosition.position, duration).OnComplete(() => DeactivateResource(i));

            if (i == (_amount - 1))
            {
                yield return new WaitForSeconds(duration);
                OnCompleted();
            }
        }
    }

    public void DeactivateResource(int _id)
    {
        resourceList[_id].SetActive(false);
    }

    public void OnCompleted()
    {
        ResourceParent.SetActive(false);

        for (int i = 0; i < resourceList.Count; i++)
        {
            resourceList[i].transform.position = startPosition;
        }
    }
}
