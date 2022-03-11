using UnityEngine;

public class DoorTrigger : TriggerEvents
{
    // public GameEvent _isInTrigger;

    public bool sendMe = true;
    public GameObject MyObject;

    public override void TriggerEnter(GameObject triggeredObject)
    {
        ArcadeManager.Instance.OpenMarketPanel();

        // _isInTrigger.Raise(true, sendMe ? MyObject : triggeredObject);
    }

    public override void TriggerExit(GameObject triggeredObject)
    {
        ArcadeManager.Instance.CloseMarketPanel();

        // _isInTrigger.Raise(false, sendMe ? MyObject : triggeredObject);
    }
}