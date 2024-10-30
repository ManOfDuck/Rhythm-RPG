using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerSoul : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6.5f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Damage")]
    [SerializeField] private float damageCooldownSeconds = 0f;
    private float damageCooldownTime = 0f;

    [Header("Controls")]
    [SerializeField] private List<KeyCode> upCodes = new() { KeyCode.UpArrow, KeyCode.W};
    [SerializeField] private List<KeyCode> downCodes = new() { KeyCode.DownArrow, KeyCode.S };
    [SerializeField] private List<KeyCode> leftCodes = new() { KeyCode.LeftArrow, KeyCode.A };
    [SerializeField] private List<KeyCode> rightCodes = new() { KeyCode.RightArrow, KeyCode.D};
    [SerializeField] private List<KeyCode> sprintCodes = new() { KeyCode.LeftShift, KeyCode.RightShift, KeyCode.Space };

    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(damageCooldownTime > 0f)
        {
            damageCooldownTime -= Time.deltaTime;
        }

        Move();
    }

    private void Move()
    {
        float currentSpeed = speed;
        foreach (KeyCode code in sprintCodes)
        {
            if (Input.GetKey(code))
            {
                currentSpeed = sprintSpeed;
            }
        }

        Vector2 currentVeloctiy = Vector2.zero;
        foreach (KeyCode code in upCodes)
        {
            if (Input.GetKey(code))
            {
                currentVeloctiy += currentSpeed * Vector2.up;
            }
        }
        foreach (KeyCode code in downCodes)
        {
            if (Input.GetKey(code))
            {
                currentVeloctiy += currentSpeed * Vector2.down;
            }
        }
        foreach (KeyCode code in leftCodes)
        {
            if (Input.GetKey(code))
            {
                currentVeloctiy += currentSpeed * Vector2.left;
            }
        }
        foreach (KeyCode code in rightCodes)
        {
            if (Input.GetKey(code))
            {
                currentVeloctiy += currentSpeed * Vector2.right;
            }
        }

        body.velocity = currentVeloctiy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageCooldownTime > 0f)
            return;

        if (collision.TryGetComponent<DropInstance>(out DropInstance drop))
        {
            BattleManager.Instance.HurtPlayer(drop.damage);
            //drop.Hit(-1, -1);

            if (drop.damage > 0f)
            {
                damageCooldownTime = damageCooldownSeconds;
            }
        }
    }
}
