Imports System.IdentityModel.Tokens
Imports System.Security.Cryptography
Imports JWT
Imports JWT.Algorithms
Imports JWT.Serializers
Imports System.Configuration

Public Class Cs_JWT

    Dim encrypt As String
    Public Function CrearPassword(longitud As Integer) As String

    End Function
    Public Function GenerateToken(CURP As String, TIPO_USER As String) As String
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
        Return Json
    End Function

End Class
