using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bullet : MonoBehaviour
{
    public int damage;
    //근접공격 범위가 파괴되지 않도록
    public bool isMelee;
    public bool isRock; //큰 공격

    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Floor" && gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
