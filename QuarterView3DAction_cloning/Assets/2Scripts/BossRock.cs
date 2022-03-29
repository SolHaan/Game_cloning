using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 1; //회전파워
    float scaleValue = 0.1f; //크기 숫자값 변수 생성
    bool isShoot; //기를 모으고 쏘는 타이밍을 관리

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    //쏘는 타이밍
    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while(!isShoot)
        {
            angularPower += 0.005f;
            scaleValue += 0.002f;
            //증가된 값을 트랜스폼, 리지드바디에 적용
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //Acceleration: 지속적으로
            yield return null;
        }
    }
}
