﻿using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Snebur.Comunicacao.WebSocket.Experimental
{

    //cross domain filho da puta

    /// <summary>
    /// This is the Flash Access Policy Server
    /// It manages sending the XML cross domain policy to flash socket clients over port 843.
    /// See http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html for details.
    /// </summary>
    /// 
    internal class AccessPolicyServer_temp : TcpServer, IDisposable
    {
        /// <summary>
        /// The pre-formatted XML response.
        /// </summary>
        private const string Response =
            "<cross-domain-policy>\r\n" +
            "\t<allow-access-from domain=\"{0}\" to-ports=\"{1}\" />\r\n" +
            "</cross-domain-policy>\r\n\0";

        private readonly string _allowedHost = "localhost";
        private readonly int _allowedPort = 80;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessPolicyServer"/> class.
        /// </summary>
        /// <param name="listenAddress">The listen address.</param>
        /// <param name="originDomain">The origin domain.</param>
        /// <param name="allowedPort">The allowed port.</param>
        public AccessPolicyServer_temp(IPAddress listenAddress, string originDomain, int allowedPort) : base(listenAddress, 843)
        {
            _allowedHost = "*";
            if (originDomain != String.Empty)
            {
                _allowedHost = originDomain;
            }

            _allowedPort = allowedPort;
        }

        /// <summary>
        /// Fires when a client connects.
        /// </summary>
        /// <param name="data">The TCP Connection.</param>
        protected override void ProcessarConexaoCliente(object data)
        {
            var connection = (TcpClient)data;
            try
            {
                connection.Client.Receive(new byte[32]);
                SendResponse(connection);
                connection.Client.Close();
            }
            catch (SocketException)
            {
                /* Ignore */
            }
        }

        /// <summary>
        /// Sends the response.
        /// </summary>
        /// <param name="connection">The TCP Connection.</param>
        public void SendResponse(TcpClient connection)
        {
            try
            {
                connection.Client.Send(
                    Encoding.UTF8.GetBytes(String.Format(Response, _allowedHost, _allowedPort.ToString(CultureInfo.InvariantCulture))));
            }
            catch (SocketException)
            {
                //Ignore
            }
        }
    }
}