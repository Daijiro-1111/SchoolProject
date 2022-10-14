using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletController : DebugParent
{
    #region enum

    #endregion

    #region const

    #endregion

    #region public property

    #endregion

    #region private property
    private Rigidbody _rb = null;

    private float _deltaTime = 0;
    #endregion

    #region public method

    #endregion

    #region private method
    [PunRPC]
    private void HitBullet()
    {
        Destroy(this.gameObject);
    }
    #endregion

    #region event
    public override void Awake()
    {
        base.Awake();
        _rb = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.AddForce(Vector3.forward * 10);
    }

    private void Update()
    {
        //_deltaTime += Time.deltaTime;
        //if (_deltaTime > 5) { photonView.RPC("DestroyThisObj", RpcTarget.All); }
        //transform.Translate(Vector3.forward * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    //private void OnCollisionEnter(Collision col)
    //{
    //    if (!photonView.IsMine)
    //    {
    //        photonView.RPC(nameof(HitBullet), RpcTarget.All);
    //    }
    //}
    #endregion
}