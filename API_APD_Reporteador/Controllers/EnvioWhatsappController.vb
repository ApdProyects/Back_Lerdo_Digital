Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Security.Authentication
Imports System.Web.Http
Imports CDTS_APD_Reporteador

Namespace Controllers
    Public Class EnvioWhatsappController
        Inherits ApiController
        Dim PathConsumo As String = "https://www.waboxapp.com/"

        <HttpGet>
        <Route("api/EnvioWhatsapp/EnvioWhatsapp")>
        Public Function EnvioWhatsapp(ByVal Telefono As String, ByVal Codigo As String, ByVal mensaje As String, ByVal archivo As String)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim picture As Object = Nothing
            Dim documento As String = ""
            Dim url As String
            Dim json As Object
            Dim random As New Random()
            random.Next(1, 10000)
            Try
                If (archivo <> "") Then
                    Using webClient As WebClient = New WebClient
                        Const _Tls12 As SslProtocols = CType(&HC00, SslProtocols)
                        Const Tls12 As SecurityProtocolType = CType(_Tls12, SecurityProtocolType)
                        ServicePointManager.SecurityProtocol = Tls12
                        Dim data As Byte() = webClient.DownloadData("http://148.235.12.11/MesajesWhatsApp/" & archivo)
                        Dim base64String As String = Convert.ToBase64String(data, 0, data.Length)
                        'documento = "data:image/png;base64," & base64String
                        documento = base64String
                    End Using

                    Dim numAleatorio As New Random()
                    Dim valorAleatorio As Double = numAleatorio.Next(100000000, 999999999)
                    Dim valorFAleatorio As String = Now.ToString("dd-MM-yyyy_HHmmss")

                    My.Computer.FileSystem.CreateDirectory(HttpContext.Current.Request.PhysicalApplicationPath + "EvidenciasNET/" + valorFAleatorio + "/" + valorAleatorio.ToString() + "/")
                    Using stream As System.IO.FileStream = System.IO.File.Create(HttpContext.Current.Request.PhysicalApplicationPath + "EvidenciasNET/" + valorFAleatorio + "/" + valorAleatorio.ToString() + "/" + archivo)
                        Dim byteArray = System.Convert.FromBase64String(documento)
                        stream.Write(byteArray, 0, byteArray.Length)
                    End Using

                    Dim Tel = Telefono.Replace("(", "")
                    Tel = Tel.Replace(")", "")
                    Tel = Tel.Replace("-", "")
                    Tel = Tel.Replace(" ", "")
                    '   ENVIAR CHAT
                    url = PathConsumo + "api/send/chat?token=7e91ba222e7ff5ffdc12e56a6b5b700663bd892f8f866" +
                                    "&uid=5218711116125" +
                                    "&to=521" + Tel +
                                    "&custom_uid=PL-" + Codigo + "-" + Now().ToString("ddMMyyyyHHmmss") + random.ToString() +
                                    "&text=" + mensaje

                    Dim api As New WebClient()
                    api.Encoding = ASCIIEncoding.UTF8
                    Try
                        json = api.DownloadString(url)

                    Catch ex As Exception
                        Return False
                    End Try

                    '   DOCUMENTOS
                    Dim urlImagen = "https://apir.grupoapd.mx/" + "EvidenciasNET/" + valorFAleatorio + "/" + valorAleatorio.ToString() + "/" + archivo



                    'Dim urlImagen = "https://apir.grupoapd.mx/EvidenciasNET/01-12-2022_121653/255115409/APD_CAMBIO_NOMBRE_84.pdf" 'HttpContext.Current.Request.PhysicalApplicationPath + "EvidenciasNET/" + valorFAleatorio + "/" + valorAleatorio.ToString() + "/" + archivo
                    Dim caption = ""
                    Dim description = ""

                    'url = PathConsumo + "api/send/image?token=8d5ae81d9ef9a7da50c6deffa36e01e161eedf221569c" +
                    '                    "&uid=5218711116125" +
                    '                    "&to=521" + Tel +
                    '                    "&custom_uid=PL-IMG-" + Codigo + "-" + Now().ToString("ddMMyyyyHHmmss") +
                    '                    "&url=" + urlImagen +
                    '                    "&Caption=" + caption +
                    '                    "&description=" + description
                    url = PathConsumo + "api/send/image?token=7e91ba222e7ff5ffdc12e56a6b5b700663bd892f8f866" +
                            "&uid=5218711116125" +
                            "&to=521" + Tel +
                            "&custom_uid=PL-IMG-" + Codigo + "-" + Now().ToString("ddMMyyyyHHmmss") + random.ToString() +
                            "&url=" + urlImagen +
                            "&Caption=" + caption +
                            "&description=" + description +
                       "&url_thumb=C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg"


                    api = New WebClient()
                    api.Encoding = ASCIIEncoding.UTF8
                    Try
                        json = api.DownloadString(url)

                        Return json
                    Catch ex As Exception
                        Return False
                    End Try
                Else
                    Dim Tel = Telefono.Replace("(", "")
                    Tel = Tel.Replace(")", "")
                    Tel = Tel.Replace("-", "")
                    Tel = Tel.Replace(" ", "")
                    '   ENVIAR CHAT
                    url = PathConsumo + "api/send/chat?token=7e91ba222e7ff5ffdc12e56a6b5b700663bd892f8f866" +
                                    "&uid=5218711116125" +
                                    "&to=521" + Tel +
                                    "&custom_uid=PL-" + Codigo + "-" + Now().ToString("ddMMyyyyHHmmss") + random.ToString() +
                                    "&text=" + mensaje

                    Dim api As New WebClient()
                    api.Encoding = ASCIIEncoding.UTF8
                    Try
                        json = api.DownloadString(url)

                    Catch ex As Exception
                        Return False
                    End Try
                End If

                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Correcto."
                Cs_Respuesta.objetoError = json
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Usuario no válido." + ex.ToString()
            End Try

            Return Cs_Respuesta
        End Function
        Function Base64ToImage(ByVal base64string As String) As System.Drawing.Image
            'Setup image and get data stream together
            Dim img As System.Drawing.Image
            Dim MS As System.IO.MemoryStream = New System.IO.MemoryStream
            Dim b64 As String = base64string.Replace(" ", "+")
            Dim b() As Byte
            'Converts the base64 encoded msg to image data
            b = Convert.FromBase64String(b64)
            MS = New System.IO.MemoryStream(b)
            'creates image
            img = System.Drawing.Image.FromStream(MS)
            Return img
        End Function
    End Class
End Namespace