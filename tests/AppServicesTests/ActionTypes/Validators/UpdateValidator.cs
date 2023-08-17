﻿using Cts.AppServices.ActionTypes;
using Cts.AppServices.ActionTypes.Validators;
using Cts.Domain.Entities.ActionTypes;
using Cts.TestData.Constants;
using FluentValidation.TestHelper;

namespace AppServicesTests.ActionTypes.Validators;

public class UpdateValidator
{
    [Test]
    public async Task ValidDto_ReturnsAsValid()
    {
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((ActionType?)null);
        var model = new ActionTypeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

        var validator = new ActionTypeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(e => e.Name);
    }

    [Test]
    public async Task DuplicateName_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ActionType(Guid.NewGuid(), TestConstants.ValidName));
        var model = new ActionTypeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

        var validator = new ActionTypeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name)
            .WithErrorMessage("The name entered already exists.");
    }

    [Test]
    public async Task DuplicateName_ForSameId_ReturnsAsValid()
    {
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ActionType(Guid.Empty, TestConstants.ValidName));
        var model = new ActionTypeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

        var validator = new ActionTypeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(e => e.Name);
    }

    [Test]
    public async Task NameTooShort_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IActionTypeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((ActionType?)null);
        var model = new ActionTypeUpdateDto() { Name = TestConstants.ShortName };

        var validator = new ActionTypeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name);
    }
}
