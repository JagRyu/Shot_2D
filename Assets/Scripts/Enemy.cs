using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int hp;
    public int enemyScore;
    public Sprite[] sprites;

    public string enemyName;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemBoom;
    public GameObject itemPower;
    public GameManager gameManager;

    public GameObject player;
    public ObjectManager objectManager;
    public float maxShotDelay;
    public float curShotDelay;

    // 맞으면 하얀색으로 변해야하니까 SpriteRenderer 필요
    SpriteRenderer spriteRenderer;

    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;


    void Update()
    {
        if (enemyName == "B")
            return;
        // 되도록 캡슐화 하기!!
        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;
        if(enemyName == "M")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
            
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Vector3 dirVecR = player.transform.position - transform.position;

            rigidR.AddForce(dirVecR.normalized * 3, ForceMode2D.Impulse);

            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Vector3 dirVecL = player.transform.position - transform.position;

            rigidL.AddForce(dirVecL.normalized * 3, ForceMode2D.Impulse);
        }

        // 한발쏘고
        curShotDelay = 0;
    }

    void Reload()
    {
        // 장전하고
        curShotDelay += Time.deltaTime;

    }

    void Awake()
    {
        //초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                hp = 3000;
                Invoke("Stop", 2);
                break;
            case "L":
                hp = 40;
                break;
            case "M":
                hp = 10;
                break;
            case "S":
                hp = 3;
                break;
        }
    }
    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }
    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        //#.Fire 4 Bullet Foward
        GameObject bulletR = objectManager.MakeObj("BulletBossB");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletRR = objectManager.MakeObj("BulletBossB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.3f;
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();

        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletL = objectManager.MakeObj("BulletBossB");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        GameObject bulletLL = objectManager.MakeObj("BulletBossB");
        bulletLL.transform.position = transform.position + Vector3.left * 0.3f;
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //#.Pattern Counting
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireFoward", 2);
        }
        else
        {
            Invoke("Think", 3);
        }

    }
    void FireShot()
    {
        //#.ShotGun to Player
        for (int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f,2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireShot", 3.5f);
        }
        else
        {
            Invoke("Think", 3);
        }
    }
    void FireArc()
    {
        //#.Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //목표물로 방향 = 목표물 위치 - 자신의 위치
        Vector2 dirVec = new Vector2(Mathf.Sin(Mathf.PI*10*curPatternCount/ maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireArc", 0.15f);
        }
        else
        {
            Invoke("Think", 3);
        }

    }
    void FireAround()
    {
        //#.Fire Around
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = (curPatternCount%2) == 0 ? roundNumA : roundNumB;

        for (int index = 0; index < roundNum; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //목표물로 방향 = 목표물 위치 - 자신의 위치
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                       , Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward*90;
            bullet.transform.Rotate(rotVec);
        }
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireAround", 0.8f);
        }
        else
        {
            Invoke("Think", 3);
        }

    }

    public void OnHit(int dmg)
    {

        if (hp <= 0)
            return;
        // player한테 맞음
        hp -= dmg;
        if(enemyName == "B")
        {
            anim.SetTrigger("Onhit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            //시간차
            Invoke("ReturnSprite", 0.1f);
        }
       

        if(hp <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //#. Random Ratio Item Drop
            int ran = enemyName == "B" ? 0: Random.Range(0, 10);
            if(ran < 4)
            {
                Debug.Log("Not Item");
            }
            else if (ran>3 &&ran < 6)
            {
                GameObject itemCoin = objectManager.MakeObj("Coin");
                itemCoin.transform.position = transform.position;           
            }
            else if (ran>5&&ran < 7)
            {
                GameObject itemBoom = objectManager.MakeObj("Boom");
                itemBoom.transform.position = transform.position;
            }
            else if (ran>7&&ran < 9)
            {
                GameObject itemPower = objectManager.MakeObj("Power");
                itemPower.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;

        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
