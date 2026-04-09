using System;
using System.Collections.Generic;

/// <summary>
/// 전역 이벤트 버스. 시스템 간 디커플링 통신.
/// 사용법:
///   EventBus.Subscribe&lt;EnemyDiedEvent&gt;(OnEnemyDied);
///   EventBus.Publish(new EnemyDiedEvent { enemy = this });
///   EventBus.Unsubscribe&lt;EnemyDiedEvent&gt;(OnEnemyDied);
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var existing))
            events[type] = Delegate.Combine(existing, handler);
        else
            events[type] = handler;
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var existing))
        {
            var result = Delegate.Remove(existing, handler);
            if (result == null)
                events.Remove(type);
            else
                events[type] = result;
        }
    }

    public static void Publish<T>(T eventData) where T : struct
    {
        if (events.TryGetValue(typeof(T), out var handler))
            ((Action<T>)handler)?.Invoke(eventData);
    }

    public static void Clear()
    {
        events.Clear();
    }
}

// ── 게임 이벤트 정의 ──

public struct GameStateChangedEvent
{
    public GameState previousState;
    public GameState newState;
}

public struct RunStartedEvent { }
public struct RunEndedEvent
{
    public bool victory;
    public int earnedCurrency;
}

public struct EnemyDiedEvent
{
    public UnityEngine.GameObject enemy;
    public UnityEngine.Vector3 position;
}

public struct PlayerDiedEvent { }

public struct PlayerDamagedEvent
{
    public float damage;
    public float currentHP;
    public float maxHP;
}

public struct StaminaChangedEvent
{
    public float current;
    public float max;
}

public struct AreaClearedEvent
{
    public int areaIndex;
}
