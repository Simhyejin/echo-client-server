using System;
using System.Net.Sockets;
using System.Text;


namespace Client_Server
{
    class Client
    {
        /** 
         * Client
         * - one or more instances can run simultaneously
         * - needs not to communicate each other but only with the server
         * - prints out both input and echoed messsages
         * Assumptions
         * - you can use loopback address
         * - the order of running server/client may be determined by yourself, that is, you don't have to consider the situation that client runs before server
        */

        static void Main(string[] args)
        {
            NetworkStream ns;


            try
            {
                //
                TcpClient c_socket = new TcpClient();
                //Connect
                c_socket.Connect("127.0.0.1", 9080);


                //Print message
                Console.WriteLine("Connection established with Server");
                Console.WriteLine("If you want exit, type \"exit\"");

                while (true)
                {
                    byte[] recv_byte = new byte[c_socket.ReceiveBufferSize];
                    byte[] send_byte = new byte[c_socket.SendBufferSize];

                    string recv_data = null;
                    string send_data = null;

                    ns = c_socket.GetStream();

                    //input data from User
                    send_data = Console.ReadLine();
                    //Send data to Server
                    send_byte = Encoding.ASCII.GetBytes(send_data);
                    ns.Write(send_byte, 0, send_byte.Length);
                    ns.Flush();

                    //Close connection if get "exit"
                    if (send_data == "exit")
                    {
                        ns.Close();
                        break;
                    }

                    //Receive bytes From Server
                    ns.Read(recv_byte, 0, recv_byte.Length);
                    recv_data = Encoding.ASCII.GetString(recv_byte);

                    //Remove unnecessary message behind character '\0'
                    recv_data = recv_data.Substring(0, recv_data.IndexOf('\0'));

                    //Print Echo Message from Server
                    Console.WriteLine("> " + recv_data);


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTIOM : " + e.ToString());
            }

        }
    }
}