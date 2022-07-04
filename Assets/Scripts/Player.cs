using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public int power;
    public int maxpower;
    public int boom;
    public int maxboom;

    public bool isTouchToTop;
    public bool isTouchToBottom;
    public bool isTouchToRight;
    public bool isTouchToLeft;
    public bool isHit; // �ѹ��� �ι� �´� ���� ���ֱ�
    public bool isBoomTime;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject BoomEffect;
    public GameManager gameManager;
    public ObjectManager objectManager;
    public float maxShotDelay;
    public float curShotDelay; //���� ������

    Animator anim;
    SpriteRenderer spriteRenderer;
    //����� ����
    public int life;
    public int score;

    public GameObject []followers;

    public bool isRespawmTime;
    private void Awake()
    {
        //�ִϸ��̼� �ʱ�ȭ
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbetable();
        Invoke("Unbetable",3);
    }

    void Unbetable()
    {
        isRespawmTime = !isRespawmTime;
        if (isRespawmTime)
        {
            isRespawmTime = true;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for(int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            isRespawmTime = false;
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }
    void Update()
    {
        // �ǵ��� ĸ��ȭ �ϱ�!!
        Move();
        Fire();
        Reload();
        Boom();
    }

    void Move()
    {
        // �÷��̾� �̵� (���� ���� �̵��� + Speed + deltaTime)
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchToRight && h == 1) || (isTouchToLeft && h == -1))
        {
            h = 0;
        }

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchToTop && v == 1) || (isTouchToBottom && v == -1))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; // transform �̵��� ������ deltaTime ���������

        transform.position = curPos + nextPos;


        //�ִϸ��̼��� Ű�� �������� or ������
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            //getInteger = �������� , setInteger = �� �ֱ�
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        if (!Input.GetButton("Jump"))
            return;
        
        if (curShotDelay < maxShotDelay)
            return;


        //destroy �ݴ� Instantiate(Object, Vector3, Quaternion)
        // power one
        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");

                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");

                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                bulletCC.transform.position = transform.position;
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                break;
            
        }
         
            // �ѹ߽��
         curShotDelay = 0;
     
    }

    void Reload()
    {
        // �����ϰ�
        curShotDelay += Time.deltaTime;

    }

    void Boom()
    {
        if (!Input.GetButton("Fire1"))
            return;
        if (isBoomTime)
            return;
        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        BoomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);
        //#2. Remove Enemy
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }

        //#3. Remove Enemy Bullet
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            bulletsA[index].SetActive(false);
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            bulletsB[index].SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�÷��̾ ȭ�� ������ ������ ���ϵ��� ��� ����(colider) + ���������� rigidbody
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name) {

                case "Top":
                    isTouchToTop = true;
                    break;

                case "Bottom":
                    isTouchToBottom = true;
                    break;

                case "Right":
                    isTouchToRight = true;
                    break;

                case "Left":
                    isTouchToLeft = true;
                    break;
            }
            
        }
        // �� ����(�÷��̾�� �ı��Ǹ� �ȵ�. ���? ������ ����)
        else if(collision.gameObject.tag == "Enemy"||collision.gameObject.tag == "EnemyBullet")
        {
            if (isRespawmTime)
            {
                return;
            }
            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameManager.RespawnPlayer();

            gameObject.SetActive(false);
            //Invoke ��� x, �÷��̾� ���ʹ� gameManager ����

        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxpower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxboom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;

            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if(power == 4)
        {
            followers[0].SetActive(true);
        }
        else if (power == 5)
        {
            followers[1].SetActive(true);
        }
        else if (power == 6)
        {
            followers[2].SetActive(true);
        }
    }

    void OffBoomEffect()
    {
        BoomEffect.SetActive(false);
        isBoomTime = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {

                case "Top":
                    isTouchToTop = false;
                    break;

                case "Bottom":
                    isTouchToBottom = false;
                    break;

                case "Right":
                    isTouchToRight = false;
                    break;

                case "Left":
                    isTouchToLeft = false;
                    break;
            }

        }


    }
}

