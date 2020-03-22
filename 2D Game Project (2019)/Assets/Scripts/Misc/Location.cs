using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] private string locationName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.instance.player.gameObject)
        {
            GameManager.instance.OnLocationEnteredCallback.Invoke(locationName);
        }
    }
}
