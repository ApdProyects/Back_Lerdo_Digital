Imports System.Net
Imports System.Web.Http
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers
    Public Class RptDepartamentosController
        Inherits ApiController

        Dim Mdl_RptDepartamentos As New Mdl_RptDepartamentos
        Dim encrypt As New Cs_Encrypt

        <HttpGet>
        <Route("api/RptDeptos/RptSeguridad")>
        Public Function RptSeguridad(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptSeguridad(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptParquimetros")>
        Public Function RptParquimetros(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptParquimetros(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptPYM")>
        Public Function RptPYM(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptPYM(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptSM")>
        Public Function RptSM(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptSM(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptPS")>
        Public Function RptPS(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptPS(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptMA")>
        Public Function RptMA(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptMA(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptPC")>
        Public Function RptPC(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptPC(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptRST")>
        Public Function RptRST(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptRST(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptDeportes")>
        Public Function RptDeportes(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptDeportes(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptDUB")>
        Public Function RptDUB(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptDUB(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptAlcoholes")>
        Public Function RptAlcoholes(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptAlcoholes(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptFE")>
        Public Function RptFE(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptFE(opcion, fechainicio, fechafin, estatus)
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
        <HttpGet>
        <Route("api/RptDeptos/RptArte")>
        Public Function RptArte(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim da
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                da = Mdl_RptDepartamentos.Recupera_RptArte(opcion, fechainicio, fechafin, estatus)
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