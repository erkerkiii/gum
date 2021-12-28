namespace Gum.Pooling
{
    public interface IPoolable
    {
        void Reset();
        void Deactivate();
        void Erase();
    }
}