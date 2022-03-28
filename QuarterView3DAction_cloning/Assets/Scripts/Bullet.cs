using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bullet : MonoBehaviour
{
    public int damage;
    //�������� ������ �ı����� �ʵ���
    public bool isMelee;
    public bool isRock; //ū ����

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
