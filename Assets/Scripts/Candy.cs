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

    // 場外または生成から5秒経過で消す
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
