// Assets/Scripts/ano/core/Singleton.cs
namespace anogame.framework
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T instance;
        public static T Instance => instance ??= new T();
    }
}
