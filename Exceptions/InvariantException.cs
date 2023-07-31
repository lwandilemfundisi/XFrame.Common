using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFrame.Common.Exceptions
{
   public class InvariantException : Exception
   {
      public InvariantException() { }

      public InvariantException(string message) : base(message) { }

      public InvariantException(string message, Exception inner) : base(message, inner) { }

      protected InvariantException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
