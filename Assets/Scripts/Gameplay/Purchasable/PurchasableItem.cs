using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PurchasableItem : MonoBehaviour
{
    [Tag]
    public string targetTag;

    [EnumToggleButtons]
    public ResourceType purchasableType;

    [Space(20)]
    [ReadOnly]
    public bool isPurchased = false;

    [ReadOnly]
    [ProgressBar(0, 15)]
    public int runtimePrice;

    [Space(20)]
    [Required("required")]
    public IntVariable Price;
    [Required("required")]
    public BoolVariable showMeshStart;
    [Required("required")]
    public MaterialVariable SaleMaterial;
    [Required("required")]
    public MaterialVariable PurchasedMaterial;

    [Space(20)]
    public GameObject Mesh;
    public TextMeshProUGUI text;

    [Space(20)]
    [ReadOnly]
    public ObjectTrigger ObjectTrigger;

    [Space(20)]
    public UnityEvent OnStayEvent;
    public UnityEvent OnPurchasedEvent;

    private void Start()
    {
        text.text = "$" + Price.Value;
        runtimePrice = Price.Value;
        Mesh.GetComponent<MeshRenderer>().material = SaleMaterial.Value;
        if (showMeshStart.Value)
        {
            Mesh.SetActive(true);
        }
        else
        {
            Mesh.SetActive(false);
        }

        ObjectTrigger = GetComponent<ObjectTrigger>();
    }

    public void TriggerExit()
    {
        ObjectTrigger.triggeredObject.GetComponent<PlayerCollectable>().StopPurchasing();
    }

    public void TriggerEnter()
    {
        if (!isPurchased)
            ObjectTrigger.triggeredObject.GetComponent<PlayerCollectable>().Purchase(purchasableType, transform.position, runtimePrice, this);
    }

    public void TriggerStay()
    {
        if (isPurchased)
        {
            OnStayEvent.Invoke();
        }
    }

    public void OnPurchased()
    {
        isPurchased = true;

        Mesh.GetComponent<MeshRenderer>().material = PurchasedMaterial.Value;

        Vector3 _normalScale = Mesh.transform.localScale;
        Vector3 _scaleAnim = new Vector3(
            Mesh.transform.localScale.x + (ReturnOnePercent(Mesh.transform.localScale.x) * 10f),
            Mesh.transform.localScale.y + (ReturnOnePercent(Mesh.transform.localScale.y) * 10f),
            Mesh.transform.localScale.z + (ReturnOnePercent(Mesh.transform.localScale.z) * 10f));
        Mesh.transform.DOScale(_scaleAnim, 0.3f);

        StartCoroutine(ReturnNormalScale(_normalScale));

        if (!showMeshStart.Value)
            Mesh.SetActive(true);

        OnPurchasedEvent.Invoke();

        text.text = "";
    }

    public IEnumerator ReturnNormalScale(Vector3 _scale)
    {
        yield return new WaitForSeconds(0.6f);
        Mesh.transform.DOScale(_scale, 0.3f);
    }

    public float ReturnOnePercent(float _value)
    {
        return _value / 100f;
    }

    public void UpdateText()
    {
        text.text = "$" + runtimePrice;

        if (runtimePrice == 0)
        {
            text.text = "";
        }
    }

    public void DecreasePrice()
    {
        runtimePrice--;
        UpdateText();
        if (runtimePrice == 0)
        {
            OnPurchased();
        }
    }
}
