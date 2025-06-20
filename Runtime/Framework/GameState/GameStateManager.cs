// Packages/anogame.framework/Runtime/GameState/GameStateManager.cs
using System;
using System.Collections.Generic;

namespace anogame.framework
{
    public static class GameStateManager
    {
        private static readonly Stack<GameState> stateStack = new();

        public static GameState Current => stateStack.Count > 0 ? stateStack.Peek() : GameState.None;

        private static readonly List<IGameStateHandler> handlers = new();

        /// <summary>
        /// 状態を変更（スタックをクリアして新しい状態にする）
        /// </summary>
        public static event Action<GameState, GameState> OnStateChanged;

        public static void SetState(GameState newState)
        {
            var oldState = Current;
            stateStack.Clear();
            stateStack.Push(newState);
            NotifyTransition(oldState, newState);
        }

        public static void PushState(GameState newState)
        {
            var oldState = Current;
            stateStack.Push(newState);
            NotifyTransition(oldState, newState);
        }

        public static void PopState()
        {
            if (stateStack.Count == 0)
                return;

            var oldState = stateStack.Pop();
            var newState = Current;
            NotifyTransition(oldState, newState);
        }

        private static void NotifyTransition(GameState oldState, GameState newState)
        {
            foreach (var handler in handlers)
            {
                if (handler.TargetState == oldState)
                {
                    handler.OnExit();
                }
            }

            foreach (var handler in handlers)
            {
                if (handler.TargetState == newState)
                {
                    handler.OnEnter();
                }
            }

            OnStateChanged?.Invoke(oldState, newState);
        }

        public static void RegisterHandler(IGameStateHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public static void UnregisterHandler(IGameStateHandler handler)
        {
            handlers.Remove(handler);
        }
    }
}
