using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnResourceHit : MonoBehaviour
{
    // OnResourceHit
    [Header("OnResourceHit")]
    public FloatVariable duration;
    public Vector3Variable strength;
    public IntVariable vibration;
    public FloatVariable randomness;
    public BoolVariable fadeOut;

    [Header("Vibration")]
    public bool useVibration;

    public void StartFX()
    {
        Tween();

        /*
        if (useVibration)
            Vibration.VibrateLight();
        */
    }

    public void Tween()
    {
        Quaternion _currentValue = transform.rotation;
        transform.DOShakeRotation(duration.Value, strength.Value, vibration.Value, randomness.Value, fadeOut.Value).OnComplete(() =>
        {
            transform.DORotate(_currentValue.eulerAngles, 0.1f);
        });
    }
}
