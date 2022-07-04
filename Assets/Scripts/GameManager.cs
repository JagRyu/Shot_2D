using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//파일 읽기 위한 라이브러리
using System.IO;
public class GameManager : MonoBehaviour
{
    // 적 비행기 3개 변수화
    public string[] enemyObjs;

    // 소환할 위치 필요 5개 정도
    public Transform[] spawnPoints;
    // 1,2 초 뭐 랜덤으로 소환
    public float nextSpawnDelay;
    public float curSpawnDelay;

    //player 변수
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
        //#1.변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.리스폰 파일 읽기(열면 닫아야함)
        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if(line == null)
                break;
            
            //#3.리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //#.텍스트 파일 닫기
        stringReader.Close();

        //#.첫번째 스폰 딜레이 적용
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

        //적 생성 직후에 플레이어 변수를 넘겨주는 것으로 해결
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

        //#.리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //#.다음 리스폰 딜레이 갱신
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
        // gameOverSet 활성화
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
