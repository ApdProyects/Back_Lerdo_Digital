Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Namespace Controllers

    <EnableCors("*", "*", "*")>
    Public Class UsuariosController
        Inherits ApiController

        Dim Mdl_Usuarios As New Mdl_Usuarios
        Dim encrypt As New Cs_Encrypt
        Dim Cs_JWT As New Cs_JWT
        <HttpGet>
        Public Function index(ByVal folio As Integer)
            Return True
        End Function

        <HttpGet>
        <Route("api/auth/usuarios")>
        Public Function LoginUsuario(ByVal usuario As String, ByVal password As String)
            Dim Cs_Usuario_AC As New Cs_Usuario
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordE As String
#Disable Warning BC42024 ' Variable local sin usar: 'usuarioInt'.
            Dim usuarioInt
#Enable Warning BC42024 ' Variable local sin usar: 'usuarioInt'.
            Dim JWT As String
            Dim contr As String = encrypt.Llamada_desencriptar(password)
            passwordE = encrypt.Llamada_encriptar(password)

            If (String.IsNullOrEmpty(usuario)) Then
                usuario = ""
            End If



            If (String.IsNullOrEmpty(password)) Then
                password = ""
            End If

            Try
                Cs_Usuario_AC = Mdl_Usuarios.loginUsuario(usuario, password).FirstOrDefault
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Usuario no valido."

                Return Cs_Respuesta
            End Try

            Try




                If Not (String.IsNullOrEmpty(Cs_Usuario_AC.LUS_CONTRASENA)) Then

                    passwordE = encrypt.Llamada_encriptar(password)
                    JWT = Cs_JWT.GenerateToken(Cs_Usuario_AC.LUS_CLAVE, Cs_Usuario_AC.LRO_CLAVE)
                    Cs_Usuario_AC.TOKEN = JWT

                    Return Cs_Usuario_AC

                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "Usuario no registrado."

                    Return Cs_Respuesta
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50004
                Cs_Respuesta.mensaje = "Las contraseñas no coinciden, favor de verificarlas."

                Return Cs_Respuesta
            End Try

        End Function

        <HttpPost>
        <Route("api/auth/usuarios")>
        Public Function insertaUsuario(<FromBody()> ByVal data As Object)
            Dim Cs_Usuario_AC As New Cs_Usuario
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordE As String
            Dim da
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.
            passwordE = encrypt.Llamada_encriptar(data("contrasena").ToString())

            If (String.IsNullOrEmpty(data("usuario").ToString())) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Favor de llenar todos los campos."

                Return Cs_Respuesta
            End If



            If (String.IsNullOrEmpty(data("contrasena").ToString())) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Favor de llenar todos los campos."

                Return Cs_Respuesta
            End If
            If (String.IsNullOrEmpty(data("correo").ToString())) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Favor de llenar todos los campos."

                Return Cs_Respuesta
            End If
            If (String.IsNullOrEmpty(data("telefono").ToString())) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Favor de llenar todos los campos."

                Return Cs_Respuesta
            End If

            Try
                da = Mdl_Usuarios.registroUsuario(data("correo").ToString(), data("telefono").ToString(), data("usuario").ToString(), data("contrasena").ToString())

            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Usuario no creado."

                Return Cs_Respuesta
            End Try

            Return da
        End Function


    End Class
End Namespace