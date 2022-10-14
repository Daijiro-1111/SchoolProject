using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private const float CANDY_DELETE_TIME = 5.0f;  

    void Start()
    {
        DeleteCountStart();
    }

    public void DeleteCountStart()
    {
        StartCoroutine(CandyDelete());
    }

    // èÍäOÇ‹ÇΩÇÕê∂ê¨Ç©ÇÁ5ïbåoâﬂÇ≈è¡Ç∑
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(TagData.DEATH_AREA_TAG))
        {
            transform.gameObject.SetActive(false);
        }
    }
    private IEnumerator CandyDelete()
    {
        yield return new WaitForSeconds(CANDY_DELETE_TIME);
        transform.gameObject.SetActive(false);
    }
}
