using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum: ������ Ÿ�� (Ÿ�� �̸� ���� �ʿ�)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //GetComponent�� ù��° ������Ʈ�� ������, Ȯ���ʼ�
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        //Rotate() �Լ��� ��� ȸ���ϵ��� ȿ�� ����
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }

        if (collision.gameObject.tag == "Player")
        {
            if(type == Type.Coin)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                player.coin += value;
                Destroy(gameObject);
            }
        }
    }
}
