using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj; //아이템 프리팹
    public int[] itemPrice; //가격
    public Transform[] itemPos; //위치
    public string[] talkData;
    public Text talkText;

    Player enterPlayer;

    public void Enter(Player player) //입장
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //화면 정중앙
    }

    public void Exit() //퇴장
    {
        anim.SetTrigger("doHello"); //상점 인사
        uiGroup.anchoredPosition = Vector3.down * 1000; //아래로 다시 보냄
    }

    public void Buy(int index) //구입
    {
        int price = itemPrice[index]; //선택한 가격

        //구입을 못할 경우, 돈부족
        if(price > enterPlayer.coin)
        {
            //꼬이지 않도록 코루틴 껐다가 켜주기
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3); //랜덤 위치
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation); //구입 성공 시, 아이템 생성
    }
    
    IEnumerator Talk() //금액 부족 대사 몇초간 띄우기
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
