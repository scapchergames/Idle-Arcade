using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerCollectable : MonoBehaviour
{
    #region Collect
    [Header("Objects")]
    public List<CollectedClasse> CollectedList = new List<CollectedClasse>(4);

    [Serializable]
    public class CollectedClasse
    {
        public List<GameObject> objects = new List<GameObject>();
    }

    public GameObjectVariable[] ResourcePrefabs;
    public Transform StackParent;

    [Header("Parameters")]
    [Space(10)]
    public FloatVariable collectMoveDuration;
    public FloatVariable CollectedItemZPosition;
    public FloatVariable CollectedItemsOffset;

    [Header("Data")]
    [Space(10)]
    public int TotalCollectedCount;
    public IntVariable[] CurrentCollectedCounts;
    public IntVariable MaxCollectableCount;

    public PurchasableItem currentPurchaseItem;

    public Giver currentGiver;
    public Taker currentTaker;

    public void Collect(Vector3 _collectedItemPosition, ResourceType _collectableType)
    {
        if (ReturnCurrentCollectedCount(_collectableType).Value < MaxCollectableCount.Value)
        {
            TotalCollectedCount++;

            ArcadeManager.Instance.IncreaseResource(1, _collectableType);

            GameObject _newCollected = Instantiate(ReturnCollectPrefab(_collectableType), _collectedItemPosition, Quaternion.identity, StackParent);

            CollectedList[ReturnResourceIndex(_collectableType)].objects.Add(_newCollected);

            _newCollected.transform.DOLocalMove(new Vector3(0f, ReturnYPosition(), CollectedItemZPosition.Value), collectMoveDuration.Value)
                                   .OnComplete(() => SetStraight(_newCollected));

            /*
            if (currentGiver != null)
            {
                currentGiver.OnPlayerResourceDataChanged(ReturnCurrentCollectedCount(_collectableType).Value);
            }
            */
        }
    }

    #region Returns
    public int ReturnResourceIndex(ResourceType _collectableType)
    {
        int _currentResourceIndex = 0;

        if (_collectableType == ResourceType.Type1)
        {
            _currentResourceIndex = 1;
        }
        else if (_collectableType == ResourceType.Type2)
        {
            _currentResourceIndex = 2;
        }
        else if (_collectableType == ResourceType.Type3)
        {
            _currentResourceIndex = 3;
        }
        else if (_collectableType == ResourceType.Type4)
        {
            _currentResourceIndex = 4;
        }
        else if (_collectableType == ResourceType.Type0)
        {
            _currentResourceIndex = 0;
        }

        return _currentResourceIndex;
    }

    public IntVariable ReturnCurrentCollectedCount(ResourceType _collectableType)
    {
        IntVariable _currentCollectedCount = CurrentCollectedCounts[0];

        if (_collectableType == ResourceType.Type1)
        {
            _currentCollectedCount = CurrentCollectedCounts[1];
        }
        else if (_collectableType == ResourceType.Type2)
        {
            _currentCollectedCount = CurrentCollectedCounts[2];
        }
        else if (_collectableType == ResourceType.Type3)
        {
            _currentCollectedCount = CurrentCollectedCounts[3];
        }
        else if (_collectableType == ResourceType.Type4)
        {
            _currentCollectedCount = CurrentCollectedCounts[4];
        }
        else if (_collectableType == ResourceType.Type0)
        {
            _currentCollectedCount = CurrentCollectedCounts[0];
        }

        return _currentCollectedCount;
    }

    public GameObject ReturnCollectPrefab(ResourceType _collectableType)
    {
        GameObject _targetResource = ResourcePrefabs[0].Value;

        if (_collectableType == ResourceType.Type1)
        {
            _targetResource = ResourcePrefabs[1].Value;
        }
        else if (_collectableType == ResourceType.Type2)
        {
            _targetResource = ResourcePrefabs[2].Value;
        }
        else if (_collectableType == ResourceType.Type3)
        {
            _targetResource = ResourcePrefabs[3].Value;
        }
        else if (_collectableType == ResourceType.Type4)
        {
            _targetResource = ResourcePrefabs[4].Value;
        }
        else if (_collectableType == ResourceType.Type0)
        {
            _targetResource = ResourcePrefabs[0].Value;
        }

        return _targetResource;
    }

    public float ReturnYPosition()
    {
        return TotalCollectedCount * CollectedItemsOffset.Value;
    }

    public Transform ReturnStackParent()
    {
        return StackParent;
    }
    #endregion Returns

    public void SetStraight(GameObject _object)
    {
        _object.transform.DOLocalRotate(Vector3.zero, 0f, RotateMode.Fast);
    }
    #endregion Collect

    #region Purchase
    public void Purchase(ResourceType _purchasableType, Vector3 _targetPosition, int _price, PurchasableItem item)
    {
        currentPurchaseItem = item;
        StartCoroutine(DelayedPurchase(_purchasableType, _targetPosition, _price));
    }

    public IEnumerator DelayedPurchase(ResourceType _purchasableType, Vector3 _targetPosition, int _price)
    {
        int targetStop = (CurrentCollectedCounts[ReturnPurchaseTypeIndex(_purchasableType)].Value - _price) - 1;

        if (targetStop <= 0)
            targetStop = 0;

        for (int i = (CurrentCollectedCounts[ReturnPurchaseTypeIndex(_purchasableType)].Value - 1); i > targetStop; i--)
        {
            yield return new WaitForSeconds(0.1f);

            TotalCollectedCount--;

            ArcadeManager.Instance.DecreaseResource(1, _purchasableType);

            currentPurchaseItem.DecreasePrice();

            GameObject _item = CollectedList[ReturnPurchaseTypeIndex(_purchasableType)].objects[i];
            CollectedList[ReturnPurchaseTypeIndex(_purchasableType)].objects.Remove(_item);

            _item.transform.parent = null;
            _item.transform.DOMove(_targetPosition, collectMoveDuration.Value).OnComplete(() => Destroy(_item));

            SetPositionsRight();
        }
        // SetPositionsRight();
    }

    public void StopPurchasing()
    {
        StopCoroutine(DelayedPurchase(0, Vector3.zero, 0));
    }

    public void SetPositionsRight()
    {
        int _counter = 0;

        for (int i = 1; i < CollectedList.Count; i++)
        {
            for (int i2 = 0; i2 < CollectedList[i].objects.Count; i2++)
            {
                _counter++;

                CollectedList[i].objects[i2].transform.localPosition = new Vector3(0f, _counter * CollectedItemsOffset.Value, CollectedItemZPosition.Value);
            }
        }
    }

    public int ReturnPurchaseTypeIndex(ResourceType _purchasableType)
    {
        int _currentResourceIndex = 0;

        if (_purchasableType == ResourceType.Type0)
        {
            _currentResourceIndex = 0;
        }
        else if (_purchasableType == ResourceType.Type1)
        {
            _currentResourceIndex = 1;
        }
        else if (_purchasableType == ResourceType.Type2)
        {
            _currentResourceIndex = 2;
        }
        else if (_purchasableType == ResourceType.Type3)
        {
            _currentResourceIndex = 3;
        }
        else if (_purchasableType == ResourceType.Type4)
        {
            _currentResourceIndex = 4;
        }

        return _currentResourceIndex;
    }
    #endregion Purchase

    #region Give
    public void Give(Taker _taker, Vector3 _targetPosition, int _amount, ResourceType _takerType, Transform _takerParent)
    {
        currentTaker = _taker;
        StartCoroutine(DelayedGive(_targetPosition, _amount, _takerType, _takerParent));
    }

    public IEnumerator DelayedGive(Vector3 _targetPosition, int _amount, ResourceType _takerType, Transform _takerParent)
    {
        int targetStop = (CurrentCollectedCounts[ReturnTakerTypeIndex(_takerType)].Value - _amount) - 1;

        if (targetStop <= 0)
            targetStop = 0;

        for (int i = (CurrentCollectedCounts[ReturnTakerTypeIndex(_takerType)].Value - 1); i > targetStop; i--)
        {
            yield return new WaitForSeconds(0.1f);

            TotalCollectedCount--;

            ArcadeManager.Instance.DecreaseResource(1, ResourceType.Type1);

            GameObject _item = CollectedList[1].objects[i];
            CollectedList[1].objects.Remove(_item);

            _item.transform.parent = _takerParent;
            _item.transform.DOLocalRotate(Vector3.zero, 0f, RotateMode.Fast);
            _item.transform.DOMove(_targetPosition, collectMoveDuration.Value).OnComplete(() => currentTaker.GetResource(_item)); ;

            SetPositionsRight();
        }
    }

    public void StopGiving()
    {
        StopCoroutine(DelayedGive(Vector3.zero, 0, 0, null));
    }

    public int ReturnTakerTypeIndex(ResourceType _takerType)
    {
        int _currentResourceIndex = 0;

        if (_takerType == ResourceType.Type1)
        {
            _currentResourceIndex = 1;
        }
        else if (_takerType == ResourceType.Type2)
        {
            _currentResourceIndex = 2;
        }
        else if (_takerType == ResourceType.Type3)
        {
            _currentResourceIndex = 3;
        }
        else if (_takerType == ResourceType.Type4)
        {
            _currentResourceIndex = 4;
        }

        return _currentResourceIndex;
    }
    #endregion Give
}