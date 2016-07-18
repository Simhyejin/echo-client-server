
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client_Server
{
    public class Server
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
        Socket s_Socket;
        Socket c_Socket;
        IPEndPoint listenEP;

        public Server(int port)
        {
            try
            {
                listenEP = new IPEndPoint(IPAddress.Any, port);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Exception : {0}", e.ToString());
            }
        }

        public void Connection()
        {
            try
            {


                s_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s_Socket.Bind(listenEP);
                s_Socket.Listen(10);

                c_Socket = null;
                int count = 0; //Client Count


                //Print message
                Console.WriteLine("Start Server...");

                while (true)
                {
                    count++;
                    c_Socket = s_Socket.Accept(); //Accept

                    //Print message
                    Console.WriteLine("Connection established with client #" + count);


                    handleClient client = new handleClient();
                    client.startClient(c_Socket, count + "");
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Exception : {0}", e.ToString());
            }
            finally
            {
                s_Socket.Close();
                c_Socket.Close();
            }
        }

        public class handleClient
        {
            Socket c_socket;
            string no; //client number

            public void startClient(Socket client_socket, string client_no)
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
                    while (true)
                    {
                        byte[] recv_byte = null;
                        string recv_data = null;


                        //Receive bytes From Server
                        recv_byte = new byte[1024];
                        int byteRecv = c_socket.Receive(recv_byte);
                        recv_data = Encoding.ASCII.GetString(recv_byte);

                        //Remove unnecessary message behind character '\0'
                        recv_data = recv_data.Substring(0, recv_data.IndexOf('\0'));

                        //Close connection if get "exit"
                        if (recv_data == "exit")
                        {
                            Console.WriteLine("Closed Connection from  client #{0}",no);
                            c_socket.Close();
                            break;
                        }

                        //Print Echo Message from Server
                        Console.WriteLine("Message {0} received from client #{1}", recv_data, no);

                        byte[] send_byte = null;
                        string send_data = recv_data;
                        //Send data to Client
                        send_byte = new byte[send_data.Length];
                        send_byte = Encoding.ASCII.GetBytes(send_data);

                        int byteSent = c_socket.Send(send_byte);
                        if (byteSent < 0)
                        {
                            Console.WriteLine("Send Fail");
                        }
                        
                    }
                }
                catch (SocketException se)
                {
                    if(se.ErrorCode==10054)
                        Console.WriteLine("Forcibly closed Connection from client #{0}",no);
                    else
                        Console.WriteLine("SocketException : {0}", se.ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTIOM : " + e.ToString());
                }

            }
           
        }
    }
}