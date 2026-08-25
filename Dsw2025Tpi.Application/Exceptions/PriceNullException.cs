using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
   public class PriceNullException : Exception
    {
            public PriceNullException()
            {
            }

            public PriceNullException(string? message) : base(message)
            {
            }

            public PriceNullException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
    }
}
