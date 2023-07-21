using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class CharacterMove : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3         curPos;
    PhotonView      _pv;

    void Update()
    {
        if (_pv.IsMine)
        {
            BorderCheck();
        }
        else
        {
            transform.position = curPos;
        }
    }  

    public void Awake()
    {
        _pv = GetComponent<PhotonView>();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }

    private void BorderCheck()
    {
        // Game Screen의 최대 크기
        Vector3 lt = new Vector3(-8.05f, 2.28f, 0);
        Vector3 rb = new Vector3(3.59f, -4.23f, 0);

        Vector3 newPos = transform.position;

        if(transform.position.x < lt.x)
        {
            newPos = new Vector3(lt.x,transform.position.y, 0);
        }
        else if(transform.position.x > rb.x)
        {
            newPos = new Vector3(rb.x, transform.position.y, 0);
        }
        else if(transform.position.y > lt.y)
        {
            newPos = new Vector3(transform.position.x, lt.y, 0);
        }
        else if (transform.position.y < rb.y)
        {
            newPos = new Vector3(transform.position.x, rb.y, 0);
        }
        else
        {
            return;
        }
        transform.position = newPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(6,6);
    }

}
