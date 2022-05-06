using ApiVersioning.Models;

namespace ApiVersioning.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}