using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client_Server
{
    class Server
    {
        /** 
         *Server
         * - can accept multiple clients
         * - echo messages back to each sender
         * - prints out the state info like 'connection established with client #1', 'message blah blah blah received from client #1', etc
         * Assumptions
         * - you can use loopback address
         * - the order of running server/client may be determined by yourself, that is, you don't have to consider the situation that client runs before server
        */
        static void Main(string[] args)
        {
            TcpListener s_socket = new TcpListener(9080);
            TcpClient c_socket = default(TcpClient);
            int count = 0; //Client Count

            s_socket.Start();//Listening

            //Print message
            Console.WriteLine("Start Server...");

            while (true)
            {
                count++;
                c_socket = s_socket.AcceptTcpClient(); //Accept

                //Print message
                Console.WriteLine("Connection established with client #" + count);

                  
                handleClient client = new handleClient();
                client.startClient(c_socket, count + "");
            }

        }

    }

    public class handleClient
    {
        TcpClient c_socket;
        string no; //client number

        public void startClient(TcpClient client_socket, string client_no)
        {
            c_socket = client_socket;
            no = client_no;
            Thread thread = new Thread(work);
            thread.Start();
        }

        void work()
        {


            try
            {
                NetworkStream ns;

                while (true)
                {
                    byte[] recv_byte = new byte[c_socket.ReceiveBufferSize];
                    byte[] send_byte = new byte[c_socket.SendBufferSize];

                    string recv_data = null;
                    string send_data = null;


                    ns = c_socket.GetStream();

                    //Receive bytes From Client
                    ns.Read(recv_byte, 0, recv_byte.Length);
                    recv_data = Encoding.ASCII.GetString(recv_byte);

                    //Remove unnecessary message behind character '\0'
                    recv_data = recv_data.Substring(0, recv_data.IndexOf('\0'));

                    //Close connection if get "exit"
                    if (recv_data.Equals("exit"))
                    {
                        Console.WriteLine("Closed Connection from client #" + no);
                        c_socket.Close();
                        ns.Close();
                        break;
                    }

                    //Print Message form Client
                    Console.WriteLine("Message :" + recv_data + " received from client #" + no);


                    //Set send_data
                    send_data = recv_data;
                    send_byte = Encoding.ASCII.GetBytes(send_data);

                    //Send data to Client
                    ns.Write(send_byte, 0, send_byte.Length);
                    ns.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTIOM : " + e.ToString());
            }



        }
    }
}