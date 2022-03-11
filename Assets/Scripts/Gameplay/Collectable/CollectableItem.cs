using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectableItem : MonoBehaviour
{
    public GameObject Mesh;
    bool isCollected = false;

    [Tag]
    public string targetTag;

    public CollectableType collectableType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == targetTag && !isCollected)
        {
            isCollected = true;
            Mesh.SetActive(false);

            other.gameObject.GetComponent<PlayerCollectable>().Collect(transform.position, collectableType);
        }
    }
}
