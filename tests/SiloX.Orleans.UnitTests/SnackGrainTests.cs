using FluentAssertions;
using Fluxera.Guards;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;

namespace SiloX.Orleans.UnitTests;

[TestFixture]
public class SnackGrainTests
{
    private UnitTestsHost _testHost = null!;
    private TestServer _testServer = null!;

    // [OneTimeSetUp]
    // public void Initialize()
    // {
    //
    // }
    //
    // [OneTimeTearDown]
    // public void Dispose()
    // {
    // }

    [SetUp]
    public async Task SetUp()
    {
        // StartApplication();
        // _testHost = new UnitTestsHost();
        // await _testHost.RunAsync(Array.Empty<string>());
        // _testServer = _testHost.Server;
    }

    [TearDown]
    public void TearDown()
    {
        // StopApplication();
        // _testServer.Dispose();
    }

    [Test]
    public void Test_Guard()
    {
        var yuan1 = 10;
        var amount = 3;
        var result = Guard.Against.Negative(yuan1 - amount, nameof(amount));
        result.Should().Be(7);
    }
    
    [Test]
    public async Task CanInitializeAsync_WithValidCommand_ReturnsTrue()
    {
        // Arrange
        var snackId = new Guid("00000000-0000-0000-0000-000000000001");
        var client = _testServer.Services.GetRequiredService<IClusterClient>();
        var grain = client.GetGrain<ISnackGrain>(snackId);
        var command = new SnackInitializeCommand(snackId, "Coke", null, Guid.NewGuid(), DateTimeOffset.UtcNow, "CanInitializeAsync_WithValidCommand_ReturnsTrue");
        // Act
        var canInitialize = await grain.CanInitializeAsync(command);
        // Assert
        canInitialize.Should().BeTrue();
    }

    [Test]
    public async Task CanInitializeAsync_WithEmptyName_ReturnsFalse()
    {
        // Arrange
        var snackId = new Guid("00000000-0000-0000-0000-000000000001");
        var client = _testServer.Services.GetRequiredService<IClusterClient>();
        var grain = client.GetGrain<ISnackGrain>(snackId);
        var command = new SnackInitializeCommand(snackId, string.Empty, null, Guid.NewGuid(), DateTimeOffset.UtcNow, "CanInitializeAsync_WithEmptyName_ReturnsFalse");
        // Act
        var canInitialize = await grain.CanInitializeAsync(command);
        // Assert
        canInitialize.Should().BeFalse();
    }

    [Test]
    public async Task CanInitializeAsync_WithTooLongName_ReturnsFalse()
    {
        // Arrange
        var snackId = new Guid("00000000-0000-0000-0000-000000000001");
        var client = _testServer.Services.GetRequiredService<IClusterClient>();
        var grain = client.GetGrain<ISnackGrain>(snackId);
        var command = new SnackInitializeCommand(snackId, new string('a', 101), null, Guid.NewGuid(), DateTimeOffset.UtcNow, "CanInitializeAsync_WithTooLongName_ReturnsFalse");
        // Act
        var canInitialize = await grain.CanInitializeAsync(command);
        // Assert
        canInitialize.Should().BeFalse();
    }

    [Test]
    public async Task InitializeAsync_WithValidCommand_InitializesSnack()
    {
        // Arrange
        var snackId = new Guid("00000000-0000-0000-0000-000000000002");
        var client = _testServer.Services.GetRequiredService<IClusterClient>();
        var grain = client.GetGrain<ISnackGrain>(snackId);
        var command = new SnackInitializeCommand(snackId, "Lays", null, Guid.NewGuid(), DateTimeOffset.UtcNow, "CanInitializeAsync_WithValidCommand_ReturnsTrue");
        // Act
        var result = await grain.InitializeAsync(command);
        var state = await grain.GetStateAsync();
        // Assert
        result.IsSuccess.Should().BeTrue();
        state.Name.Should().Be("Lays");
        state.PictureUrl.Should().BeNull();
    }
}
