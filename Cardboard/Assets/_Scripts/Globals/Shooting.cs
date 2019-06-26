using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public Transform[] Turrets;
    [SerializeField] float bulletSpeed;
    [SerializeField] float coolDownBetweenBullets;
    [SerializeField] int amountPerBurst = 1;
    [SerializeField] float coolDownBetweenBursts;
    protected float bulletsTimer;
    protected float burstsTimer;
    protected int burstCounter = 0;

    [Header("Pooling Settings")]
    [SerializeField] protected int amountToPoolPerTurret;
    [SerializeField] protected GameObject objectToPool;
    protected List<Bullet> pooledObjects;
    protected Transform poolParent;

    SpriteRenderer spr;

    protected void Start()
    {
        poolParent = new GameObject("Bullet Parent").transform;
        spr = GetComponent<SpriteRenderer>();
        FillPool();
    }
    protected virtual void FillPool()
    {
        pooledObjects = new List<Bullet>();
        for (int t = 0; t < Turrets.Length; t++)
        {
            for (int i = 0; i < amountToPoolPerTurret; i++)
            {
                Bullet obj = new Bullet(t, objectToPool, poolParent);
                pooledObjects.Add(obj);
            }
        }
    }

    protected virtual void Update()
    {
        bulletsTimer += Time.deltaTime;
        if (burstCounter >= amountPerBurst)
        {
            if (burstsTimer >= coolDownBetweenBursts)
            {
                burstsTimer = 0;
                burstCounter = 0;
            }
            burstsTimer += Time.deltaTime;
        }
        UpdateBullets();
    }

    protected virtual void UpdateBullets()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledObjects[i].Update();
        }
    }

    protected void ShootAllTurrets()
    {
        if (!spr.IsVisibleFrom(Camera.main)) return;
        if (burstCounter < amountPerBurst)
        {
            if (bulletsTimer >= coolDownBetweenBullets)
            {
                bulletsTimer = 0;
                for (int i = 0; i < Turrets.Length; i++)
                {
                    Shoot(Turrets[i], i);
                }
                burstCounter++;
            }
        }
    }

    public void Shoot(Transform tr, int id)
    {
        Bullet bullet = GetPooledObject(id);
        if (bullet != null)
        {
            bullet.Spawn(tr, bulletSpeed);
        }
    }

    Bullet GetPooledObject(int id)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].Active && pooledObjects[i].ParentId == id)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

}


public class Bullet
{
    public bool Active = false;
    public float BulletSpeed;
    public int ParentId;

    protected GameObject obj;
    protected SpriteRenderer spr;
    protected bool wasVisible = false;

    public Bullet(int id, GameObject prefab, Transform parent = null)
    {
        ParentId = id;

        obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        spr = obj.GetComponent<SpriteRenderer>();
    }

    public virtual void Spawn(Transform origin, float speed)
    {
        obj.transform.position = origin.position;
        obj.transform.rotation = origin.rotation;

        BulletSpeed = speed;

        obj.SetActive(true);
        Active = true;
    }

    public void Update()
    {
        Active = obj.activeInHierarchy;
        if (Active)
        {
            CheckDead();
            Move();
        }
    }

    protected virtual void CheckDead()
    {
        if (wasVisible)
        {
            if (!spr.IsVisibleFrom(Camera.main))
            {
                obj.SetActive(false);
                wasVisible = false;
            }
        }
        else
        {
            wasVisible = true;
        }
    }

    protected virtual void Move()
    {
        obj.transform.position += obj.transform.right * BulletSpeed * Time.deltaTime;
    }
}