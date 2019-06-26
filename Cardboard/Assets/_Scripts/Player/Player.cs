using UnityEngine;

public class Player : MonoBehaviour
{
    private float size;
    private Vector3 offset;
    private Camera m_camera;

    void Start()
    {
        m_camera = Camera.main;

        size = m_camera.fieldOfView / 12.0f; // result => 5.0f
    }

    void Update()
    {
        // mouse
        Vector3 clickPosition = -Vector3.one;
        clickPosition = m_camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));

        // movemente
        if (Input.GetMouseButtonDown(0))
        {
            offset = transform.position - clickPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            transform.position = (Vector2)clickPosition + (Vector2)offset;
        }

        if (GameController.GC.state == GAME_STATE.JUGANDO)
        {
            transform.position = Move();
        }
    }

    Vector3 Move()
    {
        // limit camera
        float w = size * (16f / 9f);

        float min = -w + m_camera.transform.position.x;
        float max = w + m_camera.transform.position.x;

        return new Vector3(Mathf.Clamp(transform.position.x, min, max), Mathf.Clamp(transform.position.y, -size, size), transform.position.z);
    }

    void OnTriggerEnter(Collider info)
    {
        Debug.Log(info);

        switch (info.tag)
        {
            case "Enemy":
                TakeDamage(info.transform, 20, 0.5f);
                break;
            case "Enemy/Misile":
                TakeDamage(info.transform, 4, 0.0f);
                break;
            case "Enemy/GruntBullet":
                TakeDamage(info.transform, 2, 0.0f);
                break;
            case "Player/Life":
                TakeHealth(50);
                break;
            default:
                break;
        }

        info.gameObject.SetActive(false);
        //Destroy(info.gameObject, 0.25f);
    }

    void TakeDamage(Transform info, int damage, float time)
    {
        GameController.GC.TakeDamage(info, damage, time);
    }

    void TakeHealth(int life)
    {
        GameController.GC.TakeHealth(life);
    }

    enum PLAYER_STATE { PUSH, NO_PUSH, DEAD }
}
