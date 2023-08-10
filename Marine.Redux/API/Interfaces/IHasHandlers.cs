namespace Marine.Redux.API.Interfaces
{
    public interface IHasHandlers
    {
        void Subscribe();

        void Unsubscribe();
    }
}
