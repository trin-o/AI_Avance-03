using UnityEngine;

public class EnemyStatus : Status
{
    [Tooltip("Scripts a activar o desactivar si esta o no en la pantalla")]
    public MonoBehaviour[] scripts;

    SpriteRenderer spr;
    bool prevIsVisible;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = false;
        }
    }

    private void Update()
    {
        if (prevIsVisible != spr.IsVisibleFrom(Camera.main))
        {
            prevIsVisible = spr.IsVisibleFrom(Camera.main);
            for (int i = 0; i < scripts.Length; i++)
            {
                scripts[i].enabled = prevIsVisible;
            }
        }
    }
}
