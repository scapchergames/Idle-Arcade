using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnRegenerate : MonoBehaviour
{
    // OnRegenerate
    [Header("OnRegenerate")]
    public FloatVariable tweenTime;
    public Ease _easeType;

    public void Tween()
    {
        Vector3 _currentScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_currentScale, tweenTime.Value).SetEase(_easeType);
    }
}
