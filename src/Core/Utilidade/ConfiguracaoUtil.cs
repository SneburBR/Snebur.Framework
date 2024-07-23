﻿using Snebur.Dominio;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Snebur.Utilidade
{
    public static class ConfiguracaoUtil
    {
        public const string IDENTIFICADOR_PROPRIETARIO_GLOBAL = "IdentificadorProprietarioGlobal";
        private const string NOME_CHAVE_AMBIENTE_SERVIDOR = "AmbienteServidor";
        private static readonly Guid CHAVE_SEGURANCA_PADRAO = new Guid("0971f131-3672-47f8-a0ab-b54b3716e18f");

        private static EnumAmbienteServidor? _ambienteServidor;
        private static object _bloqueioCaminhoAppData = new object();
        private static object _bloqueio = new object();
        private static Guid? _chaveSeguranca;
        private static bool? _isLogDebugOutputSql;

        private static string _caminhoAppDataAplicacao;
        private static string _caminhoAppDataAplicacaoLogs;
        private static string _caminhoAppDataAplicacaoSemVersao;
        private static string _caminhoAppDataAplicacaoSemIdentificadorPropretario;

        private const string NOME_ARQUIVO_APP_SETTINGS = "appSettings.config";
        private const string NOME_ARQUIVO_CONNECTION_STRINGS = "connectionStrings.config";
        internal const string NOME_PADRAO_ARQUIVO_APPLICATIIONS_SETTING = "configuracao.xml";

        public static string UrlWebService => AplicacaoSnebur.Atual.UrlWebService;
        public static string UrlServicoArquivo => AplicacaoSnebur.Atual.UrlServicoArquivo;
        public static string UrlServicoDados => AplicacaoSnebur.Atual.UrlServicoDados;
        public static string UrlServicoImagem => AplicacaoSnebur.Atual.UrlServicoImagem;
        public static Guid ChaveSeguranca => LazyUtil.RetornarValorLazyComBloqueio(ref _chaveSeguranca, RetornarChaveSeguranca);

        public static bool IsLogDebugOutputSql=> LazyUtil.RetornarValorLazyComBloqueio(ref _isLogDebugOutputSql, RetornarIsLogDebugOutputSql);

        private static Guid RetornarChaveSeguranca()
        {
            if (Guid.TryParse(AplicacaoSnebur.Atual.AppSettings["ChaveSeguranca"], out var resultado))
            {
                return resultado;
            }
            return CHAVE_SEGURANCA_PADRAO;
        }
        private static bool RetornarIsLogDebugOutputSql()
        {
            if (Boolean.TryParse(AplicacaoSnebur.Atual.AppSettings["IsLogDebugOutputSql"], out var resultado))
            {
                return resultado;
            }
            return false;
        }

        public static bool IsAttachedLocalhost => AmbienteServidor == EnumAmbienteServidor.Localhost &&
                                                  DebugUtil.IsAttached;

        public static CultureInfo CulturaPortuguesBrasil { get; } = new CultureInfo("pt-BR");
        public static NameValueCollection AppSettings => AplicacaoSnebur.Atual.AppSettings;
        public static NameValueCollection ConnectionStrings => AplicacaoSnebur.Atual.ConnectionStrings;
        public static string CaminhoAppDataAplicacao
        {
            get
            {
                if (ConfiguracaoUtil._caminhoAppDataAplicacao == null || ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacao, true))
                {
                    lock (_bloqueioCaminhoAppData)
                    {
                        if (_caminhoAppDataAplicacao == null || ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacao, true))
                        {
                            var caminhoAppData = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
                            _caminhoAppDataAplicacao = Path.Combine(caminhoAppData, AplicacaoSnebur.Atual.VersaoAplicao.ToString());
                            DiretorioUtil.CriarDiretorio(_caminhoAppDataAplicacao);
                        }
                    }
                }
                return _caminhoAppDataAplicacao;
            }
        }

        public static EnumAmbienteServidor AmbienteServidor
        {
            get
            {
                if (_ambienteServidor == null)
                {
                    lock (_bloqueio)
                    {
                        if (_ambienteServidor == null)
                        {
                            _ambienteServidor = RetornarAmbienteServidor();
                        }
                    }
                }
                return _ambienteServidor.Value;
            }
        }

        public static string CaminhoAppDataAplicacaoLogs
        {
            get
            {
                if (ConfiguracaoUtil._caminhoAppDataAplicacaoLogs == null)
                {
                    var caminhoApp = ConfiguracaoUtil._caminhoAppDataAplicacao;
                    lock (_bloqueioCaminhoAppData)
                    {
                        if (ConfiguracaoUtil._caminhoAppDataAplicacaoLogs == null)
                        {
                            _caminhoAppDataAplicacaoLogs = Path.Combine(caminhoApp, "Logs");
                            DiretorioUtil.CriarDiretorio(_caminhoAppDataAplicacaoLogs);
                        }
                    }
                }
                return ConfiguracaoUtil._caminhoAppDataAplicacaoLogs;
            }
        }

        public static string RetornarCaminhoArquivoApplicationSettingsPadrao()
        {
            return Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao, NOME_PADRAO_ARQUIVO_APPLICATIIONS_SETTING);
        }

        public static string CaminhoAppDataAplicacaoSemVersao
        {
            get
            {
                if (ConfiguracaoUtil._caminhoAppDataAplicacaoSemVersao == null ||
                    ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacaoSemVersao, true))
                {
                    lock (_bloqueioCaminhoAppData)
                    {
                        if (_caminhoAppDataAplicacaoSemVersao == null || ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacaoSemVersao, true))
                        {
                            var caminhoAppData = ConfiguracaoUtil.RetornarCaminhoLocalAppData();

                            _caminhoAppDataAplicacaoSemVersao = Path.Combine(caminhoAppData, AplicacaoSnebur.Atual.NomeEmpresa);

                            if (ConfiguracaoUtil.AmbienteServidor != EnumAmbienteServidor.Producao)
                            {
                                _caminhoAppDataAplicacaoSemVersao = Path.Combine(_caminhoAppDataAplicacaoSemVersao, ConfiguracaoUtil.AmbienteServidor.ToString());
                            }
                            _caminhoAppDataAplicacaoSemVersao = Path.Combine(_caminhoAppDataAplicacaoSemVersao, AplicacaoSnebur.Atual.IdentificadorAplicacao);

                            if (AplicacaoSnebur.Atual.IsSeperarAppDataPorIdentificadorPropretario)
                            {
                                _caminhoAppDataAplicacaoSemVersao = Path.Combine(_caminhoAppDataAplicacaoSemVersao, AplicacaoSnebur.Atual.IdentificadorProprietario);
                            }
                            DiretorioUtil.CriarDiretorio(_caminhoAppDataAplicacaoSemVersao);
                        }
                    }
                }
                return _caminhoAppDataAplicacaoSemVersao;
            }
        }

        public static string CaminhoAppDataAplicacaoSemIdentificadorPropretario
        {
            get
            {
                if (ConfiguracaoUtil._caminhoAppDataAplicacaoSemIdentificadorPropretario == null ||
                    ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacaoSemVersao, true))
                {
                    lock (_bloqueioCaminhoAppData)
                    {
                        if (_caminhoAppDataAplicacaoSemIdentificadorPropretario == null ||
                            ConfiguracaoUtil.IsAtualizarCaminhoAppData(_caminhoAppDataAplicacaoSemIdentificadorPropretario, true))
                        {
                            var caminhoAppData = ConfiguracaoUtil.RetornarCaminhoLocalAppData();

                            _caminhoAppDataAplicacaoSemIdentificadorPropretario = Path.Combine(caminhoAppData, AplicacaoSnebur.Atual.NomeEmpresa);
                            if (ConfiguracaoUtil.AmbienteServidor != EnumAmbienteServidor.Producao)
                            {
                                _caminhoAppDataAplicacaoSemIdentificadorPropretario = Path.Combine(_caminhoAppDataAplicacaoSemIdentificadorPropretario, ConfiguracaoUtil.AmbienteServidor.ToString());
                            }
                            _caminhoAppDataAplicacaoSemIdentificadorPropretario = Path.Combine(_caminhoAppDataAplicacaoSemIdentificadorPropretario,
                                                                                                AplicacaoSnebur.Atual.NomeAplicacao);

                            DiretorioUtil.CriarDiretorio(_caminhoAppDataAplicacaoSemIdentificadorPropretario);
                        }
                    }
                }
                return _caminhoAppDataAplicacaoSemIdentificadorPropretario;
            }
        }

        private static bool IsAtualizarCaminhoAppData(string caminho, bool semVersao)
        {
            if (String.IsNullOrEmpty(caminho))
            {
                return true;
            }
            var di = new DirectoryInfo(caminho);
            if (!di.Exists)
            {
                return true;
            }
            if (!semVersao)
            {
                di = di.Parent;
            }
            if (di.Name == "0")
            {
                return true;
            }
            return false;
        }

        public static string CaminhoAppDataAplicacaoTemporario()
        {
            var diretorioTemporario = Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao, "Temp");
            DiretorioUtil.CriarDiretorio(diretorioTemporario);
            return diretorioTemporario;
        }

        public static void AtualizarCaminhosAppData()
        {
            _caminhoAppDataAplicacao = null;
            _caminhoAppDataAplicacaoSemVersao = null;
        }

        private static string RetornarCaminhoLocalAppData()
        {
            var caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (String.IsNullOrWhiteSpace(caminhoAppData))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
            return caminhoAppData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nomeNode"></param>
        /// <param name="caminhoOrigemDoNode"></param>
        /// <returns>Se true, o nó foi substituido</returns>
        private static bool SubstiturNoXmlArquivoConfiguracao(XmlDocument doc, string nomeNode, string caminhoOrigemDoNode)
        {
            var caminhonode = String.Format("//{0}", nomeNode);
            var nodeAppSetting = doc.SelectSingleNode(caminhonode);
            if (nodeAppSetting != null)
            {
                if (nodeAppSetting.Attributes["configSource"] != null)
                {
                    if (File.Exists(caminhoOrigemDoNode))
                    {
                        //appSettings

                        nodeAppSetting.Attributes.Remove(nodeAppSetting.Attributes["configSource"]);

                        using (var msAppSetting = new MemoryStream(File.ReadAllBytes(caminhoOrigemDoNode)))
                        {
                            var docAppSetting = new XmlDocument();
                            docAppSetting.Load(msAppSetting);

                            var nodeAppSettingOrigem = docAppSetting.SelectSingleNode(caminhonode);
                            nodeAppSetting.InnerXml = nodeAppSettingOrigem.InnerXml;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private static EnumAmbienteServidor RetornarAmbienteServidor()
        {
            var descricao = AplicacaoSnebur.Atual.AppSettings[NOME_CHAVE_AMBIENTE_SERVIDOR];
            if (String.IsNullOrEmpty(descricao))
            {
                if (DebugUtil.IsAttached)
                {
                    throw new Erro($"A chave '{NOME_CHAVE_AMBIENTE_SERVIDOR}' não foi definida no appSetting");
                }
                return EnumAmbienteServidor.Producao;
            }
            if (Enum.TryParse<EnumAmbienteServidor>(descricao, out var ambienteServidor))
            {
                return ambienteServidor;
            }
            throw new Erro($"Não foi possível converter {descricao} para o tipo {nameof(EnumAmbienteServidor)}");
        }

        public static Cor CorAmbienteServidor
        {
            get
            {
                switch (AmbienteServidor)
                {
                    case EnumAmbienteServidor.Localhost:

                        return new Cor(0, 96, 100, 1);

                    case EnumAmbienteServidor.Interno:

                        return new Cor(103, 58, 183, 1);

                    case EnumAmbienteServidor.Teste:

                        return new Cor(0, 0, 0, 0);

                    case EnumAmbienteServidor.Producao:

                        return new Cor(0, 0, 0, 0);

                    default:

                        throw new Erro("Ambiente desconhecido");
                }
            }
        }
    }
}