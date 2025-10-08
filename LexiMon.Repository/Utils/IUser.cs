namespace LexiMon.Repository.Utils;

public interface IUser
{
    string? Id { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
}