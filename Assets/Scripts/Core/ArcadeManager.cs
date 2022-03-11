using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeManager : MonoBehaviour
{
    private static ArcadeManager instance = null;

    public static ArcadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("IdleManager").AddComponent<ArcadeManager>();
            }

            return instance;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    public ArcadeData ArcadeData;

    [Space(20)]
    public Sprite[] ResourceSprites;
    public Image[] ResourceImages;
    public Text[] ResourceTexts;

    public Text CoinText;

    [Space(20)]
    public SmoothCamera CameraController;

    public OnResourceCollect coinsManager;
    public GameObject Player;

    public GameObject MarketPanel;

    bool onClicked = false;

    private void Start()
    {
        InitializeData();

        OnUpdateResource();

        // OnUpdateCoin();

        // OnGameStart Events
    }

    public void InitializeData()
    {
        for (int i = 0; i < ResourceImages.Length; i++)
        {
            ResourceImages[i].sprite = ResourceSprites[i];
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !onClicked)
        {
            PlayGameAction();
            onClicked = true;
        }
    }

    public void PlayGameAction()
    {
        CameraController.cameraState = CameraController.cameraStates[1].cameraState;
    }

    public void IncreaseResource(int _amount, CollectableType _collectableType)
    {
        if (_collectableType == CollectableType.Coin)
        {
            coinsManager.OnCollect(_amount, 1);
        }
        else
        {
            coinsManager.OnCollect(_amount, 0);
        }

        ArcadeData.CurrentResources[ReturnResourceIndex(_collectableType)].Value += _amount;

        OnUpdateResource();
    }

    public void DecreaseResource(int _amount, CollectableType _collectableType)
    {
        ArcadeData.CurrentResources[ReturnResourceIndex(_collectableType)].Value -= _amount;

        OnUpdateResource();
    }

    public void OnUpdateResource()
    {
        for (int i = 0; i < ArcadeData.CurrentResources.Length; i++)
        {
            ResourceTexts[i].text = ArcadeData.CurrentResources[i].Value.ToString();
        }
    }

    /*
    public void OnUpdateCoin()
    {
        CoinText.text = ArcadeData.CurrentCoin.ToString();
    }
    */

    public int ReturnResourceIndex(CollectableType _collectableType)
    {
        int _currentResourceIndex = 0;

        if (_collectableType == CollectableType.Type1)
        {
            _currentResourceIndex = 1;
        }
        else if (_collectableType == CollectableType.Type2)
        {
            _currentResourceIndex = 2;
        }
        else if (_collectableType == CollectableType.Type3)
        {
            _currentResourceIndex = 3;
        }
        else if (_collectableType == CollectableType.Type4)
        {
            _currentResourceIndex = 4;
        }
        else if (_collectableType == CollectableType.Coin)
        {
            _currentResourceIndex = 0;
        }

        return _currentResourceIndex;
    }

    public void OpenMarketPanel()
    {
        MarketPanel.SetActive(true);
    }

    public void CloseMarketPanel()
    {
        MarketPanel.SetActive(false);
    }

    public void SellResourceButton()
    {
        int increaseAmount = ArcadeData.CurrentResources[1].Value / ArcadeData.ResourceToCoinMultiply;

        coinsManager.OnCollect(increaseAmount, 1);

        ArcadeData.CurrentResources[0].Value += increaseAmount;
        // ArcadeData.CurrentCoin += increaseAmount;

        ArcadeData.CurrentResources[1].Value = 0;

        OnUpdateResource();

        // OnUpdateCoin();
    }
}
