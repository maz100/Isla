using Moq;

public class MockRepositoryProvider : IMockRepositoryProvider
{
    #region IMockRepositoryProvider implementation

    private readonly MockRepository _mocks;

    public MockRepositoryProvider()
    {
        _mocks = new MockRepository(MockBehavior.Default);
    }

    public MockRepository Mocks()
    {
        return _mocks;
    }

    #endregion
}