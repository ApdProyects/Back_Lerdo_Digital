Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models
Imports System.Web.Mail
Imports System.Net.Mail
Imports System.Globalization
Imports SelectPdf
Imports System.IO
Imports System.Net


Namespace Controllers

    Public Class CiudadanosController

        Inherits ApiController
        Dim Mdl_Facturacion As New Mdl_Facturacion
        Dim PathConsumo As String = "https://www.waboxapp.com/"

        Dim Cs_JWT As New Cs_JWT
        Dim Mdl_Ciudadanos As New Mdl_Ciudadanos
        Dim encrypt As New Cs_Encrypt
        Dim Mdl_Usuarios As New Mdl_Usuarios

        <HttpGet>
        Public Function index(ByVal folio As Integer)
            Return True
        End Function
        <HttpGet>
        <Route("api/Ciudadanos/LoginCiudadano")>
        Public Function LoginCiudadano(ByVal CURP As String, ByVal usuario As String, ByVal password As String)
            Dim Cs_Ciudadano_AC As New Cs_Ciudadano_AC
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordE As String
            Dim usuarioInt
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.

            passwordE = encrypt.Llamada_encriptar(password)

            If (String.IsNullOrEmpty(CURP)) Then
                CURP = ""
            End If

            If (String.IsNullOrEmpty(usuario)) Then
                usuarioInt = 0
            Else
                If (IsNumeric(usuario)) Then
                    usuarioInt = Convert.ToInt32(usuario).ToString()
                Else
                    usuarioInt = usuario
                End If
            End If

            If (String.IsNullOrEmpty(password)) Then
                password = ""
            End If

            Try
                Cs_Ciudadano_AC = Mdl_Ciudadanos.Recupera_Ciudadanos(CURP, usuarioInt, password).FirstOrDefault
                'generador de token 
                'JWT = Cs_JWT.GenerateToken(CURP, Cs_Ciudadano_AC.)


            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Usuario no válido."

                Return Cs_Respuesta
            End Try

            Try
                If Not (String.IsNullOrEmpty(Cs_Ciudadano_AC.ACI_PASSWORD)) Then

                    passwordE = encrypt.Llamada_encriptar(password)

                    If (passwordE = Cs_Ciudadano_AC.ACI_PASSWORD) Then
                        Cs_Ciudadano_AC.codigo = 1
                        Cs_Ciudadano_AC.mensaje = "Correcto"

                    Else
                        Cs_Ciudadano_AC.codigo = -1
                        Cs_Ciudadano_AC.codigoError = 400
                        Cs_Ciudadano_AC.mensaje = "Las contraseñas no coinciden."
                    End If

                    Return Cs_Ciudadano_AC

                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "El ciudadano no cuenta con un registro previo, favor de registrarse."

                    Return Cs_Respuesta
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50004
                Cs_Respuesta.mensaje = "Las contraseñas no coinciden."

                Return Cs_Respuesta
            End Try

        End Function
        <HttpGet>
        <Route("api/Ciudadanos/RetornarCiudadano")>
        Public Function RecuperaCiudadano(ByVal CURP As String)
            Dim Cs_Ciudadano_AC As New Cs_Ciudadano_AC
            Dim Cs_Respuesta As New Cs_Respuesta

            If (String.IsNullOrEmpty(CURP)) Then
                CURP = ""
            End If

            Try
                Cs_Ciudadano_AC = Mdl_Ciudadanos.Recupera_Ciudadanos(CURP, "0", "").FirstOrDefault
            Catch ex As Exception
                Cs_Ciudadano_AC.codigo = -1
                Cs_Ciudadano_AC.codigoError = 50003
                Cs_Ciudadano_AC.mensaje = "Usuario no válido."

                Return Cs_Ciudadano_AC
            End Try

            Return Cs_Ciudadano_AC
        End Function
        <HttpGet>
        <Route("api/Ciudadanos/RegistraCiudadano")>
        Public Function RegistraCiudadano(ByVal CURP As String)
            Dim Cs_Ciudadano_AC As New Cs_Ciudadano_AC
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordG As String
            Dim passwordE As String
            Dim Mensaje As String
            Dim Telefono As String

            Try
                Cs_Ciudadano_AC = Mdl_Ciudadanos.Recupera_Ciudadanos(CURP, 0, "").FirstOrDefault
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50002
                Cs_Respuesta.mensaje = "Usuario no válido."

                Return Cs_Respuesta
            End Try

            Try
                If Not (String.IsNullOrEmpty(Cs_Ciudadano_AC.ACI_CURP) And CURP = Cs_Ciudadano_AC.ACI_CURP) Then
                    If Not (String.IsNullOrEmpty(Cs_Ciudadano_AC.ACI_CELULAR)) Then

                        passwordG = encrypt.CrearPassword(6)
                        passwordE = encrypt.Llamada_encriptar(passwordG)

                        Cs_Respuesta = Mdl_Ciudadanos.Mod_Pass_Ciudadanos(CURP, passwordE)

                        Mensaje = "REGISTRO EXITOSO. " +
                                   " Usuario: " + Cs_Ciudadano_AC.ACI_CLAVE.ToString("000000") + "." +
                                   " Clave: " + passwordG.ToString() + "."

                        Telefono = Cs_Ciudadano_AC.ACI_CELULAR
                        Telefono = Replace(Telefono, "(", "")
                        Telefono = Replace(Telefono, ")", "")
                        Telefono = Replace(Telefono, "-", "")
                        Telefono = Replace(Telefono, " ", "")

                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.mensaje = "Correcto"

                        Return Cs_Respuesta
                    Else
                        Cs_Respuesta.codigo = -1
                        Cs_Respuesta.codigoError = 400
                        Cs_Respuesta.mensaje = "El CURP ingresado no cuenta con un teléfono asignado, favor pasar a las oficinas."

                        Return Cs_Respuesta
                    End If
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "El CURP ingresado no cuenta con un registro de ciudadano, favor pasar a las oficinas."

                    Return Cs_Respuesta
                End If

            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50001
                Cs_Respuesta.mensaje = "Usuario no válido."

                Return Cs_Respuesta
            End Try

        End Function
        <HttpGet>
        <Route("api/Ciudadanos/LoginUsuario")>
        Public Function LoginUsuario(ByVal usuario As String, ByVal password As String)
            Dim Cs_Usuario_AC As New Cs_Usuario
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordE As String
            Dim JWT As String
            passwordE = encrypt.Llamada_encriptar(password)

            If (String.IsNullOrEmpty(usuario)) Then
                usuario = ""
            End If



            If (String.IsNullOrEmpty(password)) Then
                password = ""
            End If

            Try
                Cs_Usuario_AC = Mdl_Usuarios.loginUsuario_NEW(Cs_Usuario_AC, usuario, passwordE)
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Usuario no válido."

                Return Cs_Respuesta
            End Try

            Try
                If Not (Cs_Usuario_AC Is Nothing) Then

                    passwordE = encrypt.Llamada_encriptar(password)
                    JWT = Cs_JWT.GenerateToken(Cs_Usuario_AC.LUS_CLAVE, Cs_Usuario_AC.LRO_CLAVE)
                    Cs_Usuario_AC.TOKEN = JWT

                    If passwordE = Cs_Usuario_AC.LUS_CONTRASENA Then
                        Return Cs_Usuario_AC
                    Else
                        Cs_Respuesta.codigo = -1
                        Cs_Respuesta.codigoError = 400
                        Cs_Respuesta.mensaje = "Las contraseñas no coinciden, intentelo nuevamente."
                        Return Cs_Respuesta
                    End If
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "Cuenta inexistente en plataforma LERDO DIGITAL, favor de crearla."

                    Return Cs_Respuesta
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50004
                Cs_Respuesta.mensaje = "Error al validar el usuario."

                Return Cs_Respuesta
            End Try
#Disable Warning BC42105 ' La función 'LoginUsuario' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
        End Function
#Enable Warning BC42105 ' La función 'LoginUsuario' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
        <HttpGet>
        <Route("api/Ciudadanos/insertaUsuario")>
        Public Function insertaUsuario(ByVal correo As String, ByVal telefono As String, ByVal usuario As String, ByVal contrasena As String)
            Dim Cs_Usuario_AC As New Cs_Usuario
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim passwordE As String
            Dim da
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.

            passwordE = encrypt.Llamada_encriptar(contrasena)

            If (String.IsNullOrEmpty(usuario)) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parametro requerido usuario."

                Return Cs_Respuesta
            End If



            If (String.IsNullOrEmpty(contrasena)) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parametro requerido contrasena."

                Return Cs_Respuesta
            End If
            If (String.IsNullOrEmpty(correo)) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parametro requerido correo."

                Return Cs_Respuesta
            End If
            If (String.IsNullOrEmpty(telefono)) Then

                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parametro requerido telefono."

                Return Cs_Respuesta
            End If

            Try
                'Cs_Usuario_AC = Mdl_Usuarios.verifiedUsuario(correo, telefono).FirstOrDefault

                'If (String.IsNullOrEmpty(Cs_Usuario_AC.LUS_CONTRASENA)) Then
                If (Mdl_Usuarios.verifiedUsuario(correo, telefono)) Then

                    da = Mdl_Usuarios.registroUsuario(correo, telefono, usuario, passwordE)


                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "Ya existe un usuario registrado con ese correo o telefono, favor de verificar su información."

                    Return Cs_Respuesta
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50004
                Cs_Respuesta.mensaje = "Usuario no válido."

                Return Cs_Respuesta
            End Try

            Try


            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Error al Crear Usuario."

                Return Cs_Respuesta
            End Try

            Return da
        End Function
        <HttpGet>
        <Route("api/prueba/ftp")>
        Public Function ftp()
            Dim htmlString As String = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
                                    <head>
                                        <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
                                        <meta content='width=device-width' name='viewport'>
                                        <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                                        <title></title>
                                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3' crossorigin='anonymous'> 
                                    </head>
                                    <body style='font-family: Arial, Verdana;'> 
                                        <div class='container-fluid'>
                                            <div class='row'>
                                                <div class='col-3 text-center' style='padding-top:30px;'>
                                                    <img alt='Lerdo Digital' src='https://apir.grupoapd.mx/img/lerdologo.jpeg' width='200px'>
                                                </div>
                                                <div class='col-6 text-center' style='padding-top:50px;'>
                                                    <font face='Arial' style='font-size: 22px;'><b>PRESIDENCIA MUNICIPAL LERDO DURANGO</b></font><br> 
                                                    <font face='Arial' size='3'><b>TESORERÍA MUNICIPAL</b></font><br>  
                                                    <font face='Arial' size='3'>AV. FRANCISCO SARABIA NO.3 NTE<br>COL. CENTRO, CP: 35150, TEL: 871 175 0000 <br> R.F.C PMC951010FE3</font>
                                                    <br><br>
                                                    <font face='Arial' size='5'><b>R&nbsp;&nbsp;E&nbsp;&nbsp;C&nbsp;&nbsp;I&nbsp;&nbsp;B&nbsp;&nbsp;O&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                   D&nbsp;&nbsp;E&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                   P&nbsp;&nbsp;A&nbsp;&nbsp;G&nbsp;&nbsp;O</b></font><br><br>
                                                </div>
                                                <div class='col-3 text-center' style='padding-top:50px;'>
                                                    <img alt='APD' src='https://apir.grupoapd.mx/img/apdlogo.png' width='190px'><br>
                                                   
                                                </div>
                                            </div>
                                            <div class='row'>
                                                <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>NOMBRE:</font><br>  
                                                    <font face='Arial' size='2'>DOMICILIO:</font><br>
                                                    <font face='Arial' size='2'>ACIVIDAD:</font>
                                                </div>
                                                <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'></font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'><b>VALOR CATASTRAL </b></font>
                                                </div>
                                                <div class='col-1' style='padding-top: 3px;'>
                                                    <font face='Arial' size='2'>FECHA:</font><br>  
                                                    <font face='Arial' size='2'>R.F.C:</font><br>
                                                    <font face='Arial' size='2'>CVE CTR:</font>
                                                </div>
                                                <div class='col-2'>
                                                    <font face='Arial' size='2'></font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                            </div>
                                            <br>
                                            <div class='row'>
                                                <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
                                                    <img alt='SAT' src='https://apir.grupoapd.mx/img/sat.jpg' width='120px'>
                                                </div>
                                                <div class='col-9' style='padding-left: 5px;'>
                                                    <table class='table' style='border: 1px solid white;' background='https://apir.grupoapd.mx/img/lerdologoMA.jpeg'> 
                                                        <tbody>
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBTOTAL</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>DESCUENTO</b></td>
                                                            <td class='text-center' style='font-size: 13px;'><b>TOTAL</b></td>
                                                          </tr>
                                                          <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
                                                            <td style='font-size: 12px; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right;'></td>
                                                         </tr>

                                                          <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
                                                            <td style='font-size: 12px; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top:35px;'></td>
                                                          </tr>
                                                          <tr> 
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 12px; padding-top: 15px;'>( TC 454)</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'></td>
                                                          </tr> 
                                                          </tr>
                                                        </tbody>
                                                    </table>
                                                </div>       
                                                <div class='col-1'>
                                                    <div style='writing-mode: vertical-lr; transform: rotate(270deg); width: 200px; height: 250px; padding-top: 70px;'>
                                                        <font face='Arial' style='font-size: 17px;'>Pago realizado en el portal <b>https://www.lerdodigital.mx</b></font>
                                                    </div>
                                                </div>
                                                <div class='col-3' style='padding-left: 50px; padding-top: 40px;'> 
                                                    <font face='Arial' size='3'><b>CANTIDAD CON LETRA:</b></font><br>  
                                                </div>
                                                <div class='col-1'> 
                                                    <font face='Arial' style='font-size: 10px;'><b>Nota Importante:</b></font><br>  
                                                </div>
                                                <div class='col-8' style='padding-right: 45px;'>
                                                    <font face='Arial' style='font-size: 11px;'></font><br> 
                                                    <font face='Arial' style='font-size: 11px;'><b>LA REPRODUCCIÓN APÓCRIFA DE ESTE COMPROBANTE, CONSTITUYE UN DELITO EN LOS TÉRMINOS DE</b></font>  
                                                    <font face='Arial' style='font-size: 11px;'><b>LAS DISPOSICIÓNES FISCALES.</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <font face='Arial' style='font-size: 11px;'><b>PAGO HECHO EN UNA SOLA EXHIBICIÓN.</b></font>
                                                </div> 
                                                <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
                                                    <div style='border: 2px solid black; width: 100%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
                                                       <font face='Arial' style='font-size: 14px;'>100 M.N.</font> 
                                                    </div>
                                                    <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio </b></font>  
                                                </div>
                                                 <div class='row'>
                                                <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
                                                      <img src= class='rounded float-start' alt='QR' width='90px'height='90px'>
                                                </div> 
                                            </div>
                                                <div class='col-3 text-center' style='padding-top:10px;'>
                                                    <img src='data:image/jpg;base64, ' alt='Codigo barras' height='35px'/>
                                                </div>
                                            </div> 
                                          </div>
                                    </body>
                                </html>"
            Dim pdf_page_size As String = "A4"
            Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize),
                    pdf_page_size, True), PdfPageSize)
            Dim pdf_orientation As String = "Portrait"
            Dim pdfOrientation As PdfPageOrientation = DirectCast(
                    [Enum].Parse(GetType(PdfPageOrientation),
                    pdf_orientation, True), PdfPageOrientation)
            Dim webPageWidth As Integer = 1024
            Try
                webPageWidth = Convert.ToInt32(1024)
            Catch
            End Try
            Dim webPageHeight As Integer = 0
            Try
                webPageHeight = Convert.ToInt32(768)
            Catch
            End Try
            Dim converter As New HtmlToPdf()
            converter.Options.PdfPageSize = pageSize
            converter.Options.PdfPageOrientation = pdfOrientation
            converter.Options.WebPageWidth = webPageWidth
            converter.Options.WebPageHeight = webPageHeight
            Dim doc As PdfDocument = converter.ConvertHtmlString(htmlString)
            Dim memoryStream As New MemoryStream()
            doc.Save(memoryStream)
            Dim ftpServerIP As String = "ftp://138.197.195.24/cut.apdacademics.mx"
            Dim ftpUserID As String = "apdacademics"
            Dim ftpPassword As String = "555Ctu1#i"


            Dim ftpPath As String = "/example.pdf"

            Dim request As FtpWebRequest = DirectCast(WebRequest.Create(ftpServerIP & ftpPath), FtpWebRequest)

            request.Credentials = New NetworkCredential(ftpUserID, ftpPassword)


            request.Method = WebRequestMethods.Ftp.UploadFile
            request.Credentials = New NetworkCredential("apdacademics", "555Ctu1#i")
            Using ftpStream As Stream = request.GetRequestStream
                memoryStream.WriteTo(ftpStream)
                memoryStream.Close()
            End Using
            doc.Close()
            Return True
        End Function
        <HttpGet>
        <Route("api/Ciudadanos/Recuperapassword")>
        Public Function Recuperapassword(ByVal telefono As String)
            Dim Cs_Usuario_AC As New Cs_Usuario
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim JWT As String
            Dim mensaje As String
            Dim passwordE As String

            If (String.IsNullOrEmpty(telefono)) Then
                telefono = ""
            End If

            Try
                Cs_Usuario_AC = Mdl_Usuarios.Recuperapassword(telefono).FirstOrDefault
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Telefono no válido."

                Return Cs_Respuesta
            End Try

            Try
                If Not (String.IsNullOrEmpty(Cs_Usuario_AC.LUS_CONTRASENA)) Then

                    JWT = Cs_JWT.GenerateToken(Cs_Usuario_AC.LUS_CLAVE, Cs_Usuario_AC.LRO_CLAVE)
                    Cs_Usuario_AC.TOKEN = JWT
                    passwordE = encrypt.Llamada_desencriptar(Cs_Usuario_AC.LUS_CONTRASENA)

                    mensaje = "Recuperacion de informacion para ingresar al portal Lerdo Digital. " +
                                   " Correo: " + Cs_Usuario_AC.LUS_CORREO + ", " +
                                   " clave: " + passwordE + ""

                    Try
                        ' Cs_Respuesta = envio_sms(mensaje, Cs_Usuario_AC.LUS_TELEFONO) ' Cs_Usuario_AC.LUS_TELEFONO
                        Cs_Respuesta = EnvioWhatsapp(Cs_Usuario_AC.LUS_TELEFONO, mensaje)
                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.codigoError = 200
                        Cs_Respuesta.mensaje = "Los datos se recuperaron correctamente, en breve recibirá un Whatsapp con sus datos de acceso."
                    Catch ex As Exception
                        Cs_Respuesta = New Cs_Respuesta
                        Cs_Respuesta.codigo = -1
                        Cs_Respuesta.codigoError = 408
                        Cs_Respuesta.mensaje = "Por el momento el sistema de recuperación se encuentra suspendido, favor de intentar más tarde."
                    End Try

                    Return Cs_Respuesta
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "El Usuario no cuenta con un registro previo, favor de registrarse."

                    Return Cs_Respuesta
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 402
                Cs_Respuesta.mensaje = "El Usuario no cuenta con un registro previo, favor de registrarse."

                Return Cs_Respuesta
            End Try

        End Function
        'Public Function ConvertFileToBase64(ByVal fileName As String) As String
        '    Return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName))
        'End Function
        <HttpGet>
        <Route("api/verifica/Qr")>
        Public Function verifica_qr(ByVal iin_clave As String)
            Dim Cs_Datos_QR As New Cs_Datos_QR
            Try

                Cs_Datos_QR = Mdl_Ciudadanos.verifica_qr(iin_clave)
                Cs_Datos_QR.codigo = 200
                Cs_Datos_QR.mensaje = "QR VALIDO"


            Catch ex As Exception
                Cs_Datos_QR.codigo = -1
                Cs_Datos_QR.codigoError = 50003
                Cs_Datos_QR.mensaje = "QR NO VALIDO"
                Cs_Datos_QR.objetoError = ex
                Return Cs_Datos_QR
            End Try

            Return Cs_Datos_QR
        End Function

        Public Function ConvertFileToBase64(ByVal fileName As String) As String
            Return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName))
        End Function

        Public Function EnvioWhatsapp(ByVal Telefono As String, ByVal mensaje As String)
            Dim Cs_Respuesta As New Cs_Respuesta

            Dim url As String
            Dim json As Object
            Dim random As New System.Random()
            Dim Codigo As String = random.Next(1000, 10000).ToString()

            Dim SentSQL As String
            Dim resultado As String = ""
            Dim Resultado_envio As String = ""

            Try
                '/* CORRECION DE TOKEN*/
                SentSQL = "SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].APD_16_FUNCIONALIDAD_ESTRUCTURAS WHERE AFE_CLAVE = 5"
                url = Mdl_Facturacion.RECUPERA_VALOR_STRING(SentSQL)
                Dim TOKEN_NUMERO As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT (AFE_HTML_INTERACION1 +'-,-'+ AFE_HTML_INTERACION2) AS TOKEN_TELOUT FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].APD_16_FUNCIONALIDAD_ESTRUCTURAS WHERE AFE_CLAVE = 5")
                Dim TOKEN_TEL() As String = Split(TOKEN_NUMERO, "-,-")
                If url.Contains("https") = True Then
                    Dim Tel = Telefono.Replace("(", "")
                    Tel = Tel.Replace(")", "")
                    Tel = Tel.Replace("-", "")
                    Tel = Tel.Replace(" ", "")

                    url = url.Replace("[TOKEN]", TOKEN_TEL(0).ToString())
                    url = url.Replace("[TELEFONO_SALIDA]", TOKEN_TEL(1).ToString())
                    url = url.Replace("[TELEFONO_DESTINO]", Tel)
                    url = url.Replace("[CODIGO]", Codigo)
                    url = url.Replace("[TIEMPO]", Now().ToString("ddMMyyyyHHmmss"))
                    url = url.Replace("[MENSAJE]", mensaje)

                    'Dim oRequest As WebRequest = WebRequest.Create(url)
                    'Dim oResponse As WebResponse = oRequest.GetResponse()
                    'Dim sr As StreamReader = New StreamReader(oResponse.GetResponseStream())

                    Dim api As New WebClient()
                    api.Encoding = ASCIIEncoding.UTF8
                    Try
                        json = api.DownloadString(url)

                    Catch ex As Exception
                        Return False
                    End Try
                    Cs_Respuesta.codigo = 1
                    Cs_Respuesta.codigoError = 200
                    Cs_Respuesta.mensaje = "MENSAJE DE RECUPERACION ENVIADA AL NUMERO:" + Tel + " CORRECTAMENTE"
                    Cs_Respuesta.objetoError = json
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "SIN ACCESO A LA BASE DE DATOS."
                End If


                'Dim Tel = Telefono.Replace("(", "")
                'Tel = Tel.Replace(")", "")
                'Tel = Tel.Replace("-", "")
                'Tel = Tel.Replace(" ", "")
                ''   ENVIAR CHAT
                'url = PathConsumo + "api/send/chat
                '                    ?token=7e91ba222e7ff5ffdc12e56a6b5b700663bd892f8f866" +
                '                    "&uid=5218711116125" +
                '                    "&to=521" + Tel +
                '                    "&custom_uid=PL-" + Codigo + "-" + Now().ToString("ddMMyyyyHHmmss") +
                '                    "&text=" + mensaje
                'Dim api As New WebClient()
                'api.Encoding = ASCIIEncoding.UTF8
                'Try
                '    json = api.DownloadString(url)

                'Catch ex As Exception
                '    Return False
                'End Try


                'Cs_Respuesta.codigo = 1
                'Cs_Respuesta.codigoError = 200
                'Cs_Respuesta.mensaje = "MENSAJE DE RECUPERACION ENVIADA AL NUMERO:" + Tel + " CORRECTAMENTE"
                'Cs_Respuesta.objetoError = json
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "MENSAJE DE RECUPERACION NO VALIDO." + ex.ToString()
            End Try

            Return Cs_Respuesta
        End Function









        <HttpGet>
        <Route("api/Ciudadanos/RecuperaRecibo")>
        Public Function RecuperaRecibo(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal correo As String)
#Disable Warning BC42024 ' Variable local sin usar: 'da'.
            Dim da
#Enable Warning BC42024 ' Variable local sin usar: 'da'.
            Dim Cs_Multas As New Cs_Multas
            Dim Cs_Ingreso As New Cs_Ingreso
            Dim Cs_MultasDet As New List(Of Cs_MultasDet)
#Disable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim JWT As String
#Enable Warning BC42024 ' Variable local sin usar: 'JWT'.
            Dim Mdl_InfoFolio As New Mdl_InfoFolio
            Dim Html As String

            Dim re = Request
            Dim Headers = re.Headers

            Try
                If (id_depto = 2) Then
                    Cs_Multas = Mdl_InfoFolio.RecuperaMultas(id_folio)
                    Cs_MultasDet = Mdl_InfoFolio.RecuperaMultasDet(Cs_Multas.VMU_CLAVE)

                    Cs_Multas.MultasDet = Cs_MultasDet

                    'Dim bs As New BarcodeSettings()
                    'bs.Type = BarCodeType.Code39
                    'bs.Data = "12345"
                    'Dim bg As New BarCodeGenerator(bs)
                    'bg.GenerateImage().Save("Code39Code.png")
                    'Process.Start("Code39Code.png")

                    Html = GenerarHTML(Cs_Multas, "")
                Else
                    Cs_Ingreso = Mdl_InfoFolio.RecuperaInfoIngresos(id_folio, id_concepto, id_depto)
                    Html = GenerarHTML_nv(Cs_Ingreso)
                End If
            Catch ex As Exception
                Cs_Multas.codigo = -1
                Cs_Multas.codigoError = 50003
                Cs_Multas.mensaje = "No se encontró el folio."

                Return Cs_Multas
            End Try

            Try
                If Not File.Exists(HttpContext.Current.Request.PhysicalApplicationPath + "Comprobantes\" & id_folio & ".pdf") Then
                    Dim htmlString As String = Html
                    Dim pdf_page_size As String = "A4"
                    Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize),
                            pdf_page_size, True), PdfPageSize)
                    Dim pdf_orientation As String = "Portrait"
                    Dim pdfOrientation As PdfPageOrientation = DirectCast(
                            [Enum].Parse(GetType(PdfPageOrientation),
                            pdf_orientation, True), PdfPageOrientation)
                    Dim webPageWidth As Integer = 1024
                    Try
                        webPageWidth = Convert.ToInt32(1024)
                    Catch
                    End Try
                    Dim webPageHeight As Integer = 0
                    Try
                        webPageHeight = Convert.ToInt32(768)
                    Catch
                    End Try

                    Dim converter As New HtmlToPdf()
                    converter.Options.PdfPageSize = pageSize
                    converter.Options.PdfPageOrientation = pdfOrientation
                    converter.Options.WebPageWidth = webPageWidth
                    converter.Options.WebPageHeight = webPageHeight
                    Dim doc As PdfDocument = converter.ConvertHtmlString(htmlString)
                    doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Comprobantes\" & id_folio & ".pdf")
                    doc.Close()
                End If

                Dim bool As Boolean = ENVIAR_CORREO(id_folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Comprobantes\" & id_folio & ".pdf")

                Cs_Multas.PDF = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Request.PhysicalApplicationPath + "Comprobantes\" & id_folio & ".pdf"))

                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Correcto"

                Return Cs_Multas

            Catch ex As Exception
                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Error al enviar el correo, inténtelo nuevamente."

                Return Cs_Multas
            End Try
        End Function
        <HttpGet>
        <Route("api/Ciudadanos/RecuperaManual")>
        Public Function RecuperaManual()
            Dim Cs_Multas As New Cs_Multas

            Try
                Cs_Multas.PDF = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Request.PhysicalApplicationPath + "manuales\MANUAL PAGOS LERDO DIGITAL.pdf"))

                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Correcto"

                Return Cs_Multas

            Catch ex As Exception
                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Error al recuperar manual, inténtelo nuevamente."

                Return Cs_Multas
            End Try
        End Function
        <HttpGet>
        <Route("api/Ciudadanos/RecuperaManualfacturacion")>
        Public Function RecuperaManualfacturacion()
            Dim Cs_Multas As New Cs_Multas

            Try
                Cs_Multas.PDF = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Request.PhysicalApplicationPath + "manuales\MANUAL FACTURACION LERDO DIGITAL.pdf"))

                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Correcto"

                Return Cs_Multas

            Catch ex As Exception
                Cs_Multas.codigo = 1
                Cs_Multas.codigoError = 200
                Cs_Multas.mensaje = "Error al recuperar manual, inténtelo nuevamente."

                Return Cs_Multas
            End Try
        End Function
        <HttpGet>
        <Route("api/correo_informe/apdacademics")>
        Public Function ENVIA_MAIL_apdacademics(ByVal EMAIL_CUERPO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_ENCABEZADO As String) As Boolean
            Try


                Dim htmlprueba As String
                htmlprueba = "<!DOCTYPE html>
<html>
  <head>
    <meta charset=" + "utf-8" + ">
    <title>Mi pagina de prueba</title>
  </head>
  <body>
    <p>prueba de la api>
    <img src=" + "images/firefox-icon.png" + "alt=" + "Mi imagen de prueba>
  </body>
</html>"
                Dim fromaddr As String = "informes@apdacademics.mx"
                Dim toaddr As String = EMAIL_DESTINATARIO
                Dim password As String = "dc1*h822L"

                EMAIL_CUERPO = htmlprueba

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "SOLICITUD DE INFORMES " + EMAIL_ENCABEZADO + " APDACADEMICS"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                '   msg.Attachments.Add(New Attachment(archivopdf))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                smtp.Host = "apdacademics.mx"
                smtp.Port = 465
                smtp.UseDefaultCredentials = False
                smtp.EnableSsl = True
                Dim nc As New System.Net.NetworkCredential(fromaddr, password)
                smtp.Credentials = nc
                smtp.Send(msg)
                Return True
            Catch ex As Exception
                Return False
            End Try
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
        Public Function GenerarHTML(Cs_Multas As Cs_Multas, img As String) As String
            Dim HTML As String = ""

            HTML += "<!DOCTYPE html> "
            HTML += "<html xmlns ='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>"
            HTML += "<head>"
            HTML += "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'/>"
            HTML += "<meta content='width=device-width' name='viewport'/>"
            HTML += "<meta content='IE=edge' http-equiv='X-UA-Compatible'/>"
            HTML += "<title></title>"
            HTML += "<style type='text/css'>"
            HTML += "body {"
            HTML += "margin: 0;"
            HTML += "padding: 0;"
            HTML += "}"
            HTML += "table,"
            HTML += "td,"
            HTML += "tr {"
            HTML += "vertical-align: top;"
            HTML += "border-collapse: collapse;"
            HTML += "}"
            HTML += "* {"
            HTML += "line-height: inherit;"
            HTML += "}"
            HTML += "a[x-apple-data-detectors=true] {"
            HTML += "color: inherit !important;"
            HTML += "text-decoration: none !important;"
            HTML += "}"
            HTML += "</style>"
            HTML += "<style id='media-query' type='text/css'>"
            HTML += "@media (max-width: 720px) {"
            HTML += ".block-grid,"
            HTML += ".col {"
            HTML += "min-width: 320px !important;"
            HTML += "max-width: 100% !important;"
            HTML += "display: block !important;"
            HTML += "}"
            HTML += ".block-grid {"
            HTML += "width: 100% !important;"
            HTML += "}"
            HTML += ".col {"
            HTML += "width: 100% !important;"
            HTML += "}"
            HTML += ".col>div {"
            HTML += "margin: 0 auto;"
            HTML += "}"
            HTML += "img.fullwidth,"
            HTML += "img.fullwidthOnMobile {"
            HTML += "max-width: 100% !important;"
            HTML += "}"
            HTML += ".no-stack .col {"
            HTML += "min-width: 0 !important;"
            HTML += "display: table-cell !important;"
            HTML += "}"
            HTML += ".no-stack.two-up .col {"
            HTML += "width: 50% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num4 {"
            HTML += "width: 33% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num8 {"
            HTML += "width: 66% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num4 {"
            HTML += "width: 33% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num3 {"
            HTML += "width: 25% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num6 {"
            HTML += "width: 50% !important;"
            HTML += "}"
            HTML += ".no-stack .col.num9 {"
            HTML += "width: 75% !important;"
            HTML += "}"
            HTML += ".video-block {"
            HTML += "max-width: none !important;"
            HTML += "}"
            HTML += ".mobile_hide {"
            HTML += "min-height: 0px;"
            HTML += "max-height: 0px;"
            HTML += "max-width: 0px;"
            HTML += "display: none;"
            HTML += "overflow: hidden;"
            HTML += "font-size: 0px;"
            HTML += "}"
            HTML += ".desktop_hide {"
            HTML += "display: block !important;"
            HTML += "max-height: none !important;"
            HTML += "}"
            HTML += "}"
            HTML += "</style>"
            HTML += "</head>"
            HTML += "<body Class='clean-body' style='margin: 0; padding: 0; -webkit-text-size-adjust: 100%; background-color: #fbfbfb;'>"
            HTML += "<Table bgcolor ='#fbfbfb' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='table-layout: fixed; vertical-align: top; min-width: 320px; Margin: 0 auto; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fbfbfb; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top;' valign='top'>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>"
            HTML += "<img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_p6nto0_amibzm.jpg'"
            HTML += "style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'/>"
            HTML += "</div></div></div></div>"
            HTML += "<div class='col num6' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 348px; width: 350px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 45px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>"
            HTML += "<table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #0C0C0C; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>"
            HTML += "</tr></tbody></table></td></tr></tbody></table>"
            HTML += "<div style='color:#555555;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:5px;padding-right:10px;padding-bottom:0px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #555555; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 18px; line-height: 1.2; word-break: break-word; text-align: center; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 22px; margin: 0;'><span style='font-size: 18px;'><strong>LerdoMX VIALIDAD</strong></span></p>"
            HTML += "</div></div>"
            HTML += "<table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 10px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>"
            HTML += "<table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #0B0B0B; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>"
            HTML += "</tr></tbody></table></td></tr></tbody></table></div></div></div>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>"
            HTML += "<div style='font-size:1px;line-height:25px'> </div><img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_2_tqrpf0_v43whg.png'"
            HTML += "style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'/>"
            HTML += "</div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Fecha</strong>:</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 17px; margin: 0;'><strong><span style='font-size: 17px; mso-ansi-font-size: 18px;'>Folio:</span></strong></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Propietario</strong>:</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Lugar</strong>:</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 17px; margin: 0;'><strong><span style='font-size: 17px; mso-ansi-font-size: 18px;'>Placas:</span></strong></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Procedencia</strong>:</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Garantía</strong>:</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Referencia</strong>:</span></p>"
            HTML += "</div></div></div></div></div>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 900px; width: 900px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'>" + Cs_Multas.VMU_INFRACCION_FECHA + "</span></p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:15px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VMU_FOLIO + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:15px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grand11e, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.PROPIETARIO + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VMU_INFRACCION_LUGAR + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:11px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VMU_PLACAS + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:15px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VMU_PROCEDENCIA + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:15px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VGR_NOMBRE + "</p>"
            HTML += "</div></div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Multas.VMU_GARANTIA_REFERENCIA + "</p>"
            HTML += "</div></div></div></div></div>"
            HTML += "</div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #4ba7ff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#4ba7ff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:0px; padding-bottom:0px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='font-size:16px;text-align:center;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif'>"
            HTML += "<table style='width:100%;background:white'>"
            HTML += "<thead style='background:white;border-bottom:2px solid black'>"
            HTML += "<tr style='border-bottom:1px solid black'>"
            HTML += "<th style='border-bottom:1px solid black'> Folio  </th>"
            HTML += "<th colspan='8' style='text-align:left;border-bottom:1px solid black'>Descripcion</th>"
            HTML += "<th style='text-align:left;border-bottom:1px solid black'>Importe</th>"
            HTML += "</tr></thead><tbody>"
            For Each c In Cs_Multas.MultasDet
                HTML += "<tr style='padding-top:15px; padding-bottom: 15px;'><td>" + c.VIN_NOMBRE + "</td>"
                HTML += "<td colspan='8' style='text-align:left'>" + c.VIN_DESCRIPCION + "</td>"
                HTML += "<td style='text-align:left'>$ " + FormatNumber(c.VMF_IMPORTE_MIXIMO, 2) + "</td></tr>"
            Next
            HTML += "</tbody><tfoot><tr>"
            HTML += "<td style='border-top:1px solid black'></td>"
            HTML += "<td class='PrecioTotal' colspan='8' style='border-top:1px solid black;text-align:right;'> Total </td>"
            HTML += "<td class='CantidadTotal' style='border-top:1px solid black'>$ " + FormatNumber(Cs_Multas.VMU_IMPORTE, 2) + "</td>"
            HTML += "</tr></tfoot></table></div></div></div></div></div></div></div>"
            HTML += "<br><br>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #f4f4f4;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#f4f4f4;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:25px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 20px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 24px; margin: 0;'><span style='font-size: 20px;'>SON (" + UCase(Letras(FormatNumber(Cs_Multas.VMU_IMPORTE, 2).ToString())) + ")</span></p>"

            HTML += "</div></div>"
            'HTML += "<div style='width:100%;position:relative;float:left;'>****img****</div>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.5;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.5; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 18px;'>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>CLAVES HOMOLOGADAS (01 A 82) Si el infractor realiza el pago de la sanción dentro del lapso de 7 días naturales contando a partir del día de la infracción pagara el UMA mínimo y a partir del octavo día pagara el UMA máximo que se encuentra en la boleta de infracción.</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>CLAVES NO HOMOLOGADAS (83 a 133) Si el infractor realiza el pago de la infracción dentro de los 7 naturales contando a partir del día de la infracción tendrá derecho a un 50% de descuento del monto de la misma y si pasa al octavo día pagara el monto total de la inflación.</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>Claves sin derecho a descuento (94,103,112,117 y 131).</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>La presente boleta de infracción ampara únicamente 7 días naturales para circular sin la garantía asegurada.</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>Las garantías se entregaran 24 horas después de haber sido elaborada la boleta</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>Acta de infracción que se levanta con fundamentos en los artículos indicados corresponden al REGLAMENTO DE PROTECCIÓN Y VIALIDAD Y MOVILIDAD URBANA DEL MUNICIPIO DE LERDO, DURANGO.</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #ffffff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#ffffff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.5;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.5; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 18px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.5; word-break: break-word; text-align: center; mso-line-height-alt: 21px; margin: 0;'>©APD Consultores en Tecnología de la Información</p>"
            HTML += "</div></div></div></div></div></div></div></div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #ffffff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#ffffff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#ffffff;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; color: #ffffff; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 12px; line-height: 1.2; text-align: center; word-break: break-word; font-family: inherit; mso-line-height-alt: 14px; margin: 0;'><span style='color: #555555;'><strong></strong><a href='#' rel='noopener' style='text-decoration: none; color: #848484;' target='_blank'></a></span></p>"
            HTML += "</div></div></div></div></div></div></div></div></td></tr></tbody></table></body></html>"

            Return HTML
        End Function
        Public Function GenerarHTML_nv(Cs_Ingreso As Cs_Ingreso) As String
            Dim HTML As String = ""
            HTML += "<html xmlns ='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head>"
            HTML += "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>"
            HTML += "<meta content='width=device-width' name='viewport'>"
            HTML += "<meta content='IE=edge' http-equiv='X-UA-Compatible'>"
            HTML += "<title></title>"
            HTML += "<style type='text/css'>body {margin: 0;padding: 0;}table,td,tr {vertical-align: top;border-collapse: collapse;}* {line-height: inherit;}a[x-apple-data-detectors=true] {color: inherit !important;text-decoration: none !important;}</style>"
            HTML += "<style id='media-query' type='text/css'>@media (max-width: 720px) {.block-grid,.col {min-width: 320px !important;max-width: 100% !important;display: block !important;}.block-grid {width: 100% !important;}.col {width: 100% !important;}.col>div {margin: 0 auto;}img.fullwidth,img.fullwidthOnMobile {max-width: 100% !important;}.no-stack .col {min-width: 0 !important;display: table-cell !important;}.no-stack.two-up .col {width: 50% !important;}.no-stack .col.num4 {width: 33% !important;}.no-stack .col.num8 {width: 66% !important;}.no-stack .col.num4 {width: 33% !important;}.no-stack .col.num3 {width: 25% !important;}.no-stack .col.num6 {width: 50% !important;}.no-stack .col.num9 {width: 75% !important;}.video-block {max-width: none !important;}.mobile_hide {min-height: 0px;max-height: 0px;max-width: 0px;display: none;overflow: hidden;font-size: 0px;}.desktop_hide {display: block !important;max-height: none !important;}}</style>"
            HTML += "</head>"
            HTML += "<body class='clean-body' style='margin: 0; padding: 0; -webkit-text-size-adjust: 100%; background-color: #fbfbfb;'>"
            HTML += "<table bgcolor='#fbfbfb' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='table-layout: fixed; vertical-align: top; min-width: 320px; Margin: 0 auto; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fbfbfb; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top;' valign='top'>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'><img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_p6nto0_amibzm.jpg' style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'></div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div class='col num6' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 348px; width: 350px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 45px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>"
            HTML += "<table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #0C0C0C; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>"
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "</table>"
            HTML += "</td>"
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "</table>"
            HTML += "<div style='color:#555555;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:5px;padding-right:10px;padding-bottom:0px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #555555; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 18px; line-height: 1.2; word-break: break-word; text-align: center; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 22px; margin: 0;'><span style='font-size: 18px;'><strong>LerdoMX VIALIDAD</strong></span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 10px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>"
            HTML += "<table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #0B0B0B; width: 100%;' valign='top' width='100%'>"
            HTML += "<tbody>"
            HTML += "<tr style='vertical-align: top;' valign='top'>"
            HTML += "<td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>"
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "</table>"
            HTML += "</td>"
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "</table>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>"
            HTML += "<div style='font-size:1px;line-height:25px'></div>"
            HTML += "<img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_2_tqrpf0_v43whg.png' style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Fecha</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 17px; margin: 0;'><strong><span style='font-size: 17px; mso-ansi-font-size: 18px;'>Folio:</span></strong></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Propietario</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Direccion</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Colonia</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Municipio</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Estado</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Codigo postal</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #232323; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'><strong>Celular</strong>:</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div class='col num3' style='display: table-cell; vertical-align: top; max-width: 700px; min-width: 500px; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 17px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 20px; mso-ansi-font-size: 18px; margin: 0;'><span style='font-size: 17px; mso-ansi-font-size: 18px;'>" + Cs_Ingreso.IIN_FECALTA + "</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:15px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.CDA_FOLIO + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:15px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grand11e, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.NOMBRE + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:15px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.DIRECCION + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:15px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.COLONIA + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:14px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.MUNICIPIO + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:14px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.ESTADO + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.CP + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='color:#232323;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:14px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #232323; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin: 0;'>" + Cs_Ingreso.CELULAR + "</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div class='col num6' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 348px; width: 350px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='font-size:16px;text-align:center;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif'></div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #4ba7ff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#4ba7ff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:0px; padding-bottom:0px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='font-size:16px;text-align:center;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif'>"
            HTML += "<table style=' padding-top:40px; width:100%;background:white;'>"
            HTML += "<thead style='background:white;border-bottom:2px solid black'>"
            HTML += "<tr style='border-bottom:1px solid black'>"
            HTML += "<th style='text-align:left;border-bottom:1px solid black'></th>"
            HTML += "</tr>"
            HTML += "</thead>"
            HTML += "<tbody>"
            HTML += "<tr style='padding-top:15px; padding-bottom: 15px;'>"
            If (Cs_Ingreso.IIN_SUBTOTAL = "") Then
                HTML += "<td style='text-align:left'>Subtotal: " + FormatNumber(0, 2) + "</td>"
            Else
                HTML += "<td style='text-align:left'>Subtotal: " + FormatNumber(Cs_Ingreso.IIN_SUBTOTAL, 2) + "</td>"
            End If
            HTML += "</tr>"
            HTML += "<tr style='padding-top:15px; padding-bottom: 15px;'>"
            If (Cs_Ingreso.IIN_SUBTOTAL = "") Then
                HTML += "<td style='text-align:left'>Desceunto: " + FormatNumber(0, 2) + "</td>"
            Else
                HTML += "<td style='text-align:left'>Desceunto: " + FormatNumber(Cs_Ingreso.IIN_DESCUENTO_IMPORTE, 2) + "</td>"
            End If
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "<tfoot>"
            HTML += "<tr>"
            If (Cs_Ingreso.IIN_SUBTOTAL = "") Then
                HTML += "<td class='CantidadTotal' style='padding-bottom:30px; border-top:1px solid black;text-align: left;'>Total: " + FormatNumber(0, 2) + "</td>"
            Else
                HTML += "<td class='CantidadTotal' style='padding-bottom:30px; border-top:1px solid black;text-align: left;'>Total: " + FormatNumber(Cs_Ingreso.IIN_TOTAL, 2) + "</td>"
            End If
            HTML += "</tr>"
            HTML += "</tfoot>"
            HTML += "</table>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #f4f4f4;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#f4f4f4;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;text-align: center;padding-right:10px;padding-bottom:25px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"

            If (Cs_Ingreso.IIN_SUBTOTAL = "") Then
                HTML += "<p style='font-size: 20px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 24px; margin: 0;'><span style='font-size: 20px;'>(0/100 M.N.)</span></p>"
            Else
                HTML += "<p style='font-size: 20px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 24px; margin: 0;'><span style='font-size: 20px;'>(" + UCase(Letras(FormatNumber(Cs_Ingreso.IIN_TOTAL, 2).ToString())) + ")</span></p>"
            End If
            HTML += "</div>"
            HTML += "</div><div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;text-align: center;padding-right:10px;padding-bottom:25px;padding-left:25px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 20px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 24px; margin: 0;'><span style='font-size: 20px;'>****img****</span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='background-color:#f4f4f4;width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #ffffff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#ffffff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#555555;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.5;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.5; font-size: 12px; color: #555555; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 18px;'>"
            HTML += "<p style='font-size: 14px; line-height: 1.5; word-break: break-word; text-align: center; mso-line-height-alt: 21px; margin: 0;'>©APD Consultores en Tecnología de la Información</p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "<div style='background-color:transparent;'>"
            HTML += "<div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: #ffffff;'>"
            HTML += "<div style='border-collapse: collapse;display: table;width: 100%;background-color:#ffffff;'>"
            HTML += "<div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>"
            HTML += "<div style='width:100% !important;'>"
            HTML += "<div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>"
            HTML += "<div style='color:#ffffff;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;line-height:1.2;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;'>"
            HTML += "<div style='line-height: 1.2; font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; color: #ffffff; mso-line-height-alt: 14px;'>"
            HTML += "<p style='font-size: 12px; line-height: 1.2; text-align: center; word-break: break-word; font-family: inherit; mso-line-height-alt: 14px; margin: 0;'><span style='color: #555555;'><a href='#' rel='noopener' style='text-decoration: none; color: #848484;' target='_blank'>Terms &amp; Conditions</a><strong>|</strong><a href='#' rel='noopener' style='text-decoration: none; color: #848484;' target='_blank'>Unsubscribe</a></span></p>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</div>"
            HTML += "</td>"
            HTML += "</tr>"
            HTML += "</tbody>"
            HTML += "</table>"
            HTML += "</body></html>"

            Return HTML
        End Function
        Public Function Letras(ByVal numero As String) As String
            Dim palabras, entero, dec, flag As String
            Dim num, x, y As Integer
            flag = "N"
            If Mid(numero, 1, 1) = "-" Then
                numero = Mid(numero, 2, numero.ToString.Length - 1).ToString
                palabras = "menos "
            End If
            For x = 1 To numero.ToString.Length
                If Mid(numero, 1, 1) = "0" Then
                    numero = Trim(Mid(numero, 2, numero.ToString.Length).ToString)
                    If Trim(numero.ToString.Length) = 0 Then palabras = ""
                Else
                    Exit For
                End If
            Next
            For y = 1 To Len(numero)
                If Mid(numero, y, 1) = "." Then
                    flag = "S"
                Else
                    If flag = "N" Then
#Disable Warning BC42104 ' La variable 'entero' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                        entero = entero + Mid(numero, y, 1)
#Enable Warning BC42104 ' La variable 'entero' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                    Else
#Disable Warning BC42104 ' La variable 'dec' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                        dec = dec + Mid(numero, y, 1)
#Enable Warning BC42104 ' La variable 'dec' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                    End If
                End If
            Next y
#Disable Warning BC42024 ' Variable local sin usar: 'enterocn'.
            Dim enterocn As Integer
#Enable Warning BC42024 ' Variable local sin usar: 'enterocn'.
            entero = (entero.Replace(",".ToString, "".ToString))

            If Len(dec) = 1 Then dec = dec & "0"
            flag = "N"
            If Val(numero) <= 999999999 Then
                For y = Len(entero) To 1 Step -1
                    num = Len(entero) - (y - 1)
                    Select Case y
                        Case 3, 6, 9
                            Select Case Mid(entero, num, 1)
                                Case "1"
#Disable Warning BC42104 ' La variable 'palabras' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                                    palabras = palabras & "Ciento "
#Enable Warning BC42104 ' La variable 'palabras' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                                Case "2"
                                    palabras = palabras & "Doscientos "
                                Case "3"
                                    palabras = palabras & "Trescientos "
                                Case "4"
                                    palabras = palabras & "Cuatrocientos "
                                Case "5"
                                    palabras = palabras & "Quinientos "
                                Case "6"
                                    palabras = palabras & "Seiscientos "
                                Case "7"
                                    palabras = palabras & "Setecientos "
                                Case "8"
                                    palabras = palabras & "Ochocientos "
                            '*hasta el 9
                                Case "9"
                                    palabras = palabras & "Novecientos "
                            End Select
                        Case 2, 5, 8
                            Select Case Mid(entero, num, 1)
                                Case "1"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        flag = "S"
                                        palabras = palabras & "Diez "
                                    End If
                                    If Mid(entero, num + 1, 1) = "1" Then
                                        flag = "S"
                                        palabras = palabras & "Once "
                                    End If
                                    If Mid(entero, num + 1, 1) = "2" Then
                                        flag = "S"
                                        palabras = palabras & "Doce "
                                    End If
                                    If Mid(entero, num + 1, 1) = "3" Then
                                        flag = "S"
                                        palabras = palabras & "Trece "
                                    End If
                                    If Mid(entero, num + 1, 1) = "4" Then
                                        flag = "S"
                                        palabras = palabras & "Catorce "
                                    End If
                                    If Mid(entero, num + 1, 1) = "5" Then
                                        flag = "S"
                                        palabras = palabras & "Quince "
                                    End If
                                    If Mid(entero, num + 1, 1) > "5" Then
                                        flag = "N"
                                        palabras = palabras & "Dieci"
                                    End If
                                Case "2"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Veinte "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Veinti"
                                        flag = "N"
                                    End If
                                Case "3"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Treinta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Treinta y "
                                        flag = "N"
                                    End If
                                Case "4"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Cuarenta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Cuarenta y "
                                        flag = "N"
                                    End If
                                Case "5"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Cincuenta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Cincuenta y "
                                        flag = "N"
                                    End If
                                Case "6"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Sesenta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Sesenta y "
                                        flag = "N"
                                    End If
                                Case "7"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Setenta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Setenta y "
                                        flag = "N"
                                    End If
                                Case "8"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Ochenta "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Ochenta y "
                                        flag = "N"
                                    End If
                                Case "9"
                                    If Mid(entero, num + 1, 1) = "0" Then
                                        palabras = palabras & "Noventa "
                                        flag = "S"
                                    Else
                                        palabras = palabras & "Noventa y "
                                        flag = "N"
                                    End If
                            End Select
                        Case 1, 4, 7
                            Select Case Mid(entero, num, 1)
                                Case "1"
                                    If flag = "N" Then
                                        If y = 1 Then
                                            palabras = palabras & "Uno "
                                        Else
                                            palabras = palabras & "Un "
                                        End If
                                    End If
                                Case "2"
                                    If flag = "N" Then palabras = palabras & "Dos "
                                Case "3"
                                    If flag = "N" Then palabras = palabras & "Tres "
                                Case "4"
                                    If flag = "N" Then palabras = palabras & "Cuatro "
                                Case "5"
                                    If flag = "N" Then palabras = palabras & "Cinco "
                                Case "6"
                                    If flag = "N" Then palabras = palabras & "Seis "
                                Case "7"
                                    If flag = "N" Then palabras = palabras & "Siete "
                                Case "8"
                                    If flag = "N" Then palabras = palabras & "Ocho "
                                Case "9"
                                    If flag = "N" Then palabras = palabras & "Nueve "
                            End Select
                    End Select
                    If y = 4 Then
                        If Mid(entero, 6, 1) <> "0" Or Mid(entero, 5, 1) <> "0" Or Mid(entero, 4, 1) <> "0" Or
                        (Mid(entero, 6, 1) = "0" And Mid(entero, 5, 1) = "0" And Mid(entero, 4, 1) = "0" And
                        Len(entero) <= 6) Then palabras = palabras & "Mil "
                    End If
                    If y = 7 Then
                        If Len(entero) = 7 And Mid(entero, 1, 1) = "1" Then
                            palabras = palabras & "Millón "
                        Else
                            palabras = palabras & "Millones "
                        End If
                    End If
                Next y

                If palabras = "" Then
                    Letras = "CERO M.N."
                Else
                    If dec <> "" Then
                        Dim AuxDec As String
                        If dec.Length > 2 Then
                            AuxDec = Mid(dec, 2, 1)
                            Letras = palabras & "" & "" & AuxDec & "/100 M.N."
                        Else
                            Letras = palabras & "" & "" & dec & "/100 M.N."
                        End If
                    Else
                        Letras = palabras & " M.N."
                    End If
                End If
                Return Letras
            End If
#Disable Warning BC42104 ' La variable 'Letras' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
            Return Letras
#Enable Warning BC42104 ' La variable 'Letras' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
        End Function
        Public Function ENVIAR_CORREO(ByVal EMAIL_PIE_PAGINA As String, ByVal EMAIL_ENCABEZADO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_MENSAJITO As String, ByVal EMAIL_ENCABEZADO_SUPERVISOR As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String) As Boolean
            Dim EMAIL_CUERPO As String = ""
            EMAIL_CUERPO = "<!DOCTYPE html>
                            <html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
                            <head>
                                <meta charset='utf-8'> <!-- utf-8 works for most cases -->
                                <meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
                                <meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
                                <meta name='x-apple-disable-message-reformatting'>  <!-- Disable auto-scale in iOS 10 Mail entirely -->
                                <title></title> <!-- The title tag shows in email notifications, like Android 4.4. -->

                                <link href='https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700' rel='stylesheet'>

                                <!-- CSS Reset : BEGIN -->
                            <style>
                            html,
                            body {
                                margin: 0 auto !important;
                                padding: 0 !important;
                                height: 100% !important;
                                width: 100% !important;
                                background: #f1f1f1;
                            }

                            /* What it does: Stops email clients resizing small text. */
                            * {
                                -ms-text-size-adjust: 100%;
                                -webkit-text-size-adjust: 100%;
                            }

                            /* What it does: Centers email on Android 4.4 */
                            div[style*='margin: 16px 0'] {
                                margin: 0 !important;
                            }

                            /* What it does: Stops Outlook from adding extra spacing to tables. */
                            table,
                            td {
                                mso-table-lspace: 0pt !important;
                                mso-table-rspace: 0pt !important;
                            }

                            /* What it does: Fixes webkit padding issue. */
                            table {
                                border-spacing: 0 !important;
                                border-collapse: collapse !important;
                                table-layout: fixed !important;
                                margin: 0 auto !important;
                            }

                            /* What it does: Uses a better rendering method when resizing images in IE. */
                            img {
                                -ms-interpolation-mode:bicubic;
                            }

                            /* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
                            a {
                                text-decoration: none;
                            }

                            /* What it does: A work-around for email clients meddling in triggered links. */
                            *[x-apple-data-detectors],  /* iOS */
                            .unstyle-auto-detected-links *,
                            .aBn {
                                border-bottom: 0 !important;
                                cursor: default !important;
                                color: inherit !important;
                                text-decoration: none !important;
                                font-size: inherit !important;
                                font-family: inherit !important;
                                font-weight: inherit !important;
                                line-height: inherit !important;
                            }

                            /* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
                            .a6S {
                                display: none !important;
                                opacity: 0.01 !important;
                            }

                            /* What it does: Prevents Gmail from changing the text color in conversation threads. */
                            .im {
                                color: inherit !important;
                            }

                            /* If the above doesn't work, add a .g-img class to any image in question. */
                            img.g-img + div {
                                display: none !important;
                            }

                            /* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
                            /* Create one of these media queries for each additional viewport size you'd like to fix */

                            /* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
                            @media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
                                u ~ div .email-container {
                                    min-width: 320px !important;
                                }
                            }
                            /* iPhone 6, 6S, 7, 8, and X */
                            @media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
                                u ~ div .email-container {
                                    min-width: 375px !important;
                                }
                            }
                            /* iPhone 6+, 7+, and 8+ */
                            @media only screen and (min-device-width: 414px) {
                                u ~ div .email-container {
                                    min-width: 414px !important;
                                }
                            }

                            </style>

                            <!-- CSS Reset : END -->

                            <!-- Progressive Enhancements : BEGIN -->
                            <style>

                              .primary{
	                            background: #0d0cb5;
                            }
                            .bg_white{
	                            background: #ffffff;
                            }
                            .bg_light{
	                            background: #fafafa;
                            }
                            .bg_black{
	                            background: #000000;
                            }
                            .bg_dark{
	                            background: rgba(0,0,0,.8);
                            }
                            .email-section{
	                            padding:2.5em;
                            }

                            /*BUTTON*/
                            .btn{
	                            padding: 5px 15px;
	                            display: inline-block;
                            }
                            .btn.btn-primary{
	                            border-radius: 5px;
	                            background: #0d0cb5;
	                            color: #ffffff;
                            }
                            .btn.btn-white{
	                            border-radius: 5px;
	                            background: #ffffff;
	                            color: #000000;
                            }
                            .btn.btn-white-outline{
	                            border-radius: 5px;
	                            background: transparent;
	                            border: 1px solid #fff;
	                            color: #fff;
                            }

                            h1,h2,h3,h4,h5,h6{
	                            font-family: 'Poppins', sans-serif;
	                            color: #000000;
	                            margin-top: 0;
                            }

                            body{
	                            font-family: 'Poppins', sans-serif;
	                            font-weight: 400;
	                            font-size: 15px;
	                            line-height: 1.8;
	                            color: rgba(0,0,0,.4);
                            }

                            a{
	                            color: #0d0cb5;
                            }

                            table{
                            }
                            /*LOGO*/

                            .logo h1{
	                            margin: 0;
                            }
                            .logo h1 a{
	                            color: #000000;
	                            font-size: 20px;
	                            font-weight: 700;
	                            text-transform: uppercase;
	                            font-family: 'Poppins', sans-serif;
                            }

                            .navigation{
	                            padding: 0;
                            }
                            .navigation li{
	                            list-style: none;
	                            display: inline-block;;
	                            margin-left: 5px;
	                            font-size: 13px;
	                            font-weight: 500;
                            }
                            .navigation li a{
	                            color: rgba(0,0,0,.4);
                            }

                            /*HERO*/
                            .hero{
	                            position: relative;
	                            z-index: 0;
                            }
                            .hero .overlay{
	                            position: absolute;
	                            top: 0;
	                            left: 0;
	                            right: 0;
	                            bottom: 0;
	                            content: '';
	                            width: 100%;
	                            /*background: #000000;*/
	                            z-index: -1;
	                            opacity: .3;
	                            color:black!important;
                            }
                            .hero .icon{
                            }
                            .hero .icon a{
	                            display: block;
	                            width: 60px;
	                            margin: 0 auto;
                            }
                            .hero .text{
	                            color: rgba(255,255,255,.8);
                            }
                            .hero .text h2{
	                            color: #ffffff;
	                            font-size: 30px;
	                            margin-bottom: 0;
                            }


                            /*HEADING SECTION*/
                            .heading-section{
                            }
                            .heading-section h2{
	                            color: #000000;
	                            font-size: 20px;
	                            margin-top: 0;
	                            line-height: 1.4;
	                            font-weight: 700;
	                            text-transform: uppercase;
                            }
                            .heading-section .subheading{
	                            margin-bottom: 20px !important;
	                            display: inline-block;
	                            font-size: 13px;
	                            text-transform: uppercase;
	                            letter-spacing: 2px;
	                            color: rgba(0,0,0,.4);
	                            position: relative;
                            }
                            .heading-section .subheading::after{
	                            position: absolute;
	                            left: 0;
	                            right: 0;
	                            bottom: -10px;
	                            content: '';
	                            width: 100%;
	                            height: 2px;
	                            background: #0d0cb5;
	                            margin: 0 auto;
                            }

                            .heading-section-white{
	                            color: rgba(255,255,255,.8);
                            }
                            .heading-section-white h2{
	                            font-family: 
	                            line-height: 1;
	                            padding-bottom: 0;
                            }
                            .heading-section-white h2{
	                            color: #ffffff;
                            }
                            .heading-section-white .subheading{
	                            margin-bottom: 0;
	                            display: inline-block;
	                            font-size: 13px;
	                            text-transform: uppercase;
	                            letter-spacing: 2px;
	                            color: rgba(255,255,255,.4);
                            }


                            .icon{
	                            text-align: center;
                            }
                            .icon img{
                            }


                            /*SERVICES*/
                            .services{
	                            background: rgba(0,0,0,.03);
                            }
                            .text-services{
	                            padding: 10px 10px 0; 
	                            text-align: center;
                            }
                            .text-services h3{
	                            font-size: 16px;
	                            font-weight: 600;
                            }

                            .services-list{
	                            padding: 0;
	                            margin: 0 0 20px 0;
	                            width: 100%;
	                            float: left;
                            }

                            .services-list img{
	                            float: left;
                            }
                            .services-list .text{
	                            width: calc(100% - 60px);
	                            float: right;
                            }
                            .services-list h3{
	                            margin-top: 0;
	                            margin-bottom: 0;
                            }
                            .services-list p{
	                            margin: 0;
                            }

                            /*BLOG*/
                            .text-services .meta{
	                            text-transform: uppercase;
	                            font-size: 14px;
                            }

                            /*TESTIMONY*/
                            .text-testimony .name{
	                            margin: 0;
                            }
                            .text-testimony .position{
	                            color: rgba(0,0,0,.3);

                            }


                            /*VIDEO*/
                            .img{
	                            width: 100%;
	                            height: auto;
	                            position: relative;
                            }
                            .img .icon{
	                            position: absolute;
	                            top: 50%;
	                            left: 0;
	                            right: 0;
	                            bottom: 0;
	                            margin-top: -25px;
                            }
                            .img .icon a{
	                            display: block;
	                            width: 60px;
	                            position: absolute;
	                            top: 0;
	                            left: 50%;
	                            margin-left: -25px;
                            }



                            /*COUNTER*/
                            .counter{
	                            width: 100%;
	                            position: relative;
	                            z-index: 0;
                            }
                            .counter .overlay{
	                            position: absolute;
	                            top: 0;
	                            left: 0;
	                            right: 0;
	                            bottom: 0;
	                            content: '';
	                            width: 100%;
	                            /*background: #000000;*/
	                            background: white;
	                            z-index: -1;
	                            opacity: .3;
	                            color:black!important;
                            }
                            .counter-text{
	                            text-align: center;
                            }
                            .counter-text .num{
	                            display: block;
	                            color: #ffffff;
	                            font-size: 34px;
	                            font-weight: 700;
                            }
                            .counter-text .name{
	                            display: block;
	                            color: rgba(255,255,255,.9);
	                            font-size: 13px;
                            }


                            /*FOOTER*/

                            .footer{
	                            color: rgba(255,255,255,.5);

                            }
                            .footer .heading{
	                            color: #ffffff;
	                            font-size: 20px;
                            }
                            .footer ul{
	                            margin: 0;
	                            padding: 0;
                            }
                            .footer ul li{
	                            list-style: none;
	                            margin-bottom: 10px;
                            }
                            .footer ul li a{
	                            color: rgba(255,255,255,1);
                            }
                            .tbn *
                            {
	                            color: #3f905f!important;
                            }

                            @media screen and (max-width: 500px) {

	                            .icon{
		                            text-align: left;
	                            }

	                            .text-services{
		                            padding-left: 0;
		                            padding-right: 20px;
		                            text-align: left;
	                            }

                            }
                            </style>


                            </head>

                            <body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #222222;'>
	                            <center style='width: 100%; background-color: #f1f1f1;'>
                                <div style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
                                  &zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
                                </div>
                                <div style='max-width: 600px; margin: 0 auto;' class='email-container'>
    	                            <!-- BEGIN BODY -->
                                  <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>
      	                            <tr>
                                      <td valign='top' class='bg_white' style='padding: 1em 2.5em;'>
          	                            <table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
          		                            <tr>
          			                            <td width='40%' class='logo' style='text-align: center;'>
			                                        <h1><a href='#'>LerdoDigital</a></h1>
			                                      </td>
			          
          		                            </tr>
          	                            </table>
                                      </td>
	                                  </tr><!-- end tr -->
				                            <tr>
                                      <td valign='middle' class='hero bg_white' style='background:rgba(239, 238, 107, 0.2); background-size: cover; height: 400px;'>
          	                            <div class='overlay'></div>
                                        <table class='tbn'>
            	                            <tr>
            		                            <td>
            			                            <div class='text' style='padding: 0 3em; text-align: center;'>
            				                            <h2>Envio de recibo</h2><br>
            				                            <h3 ><strong>FOLIO</strong>: " +
            EMAIL_ENCABEZADO + "</h3> 
            				                            <p>Su recibo de pago se envió correctamente, puede realizar su pago con la información siguiente en cajas de la presidencia o desde el portal Lerdo Digital. </p> 
	                                              </div>
            			                            </div>
            		                            </td>
            	                            </tr>
                                        </table>
                                      </td>
	                                  </tr><!-- end tr -->
	                                 <tr>
		                                  <td class='bg_white'>
		                                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
		                                      <tr>
		         
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                      <tr> 
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                      <tr>
		          
                                                </tr>
		            	                            </table>
		            	                            <table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
		            		                            <tr> 
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr --> 
		                                      <tr>
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                      <tr>
		        
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                      <tr>
		                                       <!-- <td class='bg_white email-section' style='width: 100%;'>
		            	                            <table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
		            		                            <tr>
		            			                            <td valign='middle' width='50%'>
                                                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                                      <tr>
                                                        <td>
                                                          <img src='images/bg_2.jpg' alt='' style='width: 100%; max-width: 600px; height: auto; margin: auto; display: block;'>
                                                        </td>
                                                      </tr>
                                                    </table>
                                                  </td>
                                                  <td valign='middle' width='50%'>
                                                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                                      <tr>
                                                        <td class='text-services' style='text-align: left; padding-left:25px;'>
                            	                            <div class='heading-section'>
								              	                            <h2>Nuestros servicios</h2>
								            	                            </div>
								            	                            <div class='services-list'>
								            		                            <img src='images/checked.png' alt='' style='width: 50px; max-width: 600px; height: auto; display: block;'>
								            		                            <div class='text'>
								            			                            <h3>Pagos en linea</h3>
								            			
								            		                            </div>
								            	                            </div>
								            	                            <div class='services-list'>
								            		                            <img src='images/checked.png' alt='' style='width: 50px; max-width: 600px; height: auto; display: block;'>
								            		                            <div class='text'>
								            			                            <h3>Facturacion en linea</h3>
								            			
								            		                            </div>
								            	                            </div> 	
                                                        </td>
                                                      </tr>
                                                    </table>
                                                  </td>-->
                                                </tr>
		            	                            </table>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                      <tr>
		                                        <td class='primary email-section' style='text-align:center;'>
		            	                            <div class='heading-section heading-section-white' style='    padding: 10px;
                                                padding-top: 20px;background:#3cac34;'>
		              	                            <h3 style='color:white;font-weight: bold; text-align:center;'>Porque sabemos que es muy importante</h3>
		              	                            <p style='text-align:center;'>Estamos a tu servicio en la página oficial de lerdo digital.</p>
		              	                            <p style='text-align:center;'><a href='https://www.lerdodigital.mx' class='btn btn-white-outline'>Conoce más</a></p>
		            	                            </div>
		                                        </td>
		                                      </tr><!-- end: tr -->
		                                    </table>

		                                  </td>
		                                </tr><!-- end:tr -->
                                  <!-- 1 Column Text + Button : END -->
                                  </table>
                                  <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>
      	                            <tr>
                                      <td valign='middle' class='bg_black footer email-section'>
                                        <table>
            	                            <tr>
                                           <!-- <td valign='top' width='50%' style='padding-top: 20px;'>
                                              <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                                <tr>
                                                  <td style='text-align: left; padding-right: 10px;'>
                      	                            <h3 class='heading'>About</h3>
                      	                            <p>A small river named Duden flows by their place and supplies it with the necessary regelialia.</p>
                                                  </td>
                                                </tr>
                                              </table>
                                            </td>-->
                                           <!-- <td valign='top' width='100%' style='padding-top: 20px;'>
                                              <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                                <tr>
                                                  <td style='text-align: center; padding-left: 5px; padding-right: 5px;'>
                      	                            <h3 class='heading'>Contacto</h3>
                      	                            <ul>
					                                            <li><span class='text'>direccion apd</span></li>
					                                            <li><span class='text'>+telefono ricardo</span></a></li>
					                                          </ul>
                                                  </td>
                                                </tr>
                                              </table>
                                            </td>-->
                
                                          </tr>
                                        </table>
                                      </td>
                                    </tr><!-- end: tr -->
                                    <tr>
        	                            <td valign='middle' class='bg_black footer email-section'>
        		                            <table>
            	                            <tr>
                                            <td valign='top' width='33.333%'>
                                              <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                                <tr>
                                                  <td style='text-align: center; '>
                      	                            <p>Desarrollo Web &copy;Lerdo Digital by APD Consultores en Tecnología de la Información</p>
                                                  </td>
                                                </tr>
                                              </table>
                                            </td>
               
                                          </tr>
                                        </table>
        	                            </td>
                                    </tr>
                                  </table>

                                </div>
                              </center>
                            </body>
                            </html>"
            Dim Band As Boolean = ENVIA_MAIL(EMAIL_CUERPO, EMAIL_DESTINATARIO, EMAIL_ENCABEZADO, LLEVA_ATACHMENT, NOM_ATACHMENT, archivopdf)
            Return Band
        End Function
        Public Function ENVIA_MAIL(ByVal EMAIL_CUERPO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_ENCABEZADO As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String) As Boolean
            Try
                Dim fromaddr As String = "lerdopagos@gmail.com"
                Dim toaddr As String = EMAIL_DESTINATARIO
                Dim password As String = "hzkcvupbbmmgyglt"

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "RECIBO DE PAGO FOLIO " + EMAIL_ENCABEZADO + " PORTAL LERDO DIGITAL"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                msg.Attachments.Add(New Attachment(archivopdf))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                smtp.Host = "smtp.gmail.com"
                smtp.Port = 587
                smtp.UseDefaultCredentials = False
                smtp.EnableSsl = True
                Dim nc As New System.Net.NetworkCredential(fromaddr, password)
                smtp.Credentials = nc
                smtp.Send(msg)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function


        <HttpGet>
        <Route("api/Ciudadanos/obtenerUsuario")>
        Public Function obtenerUsuario(ByVal correo As String) As IHttpActionResult
            Dim Cs_Respuesta As New Cs_Respuesta

            ' Validación básica de los parámetros de entrada
            If String.IsNullOrEmpty(correo) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400 ' Bad Request
                Cs_Respuesta.mensaje = "Se requiere al menos un parámetro de búsqueda (correo )."
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If

            Try
                Dim usuario As Cs_Usuario = Mdl_Usuarios.buscarUsuario(correo)

                If usuario IsNot Nothing Then
                    ' Crear un nuevo objeto con solo la información que deseas exponer
                    Dim usuarioPublico As New With {
                .Usuario = usuario.LUS_USUARIO,
                .Correo = usuario.LUS_CORREO,
                .Telefono = usuario.LUS_TELEFONO,
                .Contrasena = usuario.LUS_CONTRASENA
            }
                    Return Ok(usuarioPublico) ' HTTP 200 con los datos del usuario
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 404 ' Not Found
                    Cs_Respuesta.mensaje = "Usuario no encontrado."
                    Return Content(HttpStatusCode.NotFound, Cs_Respuesta)
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo =
                Cs_Respuesta.codigoError = 500 ' Internal Server Error
                Cs_Respuesta.mensaje = "Error al obtener los datos del usuario: " & ex.Message
                Return Content(HttpStatusCode.InternalServerError, Cs_Respuesta)
            End Try
        End Function

        ''nueva 
        <HttpGet>
        <Route("api/Ciudadanos/actualizarUsuario")>
        Public Function actualizarUsuario(ByVal LUS_CLAVE As Integer, ByVal LUS_USUARIO As String, ByVal LUS_TELEFONO As String, LUS_CORREO As String, OLD_PASS As String, NEW_PASS As String) As IHttpActionResult
            Dim Cs_Respuesta As New Cs_Respuesta

            ' Verificación de los parámetros requeridos
            If String.IsNullOrEmpty(LUS_USUARIO) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parámetro requerido: usuario."
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If

            If String.IsNullOrEmpty(LUS_TELEFONO) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parámetro requerido: teléfono."
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If
            If String.IsNullOrEmpty(LUS_CORREO) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parámetro requerido: email."
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If


            If LUS_CLAVE <= 0 Then ' Ajuste para validar LUS_CLAVE como entero
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parámetro requerido: clave."
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If

            If OLD_PASS <> "" And NEW_PASS = "" Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50003
                Cs_Respuesta.mensaje = "Parámetro requerido: Nueva Contraseña"
                Return Content(HttpStatusCode.BadRequest, Cs_Respuesta)
            End If

            OLD_PASS = encrypt.Llamada_encriptar(OLD_PASS)

            NEW_PASS = encrypt.Llamada_encriptar(NEW_PASS)

            Try
                ' Asume que Mdl_Usuarios.ActualizarUsuario ahora acepta los parámetros directamente
                Dim resultado As String = Mdl_Usuarios.ActualizarUsuario(LUS_CLAVE, LUS_USUARIO, LUS_TELEFONO, LUS_CORREO, OLD_PASS, NEW_PASS)

                If resultado = "ACTUALIZACION REALIZADA CON EXITO" Then
                    Cs_Respuesta.codigo = 0
                    Cs_Respuesta.mensaje = "Usuario actualizado correctamente."
                    Return Ok(Cs_Respuesta)
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = resultado
                    Return Content(HttpStatusCode.InternalServerError, Cs_Respuesta)
                End If
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 50004
                Cs_Respuesta.mensaje = "Error al actualizar usuario."
                Return Content(HttpStatusCode.InternalServerError, Cs_Respuesta)
            End Try
        End Function

    End Class
End Namespace