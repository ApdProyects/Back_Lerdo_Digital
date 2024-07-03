Imports System.Net
Imports System.Web.Http
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers
    Public Class RptIngresosController
        Inherits ApiController

        Dim Mdl_Ingresos As New Mdl_Ingresos
        Dim encrypt As New Cs_Encrypt

        <HttpGet>
        <Route("api/RptIngresos/RptIngresos")>
        Public Function RptIngresos(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String, ByVal garantia As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_Ingresos.Recupera_RptIngresos(opcion, fechainicio, fechafin, estatus, garantia)
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Problema para conectar con el servidor inténtelo nuevamente."
                Cs_Respuesta.internalMessage = ex.Message
                Cs_Respuesta.internalCode = ex.InnerException.Message


                Return Cs_Respuesta
            End Try

            Return da
        End Function
    End Class
End Namespace