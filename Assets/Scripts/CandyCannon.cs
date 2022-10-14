using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCannon : MonoBehaviour
{
    [SerializeField] private GameObject _candy;
    private string _thisCannonTag;
    private Transform _candyPool;
    private const float SHOT_COOL_TIME = 2f;
    private const int SHOT_POWER = 25;
    private const float X_SHOT_RANGE = 20;
    private const float Y_SHOT_RANGE = 5;

    void Start()
    {
        _candyPool = GameObject.FindWithTag(TagData.CANDY_POOL_TAG).transform;
        _thisCannonTag = transform.tag;

        StartCoroutine(Shot());
    }

    private IEnumerator Shot()
    {
        while (true)
        {
            // ２秒おきに発射する
            yield return new WaitForSeconds(SHOT_COOL_TIME);

            GameObject candy = InstCandy();

            // 横方向に加える力
            float randomX =0;
            // プレイヤーから見て左の大砲
            if (_thisCannonTag == TagData.LEFT_CANNON_TAG) { randomX = Random.Range(0, X_SHOT_RANGE); }
            // 右の大砲
            else if (_thisCannonTag == TagData.RIGHT_CANNON_TAG) { randomX = Random.Range(-X_SHOT_RANGE, 0); }

            // 縦方向に加える力
            float randomY = Random.Range(-Y_SHOT_RANGE, Y_SHOT_RANGE);

            // 発射
            candy.GetComponent<Rigidbody>().AddForce(-transform.parent.forward * SHOT_POWER +
                transform.right * randomX + transform.up * randomY, ForceMode.VelocityChange);
        }
    }

    private GameObject InstCandy()
    {
        // プール内の非アクティブなキャンディを探索
        foreach(Transform t in _candyPool)
        {
            if(!t.gameObject.activeSelf)
            {
                t.gameObject.SetActive(true);
                t.transform.position = transform.position;
                t.GetComponent<Rigidbody>().velocity = Vector3.zero;
                t.GetComponent<Candy>().DeleteCountStart();
                return t.gameObject;
            }
        }

        // なければ新たに生成
        return Instantiate(_candy, transform.position, Quaternion.identity,_candyPool);
    }
}
