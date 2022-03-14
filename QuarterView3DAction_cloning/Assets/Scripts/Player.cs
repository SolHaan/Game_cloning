using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //�ӵ�
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades; //������ü
    public int hasGrenades;

    //������ ����
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenade;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1; // ���ǿ��� 0���� �ױ� ������

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //�ڽĿ�����Ʈ�� ������Ʈ�� ������
    }

    void Update()
    {
        GetInput(); //�Է� ����
        Move();
        Turn();
        Jump();
        Dodge();
        Swap();
        Interation();
    }

    //�ڵ带 ��ɿ� ���� ���еǰ� �Լ��� �и��ϱ�
    void GetInput()
    {
        //Ű���� �Է�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical"); //projectsetting - inputmanager���� �����ϰ� ����
        //Shift�� ���� ���� �۵��ǵ��� GetButton() �Լ� ���
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        //normalized: ���� ���� 1�� ������ ����(���� ���ΰ� 1�̾ �밢���� �� �� ��Ʈ2�̱� ������
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //������ ����

        //ȸ�Ǹ� �� �� ȸ�Ǹ� �ϰ� �ִ� ��������
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

        //�̵� * �ӵ�, transform�� deltatime���� �ֱ�
        //���� ������(bool ���� ���� ? true�� �� �� : false �� �� ��, if�� ��� ���
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero); //�⺻�� �޸���
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //LookAt(): ������ ���͸� ���ؼ� ȸ�������ִ� �Լ�
        transform.LookAt(transform.position + moveVec); //���ư��� �������� ȸ���Ѵ�
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            //AddForce() �Լ��� �������� ���� ���ϱ�
            //ForceMode�� 4���� ��尡 ����, Impulse: �������
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    //ȸ��
    void Dodge()
    {
        //�������� �������� �߰��ؼ� ������ ȸ�Ƿ� ������
        //�׼� ���� �ٸ� �׼��� ������� �ʵ��� ���� �߰�(!isDodge)
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2; //ȸ�Ǵ� �̵��ӵ��� 2��� ����ϵ���
            anim.SetTrigger("doDodge");
            isDodge = true;

            //�ð���
            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f; //2�迴���ϱ� 0.5�� ���ϸ� ������� ���ƿ�
        isDodge = false;
    }

    //���� ��ü
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            //����� ���
            if(equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    //���� �Լ�
    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    //���� ����
    void OnCollisionEnter(Collision collision)
    {
        //�ٴڿ� �ε����� ���� ����
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            //enum Ÿ�Ժ��� + switch������ ���ܸ���ϰ� ���ǹ� ����
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //������ ��ġ�� �÷��̾� ��ġ�� ����
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    hasGrenade += item.value;
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    //������Ʈ ����
    void OnTriggerStay(Collider other)
    {
        //���� �� �ִ� ������Ʈ�� weapon�̴�
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        //Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
