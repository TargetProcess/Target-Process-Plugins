namespace Tp.Git
{
    public interface IConnectionChecker
    {
        void Check(IGitConnectionSettings settings);
    }
}
