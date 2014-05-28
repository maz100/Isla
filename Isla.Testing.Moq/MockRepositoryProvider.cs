using Moq;

public class MockRepositoryProvider : IMockRepositoryProvider
{
	#region IMockRepositoryProvider implementation

	private MockRepository _mocks;

	public MockRepositoryProvider ()
	{
		_mocks = new MockRepository(MockBehavior.Default);
	}

	public MockRepository Mocks ()
	{
		return _mocks;
	}

	#endregion
}
