using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowit
{
    public abstract class MessageBase
    {
        public string Name { get; set; }
        
    }

    public class Message : MessageBase
    {
        public string Com { get; set; }

        public bool IsSelf { get; set; }
    }
}