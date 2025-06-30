namespace UserManagement.Common.Exceptions;
public class InvalidObjectException : Exception
{
    public InvalidObjectException() : base("Object does not meet criteria for creation")
    {

    }
}
