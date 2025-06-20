// Assets/Scripts/ano/core/Singleton.cs
namespace anogame.framework
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        public static T Instance => _instance ??= new T();
    }
}
