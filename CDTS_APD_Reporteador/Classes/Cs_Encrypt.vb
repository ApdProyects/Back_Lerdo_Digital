Imports System.Security.Cryptography

Public Class Cs_Encrypt

    Dim encrypt As String
    Public Function CrearPassword(longitud As Integer) As String
        Dim caracteres As String = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789"
        Dim res As New StringBuilder()
        Dim rnd As New Random()

        While 0 < System.Math.Max(System.Threading.Interlocked.Decrement(longitud), longitud + 1)
            res.Append(caracteres(rnd.[Next](caracteres.Length)))
        End While

        Return res.ToString()
    End Function
    Public Function Llamada_encriptar(ByVal Password As String)
        encrypt = ""
        encrypt = Encriptar(Password)

        Return encrypt
    End Function
    Public Function Llamada_desencriptar(ByVal Password As String)
        encrypt = ""
        encrypt = Desencriptar(Password)

        Return encrypt
    End Function

    Private Function Encriptar(ByVal Input As String) As String

        Dim IV() As Byte = ASCIIEncoding.ASCII.GetBytes("dpaopurg") 'La clave debe ser de 8 caracteres
        Dim EncryptionKey() As Byte = Convert.FromBase64String("rpaSPvIvVLlrcmtzPU9/c67Gkj7yL1S5") 'No se puede alterar la cantidad de caracteres pero si la clave
        Dim buffer() As Byte = Encoding.UTF8.GetBytes(Input)
        Dim des As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider
        des.Key = EncryptionKey
        des.IV = IV

        Return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length()))

    End Function

    Private Function Desencriptar(ByVal Input As String) As String

        Dim IV() As Byte = ASCIIEncoding.ASCII.GetBytes("dpaopurg") 'La clave debe ser de 8 caracteres
        Dim EncryptionKey() As Byte = Convert.FromBase64String("rpaSPvIvVLlrcmtzPU9/c67Gkj7yL1S5") 'No se puede alterar la cantidad de caracteres pero si la clave
        Dim buffer() As Byte = Convert.FromBase64String(Input)
        Dim des As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider
        des.Key = EncryptionKey
        des.IV = IV
        Return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))

    End Function

End Class
