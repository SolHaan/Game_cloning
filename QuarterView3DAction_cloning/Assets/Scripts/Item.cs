using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum: 열거형 타입 (타입 이름 지정 필요)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    void Update()
    {
        //Rotate() 함수로 계속 회전하도록 효과 내기
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
