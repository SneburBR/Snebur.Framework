﻿using Snebur.Comunicacao.WebSocket.Experimental.Classes;

namespace Snebur.Comunicacao.WebSocket.Experimental.Handlers
{
    internal abstract class Authentication : IAuthentication
    {
        private static string _origin = string.Empty;
        private static string _destination = string.Empty;

        public static string Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public static string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        #region IAuthentication Members

        /// <summary>
        /// Attempts to authenticates the specified user context.
        /// If authentication fails it kills the connection.
        /// </summary>
        /// <param name="context">The user context.</param>
        public void Authenticate(ConexaoContexto context)
        {
            if (CheckAuthentication(context))
            {
                context.SessaoContexto.Protocol = context.Cabecalho.Protocol;
                context.SessaoContexto.RequestPath = context.Cabecalho.RequestPath;
                context.Cabecalho = null;
                context.IsSetup = true;
                context.SessaoContexto.OnConnected();
            }
            else
            {
                context.Disconnect();
            }
        }

        #endregion

        protected abstract bool CheckAuthentication(ConexaoContexto context);
    }
}