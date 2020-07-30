using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]


// MonoBehaviourPunCallbacksを継承すると、photonViewプロパティが使えるようになる
public class GamePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    //private Projectile projectilePrefab = default; // Projectileプレハブの参照

    private SpriteRenderer spriteRenderer;
    private float hue = 0f;
    private bool isMoving = false;


    private ProjectileManager projectileManager;

    private int projectileId = 0;


    //public FixedJoystick joystick;



    public float timecount = 0f;

    public int levelcount = 0;


    public GameObject Ball;


    private Vector3 screenPoint;
    private Vector3 offset;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //ChangeBodyColor();

        //projectileManager = GameObject.FindWithTag("ProjectileManager").GetComponent<ProjectileManager>();

        //joystick = FindObjectOfType<FixedJoystick>();


        //GameObject joy = GameObject.Find("stick");
        //joystick = joy.GetComponent<FixedJoystick>();



        //joystick.Horizontal.GetType();

    }

    private void Update()
    {
        //Debug.Log("aaa");

        timecount += Time.deltaTime;

        // 自身が生成したオブジェクトだけに移動処理を行う
        if (photonView.IsMine)
        {

            //OnMouseDown();
            //OnMouseDrag();



            if (timecount > 5f)
            {
                //GameObject suika = Instantiate(Ball) as GameObject;
                float kakudo = Random.Range(0f, 360f);
                Vector3 suikaPos = new Vector3(30f * Mathf.Cos(kakudo * Mathf.Deg2Rad) / 2, 30f * Mathf.Sin(kakudo * Mathf.Deg2Rad) / 2, 0f);

                GameObject suika = PhotonNetwork.Instantiate("Ball", suikaPos, Quaternion.identity);

                //suika.transform
                //float kakudo = Random.Range(0f, 360f);
                //Vector3 suikaPos = new Vector3(20f * Mathf.Cos(kakudo * Mathf.Deg2Rad)/2, 20f * Mathf.Sin(kakudo * Mathf.Deg2Rad)/2, 0f);

                //suika.transform.position = suikaPos;


                //プレイヤーへ発射
                //発射方向
                Vector3 shootdir = transform.position - suikaPos;

                suika.GetComponent<Rigidbody2D>().velocity = shootdir/(5f-0.1f*levelcount);

                timecount = 0f;
                levelcount++;
            }


            //void OnMouseDown()
            //{
            //    this.screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            //    this.offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            //}
            //void OnMouseDrag()
            //{
            //    Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            //    Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + this.offset;
            //    transform.position = currentPosition;
            //}

            //Debug.Log("vvv");

            //var dx = 0.1f * Input.GetAxis("Horizontal");
            //var dy = 0.1f * Input.GetAxis("Vertical");
            //transform.Translate(dx, dy, 0f);

            //transform.position = new Vector3(transform.position.x + 0.01f, 0f, 0f);

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, 0f);


                isMoving = true;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, 0f);

                isMoving = true;

            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, 0f);

                isMoving = true;

            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, 0f);

                isMoving = true;

            }
            //else
            //{
            //    isMoving = false;
            //}

            //移動用
            //Vector2 movedirection = new Vector2(joystick.Horizontal, joystick.Vertical) * 100f;

            //this.GetComponent<Rigidbody2D>().velocity = movedirection;

            //移動中は色相値変化
            //isMoving = direction

            if (isMoving)
            {
                hue = (hue + Time.deltaTime) % 1f;
            }
            else
            {
                hue = 0f;
            }



            //ChangeBodyColor();


            // 左クリックでカーソルの方向に弾を発射する処理を行う
            if (Input.GetMouseButtonDown(0))
            {
                var playerWorldPosition = transform.position;
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dp = mouseWorldPosition - playerWorldPosition;
                float angle = Mathf.Atan2(dp.y, dp.x);

                //FireProjectile(angle);

                // FireProjectile(angle)をRPCで実行する
                //photonView.RPC(nameof(FireProjectile), RpcTarget.All, ++projectileId, angle);
            }


        }

    }

    // データを送受信するメソッド
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身側が生成したオブジェクトの場合は
            // 色相値と移動中フラグのデータを送信する
            stream.SendNext(hue);
            stream.SendNext(isMoving);
        }
        else
        {
            // 他プレイヤー側が生成したオブジェクトの場合は
            // 受信したデータから色相値と移動中フラグを更新する
            hue = (float)stream.ReceiveNext();
            isMoving = (bool)stream.ReceiveNext();

            //ChangeBodyColor();
        }
    }

    private void ChangeBodyColor()
    {
        float h = hue;
        float s = 1f;
        float v = (isMoving) ? 1f : 0.5f;
        spriteRenderer.color = Color.HSVToRGB(h, s, v);
    }

    // 弾を発射するメソッド
    [PunRPC]
    //private void FireProjectile(int id, float angle)
    //{
    //    projectileManager.Fire(id, photonView.OwnerActorNr, transform.position, angle);
    //}


    //あたり判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("aaa");

        if (photonView.IsMine)
        {
            //var projectile = collision.GetComponent<Projectile>();
            //if (projectile != null && projectile.OwnerId != PhotonNetwork.LocalPlayer.ActorNumber)
            //{
            //    photonView.RPC(nameof(HitByProjectile), RpcTarget.All, projectile.Id, projectile.OwnerId);
            //}


            if (collision.gameObject.tag == "Ball")
            {
                //Destroy(this.gameObject);

                photonView.RPC(nameof(HitByProjectile), RpcTarget.All);
            }
        }
    }

    //破壊用
    [PunRPC]
    private void OnBrake()
    {

        Destroy(this.gameObject);
    }

    [PunRPC]
    private void HitByProjectile()
    {
        //projectileManager.Remove(projectileId, ownerId);


        Destroy(this.gameObject);
    }



    void OnMouseDown()
    {
        this.screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        this.offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }
    void OnMouseDrag()
    {
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + this.offset;
        transform.position = currentPosition;
    }
}