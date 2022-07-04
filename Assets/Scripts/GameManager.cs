using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//���� �б� ���� ���̺귯��
using System.IO;
public class GameManager : MonoBehaviour
{
    // �� ����� 3�� ����ȭ
    public string[] enemyObjs;

    // ��ȯ�� ��ġ �ʿ� 5�� ����
    public Transform[] spawnPoints;
    // 1,2 �� �� �������� ��ȯ
    public float nextSpawnDelay;
    public float curSpawnDelay;

    //player ����
    public GameObject player;
    public Text scoreText;
    public Image[] lifeIcon;
    public Image[] boomIcon;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" , "EnemyB" };
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        //#1.���� �ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.������ ���� �б�(���� �ݾƾ���)
        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if(line == null)
                break;
            
            //#3.������ ������ ����
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //#.�ؽ�Ʈ ���� �ݱ�
        stringReader.Close();

        //#.ù��° ���� ������ ����
        nextSpawnDelay = spawnList[0].delay;
    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        //Score
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);

    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;

        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        //�� ���� ���Ŀ� �÷��̾� ������ �Ѱ��ִ� ������ �ذ�
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = this;

        if(enemyPoint == 5 || enemyPoint == 6)
        {
            enemy.transform.Rotate(Vector3.forward *45);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.velocity = new Vector2(enemyLogic.speed*(-1), -1);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed*(-1));

        }

        //#.������ �ε��� ����
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //#.���� ������ ������ ����
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 4;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void UpdateLifeIcon(int life)
    {

        // #. UI Life Init Disable
        for (int index = 0; index < 3; index++)
        {
            lifeIcon[index].color = new Color(1, 1, 1, 0);
        }

        //#.UI Life Active
        for (int index = 0; index <life; index++)
        {
            lifeIcon[index].color = new Color(1, 1, 1,1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {

        // #. UI Life Init Disable
        for (int index = 0; index < 2; index++)
        {
            boomIcon[index].color = new Color(1, 1, 1, 0);
        }

        //#.UI Life Active
        for (int index = 0; index < boom; index++)
        {
            boomIcon[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StratExplosion(type);
    }

    public void GameOver()
    {
        // gameOverSet Ȱ��ȭ
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
