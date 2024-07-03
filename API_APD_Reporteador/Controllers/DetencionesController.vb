Imports System.Web.Http
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers
    Public Class DetencionesController
        Inherits ApiController
        Dim Mdl_Detencion As New Mdl_Detencion

        <HttpGet>
        <Route("api/Detenciones/GuardaDetencion")>
        Public Function GuardaDetencion(ByVal nombre As String, ByVal apellidos As String, ByVal ali As String, ByVal lug_detencion As String, ByVal destino As String, ByVal latitud As String, ByVal longitud As String, ByVal foto As String, ByVal imei As String)
            Dim Cs_Resultado As New Cs_Respuesta
            Dim Cs_Detencion As New Cs_Detencion

            Cs_Detencion.SDE_NOMBRE = nombre
            Cs_Detencion.SDE_APELLIDOS = apellidos
            Cs_Detencion.SDE_ALIAS = ali
            Cs_Detencion.SDE_LUG_DETENCION = lug_detencion
            Cs_Detencion.SDE_DESTINO = destino
            Cs_Detencion.SDE_LONGITUD = latitud
            Cs_Detencion.SDE_LATITUD = longitud
            Cs_Detencion.SDE_FOTO = ""
            Cs_Detencion.SDI_IMEI = imei

            Try
                Cs_Resultado = Mdl_Detencion.GuardaDetencion(Cs_Detencion)
            Catch ex As Exception
                Cs_Resultado.CODIGO = -1
                Cs_Resultado.CODIGOERROR = 503
                Cs_Resultado.MENSAJE = "Error al guardar detencion. " + ex.Message

                Return Cs_Resultado
            End Try

            Return Cs_Resultado
        End Function

        <HttpPost>
        <Route("api/Detenciones/DetencionesPost")>
        Public Function DetencionesPost(<FromBody()> ByVal data As Object)
            Dim Cs_Resultado As New Cs_Respuesta
            Dim Cs_Detencion As New Cs_Detencion
            Try

                Cs_Detencion.SDE_NOMBRE = data("nombre").ToString()
                Cs_Detencion.SDE_APELLIDOS = data("apellidos").ToString()
                Cs_Detencion.SDE_ALIAS = data("alias").ToString()
                Cs_Detencion.SDE_LUG_DETENCION = data("lug_detencion").ToString()
                Cs_Detencion.SDE_DESTINO = data("destino").ToString()
                Cs_Detencion.SDE_LONGITUD = data("latitud").ToString()
                Cs_Detencion.SDE_LATITUD = data("longitud").ToString()
                Cs_Detencion.SDE_FOTO = data("foto").ToString()
                Cs_Detencion.SDI_IMEI = data("imei").ToString()
                Cs_Detencion.SDE_FECHA_DETENCION = Convert.ToDateTime(data("fechadetencion"))
                Cs_Detencion.SUS_CLAVE = data("idusuario").ToString()
                Cs_Detencion.SPA_CLAVE = data("idpatrulla").ToString()
                Cs_Detencion.SDE_MOTIVO = data("motivo").ToString()

                Cs_Resultado = Mdl_Detencion.GuardaDetencion(Cs_Detencion)

            Catch ex As Exception
                Cs_Resultado.CODIGO = -1
                Cs_Resultado.CODIGOERROR = 503
                Cs_Resultado.MENSAJE = "Error al guardar detencion." + ex.Message()

                Return Cs_Resultado
            End Try

            Return Cs_Resultado
        End Function
        <HttpGet>
        <Route("api/Detenciones/RecuperaDetenciones")>
        Public Function RecuperaDetenciones(ByVal nombre As String, ByVal apellidos As String, ByVal alia As String, ByVal perfil As String, ByVal dependencia As String)
            Dim Ls_Cs_Detencion As New List(Of Cs_Detencion)
            Dim Cs_Detencion As New Cs_Detencion

            If (String.IsNullOrEmpty(nombre)) Then
                nombre = ""
            End If
            If (String.IsNullOrEmpty(apellidos)) Then
                apellidos = ""
            End If
            If (String.IsNullOrEmpty(alia)) Then
                alia = ""
            End If
            If (String.IsNullOrEmpty(perfil)) Then
                perfil = "0"
            End If
            If (String.IsNullOrEmpty(dependencia)) Then
                dependencia = ""
            End If

            Cs_Detencion.SDE_NOMBRE = nombre
            Cs_Detencion.SDE_APELLIDOS = apellidos
            Cs_Detencion.SDE_ALIAS = alia
            Cs_Detencion.USU_ID_PERFIL = Convert.ToInt32(perfil)

            Try
                Ls_Cs_Detencion = Mdl_Detencion.RecuperaDetenciones(Cs_Detencion, dependencia)
            Catch ex As Exception
                Cs_Detencion.CODIGO = -1
                Cs_Detencion.CODIGOERROR = 503
                Cs_Detencion.MENSAJE = "Error al recuperar usuarios. " + ex.Message

                Ls_Cs_Detencion.Add(Cs_Detencion)
                Return Ls_Cs_Detencion
            End Try
            Return Ls_Cs_Detencion
        End Function
    End Class
End Namespace