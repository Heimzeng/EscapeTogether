using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tutorial;
using ClientSocketManager;

namespace ConsoleApp1
{
    class ServerRunner
    {
        static private Server s;
        static private void Restart()
        {
            s.Stop();
            s = new Server();
            s.Start();
        }
        static void Main(string[] args)
        {
            
            Debug.Listeners.Add(new MyLog());
            s = new Server();
            s.Start();
            while (true)
            {
                String line = Console.ReadLine();
                if (line == "quit")
                    break;
                if (line == "restart")
                    Restart();
            }
        }
        /*
        static Conn connss = new Conn();
        static Conn connss2 = new Conn();
        static List<Conn> conns = new List<Conn>();
        static void Test()
        {
            Test2(connss);
        }
        static void Test2(Object conn)
        {
            Conn a = (Conn)conn;
            bool areEqual = System.Object.ReferenceEquals(conn, connss);
            Debug.WriteLine(areEqual);
            areEqual = System.Object.ReferenceEquals(a, connss);
            Debug.WriteLine(areEqual);
            conns.Add(connss);
            areEqual = System.Object.ReferenceEquals(conns[0], connss);
            Debug.WriteLine(areEqual);
            conns.Add(connss2);
            conns.RemoveAt(0);
            areEqual = System.Object.ReferenceEquals(conns[0], connss2);
            Debug.WriteLine(areEqual);
        }
        */
    }
}
