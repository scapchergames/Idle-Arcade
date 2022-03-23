using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

using RengeGames.HealthBars;

public class Giver : MonoBehaviour
{
    [Title("Giver")]
    [EnumToggleButtons]
    public ResourceType collectableType;
    [EnumToggleButtons]
    public ResourceType giverType;

    [Space(20)]
    public bool showProperties;

    [Space(20)]
    public FloatVariable GenerationSpeed;

    [Space(20)]
    public GiverPrefabs_Data giverPrefabs;
    public GameObject GiftPrefab;
    public Transform GiftParent;

    [Space(20)]

    [ReadOnly, ShowIf("showProperties")]
    public int MaxGiftCount;
    [ReadOnly, ShowIf("showProperties")]
    public int CurrentGiftCount;

    [Space(20)]
    [ReadOnly, ShowIf("showProperties")]
    public List<GameObject> GiftList = new List<GameObject>();

    // Custom Givers
    [Space(20)]
    [ShowIf("giverType", ResourceType.Type1)]
    public Vector3Variable Offset;
    float zOffset;
    float _x, _y, _z;
    int CurrentXPos;

    [ShowIf("giverType", ResourceType.Type1)]
    public IntVariable xCount, yCount, zCount;
    List<Vector3> posList = new List<Vector3>();

    [ShowIf("giverType", ResourceType.Type2)]
    [Space(20)]
    public List<Transform> SpesificPositionsList = new List<Transform>();

    [ShowIf("giverType", ResourceType.Type2)]
    public bool isGeneratingWithResource = false;

    [Space(20)]
    [ShowIf("giverType", ResourceType.Type3)]
    public TextMeshProUGUI countText;
    [ShowIf("giverType", ResourceType.Type3)]
    public UltimateCircularHealthBar hb;

    // Default
    [Space(20)]
    [ReadOnly, ShowIf("showProperties")]
    public bool generationCompleted = false;
    [ReadOnly, ShowIf("showProperties")]
    public bool startGiving = false;
    [ReadOnly, ShowIf("showProperties")]
    public float currentGenerationTime;
    [ReadOnly, ShowIf("showProperties")] // not necessary, fix it, CurrentGiftCount'a çevrilebilir gibi
    public int totalGiftCount;

    // [ShowOnly]
    [Space(20)]
    [ReadOnly, ShowIf("showProperties")]
    public PurchasableItem PurchasableItem;
    [ReadOnly, ShowIf("showProperties")]
    public ObjectTrigger ObjectTrigger;
    [ReadOnly, ShowIf("showProperties")]
    public GameObject CurrentPlayer;

    [Space(20)]
    [ShowIf("giverType", ResourceType.Type2)]
    public bool withAnimation = false;
    [ShowIf("giverType", ResourceType.Type2)]
    public Transform AnimateParent;

    [Space(20)]
    public UnityEvent OnCollectedResource;

    private GameObject _newGift;

    private void Start()
    {
        GiftPrefab = ReturnGiverPrefab();

        if(giverType == ResourceType.Type1 || giverType == ResourceType.Type2)
            PurchasableItem = GetComponent<PurchasableItem>();

        ObjectTrigger = GetComponent<ObjectTrigger>();

        if (giverType == ResourceType.Type1)
        {
            MaxGiftCount = (int)(xCount.Value * zCount.Value) * yCount.Value;
            zOffset = Offset.Value.z * 3f;
            for (int i = 0; i < MaxGiftCount; i++)
            {
                Vector3 _pos = ReturnGiftPosition();
                posList.Add(_pos);
            }
        }
        else if (giverType == ResourceType.Type2 || giverType == ResourceType.Type3)
        {
            MaxGiftCount = SpesificPositionsList.Count;
        }

        if (giverType == ResourceType.Type3)
            DisableCounterGUI();
    }

    public GameObject ReturnGiverPrefab()
    {
        GameObject _currentGiverPrefab = giverPrefabs.Prefabs[1].Value;

        if (collectableType == ResourceType.Type1)
        {
            _currentGiverPrefab = giverPrefabs.Prefabs[1].Value;
        }
        else if (collectableType == ResourceType.Type2)
        {
            _currentGiverPrefab = giverPrefabs.Prefabs[2].Value;
        }
        else if (collectableType == ResourceType.Type3)
        {
            _currentGiverPrefab = giverPrefabs.Prefabs[3].Value;
        }
        else if (collectableType == ResourceType.Type4)
        {
            _currentGiverPrefab = giverPrefabs.Prefabs[4].Value;
        }
        else if (collectableType == ResourceType.Type0)
        {
            _currentGiverPrefab = giverPrefabs.Prefabs[0].Value;
        }

        return _currentGiverPrefab;
    }

    // just giver not purchasable
    private void Update()
    {
        if (giverType == ResourceType.Type3 && startGiving)
        {
            currentGenerationTime += 0.01f;
            hb.SetPercent(currentGenerationTime);

            if (currentGenerationTime >= GenerationSpeed.Value)
            {
                totalGiftCount++;

                countText.text = totalGiftCount + "/5";

                if (totalGiftCount == 5)
                    startGiving = false;

                /*
                if (CurrentPlayer == null)
                {
                    countText.text = totalGiftCount + "/5";

                    if (totalGiftCount == 4)
                    {
                        startGiving = false;
                    }
                }
                else if (CurrentPlayer != null)
                {
                    countText.text = CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value + "/5";

                    Debug.Log(CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value + "");

                    if (CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value == 4)
                    {
                        startGiving = false;
                    }
                }
                */

                currentGenerationTime = 0f;
                GiftGenerate();
            }
        }
    }

    // Start
    public Vector3 ReturnGiftPosition()
    {
        if (posList.Count > 0)
            CurrentXPos++;

        if (CurrentXPos < xCount.Value)
        {
            _x = (float)(CurrentXPos * Offset.Value.x);
        }
        else if (CurrentXPos >= xCount.Value)
        {
            CurrentXPos = 0;
            _x = 0;

            if (Mathf.Abs(_z) < zOffset)
            {
                _z -= Offset.Value.z;
            }
            else if (Mathf.Abs(_z) >= zOffset)
            {
                _z = 0;
                _y += Offset.Value.y;
            }
        }

        return new Vector3(_x, _y, _z);
    }

    // OnTriggerEnter
    public void SetCurrentPlayer()
    {
        CurrentPlayer = ObjectTrigger.triggeredObject;
        /*
        if (giverType == ResourceType.Type3)
        {
            if (CurrentPlayer != null)
                countText.text = CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value + "/5";
        }
        */
    }

    public void OnPlayerResourceDataChanged(int _amount)
    {
        if (giverType == ResourceType.Type3)
        {
            countText.text = _amount + "/5";
        }
    }

    // OnPurchased or OnTriggerEnter
    public void GiftGenerate()
    {
        if (giverType == ResourceType.Type1)
        {
            StartCoroutine(DelayedGiftGenerate());
        }
        else if (giverType == ResourceType.Type2 || giverType == ResourceType.Type3)
        {
            StartCoroutine(DelayedGiftGenerateOnSpesificPositions());
        }
    }

    // OnStay or OnEnter
    public void GiveGift()
    {
        if (giverType == ResourceType.Type3)
        {
            /*
            if(CurrentPlayer != null)
                countText.text = CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value + "/5";
            */

            EnableCounterGUI();

            /*
            if (CurrentPlayer == null)
            {
                if (totalGiftCount < 4)
                {
                    startGiving = true;
                }
            }
            else if (CurrentPlayer != null)
            {
                if (CurrentPlayer.GetComponent<PlayerCollectable>().CurrentCollectedCounts[3].Value < 4)
                {
                    startGiving = true;
                }
            }
            */

            if (totalGiftCount < 5)
            {
                startGiving = true;
            }
        }
        else
        {
            if (GiftList.Count > 0)
            {
                StartCoroutine(DelayedGiveGift());
            }
        }
    }

    // write
    public void StopGivingGift()
    {
        startGiving = false;

        // StopCoroutine
        StopCoroutine(DelayedGiveGift());
    }

    public IEnumerator DelayedGiftGenerate()
    {
        yield return new WaitForSeconds(GenerationSpeed.Value);

        Vector3 _pos = Vector3.zero;
        if (GiftList.Count == 0)
        {
            _pos = posList[0];
        }
        else if (GiftList.Count > 0)
        {
            _pos = posList[GiftList.Count - 1];
        }

        GameObject _newGift = Instantiate(GiftPrefab, _pos, Quaternion.identity, GiftParent);
        GiftList.Add(_newGift);
        CurrentGiftCount = GiftList.Count;

        _newGift.transform.localPosition = _pos;

        if (GiftList.Count < MaxGiftCount)
        {
            GiftGenerate();
        }
        else
        {
            generationCompleted = true;
        }
    }

    public IEnumerator DelayedGiftGenerateOnSpesificPositions()
    {
        if (CurrentGiftCount < MaxGiftCount || giverType == ResourceType.Type3)
        {
            yield return new WaitForSeconds(0.25f);

            if (giverType == ResourceType.Type2)
            {
                if (withAnimation)
                {
                    _newGift = Instantiate(GiftPrefab, AnimateParent.position, Quaternion.identity, GiftParent);
                    _newGift.transform.DOMove(SpesificPositionsList[GiftList.Count].position, 0.5f);
                }
                else
                {
                    _newGift = Instantiate(GiftPrefab, SpesificPositionsList[GiftList.Count].position, Quaternion.identity, GiftParent);
                }
            }
            else if (giverType == ResourceType.Type3)
            {
                _newGift = Instantiate(GiftPrefab, SpesificPositionsList[0].position, Quaternion.identity, GiftParent);
            }
            GiftList.Add(_newGift);
            CurrentGiftCount = GiftList.Count;

            if (giverType == ResourceType.Type3)
                CollectGiftWithWait();

            if (GiftList.Count < MaxGiftCount && !isGeneratingWithResource)
            {
                if (giverType != ResourceType.Type3)
                    StartCoroutine(DelayedGiftGenerateOnSpesificPositions());
            }
            else
            {
                generationCompleted = true;
            }
        }
    }

    public IEnumerator DelayedGiveGift()
    {
        yield return new WaitForSeconds(0.25f);

        if (GiftList.Count > 0)
        {
            CurrentPlayer.GetComponent<PlayerCollectable>().Collect(GiftList[GiftList.Count - 1].transform.position, collectableType);
            GameObject _collectedGift = GiftList[GiftList.Count - 1];
            GiftList.Remove(_collectedGift);
            Destroy(_collectedGift);

            CurrentGiftCount = GiftList.Count;

            // think
            if (giverType == ResourceType.Type1)
            {
                if (CurrentXPos > 0)
                    CurrentXPos--;
            }

            if (generationCompleted)
            {
                generationCompleted = false;

                if(!isGeneratingWithResource)
                    GiftGenerate();
            }

            OnCollectedResource.Invoke();
        }
    }

    public void CollectGiftWithWait()
    {
        CurrentPlayer.GetComponent<PlayerCollectable>().Collect(GiftList[0].transform.position, collectableType);
        GameObject _collectedGift = GiftList[0];
        GiftList.Remove(_collectedGift);
        Destroy(_collectedGift);
    }

    public void OnExit()
    {
        if (giverType == ResourceType.Type3)
            DisableCounterGUI();

        StopCoroutine(DelayedGiftGenerateOnSpesificPositions());

        startGiving = false;
        currentGenerationTime = 0f;
    }

    #region GUI
    public GameObject Canvas;
    public void InitializeGUI()
    {

    }

    public void EnableCounterGUI()
    {
        Canvas.SetActive(true);
        Canvas.transform.DOScale(Vector3.one, 0.5f);
    }

    public void DisableCounterGUI()
    {
        Canvas.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Canvas.SetActive(false));
    }
    #endregion GUI
}
