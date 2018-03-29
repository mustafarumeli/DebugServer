using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DebugServer
{
    class Program
    {
        private static Socket _serverSocket;
        private static byte[] _buffer;
        private static List<Socket> _clientSockets;
        private static readonly string server_username = "ohm";
        private static readonly string server_password = "741895623ohm";
        private static readonly string server_host = "35.231.112.9";
        private static readonly string server_port = "27019";
        private static readonly string database_name = "cool_db";
        private static IMongoClient _mongoClient = new MongoClient("mongodb://" + server_username + ":" + server_password + "@" + server_host + ":" + server_port + "/" + database_name);
        private static IMongoDatabase _database = _mongoClient.GetDatabase("cool_db");
        private static IMongoCollection<BsonDocument> _debugs = _database.GetCollection<BsonDocument>(typeof(Debug).Name);
        private static Debug _current;

        static void Main(string[] args)
        {
            _clientSockets = new List<Socket>();
            _buffer = new byte[1024];
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
            _serverSocket.Listen(10);
            
            _serverSocket.BeginAccept(_buffer.Length, BeginAccept, null);
        }

        private static void BeginAccept(IAsyncResult ar)
        {
            Socket clientSocket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(clientSocket);
            clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ClientRecieveCallback, clientSocket);
            _serverSocket.BeginAccept(_buffer.Length, BeginAccept, null);
        }

        private async static void ClientRecieveCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            int read = client.EndReceive(ar);
            if (read > 0)
            {
                _current =  await GetDebug("");
                byte[] incomeBuffer = new byte[read];
                Array.Copy(_buffer,incomeBuffer,read);
                string id = Encoding.Unicode.GetString(incomeBuffer);
                // todo
                await Update();
            }

        }
        private static async Task<Debug> GetDebug(string id)
        {
            var filter = new BsonDocument { { "_id", id } };
            using (var cursor =  await _debugs.FindAsync(filter))
            {
                return JsonConvert.DeserializeObject<Debug>(cursor.First().ToString());
            }

        }
        private static async Task<bool> Update()
        {
            try
            {
                var filter = new BsonDocument { { "_id", _current._id } };
                var json = JsonConvert.SerializeObject(_current);
                await _debugs.ReplaceOneAsync(filter, BsonDocument.Parse(json));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
