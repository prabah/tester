using System;

namespace Malden.Portal.BLL
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }

    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException(string message)
            : base(message)
        {
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException()
            : base("Errors occurred during the process")
        {
        }
    }

    public class HandledException : Exception
    {
        public HandledException()
            : base("Errors occurred during the process")
        {
        }
    }

    public class ReferenceException : Exception
    {
        public ReferenceException(string message)
            : base(message)
        {
        }
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message)
            : base(message)
        {
        }
    }
}

