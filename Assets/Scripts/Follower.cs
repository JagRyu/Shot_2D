using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay; //장전 딜레이
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    void Update()
    {
        Watch();
        // 되도록 캡슐화 하기!!
        Follow();
        Fire();
        Reload();
    }
    void Watch()
    {
        //FIFO #.Input Pos
        if (!parentPos.Contains(parent.position))
        {
            parentPos.Enqueue(parent.position);
        }
        //FIFO #.Output Pos
        if (parentPos.Count > followDelay)
        {
            followPos = parentPos.Dequeue();
        }
        else if(parentPos.Count > followDelay)
        {
            followPos = parent.position;
        }
    }
    void Follow()
    {
        transform.position = followPos;

    }
    void Fire()
    {
        if (!Input.GetButton("Jump"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        
        // 한발쏘고
        curShotDelay = 0;

    }

    void Reload()
    {
        // 장전하고
        curShotDelay += Time.deltaTime;

    }
}
