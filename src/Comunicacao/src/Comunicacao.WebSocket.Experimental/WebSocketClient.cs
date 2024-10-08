﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Snebur.Comunicacao.WebSocket.Experimental.Classes;
using Snebur.Comunicacao.WebSocket.Experimental.Handlers.WebSocket.rfc6455;

namespace Snebur.Comunicacao.WebSocket.Experimental
{
    public class WebSocketClient : IDisposable
    {
        public TimeSpan ConnectTimeout = new TimeSpan(0, 0, 0, 10);
        public bool IsAuthenticated;
        public ReadyStates ReadyState = ReadyStates.CLOSED;
        public string Origin;
        public string[] SubProtocols;
        public string CurrentProtocol { get; private set; }

        public OnEventDelegate OnConnect = x => { };
        public OnEventDelegate OnConnected = x => { };
        public OnEventDelegate OnDisconnect = x => { };
        public OnEventDelegate OnReceive = x => { };
        public OnEventDelegate OnSend = x => { };

        private TcpClient _client;
        private bool _connecting;
        private ConexaoContexto _context;
        private ClientHandshake _handshake;

        private readonly string _path;
        private readonly int _port;
        private readonly string _host;

        private static Thread[] ClientThreads = new Thread[Environment.ProcessorCount];
        private static CancellationTokenSource cancellation = new CancellationTokenSource();
        private static Queue<ConexaoContexto> NewClients { get; set; }
        private static Dictionary<ConexaoContexto, WebSocketClient> ContextMapping { get; set; }

        public enum ReadyStates
        {
            CONNECTING,
            OPEN,
            CLOSING,
            CLOSED
        }

        public Boolean Connected
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }

        static WebSocketClient()
        {
            NewClients = new Queue<ConexaoContexto>();
            ContextMapping = new Dictionary<ConexaoContexto, WebSocketClient>();

            for(int i = 0; i < ClientThreads.Length; i++){
                ClientThreads[i] = new Thread(HandleClientThread);
                ClientThreads[i].Start();
            }
        }

        private static void HandleClientThread()
        {
            while (!cancellation.IsCancellationRequested)
            {
                ConexaoContexto context = null;

                while (NewClients.Count == 0)
                {
                    Thread.Sleep(10);
                    if (cancellation.IsCancellationRequested) return;
                }

                lock (NewClients)
                {
                    if (NewClients.Count == 0)
                    {
                        continue;
                    }

                    context = NewClients.Dequeue();
                }

                lock (ContextMapping)
                {
                    WebSocketClient client = ContextMapping[context];
                    client.SetupContext(context);
                }
            }
        }
        public WebSocketClient(string path)
        {
            var r = new Regex("^(wss?)://(.*)\\:([0-9]*)/(.*)$");
            var matches = r.Match(path);

            _host = matches.Groups[2].Value;
            _port = Int32.Parse(matches.Groups[3].Value);
            _path = matches.Groups[4].Value;
        }

        public void Connect()
        {
            if (_client != null) return;
            
            try
            {
                ReadyState = ReadyStates.CONNECTING;

                _client = new TcpClient();
                _connecting = true;
                _client.BeginConnect(_host, _port, OnRunClient, null);

                var waiting = new TimeSpan();
                while (_connecting && waiting < ConnectTimeout)
                {
                    var timeSpan = new TimeSpan(0, 0, 0, 0, 100);
                    waiting = waiting.Add(timeSpan);
                    Thread.Sleep(timeSpan.Milliseconds);
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Fires when a client connects.
        /// </summary>
        /// <param name="result">null</param>
        protected void OnRunClient(IAsyncResult result)
        {
            bool connectError = false;
            try
            {
                _client.EndConnect(result);
            }
            catch (Exception)
            {
                Disconnect();
                connectError = true;
            }

            using (_context = new ConexaoContexto(null, _client))
            {
                _context = new ConexaoContexto(null, _client);
                _context.BufferSize = 512;
                _context.SessaoContexto.DataFrame = new DataFrame();
                _context.SessaoContexto.SetOnConnect(OnConnect);
                _context.SessaoContexto.SetOnConnected(OnConnected);
                _context.SessaoContexto.SetOnDisconnect(OnDisconnect);
                _context.SessaoContexto.SetOnSend(OnSend);
                _context.SessaoContexto.SetOnReceive(OnReceive);
                _context.SessaoContexto.OnConnect();

                if (connectError)
                {
                    _context.SessaoContexto.OnDisconnect();
                    return;
                }

                lock (ContextMapping)
                {
                    ContextMapping[_context] = this;
                }

                lock (NewClients)
                {
                    NewClients.Enqueue(_context);
                }
            }
        }

        private void SetupContext(ConexaoContexto context)
        {
            _context.ReceiveEventArgs.UserToken = _context;
            _context.ReceiveEventArgs.Completed += ReceiveEventArgs_Completed;
            _context.ReceiveEventArgs.SetBuffer(_context.Buffer, 0, _context.Buffer.Length);

            if (_context.Connection != null && _context.Connection.Connected)
            {
                _context.ReceiveReady.Wait();

                if (!_context.Connection.Client.ReceiveAsync(_context.ReceiveEventArgs))
                {
                    ReceiveEventArgs_Completed(_context.Connection.Client, _context.ReceiveEventArgs);
                }
 

                if (!IsAuthenticated)
                {

                    Authenticate();
                }
            }
        }

        void ReceiveEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var context = (ConexaoContexto)e.UserToken;
            context.Reset();

            if (e.SocketError != SocketError.Success)
            {
                context.ReceivedByteCount = 0;
            }
            else
            {
                context.ReceivedByteCount = e.BytesTransferred;
            }

            if (context.ReceivedByteCount > 0)
            {
                ReceiveData(context);
                context.ReceiveReady.Release();
            }
            else
            {
                context.Disconnect();
            }

            _context.ReceiveReady.Wait();

            if (!_context.Connection.Client.ReceiveAsync(_context.ReceiveEventArgs))
            {
                ReceiveEventArgs_Completed(_context.Connection.Client, _context.ReceiveEventArgs);
            }
        }

        private void Authenticate()
        {
            _handshake = new ClientHandshake { Version = "8", Origin = Origin, Host = _host, Key = GenerateKey(), ResourcePath = _path, SubProtocols = SubProtocols};

            _client.Client.Send(Encoding.UTF8.GetBytes(_handshake.ToString()));
        }

        private bool CheckAuthenticationResponse(ConexaoContexto context)
        {
            var receivedData = context.SessaoContexto.DataFrame.ToString();
            var header = new Cabecalho(receivedData);
            var handshake = new ServerHandshake(header);

            if (Authentication.GenerateAccept(_handshake.Key) != handshake.Accept) return false;

            if (SubProtocols != null)
            {
                if (header.SubProtocols == null)
                {
                    return false;
                }

                foreach (var s in SubProtocols)
                {
                    if (header.SubProtocols.Contains(s) && String.IsNullOrEmpty(CurrentProtocol))
                    {
                        CurrentProtocol = s;
                    }

                }
                if(String.IsNullOrEmpty(CurrentProtocol))
                {
                    return false;
                }
            }

            ReadyState = ReadyStates.OPEN;
            IsAuthenticated = true;
            _connecting = false;
            context.SessaoContexto.OnConnected();
            return true;
        }

        private void ReceiveData(ConexaoContexto context)
        {
            if (!IsAuthenticated)
            {
                var someBytes = new byte[context.ReceivedByteCount];
                Array.Copy(context.Buffer, 0, someBytes, 0, context.ReceivedByteCount);
                context.SessaoContexto.DataFrame.Append(someBytes);
                var authenticated = CheckAuthenticationResponse(context);
                context.SessaoContexto.DataFrame.Reset();

                if (!authenticated)
                {
                    Disconnect();
                }
            }
            else
            {
                context.SessaoContexto.DataFrame.Append(context.Buffer, true);
                if (context.SessaoContexto.DataFrame.State == Handlers.WebSocket.DataFrame.DataState.Complete)
                {
                    context.SessaoContexto.OnReceive();
                    context.SessaoContexto.DataFrame.Reset();
                }
            }
        }

        private void DoReceive(IAsyncResult result)
        {
            var context = (ConexaoContexto) result.AsyncState;
            context.Reset();

            try
            {
                context.ReceivedByteCount = context.Connection.Client.EndReceive(result);
            }
            catch (Exception)
            {
                context.ReceivedByteCount = 0;
            }

            if (context.ReceivedByteCount > 0)
            {
                ReceiveData(context);
                context.ReceiveReady.Release();
            }
            else
            {
                context.Disconnect();
            }
        }

        private static String GenerateKey()
        {
            var bytes = new byte[16];
            var random = new Random();

            for (var index = 0; index < bytes.Length; index++)
            {
                bytes[index] = (byte) random.Next(0, 255);
            }

            return Convert.ToBase64String(bytes);
        }

        public void Disconnect()
        {
            _connecting = false;

            if (_client == null) return;
            var dataFrame = new DataFrame();
            dataFrame.Append(new byte[0]);

            var bytes = dataFrame.AsFrame()[0].Array;

            ReadyState = ReadyStates.CLOSING;

            bytes[0] = 0x88;
            _context.SessaoContexto.Send(bytes);
            _client.Close();
            _client = null;
            ReadyState = ReadyStates.CLOSED;
        }

        public void Send(String data)
        {
            _context.SessaoContexto.Send(data);
        }

        public void Send(byte[] data)
        {
            _context.SessaoContexto.Send(data);
        }
        
        public void Dispose()
        {
            cancellation.Cancel();
            Handler.Instance.Dispose();
        }
        
    }
}
