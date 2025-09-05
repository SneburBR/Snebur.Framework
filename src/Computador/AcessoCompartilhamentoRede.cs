using Snebur.IO;
using Snebur.Utilidade;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Snebur.Computador;


/// <summary>
/// Acessar caminho compartilha na rede  
/// </summary>
public class AcessoCompartilhamentoRede : IDisposable
{
    private string _nomeComptuadorUnc;
    private string _nomeComputador;
    private const int USUARIO_JA_CONECTADO = 1219;

    public string NomeComputador
        => this._nomeComputador;
    //    {
    //        get
    //        {
    //            return 
    //        }
    //set
    //        {
    //            this._nomeComputador = value;
    //            if (!this._nomeComputador.StartsWith(@"\\"))
    //            {
    //    this._nomeComptuadorUnc = @"\\" + this._nomeComputador;
    //}
    //            else
    //            {
    //    this._nomeComptuadorUnc = value;
    //}
    //}
    //    }

    public string? Usuario { get; set; }

    public string? Senha { get; set; }

    #region Contantes

    private const int RESOURCE_CONNECTED = 0x00000001;
    private const int RESOURCE_GLOBALNET = 0x00000002;
    private const int RESOURCE_REMEMBERED = 0x00000003;

    private const int RESOURCETYPE_ANY = 0x00000000;
    private const int RESOURCETYPE_DISK = 0x00000001;
    private const int RESOURCETYPE_PRINT = 0x00000002;

    private const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
    private const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
    private const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
    private const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
    private const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
    private const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

    private const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
    private const int RESOURCEUSAGE_CONTAINER = 0x00000002;

    private const int CONNECT_INTERACTIVE = 0x00000008;
    private const int CONNECT_PROMPT = 0x00000010;
    private const int CONNECT_REDIRECT = 0x00000080;
    private const int CONNECT_UPDATE_PROFILE = 0x00000001;
    private const int CONNECT_COMMANDLINE = 0x00000800;
    private const int CONNECT_CMD_SAVECRED = 0x00001000;

    private const int CONNECT_LOCALDRIVE = 0x00000100;

    #endregion

    #region Erros

    private const int NO_ERROR = 0;

    private const int ERROR_USER_PASSWORD_INVALID = 1326;

    private const int ERROR_ACCESS_DENIED = 5;
    private const int ERROR_ALREADY_ASSIGNED = 85;
    private const int ERROR_BAD_DEVICE = 1200;
    private const int ERROR_BAD_NET_NAME = 67;
    private const int ERROR_BAD_PROVIDER = 1204;
    private const int ERROR_CANCELLED = 1223;
    private const int ERROR_EXTENDED_ERROR = 1208;
    private const int ERROR_INVALID_ADDRESS = 487;
    private const int ERROR_INVALID_PARAMETER = 87;
    private const int ERROR_INVALID_PASSWORD = 1216;
    private const int ERROR_MORE_DATA = 234;
    private const int ERROR_NO_MORE_ITEMS = 259;
    private const int ERROR_NO_NET_OR_BAD_PATH = 1203;
    private const int ERROR_NO_NETWORK = 1222;

    private const int ERROR_BAD_PROFILE = 1206;
    private const int ERROR_CANNOT_OPEN_PROFILE = 1205;
    private const int ERROR_DEVICE_IN_USE = 2404;
    private const int ERROR_NOT_CONNECTED = 2250;
    private const int ERROR_OPEN_FILES = 2401;

    #endregion

    #region PInvoke Signatures

    [DllImport("Mpr.dll")]
    private static extern int WNetUseConnection(
        IntPtr hwndOwner,
        NETRESOURCE lpNetResource,
        string lpPassword,
        string lpUserID,
        int dwFlags,
        string? lpAccessName,
        string? lpBufferSize,
        string? lpResult
        );

    [DllImport("Mpr.dll")]
    private static extern int WNetCancelConnection2(
        string lpName,
        int dwFlags,
        bool fForce
        );

    [StructLayout(LayoutKind.Sequential)]
    private class NETRESOURCE
    {
        public int dwScope = 0;
        public int dwType = 0;
        public int dwDisplayType = 0;
        public int dwUsage = 0;
        public string lpLocalName = "";
        public string lpRemoteName = "";
        public string lpComment = "";
        public string lpProvider = "";
    }

    #endregion

    public static AcessoCompartilhamentoRede Access(string remoteComputerName)
    {
        return new AcessoCompartilhamentoRede(remoteComputerName);
    }

    public static AcessoCompartilhamentoRede Acessar(string nomeComputador,
                                                     string nomeDominio,
                                                     string usuario,
                                                     string senha)
    {
        return new AcessoCompartilhamentoRede(nomeComputador,
                                              nomeDominio + @"\" + usuario,
                                              senha);
    }

    public static AcessoCompartilhamentoRede Acessar(string nomeComputador, string usuario, string senha)
    {
        return new AcessoCompartilhamentoRede(nomeComputador, usuario, senha);
    }

    //public Exception Erro { get; private set; }

    public AcessoCompartilhamentoRede(string nomeComputador)
    {
        this._nomeComputador = nomeComputador;
        this._nomeComptuadorUnc = RedeUtil.NormalizarNomeComputadorParaUnc(nomeComputador);
        this.ConectarCompartilhamento(this._nomeComptuadorUnc, null, null, true);
    }

    public AcessoCompartilhamentoRede(string? nomeComputador, string? usuario, string? senha)
    {
        Guard.NotNull(nomeComputador);
        Guard.NotNull(usuario);
        Guard.NotNull(senha);

        this._nomeComputador = nomeComputador;
        this._nomeComptuadorUnc = RedeUtil.NormalizarNomeComputadorParaUnc(nomeComputador);
        this.Usuario = usuario;
        this.Senha = senha;

        this.ConectarCompartilhamento(this._nomeComptuadorUnc, this.Usuario, this.Senha, false);
    }

    public AcessoCompartilhamentoRede(IAcessoDiretorio acessoDiretorio) :
                                     this(acessoDiretorio.NomeComputador,
                                         acessoDiretorio.Usuario,
                                         acessoDiretorio.Senha)
    {

    }

    private void ConectarCompartilhamento(string nomeComputadorUnc,
        string? usuario,
        string? senha,
        bool usuarioPrompt)
    {
        var nr = new NETRESOURCE
        {
            dwType = RESOURCETYPE_DISK,
            lpRemoteName = nomeComputadorUnc
        };

        int resultado;
        if (usuarioPrompt)
        {
            resultado = WNetUseConnection(IntPtr.Zero, nr, "", "", CONNECT_INTERACTIVE | CONNECT_PROMPT, null, null, null);
        }
        else
        {
            Guard.NotNullOrWhiteSpace(usuario);
            Guard.NotNullOrWhiteSpace(senha);
            resultado = WNetUseConnection(IntPtr.Zero, nr, senha, usuario, 0, null, null, null);
        }

        if (resultado != NO_ERROR && resultado != USUARIO_JA_CONECTADO)
        {
            if (resultado == ERROR_USER_PASSWORD_INVALID)
            {
                if (usuario is not null &&
                    !usuario.Contains(@"\") && !IpUtil.IsIP(this.NomeComputador))
                {
                    usuario = String.Format("{0}\\{1}", this.NomeComputador, this.Usuario);
                    this.ConectarCompartilhamento(this._nomeComptuadorUnc, usuario, senha, false);
                }
                else
                {
                    throw new Win32Exception(resultado);
                }
            }
            else
            {
                throw new Win32Exception(resultado);
            }
        }
    }

    private void DesconectarDoCompartilhamento(string remoteUnc)
    {
        int result = WNetCancelConnection2(remoteUnc,
                                          CONNECT_UPDATE_PROFILE, false);

        if (result != NO_ERROR)
        {
            new Win32Exception(result);
        }
    }

    public void Dispose()
    {
        this.DesconectarDoCompartilhamento(this._nomeComptuadorUnc);
    }
}
