using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotCoach_config
{
    public interface IClientConfiguration
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
    }
}
