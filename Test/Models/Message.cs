using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    class Message
    {
        public String Username { get; set; }
        public String Content { get; set; }
        public Message()
        {
            
        }
        public Message(String username, String content)
        {
            this.Username = username;
            this.Content = content;
        }
    }
}
