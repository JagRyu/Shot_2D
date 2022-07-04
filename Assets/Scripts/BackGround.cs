using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] backgrounds;

    float viewHeight;


    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize*2;
    }
    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if(backgrounds[endIndex].position.y < (-1)*viewHeight)
        {
            Vector3 backSpritePos = backgrounds[startIndex].localPosition;
            Vector3 frontSpritePos = backgrounds[endIndex].localPosition;
            backgrounds[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1) == -1 ?( backgrounds.Length - 1) : (startIndexSave - 1);

        }
    }

    //#. 스크롤링
}
