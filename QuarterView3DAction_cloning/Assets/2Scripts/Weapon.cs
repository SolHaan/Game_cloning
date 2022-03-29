using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������
public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //��������, ���Ÿ�
    public Type type; //���� Ÿ��
    public int damage; //������
    public float rate; //���ݼӵ�
    public BoxCollider meleeArea; //�������� ����
    public TrailRenderer trailEffect; //�ֵθ��� ȿ��

    //������
    public int maxAmmo; //�ִ� źâ
    public int curAmmo; //���� ���� ź

    //���Ÿ�
    public Transform bulletPos; //������ġ
    public GameObject bullet; //������ ����
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if(type == Type.Melee) //�����϶�
        {
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0) //���Ÿ��̸鼭 ���� ź�� ������
        {
            curAmmo--; //ź �Ҹ�
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        //Trail Renderer �� BoxCollider�� �ð����� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //yield return new WaitForSeconds(0.3f);
        //meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
    }

    IEnumerator Shot()
    {
        //#1. �Ѿ� �߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        ///�Ѿ˿� �ӵ� �����ϱ�
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //������ ������ z���̹Ƿ� forward

        yield return null;
        //#2. ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //z���� �ݴ����, Ƣ����� ����
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //���ϴ� ��, ���
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //ȸ���� ��
    }
}
