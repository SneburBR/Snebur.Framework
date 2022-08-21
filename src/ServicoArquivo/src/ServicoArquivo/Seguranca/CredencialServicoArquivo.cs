﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;

namespace Snebur.ServicoArquivo
{
    public class CredencialServicoArquivo
    {
        public static Credencial ServicoArquivo
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialServicoArquivo.IDENTIFICADOR_USUARIO,
                    Senha = CredencialServicoArquivo.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "ServicoArquivo";

        private const string SENHA = "0388cb06-7431-4c5e-b70f-31129e7fd7da";
    }
}
