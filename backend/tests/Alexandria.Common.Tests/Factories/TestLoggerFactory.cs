using Alexandria.Common.Tests.Services;

namespace Alexandria.Common.Tests.Factories;

public static class TestLoggerFactory
{
    public static TestLogger<T> CreateLogger<T>()
    {
        return new TestLogger<T>();
    }
}