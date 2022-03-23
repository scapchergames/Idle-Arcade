using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Taker : MonoBehaviour
{
    [EnumToggleButtons]
    public ResourceType takerType;

    [Space(20)]
    public bool isTakerAndGiver;

    [ShowIf("isTakerAndGiver")]
    public float ConvertingSpeed = 1f;

    [Space(20)]
    public bool showProperties;

    [Space(20)]
    public Transform TakerParent;

    [Space(20)]
    public List<Transform> TakerPositions = new List<Transform>();
    [ReadOnly, ShowIf("showProperties")]
    public List<GameObject> TakedObjects = new List<GameObject>();

    [Space(20)]
    [ReadOnly, ShowIf("showProperties")]
    public int TakedObjectsCount;

    [ReadOnly, ShowIf("showProperties")]
    public int MaxTakerCount;

    [ReadOnly, ShowIf("showProperties")]
    public bool isConverting = false;

    [Space(20)]
    [ReadOnly, ShowIf("showProperties")]
    public ObjectTrigger ObjectTrigger;
    public Giver giver;

    [Space(20)]
    public GameObject Canvas;
    public Image CounterBackgroundImage;
    public Image CounterResourceImage;
    public TextMeshProUGUI CounterText;

    public GameObject ConverterObj;

    [Space(20)]
    public UnityEvent OnTaked;
    public UnityEvent OnConverted;

    private void Start()
    {
        MaxTakerCount = TakerPositions.Count;

        ObjectTrigger = GetComponent<ObjectTrigger>();

        DisableCounterGUI();
    }

    #region GUI
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

    public void TriggerExit()
    {
        ObjectTrigger.triggeredObject.GetComponent<PlayerCollectable>().StopGiving();

        // if(isTakerAndGiver)
            // StartConverting();

        DisableCounterGUI();
    }

    public void TriggerEnter()
    {
        EnableCounterGUI();

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
        CounterText.text = TakedObjectsCount + "/4";

        _object.transform.position = TakerPositions[TakedObjectsCount].position;

        // resource to resource action
        if (isTakerAndGiver)
            StartConverting();

        OnTaked.Invoke();
    }

    public void StartConverting()
    {
        if (!isConverting)
        {
            StartCoroutine(CorConverting());
        }
    }

    public IEnumerator CorConverting()
    {
        if (TakedObjectsCount > 0 && giver.CurrentGiftCount < giver.MaxGiftCount)
        {
            isConverting = true;

            yield return new WaitForSeconds(ConvertingSpeed);

            if (TakedObjectsCount > 0 && giver.CurrentGiftCount < giver.MaxGiftCount)
            {
                ConverterObj.transform.DOShakeRotation(0.5f, 90, 10, 90, true);

                GameObject _convertedObj = TakedObjects[TakedObjectsCount - 1];
                _convertedObj.transform.DOMove(ConverterObj.transform.position, 0.3f).OnComplete(() => {
                    // TakedObjects.Remove(_convertedObj);
                    Destroy(_convertedObj);
                });

                TakedObjects.Remove(_convertedObj);
                // Destroy(_convertedObj);

                TakedObjectsCount = TakedObjects.Count;
                CounterText.text = TakedObjectsCount + "/4";

                OnConverted.Invoke();
            }

            if (TakedObjectsCount > 0 && giver.CurrentGiftCount < giver.MaxGiftCount)
            {
                StartCoroutine(CorConverting());
            }

            if (TakedObjectsCount <= 0 || giver.CurrentGiftCount >= giver.MaxGiftCount)
                isConverting = false;
        }
        else if (giver.CurrentGiftCount >= giver.MaxGiftCount)
        {
            isConverting = false;
        }
    }

    public void StopConverting()
    {
        StopCoroutine(CorConverting());
    }

    #region Gizmos
    [ColorPalette]
    public Color GizmosColor;
    public Texture myTexture;
    public Vector3 gizmosTextOffset;
    public Transform giverAreaPosition;
    public Vector3 gizmosScale;

    void OnDrawGizmos() //Selected
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = GizmosColor;

        // Taker area gizmos
        Gizmos.DrawWireCube(transform.position + new Vector3(1f, -0.6f, 0f), gizmosScale);

        // Giver area gizmos
        Gizmos.DrawWireCube(giverAreaPosition.position, gizmosScale);

        // Gizmos.DrawGUITexture( new Rect(10, 10, 20, 20), myTexture);

        // drawString("sex", transform.position + gizmosTextOffset, Color.red);
    }

    #endregion Gizmos

#if UNITY_EDITOR
    static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;

        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }
#endif
}