using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

/** 
* Client
* - one or more instances can run simultaneously
* - needs not to communicate each other but only with the server
* - prints out both input and echoed messsages
* Assumptions
* - you can use loopback address
* - the order of running server/client may be determined by yourself, that is, you don't have to consider the situation that client runs before server
*/

namespace Client_Server
{
    public class Client
    {
        IPEndPoint ServerEP;
        Socket c_socket;

        public Client(int port)
        {
            try
            {
                ServerEP = new IPEndPoint(IPAddress.Loopback, port);

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
                c_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                c_socket.Connect(ServerEP);
                Console.WriteLine("Connection established with Server");
                Console.WriteLine("(If you want exit, type \"exit\")");
                Start();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                while (true)
                {
                    Console.WriteLine("Do you want connect again?(y/n)");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "y")
                    {
                        Connection();
                        break;
                    }
                    else if (input.ToLower() == "n")
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Exception : {0}", e.ToString());
            }
            finally
            {
                c_socket.Close();
            }
        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    byte[] send_byte = null;
                    string send_data = null;

                    //input data from User
                    send_data = Console.ReadLine();
                    //Send data to Server
                    send_byte = new byte[send_data.Length];
                    send_byte = Encoding.ASCII.GetBytes(send_data);

                    int byteSent = c_socket.Send(send_byte);
                    if (byteSent < 0)
                    {
                        Console.WriteLine("Send Fail");
                    }
                    
                    //Close connection if get "exit"
                    if (send_data == "exit")
                    {
                        c_socket.Close();
                        break;
                    }
                     byte[] recv_byte = null;
                    string recv_data = null;

                
                    //Receive bytes From Server
                    recv_byte = new byte[1024];
                    int byteRecv = c_socket.Receive(recv_byte);
                    recv_data = Encoding.ASCII.GetString(recv_byte);

                    //Remove unnecessary message behind character '\0'
                    recv_data = recv_data.Substring(0, recv_data.IndexOf('\0'));

                    //Print Echo Message from Server
                    Console.WriteLine("> {0}" , recv_data);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
            }
            finally
            {
                c_socket.Close();
            }            
        }

    }
}