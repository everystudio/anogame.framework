// Packages/anogame.framework/Runtime/GameState/IGameStateHandler.cs
namespace anogame.framework
{
    public interface IGameStateHandler
    {
        GameState TargetState { get; }

        void OnEnter();
        void OnExit();
    }
}
