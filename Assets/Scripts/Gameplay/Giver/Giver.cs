using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RengeGames.HealthBars;
using TMPro;

public class Giver : MonoBehaviour
{
    [Header("Giver")]
    public CollectableType collectableType;
    public GiverType giverType;

    public GameObject GiftPrefab;
    public Transform GiftParent;

    public IntVariable xCount;
    public IntVariable yCount;
    public IntVariable zCount;

    public FloatVariable GenerationSpeed;

    public Vector3Variable Offset;

    float zOffset;
    float _x, _y, _z;
    int CurrentXPos;

    int MaxGiftCount;
    int CurrentGiftCount;
    public List<GameObject> GiftList = new List<GameObject>();
    List<Vector3> posList = new List<Vector3>();

    bool generationCompleted = false;

    PurchasableItem PurchasableItem;

    public List<Transform> SpesificPositionsList = new List<Transform>();

    public TextMeshProUGUI countText;
    public UltimateCircularHealthBar hb;

    // [ShowOnly]
    public GameObject CurrentPlayer;
    public ObjectTrigger ObjectTrigger;

    public bool startGiving = false;
    public float currentGenerationTime;

    private GameObject _newGift;

    public int totalGiftCount;

    public bool isGeneratingWithResource = false;

    private void Start()
    {
        if(giverType == GiverType.Type1 || giverType == GiverType.Type2)
            PurchasableItem = GetComponent<PurchasableItem>();

        ObjectTrigger = GetComponent<ObjectTrigger>();

        if (giverType == GiverType.Type1)
        {
            MaxGiftCount = (int)(xCount.Value * zCount.Value) * yCount.Value;
            zOffset = Offset.Value.z * 3f;
            for (int i = 0; i < MaxGiftCount; i++)
            {
                Vector3 _pos = ReturnGiftPosition();
                posList.Add(_pos);
            }
        }
        else if (giverType == GiverType.Type2 || giverType == GiverType.Type3)
        {
            MaxGiftCount = SpesificPositionsList.Count;
        }
    }

    // just giver not purchasable
    private void Update()
    {
        if (giverType == GiverType.Type3 && startGiving)
        {
            currentGenerationTime += 0.01f;
            hb.SetPercent(currentGenerationTime);

            if (currentGenerationTime >= GenerationSpeed.Value)
            {
                totalGiftCount++;
                countText.text = totalGiftCount + "/5";

                if (totalGiftCount == 5)
                    startGiving = false;

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
    }

    // OnPurchased or OnTriggerEnter
    public void GiftGenerate()
    {
        if (giverType == GiverType.Type1)
        {
            StartCoroutine(DelayedGiftGenerate());
        }
        else if (giverType == GiverType.Type2 || giverType == GiverType.Type3)
        {
            StartCoroutine(DelayedGiftGenerateOnSpesificPositions());
        }
    }

    // OnStay or OnEnter
    public void GiveGift()
    {
        if (giverType == GiverType.Type3)
        {
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
        yield return new WaitForSeconds(0.25f);

        if (giverType == GiverType.Type2)
        {
            _newGift = Instantiate(GiftPrefab, SpesificPositionsList[GiftList.Count].position, Quaternion.identity, GiftParent);
        }
        else if(giverType == GiverType.Type3)
        {
            _newGift = Instantiate(GiftPrefab, SpesificPositionsList[0].position, Quaternion.identity, GiftParent);
        }
        GiftList.Add(_newGift);
        CurrentGiftCount = GiftList.Count;

        if (giverType == GiverType.Type3)
            CollectGiftWithWait();

        if (GiftList.Count < MaxGiftCount && !isGeneratingWithResource)
        {
            if(giverType != GiverType.Type3)
            StartCoroutine(DelayedGiftGenerateOnSpesificPositions());
        }
        else
        {
            generationCompleted = true;
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

            // think
            if (giverType == GiverType.Type1)
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
        StopCoroutine(DelayedGiftGenerateOnSpesificPositions());

        startGiving = false;
        currentGenerationTime = 0f;
    }
}
