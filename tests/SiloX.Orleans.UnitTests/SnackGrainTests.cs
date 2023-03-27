using FluentAssertions;
using Fluxera.Extensions.Hosting.Modules.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SiloX.Orleans.UnitTests.EventSourcing.Grains;
using SiloX.Orleans.UnitTests.Shared.Commands;

namespace SiloX.Orleans.UnitTests;

[TestFixture]
public class SnackGrainTests : StartupModuleTestBase<UnitTestsModule>
{
    [SetUp]
    public void SetUp()
    {
        StartApplication();
    }

    [TearDown]
    public void TearDown()
    {
        StopApplication();
    }

    [Test]
    public async Task GetAsync_Should_Return_Result_With_Snack_Object_When_Snack_Is_Created()
    {
        // Arrange
        var grainFactory = ApplicationLoader.ServiceProvider.GetRequiredService<IGrainFactory>();
        var grain = grainFactory.GetGrain<ISnackGrain>(Guid.NewGuid());
        var name = "Test Snack";
        var pictureUrl = "https://test.com/snack.png";
        var traceId = Guid.NewGuid();
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = "testuser";

        // Act
        var result = await grain.InitializeAsync(new SnackInitializeCommand(name, pictureUrl, traceId, operatedAt, operatedBy));
        result.IsSuccess.Should().BeTrue();
        var snackResult = await grain.GetAsync();

        // Assert
        snackResult.IsSuccess.Should().BeTrue();
        snackResult.Value.Name.Should().Be(name);
        snackResult.Value.PictureUrl.Should().Be(pictureUrl);
        snackResult.Value.CreatedAt.Should().BeCloseTo(operatedAt, TimeSpan.FromSeconds(1));
        snackResult.Value.CreatedBy.Should().Be(operatedBy);
        snackResult.Value.IsDeleted.Should().BeFalse();
    }

    [Test]
    public async Task GetAsync_Should_Return_Error_When_Snack_Is_Not_Initialized()
    {
        var grainFactory = ApplicationLoader.ServiceProvider.GetRequiredService<IGrainFactory>();
        var grain = grainFactory.GetGrain<ISnackGrain>(Guid.NewGuid());
        var result = await grain.GetAsync();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("is not initialized");
    }

    [Test]
    public async Task InitializeAsync_Should_Return_Error_When_Snack_Has_Already_Been_Created()
    {
        var grainFactory = ApplicationLoader.ServiceProvider.GetRequiredService<IGrainFactory>();
        var grain = grainFactory.GetGrain<ISnackGrain>(Guid.NewGuid());
        var command = new SnackInitializeCommand("Chips", null, Guid.NewGuid(), DateTimeOffset.UtcNow, "user");
        await grain.InitializeAsync(command);
        var result = await grain.InitializeAsync(command);
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("already exists");
    }

    [Test]
    public async Task InitializeAsync_Should_Return_Error_When_Snack_Has_Already_Been_Removed()
    {
        var grainFactory = ApplicationLoader.ServiceProvider.GetRequiredService<IGrainFactory>();
        var grain = grainFactory.GetGrain<ISnackGrain>(Guid.NewGuid());
        var command = new SnackInitializeCommand("Chips", null, Guid.NewGuid(), DateTimeOffset.UtcNow, "user");
        var removeCommand = new SnackRemoveCommand(Guid.NewGuid(), DateTimeOffset.UtcNow, "user");
        await grain.InitializeAsync(command);
        await grain.RemoveAsync(removeCommand);
        var result = await grain.InitializeAsync(command);
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("has already been removed");
    }

    [Test]
    public async Task InitializeAsync_Should_Return_Error_When_Name_Is_NullOrEmpty()
    {
        var grainFactory = ApplicationLoader.ServiceProvider.GetRequiredService<IGrainFactory>();
        var grain = grainFactory.GetGrain<ISnackGrain>(Guid.NewGuid());
        var command = new SnackInitializeCommand(null, null, Guid.NewGuid(), DateTimeOffset.UtcNow, "user");
        var result = await grain.InitializeAsync(command);
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("should not be empty");
    }
}
