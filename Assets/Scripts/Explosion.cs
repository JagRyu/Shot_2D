using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Invoke("Disable", 2f);
    }
    void Disable()
    {
        gameObject.SetActive(false);
    }
    public void StratExplosion(string target)
    {
        anim.SetTrigger("Onexplosion");
        switch (target)
        {
            case "P":
            case "M":
                transform.localScale = Vector3.one * 0.7f;
                break;
            case "S":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 2f;
                break;
            case "B":
                transform.localScale = Vector3.one * 3f;
                break;
        }
    }
}
