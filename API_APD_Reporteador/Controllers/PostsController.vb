Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models


Namespace Controllers

    <EnableCors("*", "*", "*")>
    Public Class PostsController
        Inherits ApiController

        Dim Mdl_Posts As New Mdl_Posts

        Dim Cs_JWT As New Cs_JWT

        Dim encrypt As New Cs_Encrypt



        <HttpGet>
        <Route("api/Posts/type/{type}")>
        Public Function RecuperaPosts(ByVal type As String)
            Dim da
            Dim Cs_Posts As New Cs_Posts
            Dim JWT As String


            Dim re = Request
            Dim Headers = re.Headers

            If (Headers.Contains("Authorization")) Then

                Dim token = Headers.GetValues("Authorization").First()
            Else
                Cs_Posts.codigo = -1
                Cs_Posts.codigoError = 401
                Cs_Posts.mensaje = "Usuario no autentico."
            End If
            Console.WriteLine("si")




#Disable Warning BC42104 ' La variable 'JWT' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
            Cs_JWT.DecodeJwt(JWT)
#Enable Warning BC42104 ' La variable 'JWT' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.


            If (String.IsNullOrEmpty(type)) Then
                type = ""
            End If

            Try
                da = Mdl_Posts.Recupera_posts(type)
            Catch ex As Exception
                Cs_Posts.codigo = -1
                Cs_Posts.codigoError = 50003
                Cs_Posts.mensaje = "Problema para conectar con el servidor inténtelo nuevamente."

                Return Cs_Posts
            End Try

            Return da
        End Function

    End Class
End Namespace