using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongestionTax.Domain.Exceptions
{
    public class RuleNotFoundException : Exception
    {
        public RuleNotFoundException(string message) : base(message)
        {

        }
    }
}
