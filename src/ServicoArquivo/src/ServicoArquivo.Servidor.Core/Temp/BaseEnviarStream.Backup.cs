using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.ServicoArquivo.Servidor
{
    //Imports System.Web
    //Imports System.IO
    //Imports System.Web.Caching
    //Imports Snebur.Seguranca
    
    //Public MustInherit Class BaseEnviarStreamBackup(Of TContexto As BaseContexto)
    //    Inherits BaseServicoStream
    //    Implements System.Web.IHttpAsyncHandler
    
    //#Region " Propriedades e Delegate "
    
    //    Private Property AsyncTask As AsyncTaskDelegate
    
    //    Protected Property DataHoraInicial As Date
    //    Protected Property DataHoraFinal As Date
    
    //    Protected Delegate Sub AsyncTaskDelegate(ByVal httpContext As HttpContext)
    
    //#End Region
    
    //#Region " Construtores "
    
    //    Public Sub New()
    
    //    End Sub
    
    //#End Region
    
    //#Region " Métodos "
    
    //#Region " Salvar stream  "
    
    //    Protected Overridable Sub SalvarStream(httpContext As HttpContext)
    
    //        Dim cabecalho = ServicoStreamUtils.RetornarCabecalho(httpContext)
    //        Dim contexto = Me.RetornarContexto
    
    //        If Not (cabecalho.IdStream > 0) Then
    //            Throw New Exception(String.Format("O IdBaseSteam deve ser maior 0. '{0}", cabecalho.IdStream))
    //        End If
    
    //        Dim idBaseStream As Long = cabecalho.IdStream
    //        If cabecalho.ParteAtual <= 1 Then
    //            If Not contexto.ExisteIdBaseStream(idBaseStream) Then
    //                Throw New Exception(String.Format("A BaseStream com Id {0} não foi encontrada.", idBaseStream))
    //            End If
    //        End If
    
    //        If cabecalho.TotalPartes > 0 Then
    //            Me.SalvarStreamPorPartes(httpContext, idBaseStream, contexto, cabecalho.ParteAtual, cabecalho.TotalPartes, cabecalho.CheckSum)
    //        Else
    //            Me.SalvarStreamSimples(httpContext, idBaseStream, contexto, cabecalho.CheckSum)
    //        End If
    //        Me.NotificarMonitorAsync(cabecalho, httpContext)
    
    //    End Sub
    
    //    Protected MustOverride Function RetornarContexto() As OData.BaseContexto
    
    //#Region " Monitor "
    
    //    Private Sub NotificarMonitorAsync(cabecalho As CabecalhoServicoStream, context As HttpContext)
    //        Task.Factory.StartNew(Sub() NotificarMonitor(cabecalho))
    //    End Sub
    
    //    Protected Overridable Sub NotificarMonitor(cabecalho As CabecalhoServicoStream)
    
    //    End Sub
    
    //#End Region
    
    //#Region " Salvar stream por partes "
    
    //    Private Sub SalvarStreamPorPartes(httpContext As HttpContext, IdBaseStream As Long, contexto As TContexto, parteAtual As Integer, totalPartes As Integer, checkSum As String)
    
    //        If parteAtual <= 0 Then
    //            Throw new Exception(String.Format("Partal atual da stream deve ser maior que 0. Parte atual: '{0}'", parteAtual))
    //        End If
    
    //        If parteAtual = 1 Then
    //            Me.NotificarInicioEnvioStream(httpContext, IdBaseStream, contexto)
    //        End If
    
    //        Using streamTemporaria = Me.RetornarStreamTemporaria(httpContext, parteAtual, IdBaseStream)
    //            Dim pacote(httpContext.Request.ContentLength - 1) As Byte
    //            httpContext.Request.InputStream.Read(pacote, 0, pacote.Length)
    //            streamTemporaria.Write(pacote, 0, pacote.Length)
    //        End Using
    
    //        If parteAtual = totalPartes Then
    //            Dim caminhoTemporario = Me.RetornarCaminhoTempoario(httpContext, IdBaseStream)
    //            Dim caminhoCompletoStream As String = Me.RetornarCaminhoCompletoStream(httpContext, IdBaseStream)
    //            ArquivoUtils.DeletarArquivo(caminhoCompletoStream)
    //            IO.File.Copy(caminhoTemporario, caminhoCompletoStream, True)
    //            ArquivoUtils.DeletarArquivo(caminhoTemporario)
    //            Me.EnviarStreamConcluido(httpContext, IdBaseStream, caminhoCompletoStream, checkSum, contexto)
    //        End If
    
    //    End Sub
    
    //    Private Function RetornarStreamTemporaria(httpContext As HttpContext, parteAtual As Integer, idStream As Integer) As IO.Stream
    //        Dim caminhoTemporario As String = Me.RetornarCaminhoTempoario(httpContext, idStream)
    //        If parteAtual = 1 Then
    //            ArquivoUtils.DeletarArquivo(caminhoTemporario)
    //        End If
    
    //        Dim stream As FileStream = File.OpenWrite(caminhoTemporario)
    //        stream.Seek(0, SeekOrigin.End)
    //        Return stream
    
    //    End Function
    
    //    Protected Overridable Function RetornarCaminhoTempoario(httpContext As HttpContext, idBaseStream As Long) As String
    //        Throw New NotImplementedException
    //        'Return IO.Path.Combine(Me.RetornarCaminhoRepositoTemporatio, String.Format("{0}-{1}", idBaseStream, IO.Path.GetTempFileName))
    //    End Function
    
    //#End Region
    
    //#Region " Salvar stream simples "
    
    //    Private Sub SalvarStreamSimples(httpContext As HttpContext, idBaseStream As Long, contexto As TContexto, checkSum As String)
    
    //        Me.NotificarInicioEnvioStream(httpContext, idBaseStream, contexto)
    //        Dim caminhoCompletoStream As String = Me.RetornarCaminhoCompletoStream(httpContext, idBaseStream)
    //        Dim totalBytesImagam As Long
    
    //        Using streamImagem = Me.RetornarInputStream(httpContext)
    
    //            totalBytesImagam = streamImagem.Length
    //            ArquivoUtils.DeletarArquivo(caminhoCompletoStream)
    //            Using ms As New IO.MemoryStream
    //                streamImagem.CopyTo(ms)
    
    //                Using fi As IO.FileStream = IO.File.OpenWrite(caminhoCompletoStream)
    
    //                    fi.Write(ms.ToArray, 0, ms.Length)
    //                    fi.Flush()
    //                    fi.Close()
    
    //                End Using
    
    //            End Using
    
    //        End Using
    
    //        Me.EnviarStreamConcluido(httpContext, idBaseStream, caminhoCompletoStream, checkSum, contexto)
    
    //    End Sub
    
    //    Private Function RetornarInputStream(httpContext As HttpContext) As IO.Stream
    
    //        Dim stream As IO.Stream = Nothing
    //        If httpContext.Request.Files.Count = 1 Then
    //            stream = httpContext.Request.Files(0).InputStream
    //        Else
    //            stream = httpContext.Request.InputStream
    //        End If
    
    //        If stream Is Nothing Then
    //            Throw new Exception("Salvar stream simples, A InputStream não foi definido.")
    //        End If
    //        Return stream
    //    End Function
    
    //#End Region
    
    //    Private Sub EnviarStreamConcluido(httpContext As HttpContext, IdBaseStream As String, caminhoCompletoStream As String, checksum As String, contexto As TContexto)
    
    //        If Not String.IsNullOrWhiteSpace(checksum) Then
    //            Dim checkSumArquivo = ArquivoUtils.RetornarChecksum(caminhoCompletoStream)
    //            If checkSumArquivo <> checksum Then
    //                Throw new Exception(String.Format("SercoStream. Os checksum são diferentes. '{0}' - '{1}'", checksum, checkSumArquivo))
    //            End If
    //        End If
    
    //        Dim totalBytes = New IO.FileInfo(caminhoCompletoStream).Length
    //        Me.NotificarEnvioStreamConcluido(httpContext, IdBaseStream, totalBytes, contexto)
    //    End Sub
    
    //#End Region
    
    //    Protected Overridable Function RetornarCaminhoRepositoTemporatio() As String
    //        Dim caminhoTemporario = IO.Path.Combine(Me.RetornarRepositoStream, "Temp")
    //        ArquivoUtils.CriarDiretorio(caminhoTemporario)
    //        Return caminhoTemporario
    //    End Function
    
    //    Protected Overridable Sub NotificarInicioEnvioStream(httpContext As HttpContext, IdBaseStream As Long, contexto As TContexto)
    //        Throw New NotImplementedException
    //    End Sub
    
    //    Protected Overridable Sub NotificarEnvioStreamConcluido(httpContext As HttpContext, IdBaseStream As Long, totalBytes As Long, contexto As TContexto)
    //        Throw New NotImplementedException
    //    End Sub
    
    //#End Region
    
    //#Region " IHttpAsyncHandler "
    
    //    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
    
    //        If Not (AutenticacaoUtils.UsuarioPossuiFuncao(context, EnumFuncao.ServicoStream) OrElse
    //                AutenticacaoUtils.UsuarioPossuiFuncao(context, EnumFuncao.Lightroom)) Then
    
    //            Throw ExcecaoUtils.NotificarExcecao(new Exception(String.Format("Você não possui permição para acessar ServicoStream, EnviarStream. Funcao: {0}", AutenticacaoUtils.RetornarFuncaUsuario(context))))
    
    //        End If
    
    //        Me.SalvarStream(context)
    
    //    End Sub
    
    //    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
    //        Get
    //            Return False
    //        End Get
    //    End Property
    
    //    Public Function BeginProcessRequest(context As HttpContext, cb As AsyncCallback, extraData As Object) As IAsyncResult Implements IHttpAsyncHandler.BeginProcessRequest
    //        Me.DataHoraInicial = Now
    //        Me.AsyncTask = New AsyncTaskDelegate(AddressOf ProcessRequest)
    //        Return AsyncTask.BeginInvoke(context, cb, extraData)
    //    End Function
    
    //    Public Sub EndProcessRequest(result As IAsyncResult) Implements IHttpAsyncHandler.EndProcessRequest
    //        Me.AsyncTask.EndInvoke(result)
    //    End Sub
    
    //#End Region
    
    //End Class
}