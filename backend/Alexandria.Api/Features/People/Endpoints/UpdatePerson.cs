using System.ComponentModel.DataAnnotations;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Features.People.DTOs;
using Alexandria.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Features.People.Endpoints;

public class UpdatePerson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPatch("/{id:guid}", Handle)
        .WithSummary("Updates a person record")
        .WithName(nameof(UpdatePerson))
        .Produces<Ok<UpdatePersonRequest>>()
        .Produces<BadRequest>()
        .Produces<NotFound>();

    private static async Task<Results<Ok<UpdatePersonResponse>, BadRequest, NotFound>> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdatePersonRequest request,
        [FromServices] IValidator<UpdatePersonRequest> validator,
        [FromServices] AppDbContext context)
    {
        var person = await context.People.FindAsync(id);
        if (person is null) return TypedResults.NotFound();
        
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return TypedResults.BadRequest();
        
        if (request.FirstName is not null) person.FirstName = request.FirstName;
        if (request.LastName is not null) person.LastName = request.LastName;
        if (request.MiddleNames is not null) person.MiddleNames = request.MiddleNames;
        if (request.Description is not null) person.Description = request.Description;

        await context.SaveChangesAsync();

        var response = new UpdatePersonResponse(new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName!,
            LastName = person.LastName!,
            MiddleNames = person.MiddleNames,
            Description = person.Description
        });
        
        return TypedResults.Ok(response);
    }
    
    public record UpdatePersonRequest(string? FirstName, string? LastName, string? MiddleNames, string? Description);

    public record UpdatePersonResponse(PersonDto Person);

    public class UpdatePersonRequestValidator : AbstractValidator<UpdatePersonRequest>
    {
        public UpdatePersonRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .MinimumLength(1)
                .WithMessage("First name cannot be empty");
            RuleFor(x => x.LastName)
                .MinimumLength(1)
                .WithMessage("Last name cannot be empty");
            
            RuleFor(x => x.MiddleNames)
                .MinimumLength(1)
                .WithName("Middle names cannot be empty");
            RuleFor(x => x.MiddleNames)
                .MaximumLength(100)
                .WithMessage("Middle names cannot be longer than 100 characters.");
            
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description cannot be longer than 2000 characters.");
            RuleFor(x => x.Description)
                .MinimumLength(1)
                .WithName("Description cannot be empty");
        }
    }
}