// Packages/ano.core/Runtime/GameState/IGameStateHandler.cs
namespace ano.core
{
    public interface IGameStateHandler
    {
        GameState TargetState { get; }

        void OnEnter();
        void OnExit();
    }
}
