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
    public int score;
    public int hasGrenade;

    //����ź
    public GameObject grenadeObj;
    
    //���� ��ġ�� �ѽ��
    public Camera followCamera;

    public GameManager manger;
    public AudioSource jumpSound;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown; //����
    bool gDown; //����ź ����
    bool rDown; //����
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true; //������ �� �غ�Ϸ�
    bool isBorder; //�� �浹 �÷���
    bool isDamage; //����Ÿ��
    bool isShop; //������
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1; // ���ǿ��� 0���� �ױ� ������
    float fireDelay; //���ݵ�����

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //�ڽĿ�����Ʈ�� ������Ʈ�� ������
        meshs = GetComponentsInChildren<MeshRenderer>();

        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 112500);
    }

    void Update()
    {
        GetInput(); //�Է� ����
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
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
        fDown = Input.GetButton("Fire1"); //���콺 ����
        gDown = Input.GetButtonDown("Fire2"); //���콺 ������
        rDown = Input.GetButtonDown("Reload"); //R
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //������ ����

        //ȸ�Ǹ� �� �� ȸ�Ǹ� �ϰ� �ִ� ��������
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady || isDead) //���� �߿��� �̵� �Ұ� �ǵ��� ����, ���� ��
            moveVec = Vector3.zero;

        //�̵� * �ӵ�, transform�� deltatime���� �ֱ�
        //���� ������(bool ���� ���� ? true�� �� �� : false �� �� ��, if�� ��� ���
        if (!isBorder) //�̵� ����
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero); //�⺻�� �޸���
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#1. Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); //���ư��� �������� ȸ���Ѵ�

        //#2. ���콺�� ���� ȸ��
        if(fDown && !isDead) //���콺 Ŭ�� �ÿ���
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            //ray�� ���� floor ��ġ
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0; //���� ����
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            //AddForce() �Լ��� �������� ���� ���ϱ�
            //ForceMode�� 4���� ��尡 ����, Impulse: �������
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            jumpSound.Play();
        }
    }

    //����ź
    void Grenade()
    {
        //����ź ���� ������ ���� ���ǵ�
        if (hasGrenade == 0)
            return;

        if(gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                //������ ����ź�� ������ٵ� Ȱ���Ͽ� ������ ���� ����
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse); //ȸ��

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }

    //����
    void Attack()
    {
        if (equipWeapon == null) //���Ⱑ �������� ����ǵ��� ������� üũ
            return;

        //���ݵ����̿� �ð��� �����ְ� ���ݰ��� ���θ� Ȯ��
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay; //���Ӻ��� ���� �����ؼ� ������

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead) //ȸ��, ��ü�Ҷ� ����
        {
            equipWeapon.Use(); //���⿡ �ִ� �Լ� ����
            //���׿����ڸ� ����ؼ� �����̸� ����, ���Ÿ��� ���
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0; //���� ���ݱ��� ��ٸ����� ������ 0
        }
    }

    //������
    void Reload()
    {
        if (equipWeapon == null) //���� ������
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //����
            return;

        if (ammo == 0) //�Ѿ� ����
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f); //���� 3��
        }
    }

    void ReloadOut()
    {
        //�÷��̾ ������ ź�� ����ؼ� ����ϱ�
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo; //����� ź�� ����
        ammo -= reAmmo; //�� ������ŭ ���� ź�� �����
        isReload = false;
    }

    //ȸ��
    void Dodge()
    {
        //�������� �������� �߰��ؼ� ������ ȸ�Ƿ� ������
        //�׼� ���� �ٸ� �׼��� ������� �ʵ��� ���� �߰�(!isDodge)
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead)
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

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {
            //����� ���
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

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
        if (iDown && nearObject != null && !isJump && !isDodge && !isShop && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    //������ ���� ���� ��ġ��
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    //�� ��� ����
    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //Raycast: Ray�� ��� ��� ������Ʈ�� �����ϴ� �Լ�
        //(��ġ, ����, ����, ���̾��ũ)
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
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
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true); //����ź ������� ����ü�� Ȱ��ȭ �ǵ��� ����
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage; //ü�±���

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamege(isBossAtk)); //���׼� �ڷ�ƾ
            }

            //������ٵ� ����
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }
    
    IEnumerator OnDamege(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f); //���� Ÿ�� ����

        isDamage = false; //��������
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manger.GameOver();
    }

    //������Ʈ ����
    void OnTriggerStay(Collider other)
    {
        //���� �� �ִ� ������Ʈ�� weapon�̴�
        if (other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Shop") //���� �Լ� ȣ��
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
