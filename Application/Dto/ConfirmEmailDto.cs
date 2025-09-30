namespace Application.Dto
{
    public record ConfirmEmailDto(
        string Email,
        string ConfirmationCode
    );
}