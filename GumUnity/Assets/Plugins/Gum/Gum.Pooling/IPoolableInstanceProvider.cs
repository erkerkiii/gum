namespace Gum.Pooling
{
    public interface IPoolableInstanceProvider<out T>
    {
        T Create(object[] args = null);
    }
}