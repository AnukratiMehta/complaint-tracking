using Cts.Domain.Entities.ActionTypes;
using Cts.Domain.Entities.BaseEntities;
using Cts.Domain.Exceptions;
using Cts.TestData.Constants;

namespace DomainTests.ActionTypes.Manager;

public class ChangeName
{
    [Test]
    public async Task WhenNewNameIsValid_ChangesName()
    {
        var item = new ActionType(Guid.Empty, TestConstants.ValidName);
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(TestConstants.NewValidName, Arg.Any<CancellationToken>())
            .Returns((ActionType?)null);
        var manager = new ActionTypeManager(repoMock);

        await manager.ChangeNameAsync(item, TestConstants.NewValidName);

        item.Name.Should().BeEquivalentTo(TestConstants.NewValidName);
    }

    [Test]
    public async Task WhenNewNameIsUnchanged_CompletesWithNoChange()
    {
        var item = new ActionType(Guid.Empty, TestConstants.ValidName);
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(TestConstants.ValidName, Arg.Any<CancellationToken>())
            .Returns(item);
        var manager = new ActionTypeManager(repoMock);

        await manager.ChangeNameAsync(item, TestConstants.ValidName);

        item.Name.Should().BeEquivalentTo(TestConstants.ValidName);
    }

    [Test]
    public async Task WhenNewNameAlreadyExists_Throws()
    {
        var item = new ActionType(Guid.Empty, TestConstants.ValidName);
        var existingItem = new ActionType(Guid.NewGuid(), TestConstants.NewValidName);
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(TestConstants.NewValidName, Arg.Any<CancellationToken>())
            .Returns(existingItem);
        var manager = new ActionTypeManager(repoMock);

        var action = async () => await manager.ChangeNameAsync(item, TestConstants.NewValidName);

        (await action.Should().ThrowAsync<NameAlreadyExistsException>())
            .WithMessage($"An entity with that name already exists. Name: {TestConstants.NewValidName}");
    }

    [Test]
    public async Task WhenNewNameIsInvalid_Throws()
    {
        var item = new ActionType(Guid.Empty, TestConstants.ValidName);
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(TestConstants.NewValidName, Arg.Any<CancellationToken>())
            .Returns((ActionType?)null);
        var manager = new ActionTypeManager(repoMock);

        var action = async () => await manager.ChangeNameAsync(item, TestConstants.ShortName);

        (await action.Should().ThrowAsync<ArgumentException>())
            .WithMessage($"The length must be at least the minimum length '{SimpleNamedEntity.MinNameLength}'.*");
    }
}
