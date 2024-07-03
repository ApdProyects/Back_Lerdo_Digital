Imports System.Net
Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models
Imports System.Web.Mail
Imports System.Net.Mail
Imports System.Globalization
Imports SelectPdf
Imports System.IO

Namespace Controllers
    Public Class EnvioSMSMasivoController
        Inherits ApiController
        Dim Cs_JWT As New Cs_JWT
        Dim Mdl_Ciudadanos As New Mdl_Ciudadanos
        Dim encrypt As New Cs_Encrypt
        Dim Mdl_Usuarios As New Mdl_Usuarios

        <HttpGet>
        <Route("api/EnvioSMSMasivo/enviosms")>
        Public Function enviosms()
            Dim Cs_Usuario_AC As New List(Of Cs_ResultadosUPD)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim Cs_Respuesta_envio As New Cs_Respuesta
            Dim mensaje As String

            Try

                Cs_Usuario_AC = Mdl_Usuarios.recuperaResultadosUPD()
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Telefono no válido."

                Return Cs_Respuesta
            End Try

            Try
                For Each u In Cs_Usuario_AC
                    If (u.RES_ENVIADO = "NO") Then
                        If (u.RES_RESULTADO = "SI") Then
                            mensaje = u.RES_NOMBRE.ToUpper() + ", fuiste aprobado para ingresar a la institucion UPD, con una puntuacion de " + u.RES_PUNTOS + ", para saber mas ingresa a https://upd.apdacademics.mx"

                            mensaje = mensaje.Replace("Á", "A")
                            mensaje = mensaje.Replace("É", "E")
                            mensaje = mensaje.Replace("Í", "I")
                            mensaje = mensaje.Replace("Ó", "O")
                            mensaje = mensaje.Replace("Ú", "U")
                            mensaje = mensaje.Replace("Ñ", "N")

                            Cs_Respuesta = envio_sms(mensaje, u.RES_TELEFONO)
                            Cs_Respuesta_envio = Mdl_Usuarios.ActualizaEnvio(u.RES_CLAVE)
                        ElseIf (u.RES_RESULTADO = "NO") Then
                            mensaje = u.RES_NOMBRE.ToUpper() + ", no fuiste aprobado para ingresar a la institucion UPD, con una puntuacion de " + u.RES_PUNTOS + ", para saber mas ingresa a https://upd.apdacademics.mx"

                            mensaje = mensaje.Replace("Á", "A")
                            mensaje = mensaje.Replace("É", "E")
                            mensaje = mensaje.Replace("Í", "I")
                            mensaje = mensaje.Replace("Ó", "O")
                            mensaje = mensaje.Replace("Ú", "U")
                            mensaje = mensaje.Replace("Ñ", "N")

                            Cs_Respuesta = envio_sms(mensaje, u.RES_TELEFONO)
                            Cs_Respuesta_envio = Mdl_Usuarios.ActualizaEnvio(u.RES_CLAVE)
                        End If
                    End If
                Next

                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Correcto."
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 402
                Cs_Respuesta.mensaje = "Error."

                Return Cs_Respuesta
            End Try

            Return Cs_Respuesta
        End Function
        Public Function envio_sms(ByVal MensajeSMS As String, ByVal CadenaSMS As String)

            Dim GAF_CLAVE As Integer = 0
            Dim GAF_NOMBRE As String = ""

            Dim sms_envia As New ServiceReference1.smsexpertPortTypeClient ''mx.smsexpert.ext2..smsexpert
            Dim result As String
            Dim NumSMS As Integer
            Dim strUser As String
            Dim strEntidad As String
            Dim Cs_Respuesta As New Cs_Respuesta

            strEntidad = "{" & Chr(34) & "USUARIO" & Chr(34) & ":" & Chr(34) & "APD" & Chr(34) & ", " & Chr(34) & "CONTRASENA" & Chr(34) & ":" & Chr(34) & "xBoIhd1m98hM9l0py5l1" & Chr(34) & "}"
            strUser = "{" & Chr(34) & "TIPO_USUARIO" & Chr(34) & ":" & Chr(34) & "PYXTER" & Chr(34) & "," & Chr(34) & "NUMERO" & Chr(34) & ":" & Chr(34) & "APD_2017" & Chr(34) & "," & Chr(34) & "NIP" & Chr(34) & ":" & Chr(34) & "8224" & Chr(34) & "}"

            Try
                NumSMS = sms_envia.consultar_saldo("SMSEXPERT_EXT1", strEntidad, strUser, "SMS", "APD_PL")
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.mensaje = "Error al enviar correo." + ex.Message

                Return Cs_Respuesta
            End Try

            If NumSMS > 1 Then

                Try
                    result = sms_envia.envio_sms("SMSEXPERT_EXT1", strEntidad, strUser, "APD_PL", CadenaSMS, MensajeSMS)

                    Cs_Respuesta.codigo = 1
                    Cs_Respuesta.mensaje = "Correo enviado."

                    Return Cs_Respuesta
                Catch ex As Exception
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.mensaje = "Error al enviar correo." + ex.Message

                    Return Cs_Respuesta
                End Try
            Else
            End If
#Disable Warning BC42105 ' La función 'envio_sms' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
        End Function
#Enable Warning BC42105 ' La función 'envio_sms' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
    End Class
End Namespace