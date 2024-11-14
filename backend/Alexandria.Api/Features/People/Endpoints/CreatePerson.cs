using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Domain;
using Alexandria.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Features.People.Endpoints;

public class CreatePerson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new person")
        .WithName(nameof(CreatePerson))
        .Produces<CreatedAtRoute<CreatePersonResponse>>()
        .Produces<BadRequest>();
    
    private static async Task<Results<CreatedAtRoute<CreatePersonResponse>, BadRequest>> Handle(
        [FromBody] CreatePersonRequest request,
        [FromServices] IValidator<CreatePersonRequest> validator,
        [FromServices] AppDbContext context)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return TypedResults.BadRequest();

        var personResult = Person.Create(request.FirstName, request.LastName, request.MiddleNames, request.Description);
        if (!personResult.IsSuccess) return TypedResults.BadRequest();
        
        var person = personResult.Value!;
        await context.People.AddAsync(person);
        await context.SaveChangesAsync();
        
        var response = new CreatePersonResponse(person.Id);
        return TypedResults.CreatedAtRoute(response, nameof(GetPerson), response);
    }
    
    public record CreatePersonRequest(string FirstName, string LastName, string? MiddleNames, string? Description);
    public record CreatePersonResponse(Guid Id);

    public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
    {
        public CreatePersonRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("First name cannot be null or empty.");
        
            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Last name cannot be null or empty.");
        
            RuleFor(x => x.MiddleNames)
                .MaximumLength(100)
                .WithMessage("Middle names cannot be longer than 100 characters.");
        
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description cannot be longer than 2000 characters.");
        }
    }
}