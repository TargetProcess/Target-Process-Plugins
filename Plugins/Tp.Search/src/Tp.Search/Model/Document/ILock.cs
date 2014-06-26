namespace Tp.Search.Model.Document
{
	interface ILock
	{
		LockToken Lock();
		DocumentIndexTypeToken Token { get; }
	}
}