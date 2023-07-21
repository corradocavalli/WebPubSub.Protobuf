using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubCore
{
    public class Message
    {
        public string? SenderId { get; set; }
        public string? Text { get; set; }
    }    
}
