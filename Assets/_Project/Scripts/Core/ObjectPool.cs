using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 범용 오브젝트 풀. 이펙트, 투사체 등 빈번한 생성/삭제 최적화.
/// 사용법:
///   var pool = new ObjectPool(prefab, 10, transform);
///   var obj = pool.Get(position, rotation);
///   pool.Return(obj);
/// </summary>
public class ObjectPool
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Queue<GameObject> pool = new();

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateInstance());
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        var obj = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private GameObject CreateInstance()
    {
        var obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }
}

/// <summary>
/// MonoBehaviour 풀 매니저. Inspector에서 Prefab과 초기 개수를 설정.
/// 씬에 하나 배치해두면 다른 스크립트에서 PoolManager.Instance.GetPool() 사용 가능.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, ObjectPool> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public ObjectPool GetPool(GameObject prefab, int initialSize = 5)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool(prefab, initialSize, transform);
            pools[prefab] = pool;
        }
        return pool;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GetPool(prefab).Get(position, rotation);
    }

    public void Despawn(GameObject prefab, GameObject instance)
    {
        if (pools.TryGetValue(prefab, out var pool))
            pool.Return(instance);
        else
            instance.SetActive(false);
    }
}
