using AutoFixture;
using SFA.DAS.TrackProgress.Api.Tests;

namespace SFA.DAS.TrackProgress.Api.Configuration;

public class ApiFixture
{
    private TrackProgressApiFactory factory = null!;
    private protected Fixture fixture = null!;
    private protected HttpClient client = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        factory = new TrackProgressApiFactory();
        client = factory.CreateClient();
    }

    [SetUp]
    public void Setup()
    {
        fixture = new Fixture();
    }
}