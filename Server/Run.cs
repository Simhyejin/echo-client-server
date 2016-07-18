using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Server
{
    class Run
    {
        static void Main(string[] args)
        {

            Server s = new Server(9080);
            s.Connection();
            
        }
    }
}
