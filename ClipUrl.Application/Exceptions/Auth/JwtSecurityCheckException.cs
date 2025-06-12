namespace ClipUrl.Application.Exceptions.Auth
{
    public class JwtSecurityCheckException : Exception
    {
        public JwtSecurityCheckException(string message) : base(message) { }
    }
}
