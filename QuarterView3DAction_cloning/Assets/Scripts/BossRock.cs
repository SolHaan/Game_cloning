using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 1; //ȸ���Ŀ�
    float scaleValue = 0.1f; //ũ�� ���ڰ� ���� ����
    bool isShoot; //�⸦ ������ ��� Ÿ�̹��� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    //��� Ÿ�̹�
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
            //������ ���� Ʈ������, ������ٵ� ����
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //Acceleration: ����������
            yield return null;
        }
    }
}
