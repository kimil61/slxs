using UnityEngine;

/// <summary>
/// HUD 전체 관리. EventBus로 데이터 수신, 각 UI 요소에 전달.
/// Canvas 오브젝트에 붙일 것.
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private PlayerHPBar playerHPBar;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private BossHPBar bossHPBar;
    [SerializeField] private DeathScreen deathScreen;
    [SerializeField] private PauseMenu pauseMenu;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Subscribe<StaminaChangedEvent>(OnStaminaChanged);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Unsubscribe<StaminaChangedEvent>(OnStaminaChanged);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent e)
    {
        if (playerHPBar != null)
            playerHPBar.UpdateHP(e.currentHP, e.maxHP);
    }

    private void OnStaminaChanged(StaminaChangedEvent e)
    {
        if (staminaBar != null)
            staminaBar.UpdateStamina(e.current, e.max);
    }

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        if (deathScreen != null)
            deathScreen.Show();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (pauseMenu == null) return;

        if (e.newState == GameState.Paused)
            pauseMenu.Show();
        else if (e.previousState == GameState.Paused)
            pauseMenu.Hide();
    }

    /// <summary>
    /// 보스 HP바 활성화. 보스 전투 시작 시 호출.
    /// </summary>
    public void ShowBossHP(string bossName, float current, float max)
    {
        if (bossHPBar != null)
            bossHPBar.Show(bossName, current, max);
    }

    public void UpdateBossHP(float current, float max)
    {
        if (bossHPBar != null)
            bossHPBar.UpdateHP(current, max);
    }

    public void HideBossHP()
    {
        if (bossHPBar != null)
            bossHPBar.Hide();
    }
}
