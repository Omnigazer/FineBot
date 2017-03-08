using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRCBot
{
    public class Client
    {
        public int port;
        public IPAddress ipAddress;
        public Socket client;
        public ITextInterface client_interface;
        public StringBuilder currentString;

        public Client(IPAddress ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            currentString = new StringBuilder(0, 65536);            
        }

        public async Task Start()
        {            
            try
            {
                // Establish the remote endpoint for the socket.                                 
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                client.LingerState = new LingerOption(true, 1);

                // Connect to the remote endpoint.
                await AsyncConnect(remoteEP);
                HandleConnection();  

            }
            catch (Exception e)
            {               
                OutputMessage(e.Message);
                OutputMessage(e.ToString());
            }
        }

        public void OutputMessage(string message)
        {
            // Console.WriteLine(message);
        }

        public async Task AsyncConnect(IPEndPoint remoteEP)
        {
            await Task.Factory.FromAsync(client.BeginConnect(remoteEP, null, null), client.EndConnect);
            return;
        }

        public async void HandleConnection()
        {
            int bytesRead;
            StateObject state = new StateObject();

            try
            {
                while ((bytesRead = await Task<int>.Factory.FromAsync(client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, null, null), client.EndReceive)) > 0)
                {
                    string str = Encoding.GetEncoding(1251).GetString(state.buffer, 0, bytesRead);
                    OutputMessage(str);
                    currentString.Append(str);
                    //object command = Slicer.Slice(current_stream, state);
                    string command = Slicer.Slice(currentString, state);                    
                    while (command.Length > 0)
                    {
                        client_interface.HandleCommand(command);
                        command = Slicer.Slice(currentString, state);
                    }
                }
            }
            catch (SocketException exception)
            {
                OutputMessage(exception.Message);
                return;
            }
        }

        public void Send(byte[] data)
        {
            // Begin sending the data to the remote device.
            client.BeginSend(data, 0, data.Length, 0,
                null, client);
        }
    }
}