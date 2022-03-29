using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj; //������ ������
    public int[] itemPrice; //����
    public Transform[] itemPos; //��ġ
    public string[] talkData;
    public Text talkText;

    Player enterPlayer;

    public void Enter(Player player) //����
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //ȭ�� ���߾�
    }

    public void Exit() //����
    {
        anim.SetTrigger("doHello"); //���� �λ�
        uiGroup.anchoredPosition = Vector3.down * 1000; //�Ʒ��� �ٽ� ����
    }

    public void Buy(int index) //����
    {
        int price = itemPrice[index]; //������ ����

        //������ ���� ���, ������
        if(price > enterPlayer.coin)
        {
            //������ �ʵ��� �ڷ�ƾ ���ٰ� ���ֱ�
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3); //���� ��ġ
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation); //���� ���� ��, ������ ����
    }
    
    IEnumerator Talk() //�ݾ� ���� ��� ���ʰ� ����
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
