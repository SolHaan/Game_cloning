using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum: 열거형 타입 (타입 이름 지정 필요)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //GetComponent는 첫번째 컴포넌트를 가져옴, 확인필수
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        //Rotate() 함수로 계속 회전하도록 효과 내기
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
