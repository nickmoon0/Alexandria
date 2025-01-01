using Alexandria.Api.Users.DTOs;
using Alexandria.Application.Entries.Responses;

namespace Alexandria.Api.Entries.DTOs;

public class CommentDto
{
    public Guid? Id { get; init; }
    public string? Content { get; init; }
    public UserDto? CreatedBy { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }

    public static CommentDto? FromCommentResponse(CommentResponse? commentResponse) =>
        commentResponse == null
            ? null
            : new CommentDto
            {
                Id = commentResponse.Id,
                Content = commentResponse.Content,
                CreatedBy = UserDto.FromUserResponse(commentResponse.CreatedBy),
                CreatedAtUtc = commentResponse.CreatedAtUtc,
                DeletedAtUtc = commentResponse.DeletedAtUtc
            };
}