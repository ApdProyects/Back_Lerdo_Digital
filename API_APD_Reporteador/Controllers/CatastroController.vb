Imports System.Net
Imports System.Web.Http
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers
    Public Class CatastroController
        Inherits ApiController

        Dim Mdl_Catastro As New Mdl_Catastro

        <HttpGet>
        <Route("api/Catastro/GeneraCodigoAleatorio")>
        Public Function GeneraCodigoAleatorio(ByVal clave As String)
#Disable Warning BC42024 ' Variable local sin usar: 'da'.
            Dim da
#Enable Warning BC42024 ' Variable local sin usar: 'da'.
#Disable Warning BC42024 ' Variable local sin usar: 'codigo'.
            Dim codigo As String
#Enable Warning BC42024 ' Variable local sin usar: 'codigo'.
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim telefono = ""
            Dim mensaje = ""
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim Array As ArrayList

            Array = Mdl_Catastro.Valida_Codigo_Catastro(clave, 0)

            If (Array.Count = 0) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "La clave catastral no existe, intentelo nuevamente."
                Return Cs_Respuesta
            End If

            telefono = Mdl_Catastro.Recupera_telefono_Catastro(clave)

            Dim rnd As New Random()
            Dim numAleatorio As Integer = rnd.Next(100000, 999999)

            mensaje = "Validacion de acceso a cobro del portal lerdo digital MX. " +
                                   "Codigo de confirmacion: " + numAleatorio.ToString + ", " +
                                   "el codigo generado tiene un tiempo de expiracion de 5 minutos."

            Try
                Mdl_Catastro.Actualiza_Codigo_Catastro(clave, numAleatorio)
                Cs_Respuesta = envio_sms(mensaje, telefono) ' Cs_Usuario_AC.LUS_TELEFONO

                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Los datos se recuperaron correctamente, en breve recibirá un mensaje SMS con sus datos de acceso."
            Catch ex As Exception
                Cs_Respuesta = New Cs_Respuesta
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 408
                Cs_Respuesta.mensaje = "Por el momento el sistema de recuperación se encuentra suspendido, favor de intentar más tarde."
            End Try

            Return Cs_Respuesta
        End Function

        '/*     ESTA FUNCION, SI SE UTILIZA      */
        <HttpGet>
        <Route("api/Catastro/ValidaCodigoAleatorio")>
        Public Function ValidaCodigoAleatorio(ByVal clave As String, ByVal codigo As String)
            Dim Array As New ArrayList
            Dim ArrayCatastro As New ArrayList
            Dim tipo As String
            Dim mensaje = ""
            Dim Cs_Respuesta As New Cs_Respuesta_pago

            If (clave.Count() < 11) Then
                tipo = 2
            Else
                tipo = 1
            End If

            Try
                ArrayCatastro = Mdl_Catastro.traer_info_catastro(clave, tipo)

                If ArrayCatastro.Count < 1 Then
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "El folio o la clave catastral no existe, inténtelo nuevamente."
                    Return Cs_Respuesta
                Else

                    'ITEM ARRAY: (0) = 999
                    'Validamos si el ITEM ARRAY ES 999, si es 999 es porque LA CLAVE CATASTRAL NECESITA SER VERIFICADA

                    'Validamos el item array de ArrayCatastro

                    Dim primeraFila As DataRow = ArrayCatastro(0)

                    Dim itemArrayPrimeraFila As Object() = primeraFila.ItemArray

                    If Convert.ToInt32(itemArrayPrimeraFila(0)) = 999 Then

                        Cs_Respuesta.codigo = -2
                        Cs_Respuesta.codigoError = 200
                        Cs_Respuesta.mensaje = $"La clave {clave.ToString} requiere pasar a CATASTRO para su REVISION INMEDIATA."
                        Return Cs_Respuesta

                    End If

                    Cs_Respuesta.codigo = 1
                    Cs_Respuesta.codigoError = 200
                    Cs_Respuesta.mensaje = "Correcto."
                    Cs_Respuesta.Cs_InfoFolio = ArrayCatastro
                    Return Cs_Respuesta
                End If

            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Hubo un error al buscar la información, inténtelo nuevamente."
                Return Cs_Respuesta
            End Try

            'Array = Mdl_Catastro.Valida_Codigo_Catastro(clave, codigo)
            'Dim fechaExp As DateTime
            'fechaExp = Array(0)(5)
            'If (Array.Count > 0) Then
            '    If (fechaExp < Now) Then
            '        Cs_Respuesta.codigo = -1
            '        Cs_Respuesta.codigoError = 400
            '        Cs_Respuesta.mensaje = "El tiempo del codigo expiro, favor de generar uno nuevo."
            '        Return Cs_Respuesta
            '    End If
            '    If (codigo = Array(0)(3)) Then
            '        ArrayCatastro = Mdl_Catastro.traer_info_catastro(clave)
            '        Cs_Respuesta.codigo = 1
            '        Cs_Respuesta.codigoError = 200
            '        Cs_Respuesta.mensaje = "Correcto."
            '        Cs_Respuesta.Cs_InfoFolio = ArrayCatastro
            '        Return Cs_Respuesta
            '    Else
            '        Cs_Respuesta.codigo = -1
            '        Cs_Respuesta.codigoError = 400
            '        Cs_Respuesta.mensaje = "El codigo de validacion no es correcto, intentelo nuevamente."
            '        Return Cs_Respuesta
            '    End If
            'Else
            '    Cs_Respuesta.codigo = -1
            '    Cs_Respuesta.codigoError = 400
            '    Cs_Respuesta.mensaje = "La clave catastral no existe, intentelo nuevamente."
            '    Return Cs_Respuesta
            'End If

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

    End Class
End Namespace