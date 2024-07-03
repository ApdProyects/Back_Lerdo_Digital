#Disable Warning BC40056 ' El espacio de nombres o el tipo especificado en el 'System.IdentityModel.Tokens' Imports no contienen ningún miembro público o no se encuentran. Asegúrese de que el espacio de nombres o el tipo se hayan definido y de que contengan al menos un miembro público. Asegúrese de que el nombre del elemento importado no use ningún alias.
Imports System.IdentityModel.Tokens
#Enable Warning BC40056 ' El espacio de nombres o el tipo especificado en el 'System.IdentityModel.Tokens' Imports no contienen ningún miembro público o no se encuentran. Asegúrese de que el espacio de nombres o el tipo se hayan definido y de que contengan al menos un miembro público. Asegúrese de que el nombre del elemento importado no use ningún alias.
Imports System.Security.Cryptography
Imports JWT
Imports JWT.Algorithms
Imports JWT.Serializers
Imports System.Configuration

Public Class Cs_JWT

    Dim encrypt As String
    Public Function CrearPassword(longitud As Integer) As String

#Disable Warning BC42105 ' La función 'CrearPassword' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
    End Function
#Enable Warning BC42105 ' La función 'CrearPassword' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
    Public Function GenerateToken(CURP As Integer, TIPO_USER As Integer) As String
        Dim HMAC = New HMACSHA256()

        Dim payload As New Dictionary(Of String, Object)

        ' Step 2: add 4 entries.
        payload.Add("CURP", CURP)
        payload.Add("TIPO_USER", TIPO_USER)
        'payload.Add("sii", 1)


        Dim secret = ConfigurationManager.AppSettings("secret")


        Dim algorithm = New HMACSHA256Algorithm()
        Dim serializer = New JsonNetSerializer()
        Dim urlEncoder = New JwtBase64UrlEncoder()
        Dim Encoder = New JwtEncoder(algorithm, serializer, urlEncoder)

        Dim token = Encoder.Encode(payload, secret)
        Console.WriteLine(token)

        Return token

    End Function
    Public Function DecodeJwt(token As String) As Object
        Dim Json
        Try

            Dim serializer = New JsonNetSerializer()
            Dim Provider = New UtcDateTimeProvider()
            Dim validator = New JwtValidator(serializer, Provider)
            Dim urlEncoder = New JwtBase64UrlEncoder()
            Dim algorithm = New HMACSHA256Algorithm()
            Dim Decoder = New JwtDecoder(serializer, validator, urlEncoder, algorithm)
            Dim secret = ConfigurationManager.AppSettings("secret")
            Json = Decoder.Decode(token, secret, True)
            Console.WriteLine(Json)
        Catch ex As Exception

            Console.WriteLine("Token has expired")


        End Try
#Disable Warning BC42104 ' La variable 'Json' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
        Return Json
#Enable Warning BC42104 ' La variable 'Json' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
    End Function

End Class
