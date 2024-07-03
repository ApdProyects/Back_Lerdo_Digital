Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers
    Public Class InfoFolioController
        Inherits ApiController

        Dim Mdl_InfoFolio As New Mdl_InfoFolio
        Dim Cs_JWT As New Cs_JWT
        Dim encrypt As New Cs_Encrypt

        <HttpGet>
        <Route("api/InfoFolio/RecuperaInfoFolio")>
        Public Function RecuperaInfoFolio(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer)
            Dim da
            Dim Cs_InfoFolio As New Cs_InfoFolio
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.

            Dim re = Request
            Dim Headers = re.Headers

            Try
                da = Mdl_InfoFolio.Recupera_datos_folio(id_folio, id_concepto, id_depto)


                If (da.Count = 0) Then
                    Cs_InfoFolio.codigo = -1
                    Cs_InfoFolio.codigoError = 50004
                    Cs_InfoFolio.mensaje = "No se encontró el folio."

                    Return Cs_InfoFolio
                End If
            Catch ex As Exception
                Cs_InfoFolio.codigo = -1
                Cs_InfoFolio.codigoError = 50003
                Cs_InfoFolio.mensaje = "No se encontró el folio."

                Return Cs_InfoFolio
            End Try

            Return da
        End Function

        <HttpGet>
        <Route("api/InfoFolio/RecuperaInfoFolioCatastro")>
        Public Function RecuperaInfoFolioCatastro(ByVal clave As String)
            Dim da
            Dim Cs_InfoFolio As New Cs_InfoFolio
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.

            Dim re = Request
            Dim Headers = re.Headers

            Try
                da = Mdl_InfoFolio.Recupera_datos_folio_catastro(clave)
                If (da.Count = 0) Then
                    Cs_InfoFolio.codigo = -1
                    Cs_InfoFolio.codigoError = 50004
                    Cs_InfoFolio.mensaje = "No se encontró el folio."

                    Return Cs_InfoFolio
                End If
            Catch ex As Exception
                Cs_InfoFolio.codigo = -1
                Cs_InfoFolio.codigoError = 50003
                Cs_InfoFolio.mensaje = "No se encontró el folio."

                Return Cs_InfoFolio
            End Try

            Return da
        End Function
    End Class
End Namespace