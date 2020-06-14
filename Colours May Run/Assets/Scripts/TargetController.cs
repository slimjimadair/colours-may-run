using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag == "Player")
        {
            transform.parent.GetComponent<Game>().Complete();
        }
    }
}
