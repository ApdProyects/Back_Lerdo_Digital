Imports System.Net
Imports System.Web.Http
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models.Reportes

Namespace Controllers
    Public Class ReportesController
        Inherits ApiController

        Dim Mdl_Reportes As New Mdl_Reportes
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteSexo")>
        Public Function RecuperaReporteSexo()
            Dim Cs_RptSexo As New Cs_RptSexo

            Try
                Cs_RptSexo = Mdl_Reportes.RecuperaReporteSexo()
                Cs_RptSexo.CODIGO = 1
                Cs_RptSexo.CODIGOERROR = 200
                Cs_RptSexo.MENSAJE = "Correcto"
            Catch ex As Exception
                Cs_RptSexo = New Cs_RptSexo
                Cs_RptSexo.CODIGO = -1
                Cs_RptSexo.CODIGOERROR = 503
                Cs_RptSexo.MENSAJE = "Error al recuperar Informacion del reporte. "

                Return Cs_RptSexo
            End Try
            Return Cs_RptSexo
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteEdad")>
        Public Function RecuperaReporteEdad()
            Dim Cs_RptEdad As New Cs_RptEdad

            Try
                Cs_RptEdad = Mdl_Reportes.RecuperaReporteEdad()
                Cs_RptEdad.CODIGO = 1
                Cs_RptEdad.CODIGOERROR = 200
                Cs_RptEdad.MENSAJE = "Correcto"
            Catch ex As Exception
                Cs_RptEdad = New Cs_RptEdad
                Cs_RptEdad.CODIGO = -1
                Cs_RptEdad.CODIGOERROR = 503
                Cs_RptEdad.MENSAJE = "Error al recuperar Informacion del reporte. "

                Return Cs_RptEdad
            End Try
            Return Cs_RptEdad
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteHorario")>
        Public Function RecuperaReporteHorario()
            Dim Cs_RptHorario As New Cs_RptHorario

            Try
                Cs_RptHorario = Mdl_Reportes.RecuperaReporteHorario()
                Cs_RptHorario.CODIGO = 1
                Cs_RptHorario.CODIGOERROR = 200
                Cs_RptHorario.MENSAJE = "Correcto"
            Catch ex As Exception
                Cs_RptHorario = New Cs_RptHorario
                Cs_RptHorario.CODIGO = -1
                Cs_RptHorario.CODIGOERROR = 503
                Cs_RptHorario.MENSAJE = "Error al recuperar Informacion del reporte. "

                Return Cs_RptHorario
            End Try
            Return Cs_RptHorario
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteMotivo")>
        Public Function RecuperaReporteMotivo()
            Dim L_Cs_RptMotivo As New List(Of Cs_RptMotivo)
            Dim Cs_RptMotivo As New Cs_RptMotivo

            Try
                L_Cs_RptMotivo = Mdl_Reportes.RecuperaReporteMotivo()
            Catch ex As Exception
                Cs_RptMotivo = New Cs_RptMotivo
                Cs_RptMotivo.CODIGO = -1
                Cs_RptMotivo.CODIGOERROR = 503
                Cs_RptMotivo.MENSAJE = "Error al recuperar Informacion del reporte. "

                Return Cs_RptMotivo
            End Try
            Return L_Cs_RptMotivo
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReportePago")>
        Public Function RecuperaReportePago()
            Dim L_Cs_RptPago As New List(Of Cs_RptPago)
            Dim Cs_RptPago As New Cs_RptPago

            Try
                L_Cs_RptPago = Mdl_Reportes.RecuperaReportePago()
            Catch ex As Exception
                Cs_RptPago = New Cs_RptPago
                Cs_RptPago.CODIGO = -1
                Cs_RptPago.CODIGOERROR = 503
                Cs_RptPago.MENSAJE = "Error al recuperar Informacion del reporte. "

                Return Cs_RptPago
            End Try
            Return L_Cs_RptPago
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteTiempos")>
        Public Function RecuperaReporteTiempos()
            Dim L_Cs_RptTiempos As New List(Of Cs_RptTiempos)
            Dim Cs_RptTiempos As New Cs_RptTiempos

            Try
                L_Cs_RptTiempos = Mdl_Reportes.RecuperaReporteTiempos()
            Catch ex As Exception
                Cs_RptTiempos = New Cs_RptTiempos
                Cs_RptTiempos.CODIGO = -1
                Cs_RptTiempos.CODIGOERROR = 503
                Cs_RptTiempos.MENSAJE = "Error al recuperar Informacion del reporte. " + ex.Message

                Return Cs_RptTiempos
            End Try
            Return L_Cs_RptTiempos
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteCobro")>
        Public Function RecuperaReporteCobro()
            Dim L_Cs_RptCobro As New List(Of Cs_RptCobro)
            Dim Cs_RptCobro As New Cs_RptCobro

            Try
                L_Cs_RptCobro = Mdl_Reportes.RecuperaReporteCobro()
            Catch ex As Exception
                Cs_RptCobro = New Cs_RptCobro
                Cs_RptCobro.CODIGO = -1
                Cs_RptCobro.CODIGOERROR = 503
                Cs_RptCobro.MENSAJE = "Error al recuperar Informacion del reporte. " + ex.Message

                Return Cs_RptCobro
            End Try
            Return L_Cs_RptCobro
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteMultasAgente")>
        Public Function RecuperaReporteMultasAgente(ByVal fechaInicio As DateTime, ByVal FechaFin As DateTime)
            Dim L_Cs_RptVialidad As New List(Of Cs_RptVialidad)
            Dim Cs_RptVialidad As New Cs_RptVialidad

            Try
                L_Cs_RptVialidad = Mdl_Reportes.RecuperaReporteMultasAgentes(fechaInicio, FechaFin)
            Catch ex As Exception
                Cs_RptVialidad = New Cs_RptVialidad
                Cs_RptVialidad.CODIGO = -1
                Cs_RptVialidad.CODIGOERROR = 503
                Cs_RptVialidad.MENSAJE = "Error al recuperar Informacion del reporte. " + ex.Message

                Return Cs_RptVialidad
            End Try
            Return L_Cs_RptVialidad
        End Function
        <HttpGet>
        <Route("api/Reportes/RecuperaReporteMultasDinero")>
        Public Function RecuperaReporteMultasDinero(ByVal fechaInicio As DateTime, ByVal FechaFin As DateTime)
            Dim L_Cs_RptVialidad As New List(Of Cs_RptVialidad)
            Dim Cs_RptVialidad As New Cs_RptVialidad

            Try
                L_Cs_RptVialidad = Mdl_Reportes.RecuperaReporteMultasDinero(fechaInicio, FechaFin)
            Catch ex As Exception
                Cs_RptVialidad = New Cs_RptVialidad
                Cs_RptVialidad.CODIGO = -1
                Cs_RptVialidad.CODIGOERROR = 503
                Cs_RptVialidad.MENSAJE = "Error al recuperar Informacion del reporte. " + ex.Message

                Return Cs_RptVialidad
            End Try
            Return L_Cs_RptVialidad
        End Function
    End Class
End Namespace