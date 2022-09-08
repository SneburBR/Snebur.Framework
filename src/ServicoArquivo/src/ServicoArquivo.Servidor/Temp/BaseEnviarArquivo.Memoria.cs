namespace Snebur.ServicoArquivo.Servidor
{
    //Imports System.Web
    //Imports System.IO
    //Imports System.Web.Caching

    //Partial Public Class BaseEnviarStream(Of TContexto As BaseContexto)

    //    Private Sub SalvarStreamPorPartesMemeoria(httpContext As HttpContext, IdBaseStream As Long, contexto As TContexto, cabecalho As CabecalhoServicoStream)

    //        If cabecalho.EnvioConcluido Then

    //            Me.RemoverBaseStreamEnvioVMCACHE(cabecalho.IdStream)

    //        Else

    //            Dim pacote(httpContext.Request.ContentLength - 1) As Byte
    //            httpContext.Request.InputStream.Read(pacote, 0, pacote.Length)
    //            httpContext.Request.InputStream.Dispose()

    //            If cabecalho.ParteAtual <= 0 Then
    //                Throw new Exception(String.Format("Partal atual da stream deve ser maior que 0. Parte atual: '{0}'", cabecalho.ParteAtual))
    //            End If

    //            If cabecalho.ParteAtual = 1 Then
    //                Me.NotificarInicioEnvioStream(httpContext, IdBaseStream, contexto)
    //            End If

    //            Dim baseStreamEnvio = Me.RetornarBaseStreamEnvioVM(cabecalho)
    //            baseStreamEnvio.AdicionarPacote(pacote)
    //            '
    //            If cabecalho.ParteAtual = cabecalho.TotalPartes Then

    //                Dim bytesStream = baseStreamEnvio.RetornarBytes
    //                Dim caminhoCompletoStream As String = Me.RetornarCaminhoCompletoStream(httpContext, IdBaseStream)
    //                ArquivoUtils.DeletarArquivo(caminhoCompletoStream)
    //                IO.File.WriteAllBytes(caminhoCompletoStream, bytesStream)
    //                Me.EnviarStreamConcluido(httpContext, IdBaseStream, caminhoCompletoStream, cabecalho.CheckSum, contexto)

    //            End If

    //        End If

    //    End Sub

    //    Private Function RetornarCHAVE_CACHE(IdBaseStream As Long) As String
    //        Return String.Format("CHACHE_STREAM_{0}", IdBaseStream)
    //    End Function
    //    Private Function RetornarBaseStreamEnvioVMCACHE(IdBaseStream As Long) As BaseStreamEnvioViewModel
    //        Dim CHAVE = RetornarCHAVE_CACHE(IdBaseStream)
    //        If HttpRuntime.Cache(CHAVE) Is Nothing Then
    //            HttpRuntime.Cache.Insert(CHAVE, New BaseStreamEnvioViewModel(IdBaseStream), Nothing, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1))
    //        End If
    //        Return HttpRuntime.Cache(CHAVE)
    //    End Function

    //    Private Sub RemoverBaseStreamEnvioVMCACHE(IdBaseStream As Long)
    //        Dim CHAVE = RetornarCHAVE_CACHE(IdBaseStream)
    //        HttpRuntime.Cache.Remove(CHAVE)
    //    End Sub

    //    Private Function RetornarBaseStreamEnvioVM(cabecalho As CabecalhoServicoStream) As BaseStreamEnvioViewModel
    //        Dim baseStreamEnvio = Me.RetornarBaseStreamEnvioVMCACHE(cabecalho.IdStream)
    //        If cabecalho.ParteAtual = 1 Then
    //            baseStreamEnvio.LimbarBytes()
    //        End If
    //        baseStreamEnvio.ValidarBytes(cabecalho.ParteAtual, cabecalho.TamanhoPacote)
    //        Return baseStreamEnvio
    //    End Function

    //End Class
}