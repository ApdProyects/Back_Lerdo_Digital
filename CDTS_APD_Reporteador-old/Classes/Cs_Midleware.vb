Imports System.IdentityModel.Policy
Imports System.IdentityModel.Tokens
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters
Imports JWT
Imports JWT.Algorithms
Imports JWT.Serializers
Namespace Authentication
    Public Class Cs_Midleware
        Inherits FilterAttribute
        Implements IAuthorizationFilter







        Public Function auth()
            Dim re = New HttpRequestMessage()
            Dim Headers = re.Headers


            Console.WriteLine("si")

        End Function

        Public Function ExecuteAuthorizationFilterAsync(actionContext As HttpActionContext, cancellationToken As CancellationToken, continuation As Func(Of Task(Of HttpResponseMessage))) As Task(Of HttpResponseMessage) Implements IAuthorizationFilter.ExecuteAuthorizationFilterAsync
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace





