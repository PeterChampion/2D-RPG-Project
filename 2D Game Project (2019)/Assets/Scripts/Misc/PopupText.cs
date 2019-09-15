using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public TextMeshPro content;

    private void Awake()
    {
        content = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        Destroy(gameObject, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0.05f, 0);
    }
}
