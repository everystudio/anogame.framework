// Packages/ano.core/Runtime/GameState/GameStateManager.cs
using System;
using System.Collections.Generic;

namespace ano.core
{
    public static class GameStateManager
    {
        private static readonly Stack<GameState> _stateStack = new();

        public static GameState Current => _stateStack.Count > 0 ? _stateStack.Peek() : GameState.None;

        private static readonly List<IGameStateHandler> _handlers = new();

        /// <summary>
        /// 状態を変更（スタックをクリアして新しい状態にする）
        /// </summary>
        public static event Action<GameState, GameState> OnStateChanged;

        public static void SetState(GameState newState)
        {
            var oldState = Current;
            _stateStack.Clear();
            _stateStack.Push(newState);
            NotifyTransition(oldState, newState);
        }

        public static void PushState(GameState newState)
        {
            var oldState = Current;
            _stateStack.Push(newState);
            NotifyTransition(oldState, newState);
        }

        public static void PopState()
        {
            if (_stateStack.Count == 0)
                return;

            var oldState = _stateStack.Pop();
            var newState = Current;
            NotifyTransition(oldState, newState);
        }

        private static void NotifyTransition(GameState oldState, GameState newState)
        {
            foreach (var handler in _handlers)
            {
                if (handler.TargetState == oldState)
                {
                    handler.OnExit();
                }
            }

            foreach (var handler in _handlers)
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
            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }

        public static void UnregisterHandler(IGameStateHandler handler)
        {
            _handlers.Remove(handler);
        }
    }
}
