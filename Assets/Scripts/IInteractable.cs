/// <summary>
/// 상호작용 가능한 오브젝트가 구현하는 인터페이스.
/// Collider 필수 (레이캐스트 감지용).
/// </summary>
public interface IInteractable
{
    string InteractionPrompt { get; }
    void Interact();
    void OnFocusEnter();
    void OnFocusExit();
}