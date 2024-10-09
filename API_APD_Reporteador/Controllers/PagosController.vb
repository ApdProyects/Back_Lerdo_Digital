Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models
Imports System.Web.Mail
Imports System.Net.Mail
Imports System.Globalization
Imports SelectPdf
Imports iTextSharp.text
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports iTextSharp.text.pdf.BarcodeCodabar
Imports System.IO
Imports Image = System.Drawing.Image
Imports ZXing
Imports ZXing.QrCode
Imports ZXing.Common
Imports System.Data.SqlTypes
Imports System.Web.Services.Description

Namespace Controllers
    Public Class PagosController
        Inherits ApiController
        Dim Cs_JWT As New Cs_JWT
        Dim Mdl_Ciudadanos As New Mdl_Ciudadanos
        Dim encrypt As New Cs_Encrypt
        Dim Mdl_Pagos As New Mdl_Pagos
        Dim Mdl_InfoFolio As New Mdl_InfoFolio
        Dim Mdl_Catastro As New Mdl_Catastro
        Dim Cs_Imagenes As New Cs_Imagenes
        Dim Html_Recibos As New HTML_Recibos

        <HttpGet>
        Public Function index(ByVal folio As Integer)
            Return True
        End Function

        Public Function genera_QR(ByVal iin_clave As String)
            Dim writer As New BarcodeWriter()
            writer.Format = BarcodeFormat.QR_CODE
            Dim result As Bitmap = writer.Write("https://lerdodigital.mx/documentacion/QR/" + iin_clave)
            Using ms As New MemoryStream()
                result.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                Dim imageString As String = Convert.ToBase64String(ms.ToArray())
                Return "data:image/png;base64," + imageString
            End Using
        End Function

        <HttpGet>
        <Route("api/Pagos/genera_recibo")>
        Public Function genera_recibo(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal usuario As String,
                                      ByVal RESULTADO_PAYW As String, ByVal ID_AFILIACION As String, ByVal FECHA_RSP_CTE As DateTime, ByVal CODIGO_AUT As String,
                                      ByVal REFERENCIA As String, ByVal FECHA_REQ_CTE As DateTime, ByVal Set_Cookie As String, ByVal pago As String, ByVal correo As String) As Cs_Respuesta_pago

            Dim Cs_Pago As New Cs_Pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim Cs_InfoFolio As New ArrayList
            Dim QR As String
            Dim Concepto As String = ""

            Cs_Pago.id_folio = id_folio
            Cs_Pago.id_concepto = id_concepto
            Cs_Pago.id_depto = id_depto
            Cs_Pago.usuario = usuario
            Cs_Pago.RESULTADO_PAYW = RESULTADO_PAYW
            Cs_Pago.ID_AFILIACION = ID_AFILIACION
            Cs_Pago.FECHA_RSP_CTE = FECHA_RSP_CTE
            Cs_Pago.CODIGO_AUT = CODIGO_AUT
            Cs_Pago.REFERENCIA = REFERENCIA

            Cs_Pago.FECHA_REQ_CTE = FECHA_REQ_CTE
            Cs_Pago.Set_Cookie = Set_Cookie
            Cs_Pago.pago = pago

            Try

                Dim tipo As String = ""
                If (id_folio.Count < 11) Then
                    tipo = 2
                Else
                    tipo = 1
                End If

                Cs_InfoFolio = Mdl_Catastro.traer_info_catastro(id_folio, tipo)

                ' Cs_Respuesta_pago = Mdl_Pagos.GuardaPagoCatastro(Cs_Pago, id_depto, id_concepto)
                ' Cs_Respuesta_pago = Mdl_Pagos.GeneraCobroCatastro(Cs_Pago)

                'Cs_Respuesta_pago.folio = "11610683"

                Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

                Cs_Respuesta_pago.folio = "APD" + IIN_CLAVE
                'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
                Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

                Dim detalle


                For Each c In Cs_InfoFolio
                    detalle = c.ItemArray
                Next

                Dim numero2letras = UCase(letras(pago))

                Dim conceptos As String = ""
                Dim subtotales As String = ""
                Dim descuentos As String = ""
                Dim totales As String = ""

                Dim FolioRecibo As Integer = 0
                FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
                Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)
                QR = genera_QR(IIN_CLAVE)

                If Not IsNothing(detalle(34)) AndAlso detalle(34) > 0 Then
                    conceptos += "IMPUESTO DEL EJERCICIO 2023 PREDIAL<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(35)) AndAlso detalle(35) > 0 Then
                    conceptos += "REZAGOS DE EJERCICIOS ANTERIORES<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(41)) AndAlso detalle(41) > 0 Then
                    conceptos += "GASTOS DE EJECUCION PREDIAL<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(42)) AndAlso detalle(42) > 0 Then
                    conceptos += "MULTAS PREDIAL<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(43)) AndAlso detalle(43) > 0 Then
                    conceptos += "RECARGOS DE EJERCICIOS ANTERIORES<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(44)) AndAlso detalle(44) > 0 Then
                    conceptos += "DIFERENCIAS<br><br>"
                    subtotales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
                    totales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
                End If
                If Not IsNothing(detalle(60)) AndAlso detalle(60) > 0 Then
                    descuentos = FormatCurrency(Convert.ToDouble(detalle(60)).ToString()) & "<br>"
                End If

                Dim year = Format(detalle(4), "yyyy")
                Dim mes = Format(detalle(4), "MM")

                Dim TextoDescuento = ""
                If (mes = "01") Then
                    TextoDescuento = "PAGO OPORTUNO 1o MES DEL 2023"
                ElseIf (mes = "02") Then
                    TextoDescuento = "PAGO OPORTUNO 2o MES DEL 2023"
                ElseIf (mes = "03") Then
                    TextoDescuento = "PAGO OPORTUNO 3o MES DEL 2023"
                Else
                    TextoDescuento = ""
                End If

                Dim html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
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
                                                    <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
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
                                                    <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
                                                   
                                                </div>
                                            </div>
                                            <div class='row'>
                                                <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>NOMBRE:</font><br>  
                                                    <font face='Arial' size='2'>DOMICILIO:</font><br>
                                                    <font face='Arial' size='2'>ACIVIDAD:</font>
                                                </div>
                                                <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>" & detalle(7) & "</font><br>  
                                                    <font face='Arial' size='2'>" & detalle(8) & "</font><br>
                                                    <font face='Arial' size='2'><b>VALOR CATASTRAL " & (FormatCurrency(Convert.ToDouble(detalle(31)))).ToString() & "</b></font>
                                                </div>
                                                <div class='col-1' style='padding-top: 3px;'>
                                                    <font face='Arial' size='2'>FECHA:</font><br>  
                                                    <font face='Arial' size='2'>R.F.C:</font><br>
                                                    <font face='Arial' size='2'>CVE CTR:</font>
                                                </div>
                                                <div class='col-2'>
                                                    <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'>" & id_folio & "</font>
                                                </div>
                                            </div>
                                            <br>
                                            <div class='row'>
                                                <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
                                                    <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
                                                </div>
                                                <div class='col-9' style='padding-left: 5px;'>
                                                    <table class='table' style='border: 1px solid white;' background='C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg'> 
                                                        <tbody>
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBTOTAL</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>DESCUENTO</b></td>
                                                            <td class='text-center' style='font-size: 13px;'><b>TOTAL</b></td>
                                                          </tr>
                                                          <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
                                                            <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
                                                         </tr>

                                                          <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br>
                                                                                                                        " + (Convert.ToInt32(year) - 1).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 2).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 3).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 4).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 5).ToString() + "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top:35px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
                                                          </tr>
                                                          <tr> 
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 12px; padding-top: 15px;'>( TC " & (FormatCurrency(Convert.ToDouble(pago))).ToString() & " )</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
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
                                                       <font face='Arial' style='font-size: 14px;'>" & numero2letras.ToString() & " </font> 
                                                    </div>
                                                    <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>  
                                                </div>
                                                 <div class='row'>
                                                <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
                                                      <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
                                                </div> 
                                            </div>
                                                <div class='col-3 text-center' style='padding-top:10px;'>
                                                    <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
                                                </div>
                                            </div> 
                                          </div>
                                    </body>
                                </html>"

                Dim htmlString As String = html
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
                doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
                doc.Close()

                Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
                ' Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")

            Catch ex As Exception
                Cs_Respuesta_pago.codigo = -1
                Cs_Respuesta_pago.codigoError = 400
                Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
                Cs_Respuesta_pago.folio = ""
            End Try

            Return Cs_Respuesta_pago
        End Function

        <HttpGet>
        <Route("api/Pagos/RealizarPagos2")>
        Public Function RealizarPagos2(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal usuario As String,
                                      ByVal RESULTADO_PAYW As String, ByVal ID_AFILIACION As String, ByVal FECHA_RSP_CTE As DateTime, ByVal CODIGO_AUT As String,
                                      ByVal REFERENCIA As String, ByVal FECHA_REQ_CTE As DateTime, ByVal Set_Cookie As String, ByVal pago As Double, ByVal correo As String) As Cs_Respuesta_pago
            Dim Cs_Pago As New Cs_Pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim Cs_InfoFolio As New ArrayList
            Dim QR As String
            Dim Concepto As String = ""

            Cs_Pago.id_folio = id_folio
            Cs_Pago.id_concepto = id_concepto
            Cs_Pago.id_depto = id_depto
            Cs_Pago.usuario = usuario
            Cs_Pago.RESULTADO_PAYW = RESULTADO_PAYW
            Cs_Pago.ID_AFILIACION = ID_AFILIACION
            Cs_Pago.FECHA_RSP_CTE = FECHA_RSP_CTE
            Cs_Pago.CODIGO_AUT = CODIGO_AUT
            Cs_Pago.REFERENCIA = REFERENCIA

            Cs_Pago.FECHA_REQ_CTE = FECHA_REQ_CTE
            Cs_Pago.Set_Cookie = Set_Cookie
            Cs_Pago.pago = pago

            Try

                Dim tipo As String = ""
                If (id_folio.Count < 11) Then
                    tipo = 2
                Else
                    tipo = 1
                End If

                Cs_InfoFolio = Mdl_Catastro.traer_info_catastro(id_folio, tipo)

                Cs_Respuesta_pago = Mdl_Pagos.GuardaPagoCatastro(Cs_Pago, id_depto, id_concepto)
                Cs_Respuesta_pago = Mdl_Pagos.GeneraCobroCatastro(Cs_Pago)


            Catch ex As Exception
                Cs_Respuesta_pago.codigo = -1
                Cs_Respuesta_pago.codigoError = 400
                Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
                Cs_Respuesta_pago.folio = ""
            End Try

            Return Cs_Respuesta_pago
        End Function

        <HttpGet>
        <Route("api/Pagos/recibo_pago")>
        Public Function recibo_pago(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal usuario As String,
                                      ByVal RESULTADO_PAYW As String, ByVal ID_AFILIACION As String, ByVal FECHA_RSP_CTE As DateTime, ByVal CODIGO_AUT As String,
                                      ByVal REFERENCIA As String, ByVal FECHA_REQ_CTE As DateTime, ByVal Set_Cookie As String, ByVal pago As String, ByVal correo As String) As Cs_Respuesta_pago
            Dim Cs_Pago As New Cs_Pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim Cs_InfoFolio As New ArrayList
            Dim QR As String
            Dim Concepto As New List(Of Cs_Conceptos)
            Dim UpdateValidaQr
            Cs_Pago.id_folio = id_folio
            Cs_Pago.id_concepto = id_concepto
            Cs_Pago.id_depto = id_depto
            Cs_Pago.usuario = usuario
            Cs_Pago.RESULTADO_PAYW = RESULTADO_PAYW
            Cs_Pago.ID_AFILIACION = ID_AFILIACION
            Cs_Pago.FECHA_RSP_CTE = FECHA_RSP_CTE
            Cs_Pago.CODIGO_AUT = CODIGO_AUT
            Cs_Pago.REFERENCIA = REFERENCIA
            Cs_Pago.FECHA_REQ_CTE = FECHA_REQ_CTE
            Cs_Pago.Set_Cookie = Set_Cookie
            Cs_Pago.pago = pago
            Dim conceptos As String = ""
            Dim subtotales As String = ""
            Dim descuentos As String = ""
            Dim totales As String = ""
            Try
                If (id_folio.Count >= 14) Then
                    Cs_InfoFolio = Mdl_InfoFolio.Recupera_datos_folio(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)


                    Concepto = Mdl_InfoFolio.Recupera_Conceptos(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)
                    For Each i In Concepto
                        conceptos += i.ICO_NOMBRE & "  " & Cs_Pago.id_folio.ToString() & "<br><br>"
                        subtotales += FormatCurrency(Convert.ToDouble(i.IIN_SUBTOTAL).ToString()) & "<br><br>"
                        descuentos += FormatCurrency(Convert.ToDouble(i.IIN_DESCUENTO_IMPORTE).ToString()) & "<br><br>"
                        totales += FormatCurrency(Convert.ToDouble(i.IIN_TOTAL).ToString()) & "<br><br>"
                    Next

                    Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

                    Cs_Respuesta_pago.folio = "APD" + id_folio 'Format(id_depto, "00")
                    'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
                    Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

                    Dim cabecero = Cs_Respuesta_pago.Cs_InfoFolio
                    Dim cuantos = Cs_InfoFolio.Count
                    Dim numero2letras = UCase(letras(pago))
                    Dim html = ""

                    Dim FolioRecibo As Integer = 0

                    FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
                    Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)



                    'conceptos = Concepto.ToString() & "  " & Cs_Pago.id_folio.ToString() & "<br><br>"
                    'subtotales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(3)).ToString()) & "<br><br>"
                    'totales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(5)).ToString()) & "<br><br>"

                    Dim TextoDescuento = ""
                    QR = genera_QR(Cs_Respuesta_pago.VQR_CLAVE)
                    If (Convert.ToDouble(Cs_InfoFolio(0)(3)) > Convert.ToDouble(Cs_InfoFolio(0)(5))) Then
                        descuentos = FormatCurrency((Convert.ToDouble(Cs_InfoFolio(0)(3)) - Convert.ToDouble(Cs_InfoFolio(0)(5)))).ToString() & "<br><br>"
                        TextoDescuento = "Subsidio por Incentivo"
                    End If

                    'html = Html_Recibos.Generico(cabecero, cuantos, numero2letras, Cs_Respuesta_pago, pago, id_folio)
                    html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
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
                                                    <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
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
                                                    <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
                                                    <font face='Arial' size='4'><b>N°</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face='Arial' color='red' size='4'><b>607" + String.Format("{0:000000}", FolioRecibo) + "</b></font>  
                                                </div>
                                            </div>
                                            <div class='row'>
                                                <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>NOMBRE:</font><br>  
                                                    <font face='Arial' size='2'>DOMICILIO:</font><br>
                                                    <font face='Arial' size='2'>ACIVIDAD:</font>
                                                </div>
                                                <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>" & cabecero(0)(6) & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                                <div class='col-1' style='padding-top: 3px;'>
                                                    <font face='Arial' size='2'>FECHA:</font><br>  
                                                    <font face='Arial' size='2'>R.F.C:</font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                                <div class='col-2'>
                                                    <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                            </div>
                                            <br>
                                            <div class='row'>
                                                <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
                                                    <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
                                                </div>
                                                <div class='col-9' style='padding-left: 5px;'>
                                                    <table class='table' style='border: 1px solid white;' background='C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg'> 
                                                        <tbody>
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBTOTAL</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>DESCUENTO</b></td>
                                                            <td class='text-center' style='font-size: 13px;'><b>TOTAL</b></td>
                                                          </tr>
                                                          <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
                                                            <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
                                                         </tr>

                                                          <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top:35px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
                                                          </tr>
                                                          <tr> 
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 12px; padding-top: 15px;'>( TC " & (FormatCurrency(Convert.ToDouble(pago))).ToString() & " )</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
                                                          <td style='font-size: 12px; text-align: right; padding-top: 10px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
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
                                                <div class='col-8' style='padding-left: 50px; padding-bottom: 10px;'>
                                                    <div style='border: 2px solid black; width: 100%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
                                                       <font face='Arial' style='font-size: 12px;'>" & numero2letras.ToString() & " M.N.</font> 
                                                    </div>
                                                    <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>  
                                                </div>
                                                           <div class='row'>
                                                <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
                                                      <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
                                                </div> 
                                            </div>
                                                <div class='col-4 text-center' style='padding-top:10px;'>
                                                    <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
                                                </div>
                                            </div> 
                                          </div>
                                    </body>
                                </html>"

                    Dim htmlString As String = html
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
                    ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
                    doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio & ".pdf")
                    doc.Close()

                    'Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
                    'Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")


                End If
            Catch ex As Exception
                Cs_Respuesta_pago.codigo = -1
                Cs_Respuesta_pago.codigoError = 400
                Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
                Cs_Respuesta_pago.folio = ""
            End Try

            Return Cs_Respuesta_pago
        End Function

        ''RUTA NUEVA PARA GENERAR ODP DE LERDO DIGITAL
        <HttpGet>
        <Route("api/Pagos/RealizarPagos")>
        Public Function RealizarPagos(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal usuario As String,
                                      ByVal RESULTADO_PAYW As String, ByVal ID_AFILIACION As String, ByVal FECHA_RSP_CTE As DateTime, ByVal CODIGO_AUT As String,
                                      ByVal REFERENCIA As String, ByVal FECHA_REQ_CTE As DateTime, ByVal Set_Cookie As String, ByVal pago As String, ByVal correo As String) As Cs_Respuesta_pago
            Dim culture As New CultureInfo("en-US")
            Dim Cs_Pago As New Cs_Pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim Cs_InfoFolio As New ArrayList
            Dim QR As String
            'Dim Concepto As String = ""
            Dim Concepto As New List(Of Cs_Conceptos)
            Dim UpdateValidaQr
            Dim folio_replica = id_folio
            Dim sqlString = ""
            Dim FOLO_CATASTRO As String = ""
            Dim ES_MULTA As String = ""
            Cs_Pago.id_folio = id_folio
            Cs_Pago.id_concepto = id_concepto
            Cs_Pago.id_depto = id_depto
            Cs_Pago.usuario = usuario
            Cs_Pago.RESULTADO_PAYW = RESULTADO_PAYW
            Cs_Pago.ID_AFILIACION = ID_AFILIACION
            Cs_Pago.FECHA_RSP_CTE = FECHA_RSP_CTE
            Cs_Pago.CODIGO_AUT = CODIGO_AUT
            Cs_Pago.REFERENCIA = REFERENCIA
            Cs_Pago.FECHA_REQ_CTE = FECHA_REQ_CTE
            Cs_Pago.Set_Cookie = Set_Cookie
            Cs_Pago.pago = pago '.ToString("F2", culture)



            Try
                If (id_depto = 11) Then
                    sqlString = "select TOP 1 'EXISTE FOLO EN INGRESOS' from [SRV_VIALIDAD].[APDSGEDB_PL].DBO.ING_03_INGRESOS i where CDA_FOLIO = '" + id_folio + "' "
                    FOLO_CATASTRO = Mdl_Pagos.RECUPERA_VALOR_STRING(sqlString)
                End If


                If (id_folio.Count >= 14 Or FOLO_CATASTRO = "EXISTE FOLO EN INGRESOS") Then
                    Dim conceptos As String = ""
                    Dim subtotales As String = ""
                    Dim descuentos As String = ""
                    Dim totales As String = ""

                    Cs_InfoFolio = Mdl_InfoFolio.Recupera_datos_folio(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)

                    Cs_Respuesta_pago = Mdl_Pagos.GuardaPago(Cs_Pago, id_depto, id_concepto)
                    Cs_Respuesta_pago = Mdl_Pagos.GeneraCobro(Cs_Pago)

                    UpdateValidaQr = Mdl_Pagos.actualiza_Qr_validacion(Cs_Respuesta_pago.folio)

                    Cs_InfoFolio = Mdl_InfoFolio.Recupera_datos_folio(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)
                    Concepto = Mdl_InfoFolio.Recupera_Conceptos(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)
                    For Each i In Concepto
                        conceptos += i.ICO_NOMBRE & "  " & Cs_Pago.id_folio.ToString() & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(i.IIN_SUBTOTAL)).ToString("F2", culture) & "<br><br>"
                        descuentos += "$" & (Convert.ToDouble(i.IIN_DESCUENTO_IMPORTE)).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(i.IIN_TOTAL)).ToString("F2", culture) & "<br><br>"
                    Next
                    Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

                    Cs_Respuesta_pago.folio = "APD" + id_folio 'Format(id_depto, "00")
                    'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
                    Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

                    Dim cabecero = Cs_Respuesta_pago.Cs_InfoFolio
                    Dim cuantos = Cs_InfoFolio.Count
                    Dim numero2letras = UCase(letras(Cs_Pago.pago))
                    Dim html = ""

                    Dim FolioRecibo As Integer = 0

                    FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
                    Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)



                    'conceptos = Concepto.ToString() & "  " & Cs_Pago.id_folio.ToString() & "<br><br>"
                    'subtotales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(3)).ToString()) & "<br><br>"
                    'totales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(5)).ToString()) & "<br><br>"

                    Dim TextoDescuento = ""
                    QR = genera_QR(Cs_Respuesta_pago.VQR_CLAVE)
                    If (Convert.ToDouble(Cs_InfoFolio(0)(3)) > Convert.ToDouble(Cs_InfoFolio(0)(5))) Then
                        ''  descuentos = FormatCurrency((Convert.ToDouble(Cs_InfoFolio(0)(3)) - Convert.ToDouble(Cs_InfoFolio(0)(5)))).ToString() & "<br><br>"
                        TextoDescuento = "Subsidio por Incentivo"
                    End If
                    sqlString = "EXEC PA_WEB_VERIFICAR_MUTLA_RETORNA_FORMATO_PAGO '" + id_folio + "'"
                    ES_MULTA = Mdl_Pagos.RECUPERA_VALOR_STRING(sqlString)
                    If (ES_MULTA = "-1") Then
                        html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
                                    <head>
                                        <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
                                        <meta content='width=device-width' name='viewport'>
                                        <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                                        <title></title>
                                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3' crossorigin='anonymous'> 
                                    
                                        <style>
                                            .tabla-con-fondo {
                                               background-image: url('C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg');
                                               background-size: 50%; /* Ajusta el tamaño de la imagen para cubrir el fondo */
                                               background-position: left; /* Centra la imagen en el fondo */
                                               background-repeat: no-repeat; /* Evita que la imagen se repita */
                                               /* Agregamos el margin al background */
                                        
                                    
                                                 margin: 0 auto;
                                                 border: 1px solid rgb(255, 255, 255);
                                             }
                                         </style>
                                    </head>
                                    <body style='font-family: Arial, Verdana;'> 
                                        <div class='container-fluid'>
                                            <div class='row'>
                                                <div class='col-3 text-center' style='padding-top:30px;'>
                                                    <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
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
                                                    <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
                                                    <font face='Arial' size='4'><b>N°</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face='Arial' color='red' size='4'><b>818" + String.Format("{0:000000}", FolioRecibo) + "</b></font>  
                                                </div>
                                            </div>
                                            <div class='row'>
                                                <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>NOMBRE:</font><br>  
                                                    <font face='Arial' size='2'>DOMICILIO:</font><br>
                                                    <font face='Arial' size='2'>ACIVIDAD:</font>
                                                </div>
                                                <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>" & cabecero(0)(6) & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                                <div class='col-1' style='padding-top: 3px;'>
                                                    <font face='Arial' size='2'>FECHA:</font><br>  
                                                    <font face='Arial' size='2'>R.F.C:</font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                                <div class='col-2'>
                                                    <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'></font>
                                                </div>
                                            </div>
                                            <br>
                                            <div class='row'>
                                                <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
                                                    <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
                                                    <br><br>
                                                    <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
                                                </div>
                                                <div class='col-9' style='padding-left: 5px;'>
                                                    <table class='table tabla-con-fondo'> 
                                                        <tbody>
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>TOTAL</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBSIDIO</b></td>
                                                            <td class='text-center' style='font-size: 13px;'><b>NETO A PAGAR</b></td>
                                                          </tr>
                                                          <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
                                                            <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
                                                         </tr>

                                                          <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top:35px;'> $" & pago.ToString() & "</td>
                                                          </tr>
                                                          <tr> 
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 12px; padding-top: 15px;'>( TC $" & pago.ToString() & " )</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
                                                          <td style='font-size: 12px; text-align: right; padding-top: 10px;'> $" & pago.ToString() & "</td>
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
                                                    <font face='Arial' style='font-size: 11px;'>Este recibo no es válido sin la certificación de la máquina registradora o sello de la oficina y firma del cajero responsable,<b>LA REPRODUCCIÓN APÓCRIFA DE ESTE COMPROBANTE, CONSTITUYE UN DELITO EN LOS TÉRMINOS DE</b></font>  
                                                    <font face='Arial' style='font-size: 11px;'><b>LAS DISPOSICIÓNES FISCALES.</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <font face='Arial' style='font-size: 11px;'><b>PAGO HECHO EN UNA SOLA EXHIBICIÓN.</b></font>
                                                </div> 
                                         <div class='container'>
                                    <div class='row'>
                                      <div class='col'>
                                        <div  style='padding-left: 50px; padding-bottom: 10px;'>
                                            <div style='border: 2px solid black; width: 75%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
                        
                                                <font face='Arial' style='font-size: 12px;'>" & numero2letras.ToString() & " M.N.</font>
                                            </div>
                                            <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>
                                           </div> 
                                      </div>
                                      <div class='col col-lg-2'>
                                        <div  style='padding-bottom: 10px;'>
                                          <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "'alt='Codigo barras' height='35px'/>
                                            <br><b>CONTRIBUYENTE</b>
                                        </div>
                                      </div>
              
                                    </div>
                                  </div>
          
                                </div>
                            </div>
                        </body>
                        </html>"
                    Else
                        html = ES_MULTA
                        html = html.Replace("[FECHA]", Format(Now, "dd/MM/yyyy hh:mm tt"))
                        html = html.Replace("[FOLIORECIBO]", String.Format("{0:000000}", FolioRecibo))
                        html = html.Replace("[CODEQR]", QR)
                        html = html.Replace("[NUMERO_CON_LETRA]", numero2letras.ToString())
                        html = html.Replace("[CODE_BARRAS]", "data:image/jpg;base64, " + CodigoBarras.ToString())
                    End If


                    Dim htmlString As String = html
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
                    ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
                    doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio & ".pdf")
                    doc.Close()

                    'Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
                    'Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
                    Dim Sql = "select TOP 1 * from [SRV_VIALIDAD].[APDSGEDB_PL].DBO.COR_01_VEHICULOS  WHERE   CVH_ESTATUS = 'RESGUARDO' AND CVH_INFRACCION_FOLIO = '" + Replace(Cs_Respuesta_pago.folio, "APD", "").ToString() + "'"
                    Dim check_corralon = Mdl_Ciudadanos.Ejecuta_sql_select(Sql)

                    If check_corralon.Count > 0 Then
                        Cs_Respuesta_pago.CORRALON = "SI"
                    Else
                        Cs_Respuesta_pago.CORRALON = "NO"
                    End If




                    Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
                    Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")

                Else
                    Dim tipo As String = ""
                    If (id_folio.Count < 11) Then
                        tipo = 2
                    Else
                        tipo = 1
                    End If

                    Cs_InfoFolio = Mdl_Catastro.traer_info_catastro(id_folio, tipo)

                    Cs_Respuesta_pago = Mdl_Pagos.GuardaPagoCatastro(Cs_Pago, id_depto, id_concepto)
                    Cs_Respuesta_pago = Mdl_Pagos.GeneraCobroCatastro(Cs_Pago)
                    UpdateValidaQr = Mdl_Pagos.actualiza_Qr_validacion(Cs_Respuesta_pago.folio)
                    Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

                    Cs_Respuesta_pago.folio = "APD" + IIN_CLAVE
                    'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
                    Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

                    Dim detalle


                    For Each c In Cs_InfoFolio
                        detalle = c.ItemArray
                    Next

                    Dim numero2letras = UCase(letras(pago))

                    Dim conceptos As String = ""
                    Dim subtotales As String = ""
                    Dim descuentos As String = ""
                    Dim totales As String = ""

                    Dim FolioRecibo As Integer = 0
                    FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
                    Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)
                    QR = genera_QR(Cs_Respuesta_pago.VQR_CLAVE)

                    If Not IsNothing(detalle(34)) AndAlso detalle(34) > 0 Then
                        conceptos += "IMPUESTO DEL EJERCICIO 2024 PREDIAL<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
                        ''totales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(34)) - Convert.ToDouble(detalle(60)).ToString()) & "<br><br>"

                        ''subtotales += "$" & (Convert.ToDouble(i.IIN_SUBTOTAL)).ToString("F2", culture) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(34))).ToString("F2", culture) & "<br><br>"
                        totales += (Convert.ToDouble(detalle(34)) - Convert.ToDouble(detalle(60))).ToString("F2", culture) & "<br><br>"

                    End If
                    If Not IsNothing(detalle(35)) AndAlso detalle(35) > 0 Then
                        conceptos += "REZAGOS DE EJERCICIOS ANTERIORES<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(35))).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(detalle(35))).ToString("F2", culture) & "<br><br>"

                    End If
                    If Not IsNothing(detalle(41)) AndAlso detalle(41) > 0 Then
                        conceptos += "GASTOS DE EJECUCION PREDIAL<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(41))).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(detalle(41))).ToString("F2", culture) & "<br><br>"
                    End If
                    If Not IsNothing(detalle(42)) AndAlso detalle(42) > 0 Then
                        conceptos += "MULTAS PREDIAL<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(42))).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(detalle(42))).ToString("F2", culture) & "<br><br>"
                    End If
                    If Not IsNothing(detalle(43)) AndAlso detalle(43) > 0 Then
                        conceptos += "RECARGOS DE EJERCICIOS ANTERIORES<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(43))).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(detalle(43))).ToString("F2", culture) & "<br><br>"
                    End If
                    If Not IsNothing(detalle(44)) AndAlso detalle(44) > 0 Then
                        conceptos += "DIFERENCIAS<br><br>"
                        'subtotales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
                        'totales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
                        subtotales += "$" & (Convert.ToDouble(detalle(44))).ToString("F2", culture) & "<br><br>"
                        totales += "$" & (Convert.ToDouble(detalle(44))).ToString("F2", culture) & "<br><br>"
                    End If
                    'If Not IsNothing(detalle(63)) AndAlso detalle(63) > 0 Then
                    '    conceptos += "ACTUALIZACION<br><br>"
                    '    subtotales += FormatCurrency(Convert.ToDouble(detalle(63)).ToString()) & "<br><br>"
                    '    totales += FormatCurrency(Convert.ToDouble(detalle(63)).ToString()) & "<br><br>"
                    'End If
                    If Not IsNothing(detalle(60)) AndAlso detalle(60) > 0 Then
                        'descuentos = FormatCurrency(Convert.ToDouble(detalle(60)).ToString()) & "<br>"
                        descuentos = "$" & (Convert.ToDouble(detalle(60))).ToString("F2", culture) & "<br><br>"

                    End If

                    Dim year = Format(detalle(4), "yyyy")
                    Dim mes = Format(detalle(4), "MM")

                    Dim TextoDescuento = ""
                    If (mes = "01") Then
                        TextoDescuento = "PAGO OPORTUNO 1o MES DEL 2024"
                    ElseIf (mes = "02") Then
                        TextoDescuento = "PAGO OPORTUNO 2o MES DEL 2024"
                    ElseIf (mes = "03") Then
                        TextoDescuento = "PAGO OPORTUNO 3o MES DEL 2024"
                    Else
                        TextoDescuento = ""
                    End If

                    Dim html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
                                    <head>
                                        <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
                                        <meta content='width=device-width' name='viewport'>
                                        <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                                        <title></title>
                                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3' crossorigin='anonymous'> 
                                    
                                           <style>
                                           .tabla-con-fondo {
                                              background-image: url('C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg');
                                              background-size: 50%; /* Ajusta el tamaño de la imagen para cubrir el fondo */
                                              background-position: left; /* Centra la imagen en el fondo */
                                              background-repeat: no-repeat; /* Evita que la imagen se repita */
                                              /* Agregamos el margin al background */
                                       
                                   
                                                margin: 0 auto;
                                                border: 1px solid rgb(255, 255, 255);
                                            }
                                        </style>


                                    </head>
                                    <body style='font-family: Arial, Verdana;'> 
                                        <div class='container-fluid'>
                                            <div class='row'>
                                                <div class='col-3 text-center' style='padding-top:30px;'>
                                                    <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
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
                                                    <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
                                                    <font face='Arial' size='4'><b>N°</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face='Arial' color='red' size='4'><b>818" + String.Format("{0:000000}", FolioRecibo) + "</b></font>  

                                                </div>
                                            </div>
                                            <div class='row'>
                                                <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>NOMBRE:</font><br>  
                                                    <font face='Arial' size='2'>DOMICILIO:</font><br>
                                                    <font face='Arial' size='2'>ACIVIDAD:</font>
                                                </div>
                                                <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
                                                    <font face='Arial' size='2'>" & detalle(7) & "</font><br>  
                                                    <font face='Arial' size='2'>" & detalle(8) & "</font><br>
                                                    <font face='Arial' size='2'><b>VALOR CATASTRAL $ " & (Convert.ToDouble(detalle(31))).ToString("F2", culture) & "</b></font>
                                                </div>
                                                <div class='col-1' style='padding-top: 3px;'>
                                                    <font face='Arial' size='2'>FECHA:</font><br>  
                                                    <font face='Arial' size='2'>R.F.C:</font><br>
                                                    <font face='Arial' size='2'>CVE CTR:</font>
                                                </div>
                                                <div class='col-2'>
                                                    <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
                                                    <font face='Arial' size='2'></font><br>
                                                    <font face='Arial' size='2'>" & id_folio & "</font>
                                                </div>
                                            </div>
                                            <br>
                                            <div class='row'>
                                                <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
                                                    <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
                                                    <br><br>
                                                    <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
                                                </div>
                                                <div class='col-9' style='padding-left: 5px;'>
                                                    <table class='table tabla-con-fondo'> 
                                                        <tbody>
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>TOTAL</b></td>
                                                            <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBSIDIO</b></td>
                                                            <td class='text-center' style='font-size: 13px;'><b>NETO A PAGAR</b></td>
                                                          </tr>
                                                          <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
                                                            <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
                                                         </tr>

                                                          <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
                                                            <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br>
                                                                                                                        " + (Convert.ToInt32(year) - 1).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 2).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 3).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 4).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        " + (Convert.ToInt32(year) - 5).ToString() + "</td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
                                                            <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top:35px;'> $ " & pago.ToString() & "</td>
                                                          </tr>
                                                          <tr> 
                                                          <tr style='border: 2px solid black;'>
                                                            <td style='font-size: 12px; padding-top: 15px;'>( TC $ " & pago.ToString() & " )</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
                                                            <td style='font-size: 12px; text-align: right; padding-top: 10px;'> $ " & pago.ToString() & "</td>
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
                                                    <font face='Arial' style='font-size: 11px;'>Este recibo no es válido sin la certificación de la máquina registradora o sello de la oficina y firma del cajero responsable,<b>LA REPRODUCCIÓN APÓCRIFA DE ESTE COMPROBANTE, CONSTITUYE UN DELITO EN LOS TÉRMINOS DE</b></font>  
                                                    <font face='Arial' style='font-size: 11px;'><b>LAS DISPOSICIÓNES FISCALES.</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <font face='Arial' style='font-size: 11px;'><b>PAGO HECHO EN UNA SOLA EXHIBICIÓN.</b></font>
                                                </div> 
                                                 <div class='container'>
                                                    <div class='row'>
                                                      <div class='col'>
                                                        <div  style='padding-left: 50px; padding-bottom: 10px;'>
                                                            <div style='border: 2px solid black; width: 75%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
                        
                                                                <font face='Arial' style='font-size: 14px;'>" & numero2letras.ToString() & " M.N.</font>
                                                            </div>
                                                            <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>
                                                           </div> 
                                                      </div>
                                                      <div class='col col-lg-2'>
                                                        <div  style='padding-bottom: 10px;'>
                                                             <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
                                                            <br><b style='margin-left:50px;'>CONTRIBUYENTE</b>
                                                        </div>
                                                      </div>
              
                                                    </div>
                                                  </div>
          
                                                </div>
                                            </div>
                                        </body>
                                        </html>"



                    Dim htmlString As String = html
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
                    ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
                    doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
                    doc.Close()

                    'Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
                    'Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")


                    Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
                    Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")

                End If
            Catch ex As Exception
                Cs_Respuesta_pago.codigo = -1
                Cs_Respuesta_pago.codigoError = 400
                Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
                Cs_Respuesta_pago.folio = ""
            End Try

            '' enviar folio a replica. METODO PARA PAGAR. AGREGADO EN ABRIL 2024
            Try
                If id_depto = 2 Or id_depto = 34 Then
                    'notas este SP corre en el servidor de mutipagos.
                    Try

                        'Dim CS_Facturas_Lista As New CS_Facturas_Lista
                        sqlString = "EXEC [DBO].[PA_Traer_ACTUALIZA_DATA_SRV_VIALIDAD_A_SRV_MULTIPAGOS] '" & folio_replica & "'"
                        Dim Aux2 As String = Mdl_Pagos.RECUPERA_VALOR_STRING_REPLICAS(sqlString)    ' RETORNA UN VALOR STRING
                        Cs_Respuesta_pago.mensaje = Cs_Respuesta_pago.mensaje + "|| Enviado a pagos y " + Aux2
                    Catch ex As Exception
                        Cs_Respuesta_pago.mensaje = Cs_Respuesta_pago.mensaje + " || " + "Error, " + ex.Message
                    End Try

                End If
            Catch ex As Exception
                Cs_Respuesta_pago.mensaje = Cs_Respuesta_pago.mensaje + " || " + "Error, " + ex.Message
            End Try
            '/* AGREGADO EN ABRIL 2024*/
            Return Cs_Respuesta_pago
        End Function

        ''RUTA NUEVA PARA GENERAR ODP DE CORRALON
        <HttpPost>
        <Route("api/Pagos/ODP_Corralon")>
        Public Function ODP_Corralon(<FromBody()> ByVal data As Object)
            Dim culture As New CultureInfo("en-US")
            Dim Cs_Pago As New Cs_Pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim QR As String
            Dim Concepto As New List(Of Cs_Conceptos)
            Dim sql As String = ""
            Dim FOLIO = data("FOLIO").ToString()
            Dim CORREO = data("CORREO").ToString()
            Try

                Dim Sp_ODP_corralon = Mdl_Pagos.GeneraODP_corralon(FOLIO)
                If Sp_ODP_corralon = 99 Then
                    Cs_Respuesta_pago.mensaje = "ERROR AL EJECUTAR EL SP"
                    Cs_Respuesta_pago.codigo = -1
                    Cs_Respuesta_pago.codigoError = 400
                    Return Cs_Respuesta_pago
                Else

                    sql = "select CVH_FOLIO from [SRV_VIALIDAD].[APDSGEDB_PL].DBO.COR_01_VEHICULOS  WHERE   CVH_ESTATUS = 'RESGUARDO'AND CVH_INFRACCION_FOLIO = '" + FOLIO + "'"
                    Dim CVH_FOLIO = Mdl_Ciudadanos.Ejecuta_sql_select(sql)
                    sql = "SELECT top 1
                           CONVERT(NVARCHAR,A.AYT_FECALTA,103) AS AYT_FECALTA
                            ,A.AYT_IMPORTE
                            ,CASE WHEN A.AYT_DESCRIPCION = '' OR A.AYT_DESCRIPCION IS NULL THEN 'S/N' ELSE A.AYT_DESCRIPCION END as AYT_DESCRIPCION
                            ,CASE WHEN C.AYT_NOMBRE = '' OR C.AYT_NOMBRE IS NULL THEN 'S/N' ELSE C.AYT_NOMBRE END AS AYT_NOMBRE
                            ,CASE WHEN C.AYT_DIRECCION = '' OR C.AYT_DIRECCION IS NULL THEN 'S/N' ELSE C.AYT_DIRECCION END  AS AYT_DIRECCION
                            FROM [SRV_VIALIDAD].[APDSGEDB_PL].DBO.AYT_01_COBROS A LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].DBO.AYT_02_CIUDADANOS C ON C.ACI_CLAVE=A.ACI_CLAVE
                            WHERE AYT_FOLIO='" + CVH_FOLIO(0)(0).ToString() + "' ORDER BY AYT_CLAVE DESC"

                    Dim datos_ODP = Mdl_Ciudadanos.Ejecuta_sql_select(sql)
                    Dim AYT_FECALTA = datos_ODP(0)(0).ToString()
                    Dim AYT_IMPORTE = datos_ODP(0)(1)
                    Dim AYT_DESCRIPCION = datos_ODP(0)(2).ToString()
                    Dim AYT_NOMBRE = datos_ODP(0)(3).ToString()
                    Dim AYT_DIRECCION = datos_ODP(0)(4).ToString()
                    Concepto = Mdl_InfoFolio.Recupera_Conceptos_ODP_corralon(CVH_FOLIO(0)(0).ToString())
                    'For Each i In Concepto
                    '    conceptos += i.ICO_NOMBRE & "  " & FOLIO & "<br><br>"
                    '    totales += FormatCurrency(Convert.ToDouble(i.IIN_TOTAL).ToString()) & "<br><br>"
                    'Next
                    Dim concepto1 = Concepto.Item(0).ICO_NOMBRE
                    Dim concepto2 = Concepto.Item(1).ICO_NOMBRE
                    Dim total1 = Concepto.Item(0).IIN_TOTAL
                    Dim total2 = Concepto.Item(1).IIN_TOTAL
                    Dim numero2letras = UCase(letras(AYT_IMPORTE))
                    Dim html = ""
                    Dim CodigoBarras = generarCodigo(CVH_FOLIO(0)(0).ToString())
                    sql = "SELECT TOP 1 VQR_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL_BIB_DIGITAL].DBO.[QR_01_VALIDACION] WHERE VQR_TIPO = 6  AND VGR_FOLIO = '" + CVH_FOLIO(0)(0).ToString() + "' ORDER BY 1 DESC"
                    Dim VQR_CLAVE = Mdl_Ciudadanos.Ejecuta_sql_select(sql)
                    If VQR_CLAVE.Count = 0 Then
                        QR = genera_QR("1")
                    Else
                        QR = genera_QR(VQR_CLAVE(0)(0).ToString())
                    End If
                    'QR = genera_QR(VQR_CLAVE(0)(0).ToString())
                    html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
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
                                                                <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
                                                            </div>
                                                            <div class='col-6 text-center' style='padding-top:50px;'>
                                                                <font face='Arial' style='font-size: 25px;'><b>ORDEN DE PAGO CORRALON</b></font><br> 
                                                            </div>
                                                            <div class='col-3 text-center' style='padding-top:50px;'>
                                                                <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
                  
                                                            </div>
                                                        </div>
                                                        <div class='row'>
                                                            <div class='col-1' style='padding-left: 50px; padding-top: 5px;'>
                                                                <font face='Arial' size='3'><b>FOLIO:</b></font><br><br> 
                                                                <font face='Arial' size='3'><b>CIUDADANO:</b></font><br><br> 
                                                                <font face='Arial' size='3'><b>DIRECCION:</b></font><br> 
                                                            </div>
                                                            <div class='col-8' style='padding-left: 75px; padding-top: 5px;'>
                                                                <font face='Arial' size='3'>" + CVH_FOLIO(0)(0).ToString() + "</font><br><br>   
                                                                <font face='Arial' size='3'>" + AYT_NOMBRE.ToString() + "</font><br><br> 
                                                                <font face='Arial' size='3'>" + AYT_DIRECCION.ToString() + "</font>
                                                            </div>
                                                            <div class='col-1' style='padding-top: 5px;'>
                                                                <font face='Arial' size='3'><b>FECHA:&nbsp&nbsp" + AYT_FECALTA.ToString() + "</b></font><br>  
           
                                                                <font face='Arial' size='3'></font>
                                                                       <div class='col-4 text-center' style='padding-top:10px;'>
                                                          
                                                           <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
                                                            </div>
                                                            </div>
                                                            <div class='col-2'>
                                                                <font face='Arial' size='2'></font><br>  
                                                                <font face='Arial' size='2'></font><br>
                                                                <font face='Arial' size='2'></font>
                                                            </div>
                                                        </div>
                                                        <br> <br> <br>
                                                        <div class='row'>
           
                                                            <div class='col-11' style='padding-left: 51px;'>
                   
                                                                    <table class='table' style='border: 1px solid black;'>
                                                                <!--<table class='table' style='border: 1px solid white; background: url(C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg) no-repeat;'>--> 
                                                                    <tbody>
                                                                      <tr style='border: 2px solid black;'>
                                                                        <td style='font-size: 14px; border-right: 2px solid black;text-align: center;'><b>CANTIDAD</b></td>
                                                                        <td class='text-left' style='font-size: 14px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
                                                                        <td class='text-center' style='font-size: 14px; border-right: 2px solid black;'><b>PRECIO</b></td>
                                                                        <td class='text-center' style='font-size: 14px;'><b>SUBTOTAL</b></td>
                                                                      </tr>
                                                                      <tr style='border-left: 2px solid black; border-right: 2px solid black;'> 
                                                                        <td style='font-size: 13px; border-right: 2px solid black;text-align: center;'>1</td>
                                                                        <td style='font-size: 13px; text-align: left; border-right: 2px solid black;'>" + concepto1.ToString() + " " + CVH_FOLIO(0)(0).ToString() + "</td>
                                                                        <td style='font-size: 13px; text-align: right; border-right: 2px solid black;'> $ " + total1.ToString() + "</td>
                                                                        <td style='font-size: 13px; text-align: right;'> $ " + total1.ToString() + "</td>
                                                                     </tr>

                                                                     <tr style='border-left: 2px solid black; border-right: 2px solid black;'> 
                                                                        <td style='font-size: 13px; border-right: 2px solid black;text-align: center;'>1</td>
                                                                        <td style='font-size: 13px; text-align: left; border-right: 2px solid black;'>" + concepto2.ToString() + " " + CVH_FOLIO(0)(0).ToString() + "</td>
                                                                        <td style='font-size: 13px; text-align: right; border-right: 2px solid black;'> $ " + total2.ToString() + "</td>
                                                                        <td style='font-size: 13px; text-align: right;'> $ " + total2.ToString() + "</td>
                                                                     </tr>
                                                                      <tr> 
                                                                      <tr style='border: 2px solid black;'>
                           
                                                                      </tr> 
                                                                      </tr>
                                                                    </tbody>
                                                                </table>

                
                                                            </div>    
                                                            <div class='col-5' style='padding-left: 51px;'>
                                                                <p><b>OBSERVACIONES:</b></p>
                                                               <p style='font-size: 13px;'>" + AYT_DESCRIPCION.ToString() + "</p>     
                                                            </div>   
                                                            <div class='col-6' style='padding-left: 200px;'>
                                                                <p style='font-size: 14px;'><b>SUBTOTAL:&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp $ " + (Convert.ToDouble(AYT_IMPORTE)).ToString("F2", culture) + "</b></p>
                                                                <p style='font-size: 14px;'><b>TOTAL A PAGAR:&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp $" + (Convert.ToDouble(AYT_IMPORTE)).ToString("F2", culture) + "</b></p>
                                                            </div>

                                                            <div class='col-5' style='padding-left: 50px; padding-top: 40px;'> 
                                                                <font face='Arial' size='3'><b>IMPORTE EN LETRA:</b></font><br>  
                                                            </div>
                                                            <div class='col-8' style='padding-left: 50px; padding-bottom: 10px;'>
                                                                <div style='border: 2px solid black; width: 100%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
                                                            
                                                             <font face='Arial' style='font-size: 12px;'>" & numero2letras.ToString() & " </font> 

                                                                </div>
                
                                                            </div>
                                                           <div class='row'>
                                                                <div class='col-9' style='padding-left: 30px; padding-bottom: 10px;'>
                                                             
                                                                    <img src='" + QR + "' class='rounded float-start' alt='QR' width='100px'height='100px'>
                                                                </div> 
                                                            </div>
                                                     
                                                        </div> 
                                                      </div>
                                                </body>
                                            </html>
                                            "
                    Dim htmlString As String = html
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
                    doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\ordenes\" & CVH_FOLIO(0)(0) & ".pdf")
                    doc.Close()
                    Dim bool As Boolean = ENVIAR_CORREO_CORRALON(CVH_FOLIO(0)(0).ToString(), CVH_FOLIO(0)(0).ToString(), CORREO, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\ordenes\" & CVH_FOLIO(0)(0) & ".pdf")
                    Cs_Respuesta_pago.codigo = 200
                    Cs_Respuesta_pago.codigoError = 200
                    Cs_Respuesta_pago.mensaje = "ORDEN DE PAGO GENERADO CORRECTAMENTE,EL DOCUMENTO FUÉ ENVIADO AL CORREO: " + CORREO + ""
                    Cs_Respuesta_pago.folio = ""
                End If
            Catch ex As Exception
                Cs_Respuesta_pago.codigo = -1
                Cs_Respuesta_pago.codigoError = 400
                Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
                Cs_Respuesta_pago.folio = ""
            End Try

            Return Cs_Respuesta_pago
        End Function

        'COPIA DE GENERAR PAGOS PARA CAMBIAR LO DEL RECIBO ,UN SOLO CONCEPTO ARROJA
        '<HttpGet>
        '<Route("api/Pagos/RealizarPagos")>
        'Public Function RealizarPagos(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer, ByVal usuario As String,
        '                              ByVal RESULTADO_PAYW As String, ByVal ID_AFILIACION As String, ByVal FECHA_RSP_CTE As DateTime, ByVal CODIGO_AUT As String,
        '                              ByVal REFERENCIA As String, ByVal FECHA_REQ_CTE As DateTime, ByVal Set_Cookie As String, ByVal pago As Double, ByVal correo As String) As Cs_Respuesta_pago
        '    Dim Cs_Pago As New Cs_Pago
        '    Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
        '    Dim Cs_InfoFolio As New ArrayList
        '    Dim QR As String
        '    Dim Concepto As String = ""
        '    Dim UpdateValidaQr
        '    Cs_Pago.id_folio = id_folio
        '    Cs_Pago.id_concepto = id_concepto
        '    Cs_Pago.id_depto = id_depto
        '    Cs_Pago.usuario = usuario
        '    Cs_Pago.RESULTADO_PAYW = RESULTADO_PAYW
        '    Cs_Pago.ID_AFILIACION = ID_AFILIACION
        '    Cs_Pago.FECHA_RSP_CTE = FECHA_RSP_CTE
        '    Cs_Pago.CODIGO_AUT = CODIGO_AUT
        '    Cs_Pago.REFERENCIA = REFERENCIA
        '    Cs_Pago.FECHA_REQ_CTE = FECHA_REQ_CTE
        '    Cs_Pago.Set_Cookie = Set_Cookie
        '    Cs_Pago.pago = pago

        '    Try
        '        If (id_folio.Count >= 14) Then
        '            Cs_InfoFolio = Mdl_InfoFolio.Recupera_datos_folio(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)

        '            Cs_Respuesta_pago = Mdl_Pagos.GuardaPago(Cs_Pago, id_depto, id_concepto)
        '            Cs_Respuesta_pago = Mdl_Pagos.GeneraCobro(Cs_Pago)

        '            UpdateValidaQr = Mdl_Pagos.actualiza_Qr_validacion(Cs_Respuesta_pago.folio)


        '            Cs_InfoFolio = Mdl_InfoFolio.Recupera_datos_folio(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)
        '            Concepto = Mdl_InfoFolio.RecuperaConcepto(Cs_Pago.id_folio, Cs_Pago.id_depto, Cs_Pago.id_concepto)

        '            Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

        '            Cs_Respuesta_pago.folio = "APD" + id_folio 'Format(id_depto, "00")
        '            'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
        '            Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

        '            Dim cabecero = Cs_Respuesta_pago.Cs_InfoFolio
        '            Dim cuantos = Cs_InfoFolio.Count
        '            Dim numero2letras = UCase(letras(pago))
        '            Dim html = ""

        '            Dim FolioRecibo As Integer = 0

        '            FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
        '            Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)

        '            Dim conceptos As String = ""
        '            Dim subtotales As String = ""
        '            Dim descuentos As String = ""
        '            Dim totales As String = ""

        '            conceptos = Concepto.ToString() & "  " & Cs_Pago.id_folio.ToString() & "<br><br>"
        '            subtotales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(3)).ToString()) & "<br><br>"
        '            totales = FormatCurrency(Convert.ToDouble(Cs_InfoFolio(0)(5)).ToString()) & "<br><br>"

        '            Dim TextoDescuento = ""
        '            QR = genera_QR(Cs_Respuesta_pago.VQR_CLAVE)
        '            If (Convert.ToDouble(Cs_InfoFolio(0)(3)) > Convert.ToDouble(Cs_InfoFolio(0)(5))) Then
        '                descuentos = FormatCurrency((Convert.ToDouble(Cs_InfoFolio(0)(3)) - Convert.ToDouble(Cs_InfoFolio(0)(5)))).ToString() & "<br><br>"
        '                TextoDescuento = "Subsidio por Incentivo"
        '            End If

        '            'html = Html_Recibos.Generico(cabecero, cuantos, numero2letras, Cs_Respuesta_pago, pago, id_folio)
        '            html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
        '                            <head>
        '                                <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
        '                                <meta content='width=device-width' name='viewport'>
        '                                <meta content='IE=edge' http-equiv='X-UA-Compatible'>
        '                                <title></title>
        '                                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3' crossorigin='anonymous'> 
        '                            </head>
        '                            <body style='font-family: Arial, Verdana;'> 
        '                                <div class='container-fluid'>
        '                                    <div class='row'>
        '                                        <div class='col-3 text-center' style='padding-top:30px;'>
        '                                            <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
        '                                        </div>
        '                                        <div class='col-6 text-center' style='padding-top:50px;'>
        '                                            <font face='Arial' style='font-size: 22px;'><b>PRESIDENCIA MUNICIPAL LERDO DURANGO</b></font><br> 
        '                                            <font face='Arial' size='3'><b>TESORERÍA MUNICIPAL</b></font><br>  
        '                                            <font face='Arial' size='3'>AV. FRANCISCO SARABIA NO.3 NTE<br>COL. CENTRO, CP: 35150, TEL: 871 175 0000 <br> R.F.C PMC951010FE3</font>
        '                                            <br><br>
        '                                            <font face='Arial' size='5'><b>R&nbsp;&nbsp;E&nbsp;&nbsp;C&nbsp;&nbsp;I&nbsp;&nbsp;B&nbsp;&nbsp;O&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                           D&nbsp;&nbsp;E&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                           P&nbsp;&nbsp;A&nbsp;&nbsp;G&nbsp;&nbsp;O</b></font><br><br>
        '                                        </div>
        '                                        <div class='col-3 text-center' style='padding-top:50px;'>
        '                                            <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>
        '                                            <font face='Arial' size='4'><b>N°</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face='Arial' color='red' size='4'><b>607" + String.Format("{0:000000}", FolioRecibo) + "</b></font>  
        '                                        </div>
        '                                    </div>
        '                                    <div class='row'>
        '                                        <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>NOMBRE:</font><br>  
        '                                            <font face='Arial' size='2'>DOMICILIO:</font><br>
        '                                            <font face='Arial' size='2'>ACIVIDAD:</font>
        '                                        </div>
        '                                        <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>" & cabecero(0)(6) & "</font><br>  
        '                                            <font face='Arial' size='2'></font><br>
        '                                            <font face='Arial' size='2'></font>
        '                                        </div>
        '                                        <div class='col-1' style='padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>FECHA:</font><br>  
        '                                            <font face='Arial' size='2'>R.F.C:</font><br>
        '                                            <font face='Arial' size='2'></font>
        '                                        </div>
        '                                        <div class='col-2'>
        '                                            <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
        '                                            <font face='Arial' size='2'></font><br>
        '                                            <font face='Arial' size='2'></font>
        '                                        </div>
        '                                    </div>
        '                                    <br>
        '                                    <div class='row'>
        '                                        <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
        '                                            <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
        '                                        </div>
        '                                        <div class='col-9' style='padding-left: 5px;'>
        '                                            <table class='table' style='border: 1px solid white;' background='C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg'> 
        '                                                <tbody>
        '                                                  <tr style='border: 2px solid black;'>
        '                                                    <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
        '                                                    <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBTOTAL</b></td>
        '                                                    <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>DESCUENTO</b></td>
        '                                                    <td class='text-center' style='font-size: 13px;'><b>TOTAL</b></td>
        '                                                  </tr>
        '                                                  <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
        '                                                    <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
        '                                                    <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
        '                                                 </tr>

        '                                                  <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
        '                                                    <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br></td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top:35px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
        '                                                  </tr>
        '                                                  <tr> 
        '                                                  <tr style='border: 2px solid black;'>
        '                                                    <td style='font-size: 12px; padding-top: 15px;'>( TC " & (FormatCurrency(Convert.ToDouble(pago))).ToString() & " )</td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
        '                                                  <td style='font-size: 12px; text-align: right; padding-top: 10px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
        '                                                  </tr> 
        '                                                  </tr>
        '                                                </tbody>
        '                                            </table>
        '                                        </div>       
        '                                        <div class='col-1'>
        '                                            <div style='writing-mode: vertical-lr; transform: rotate(270deg); width: 200px; height: 250px; padding-top: 70px;'>
        '                                                <font face='Arial' style='font-size: 17px;'>Pago realizado en el portal <b>https://www.lerdodigital.mx</b></font>
        '                                            </div>
        '                                        </div>
        '                                        <div class='col-3' style='padding-left: 50px; padding-top: 40px;'> 
        '                                            <font face='Arial' size='3'><b>CANTIDAD CON LETRA:</b></font><br>  
        '                                        </div>
        '                                        <div class='col-1'> 
        '                                            <font face='Arial' style='font-size: 10px;'><b>Nota Importante:</b></font><br>  
        '                                        </div>
        '                                        <div class='col-8' style='padding-right: 45px;'>
        '                                            <font face='Arial' style='font-size: 11px;'></font><br> 
        '                                            <font face='Arial' style='font-size: 11px;'><b>LA REPRODUCCIÓN APÓCRIFA DE ESTE COMPROBANTE, CONSTITUYE UN DELITO EN LOS TÉRMINOS DE</b></font>  
        '                                            <font face='Arial' style='font-size: 11px;'><b>LAS DISPOSICIÓNES FISCALES.</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                            <font face='Arial' style='font-size: 11px;'><b>PAGO HECHO EN UNA SOLA EXHIBICIÓN.</b></font>
        '                                        </div> 
        '                                        <div class='col-8' style='padding-left: 50px; padding-bottom: 10px;'>
        '                                            <div style='border: 2px solid black; width: 100%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
        '                                               <font face='Arial' style='font-size: 12px;'>" & numero2letras.ToString() & " M.N.</font> 
        '                                            </div>
        '                                            <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>  
        '                                        </div>
        '                                                   <div class='row'>
        '                                        <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
        '                                              <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
        '                                        </div> 
        '                                    </div>
        '                                        <div class='col-4 text-center' style='padding-top:10px;'>
        '                                            <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
        '                                        </div>
        '                                    </div> 
        '                                  </div>
        '                            </body>
        '                        </html>"

        '            Dim htmlString As String = html
        '            Dim pdf_page_size As String = "A4"
        '            Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize),
        '                    pdf_page_size, True), PdfPageSize)
        '            Dim pdf_orientation As String = "Portrait"
        '            Dim pdfOrientation As PdfPageOrientation = DirectCast(
        '                    [Enum].Parse(GetType(PdfPageOrientation),
        '                    pdf_orientation, True), PdfPageOrientation)
        '            Dim webPageWidth As Integer = 1024
        '            Try
        '                webPageWidth = Convert.ToInt32(1024)
        '            Catch
        '            End Try
        '            Dim webPageHeight As Integer = 0
        '            Try
        '                webPageHeight = Convert.ToInt32(768)
        '            Catch
        '            End Try

        '            Dim converter As New HtmlToPdf()
        '            converter.Options.PdfPageSize = pageSize
        '            converter.Options.PdfPageOrientation = pdfOrientation
        '            converter.Options.WebPageWidth = webPageWidth
        '            converter.Options.WebPageHeight = webPageHeight
        '            Dim doc As PdfDocument = converter.ConvertHtmlString(htmlString)
        '            ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
        '            doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio & ".pdf")
        '            doc.Close()

        '            'Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
        '            'Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")

        '            Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
        '            Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")

        '        Else
        '            Dim tipo As String = ""
        '            If (id_folio.Count < 11) Then
        '                tipo = 2
        '            Else
        '                tipo = 1
        '            End If

        '            Cs_InfoFolio = Mdl_Catastro.traer_info_catastro(id_folio, tipo)

        '            Cs_Respuesta_pago = Mdl_Pagos.GuardaPagoCatastro(Cs_Pago, id_depto, id_concepto)
        '            Cs_Respuesta_pago = Mdl_Pagos.GeneraCobroCatastro(Cs_Pago)
        '            UpdateValidaQr = Mdl_Pagos.actualiza_Qr_validacion(Cs_Respuesta_pago.folio)
        '            Dim IIN_CLAVE As String = Cs_Respuesta_pago.folio ' 27257

        '            Cs_Respuesta_pago.folio = "APD" + IIN_CLAVE
        '            'Cs_Respuesta_pago.folio = Cs_Respuesta_pago.folio + Format(Int32.Parse(IIN_CLAVE), "00000000")
        '            Cs_Respuesta_pago.Cs_InfoFolio = Cs_InfoFolio

        '            Dim detalle


        '            For Each c In Cs_InfoFolio
        '                detalle = c.ItemArray
        '            Next

        '            Dim numero2letras = UCase(letras(pago))

        '            Dim conceptos As String = ""
        '            Dim subtotales As String = ""
        '            Dim descuentos As String = ""
        '            Dim totales As String = ""

        '            Dim FolioRecibo As Integer = 0
        '            FolioRecibo = Mdl_InfoFolio.RecuperarFolioRecibo()
        '            Dim CodigoBarras = generarCodigo(Cs_Respuesta_pago.folio)
        '            QR = genera_QR(Cs_Respuesta_pago.VQR_CLAVE)

        '            If Not IsNothing(detalle(34)) AndAlso detalle(34) > 0 Then
        '                conceptos += "IMPUESTO DEL EJERCICIO 2023 PREDIAL<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(34)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(35)) AndAlso detalle(35) > 0 Then
        '                conceptos += "REZAGOS DE EJERCICIOS ANTERIORES<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(35)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(41)) AndAlso detalle(41) > 0 Then
        '                conceptos += "GASTOS DE EJECUCION PREDIAL<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(41)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(42)) AndAlso detalle(42) > 0 Then
        '                conceptos += "MULTAS PREDIAL<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(42)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(43)) AndAlso detalle(43) > 0 Then
        '                conceptos += "RECARGOS DE EJERCICIOS ANTERIORES<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(43)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(44)) AndAlso detalle(44) > 0 Then
        '                conceptos += "DIFERENCIAS<br><br>"
        '                subtotales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
        '                totales += FormatCurrency(Convert.ToDouble(detalle(44)).ToString()) & "<br><br>"
        '            End If
        '            If Not IsNothing(detalle(60)) AndAlso detalle(60) > 0 Then
        '                descuentos = FormatCurrency(Convert.ToDouble(detalle(60)).ToString()) & "<br>"
        '            End If

        '            Dim year = Format(detalle(4), "yyyy")
        '            Dim mes = Format(detalle(4), "MM")

        '            Dim TextoDescuento = ""
        '            If (mes = "01") Then
        '                TextoDescuento = "PAGO OPORTUNO 1o MES DEL 2023"
        '            ElseIf (mes = "02") Then
        '                TextoDescuento = "PAGO OPORTUNO 2o MES DEL 2023"
        '            ElseIf (mes = "03") Then
        '                TextoDescuento = "PAGO OPORTUNO 3o MES DEL 2023"
        '            Else
        '                TextoDescuento = ""
        '            End If

        '            Dim html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
        '                            <head>
        '                                <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
        '                                <meta content='width=device-width' name='viewport'>
        '                                <meta content='IE=edge' http-equiv='X-UA-Compatible'>
        '                                <title></title>
        '                                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3' crossorigin='anonymous'> 
        '                            </head>
        '                            <body style='font-family: Arial, Verdana;'> 
        '                                <div class='container-fluid'>
        '                                    <div class='row'>
        '                                        <div class='col-3 text-center' style='padding-top:30px;'>
        '                                            <img alt='Lerdo Digital' src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' width='200px'>
        '                                        </div>
        '                                        <div class='col-6 text-center' style='padding-top:50px;'>
        '                                            <font face='Arial' style='font-size: 22px;'><b>PRESIDENCIA MUNICIPAL LERDO DURANGO</b></font><br> 
        '                                            <font face='Arial' size='3'><b>TESORERÍA MUNICIPAL</b></font><br>  
        '                                            <font face='Arial' size='3'>AV. FRANCISCO SARABIA NO.3 NTE<br>COL. CENTRO, CP: 35150, TEL: 871 175 0000 <br> R.F.C PMC951010FE3</font>
        '                                            <br><br>
        '                                            <font face='Arial' size='5'><b>R&nbsp;&nbsp;E&nbsp;&nbsp;C&nbsp;&nbsp;I&nbsp;&nbsp;B&nbsp;&nbsp;O&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                           D&nbsp;&nbsp;E&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                           P&nbsp;&nbsp;A&nbsp;&nbsp;G&nbsp;&nbsp;O</b></font><br><br>
        '                                        </div>
        '                                        <div class='col-3 text-center' style='padding-top:50px;'>
        '                                            <img alt='APD' src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' width='190px'><br>

        '                                        </div>
        '                                    </div>
        '                                    <div class='row'>
        '                                        <div class='col-1' style='padding-left: 50px; padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>NOMBRE:</font><br>  
        '                                            <font face='Arial' size='2'>DOMICILIO:</font><br>
        '                                            <font face='Arial' size='2'>ACIVIDAD:</font>
        '                                        </div>
        '                                        <div class='col-8' style='padding-left: 55px; padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>" & detalle(7) & "</font><br>  
        '                                            <font face='Arial' size='2'>" & detalle(8) & "</font><br>
        '                                            <font face='Arial' size='2'><b>VALOR CATASTRAL " & (FormatCurrency(Convert.ToDouble(detalle(31)))).ToString() & "</b></font>
        '                                        </div>
        '                                        <div class='col-1' style='padding-top: 3px;'>
        '                                            <font face='Arial' size='2'>FECHA:</font><br>  
        '                                            <font face='Arial' size='2'>R.F.C:</font><br>
        '                                            <font face='Arial' size='2'>CVE CTR:</font>
        '                                        </div>
        '                                        <div class='col-2'>
        '                                            <font face='Arial' size='2'>" & Format(Now, "dd/MM/yyyy hh:mm tt") & "</font><br>  
        '                                            <font face='Arial' size='2'></font><br>
        '                                            <font face='Arial' size='2'>" & id_folio & "</font>
        '                                        </div>
        '                                    </div>
        '                                    <br>
        '                                    <div class='row'>
        '                                        <div class='col-2' style='padding-left: 50px; padding-top: 38px;'>
        '                                            <img alt='SAT' src='C:\inetpub\API_LERDO_DIGITAL\img\sat.jpg' width='120px'>
        '                                        </div>
        '                                        <div class='col-9' style='padding-left: 5px;'>
        '                                            <table class='table' style='border: 1px solid white;' background='C:\inetpub\API_LERDO_DIGITAL\img\lerdologoMA.jpeg'> 
        '                                                <tbody>
        '                                                  <tr style='border: 2px solid black;'>
        '                                                    <td style='font-size: 13px; border-right: 2px solid black;'><b>CONCEPTO</b></td>
        '                                                    <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>SUBTOTAL</b></td>
        '                                                    <td class='text-center' style='font-size: 13px; border-right: 2px solid black;'><b>DESCUENTO</b></td>
        '                                                    <td class='text-center' style='font-size: 13px;'><b>TOTAL</b></td>
        '                                                  </tr>
        '                                                  <tr style='height: 175px; border-left: 2px solid black; border-right: 2px solid black;'> 
        '                                                    <td style='font-size: 12px; border-right: 2px solid black;'>" & conceptos & "</td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & subtotales & "</td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'>" & descuentos & "</td>
        '                                                    <td style='font-size: 12px; text-align: right;'>" & totales & "</td>
        '                                                 </tr>

        '                                                  <tr style='border-left: 2px solid black; border-right: 2px solid black;'>
        '                                                    <td style='font-size: 12px; border-right: 2px solid black;'>" + TextoDescuento + "<br><br>
        '                                                                                                                " + (Convert.ToInt32(year) - 1).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                                                                " + (Convert.ToInt32(year) - 2).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                                                                " + (Convert.ToInt32(year) - 3).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                                                                " + (Convert.ToInt32(year) - 4).ToString() + ",&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                                                                                                " + (Convert.ToInt32(year) - 5).ToString() + "</td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black;'></td>
        '                                                    <td style='font-size: 12px; text-align: right; border-right: 2px solid black; padding-top:35px;'>Total:</td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top:35px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
        '                                                  </tr>
        '                                                  <tr> 
        '                                                  <tr style='border: 2px solid black;'>
        '                                                    <td style='font-size: 12px; padding-top: 15px;'>( TC " & (FormatCurrency(Convert.ToDouble(pago))).ToString() & " )</td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top: 15px;'></td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top: 10px;'>Pago Con:</td>
        '                                                    <td style='font-size: 12px; text-align: right; padding-top: 10px;'>" & (FormatCurrency(Convert.ToDouble(pago))).ToString() & "</td>
        '                                                  </tr> 
        '                                                  </tr>
        '                                                </tbody>
        '                                            </table>
        '                                        </div>       
        '                                        <div class='col-1'>
        '                                            <div style='writing-mode: vertical-lr; transform: rotate(270deg); width: 200px; height: 250px; padding-top: 70px;'>
        '                                                <font face='Arial' style='font-size: 17px;'>Pago realizado en el portal <b>https://www.lerdodigital.mx</b></font>
        '                                            </div>
        '                                        </div>
        '                                        <div class='col-3' style='padding-left: 50px; padding-top: 40px;'> 
        '                                            <font face='Arial' size='3'><b>CANTIDAD CON LETRA:</b></font><br>  
        '                                        </div>
        '                                        <div class='col-1'> 
        '                                            <font face='Arial' style='font-size: 10px;'><b>Nota Importante:</b></font><br>  
        '                                        </div>
        '                                        <div class='col-8' style='padding-right: 45px;'>
        '                                            <font face='Arial' style='font-size: 11px;'></font><br> 
        '                                            <font face='Arial' style='font-size: 11px;'><b>LA REPRODUCCIÓN APÓCRIFA DE ESTE COMPROBANTE, CONSTITUYE UN DELITO EN LOS TÉRMINOS DE</b></font>  
        '                                            <font face='Arial' style='font-size: 11px;'><b>LAS DISPOSICIÓNES FISCALES.</b></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        '                                            <font face='Arial' style='font-size: 11px;'><b>PAGO HECHO EN UNA SOLA EXHIBICIÓN.</b></font>
        '                                        </div> 
        '                                        <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
        '                                            <div style='border: 2px solid black; width: 100%; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;'>
        '                                               <font face='Arial' style='font-size: 14px;'>" & numero2letras.ToString() & " M.N.</font> 
        '                                            </div>
        '                                            <font face='Arial' style='font-size: 14px;'><b>PUEDES GENERAR FACTURA EN www.lerdodigital.mx con el Folio " & Cs_Respuesta_pago.folio & "</b></font>  
        '                                        </div>
        '                                         <div class='row'>
        '                                        <div class='col-9' style='padding-left: 50px; padding-bottom: 10px;'>
        '                                              <img src='" + QR + "' class='rounded float-start' alt='QR' width='90px'height='90px'>
        '                                        </div> 
        '                                    </div>
        '                                        <div class='col-3 text-center' style='padding-top:10px;'>
        '                                            <img src='data:image/jpg;base64, " + CodigoBarras.ToString() + "' alt='Codigo barras' height='35px'/>
        '                                        </div>
        '                                    </div> 
        '                                  </div>
        '                            </body>
        '                        </html>"

        '            Dim htmlString As String = html
        '            Dim pdf_page_size As String = "A4"
        '            Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize),
        '            pdf_page_size, True), PdfPageSize)
        '            Dim pdf_orientation As String = "Portrait"
        '            Dim pdfOrientation As PdfPageOrientation = DirectCast(
        '            [Enum].Parse(GetType(PdfPageOrientation),
        '            pdf_orientation, True), PdfPageOrientation)
        '            Dim webPageWidth As Integer = 1024
        '            Try
        '                webPageWidth = Convert.ToInt32(1024)
        '            Catch
        '            End Try
        '            Dim webPageHeight As Integer = 0
        '            Try
        '                webPageHeight = Convert.ToInt32(768)
        '            Catch
        '            End Try

        '            Dim converter As New HtmlToPdf()
        '            converter.Options.PdfPageSize = pageSize
        '            converter.Options.PdfPageOrientation = pdfOrientation
        '            converter.Options.WebPageWidth = webPageWidth
        '            converter.Options.WebPageHeight = webPageHeight
        '            Dim doc As PdfDocument = converter.ConvertHtmlString(htmlString)
        '            ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
        '            doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
        '            doc.Close()

        '            'Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")
        '            'Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio.Replace("-", "") & ".pdf")


        '            Dim bool As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")
        '            Dim bool2 As Boolean = ENVIAR_CORREO(Cs_Respuesta_pago.folio, id_folio, "cobrosrealizados@gmail.com", "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & id_folio.Replace("-", "") & ".pdf")

        '        End If
        '    Catch ex As Exception
        '        Cs_Respuesta_pago.codigo = -1
        '        Cs_Respuesta_pago.codigoError = 400
        '        Cs_Respuesta_pago.mensaje = "Error, " + ex.Message
        '        Cs_Respuesta_pago.folio = ""
        '    End Try

        '    Return Cs_Respuesta_pago
        'End Function
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
	                            font-size: 12px;
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
            				                            <h2>Pago Realizado</h2><br>
            				                            <h3 ><strong>FOLIO</strong>: " +
            EMAIL_ENCABEZADO + "</h3>
            				                            <h3 ><strong>FOLIO PARA FACTURACIÓN</strong>: " +
            EMAIL_PIE_PAGINA + "</h3><br>
            				                            <p>Su pago a sido realizado, adjuntamos su comprobante de pago. </p> 
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
		              	                            <p style='text-align:center;'>Ahora podras facturar tus pagos desde la página oficial de lerdo digital.</p>
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
                Dim RESPUESTA_AR() As String = Split(Mdl_Pagos.RECUPERA_VALOR_STRING("SELECT (CCW_CORREO + ';;--;;'+ CCW_PASSWORD_APLICACION + ';;--;;' + CCW_PUERTO + ';;--;;' +CCW_HOST) RESPUESTA
                                                                                      FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_34_CORREOS_WEB WHERE CCW_CLAVE = 2 "), ";;--;;")

                'Dim fromaddr As String = "lerdopagos@gmail.com"
                Dim fromaddr As String = RESPUESTA_AR(0)
                Dim toaddr As String = EMAIL_DESTINATARIO
                'Dim password As String = "hzkcvupbbmmgyglt"
                Dim password As String = RESPUESTA_AR(1)

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "RECIBO DE PAGO FOLIO " + EMAIL_ENCABEZADO + " PORTAL LERDO DIGITAL"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                msg.Attachments.Add(New Attachment(archivopdf))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                'smtp.Host = "smtp.gmail.com"
                'smtp.Port = 587
                smtp.Host = RESPUESTA_AR(3)
                smtp.Port = Integer.Parse(RESPUESTA_AR(2))
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
        Public Function ENVIAR_CORREO_CORRALON(ByVal EMAIL_PIE_PAGINA As String, ByVal EMAIL_ENCABEZADO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_MENSAJITO As String, ByVal EMAIL_ENCABEZADO_SUPERVISOR As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String) As Boolean
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
	                            font-size: 12px;
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
            				                            <h2>ORDEN DE PAGO</h2><br>
            				                            <h3 ><strong>FOLIO</strong>: " +
            EMAIL_ENCABEZADO + "</h3>
            				                           
            				                            <p>HA GENERADO SU ORDEN DE PAGO, ADJUNTAMOS SU ORDEN EN PDF. </p> 
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
								            			                            <h3>ORDEN DE PAGO CORRALON</h3>
								            			
								            		                            </div>
								            	                            </div>
								            	                            <div class='services-list'>
								            		                            <img src='images/checked.png' alt='' style='width: 50px; max-width: 600px; height: auto; display: block;'>
								            		                       
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
		              	                            <p style='text-align:center;'>Ahora podras facturar tus pagos desde la página oficial de lerdo digital.</p>
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
            Dim Band As Boolean = ENVIA_MAIL_CORRALON(EMAIL_CUERPO, EMAIL_DESTINATARIO, EMAIL_ENCABEZADO, LLEVA_ATACHMENT, NOM_ATACHMENT, archivopdf)

            Return Band
        End Function
        Public Function ENVIA_MAIL_CORRALON(ByVal EMAIL_CUERPO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_ENCABEZADO As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String) As Boolean
            Try
                Dim RESPUESTA_AR() As String = Split(Mdl_Pagos.RECUPERA_VALOR_STRING("SELECT (CCW_CORREO + ';;--;;'+ CCW_PASSWORD_APLICACION + ';;--;;' + CCW_PUERTO + ';;--;;' +CCW_HOST) RESPUESTA
                                                                                      FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_34_CORREOS_WEB WHERE CCW_CLAVE = 2 "), ";;--;;")

                'Dim fromaddr As String = "lerdopagos@gmail.com"
                Dim fromaddr As String = RESPUESTA_AR(0)
                Dim toaddr As String = EMAIL_DESTINATARIO
                'Dim password As String = "hzkcvupbbmmgyglt"
                Dim password As String = RESPUESTA_AR(1)


                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "ORDE DE PAGO CORRALON " + EMAIL_ENCABEZADO + " PORTAL LERDO DIGITAL"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                msg.Attachments.Add(New Attachment(archivopdf))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                'smtp.Host = "smtp.gmail.com"
                'smtp.Port = 587
                smtp.Host = RESPUESTA_AR(3)
                smtp.Port = Integer.Parse(RESPUESTA_AR(2))

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
        Public Function letras(ByVal nCifra As Object) As String

            Dim numero2letras = Mdl_Pagos.RECUPERA_VALOR_STRING("Select dbo.CantidadConLetra(" + nCifra + ")")
            Return numero2letras
            '            ' Defino variables
            '            Dim cifra, bloque, decimales, cadena As String
            '#Disable Warning BC42024 ' Variable local sin usar: 'longituid'.
            '            Dim longituid, posision, unidadmil As Byte
            '#Enable Warning BC42024 ' Variable local sin usar: 'longituid'.
            '            ' En caso de que unidadmil sea:
            '            ' 0 = cientos
            '            ' 1 = miles
            '            ' 2 = millones
            '            ' 3 = miles de millones
            '            ' 4 = billones
            '            ' 5 = miles de billones
            '            ' Reemplazo el símbolo decimal por un punto (.) y luego guardo la parte entera y la decimal por separado
            '            ' Es necesario poner el cero a la izquierda del punto así si el valor es de sólo decimales, se lo fuerza
            '            ' a colocar el cero para que no genere error
            '            cifra = Format(CType(nCifra, Decimal), "###############0.#0")
            '            decimales = Mid(cifra, Len(cifra) - 1, 2)
            '            cifra = Left(cifra, Len(cifra) - 3)
            '            ' Verifico que el valor no sea cero
            '            If cifra = "0" Then
            '                Return IIf(decimales = "00", "cero", "cero con " & decimales & "/100")
            '            End If
            '            ' Evaluo su longitud (como mínimo una cadena debe tener 3 dígitos)
            '            If Len(cifra) < 3 Then
            '                cifra = Rellenar(cifra, 3)
            '            End If
            '            ' Invierto la cadena
            '            cifra = Invertir(cifra)
            '            ' Inicializo variables
            '            posision = 1
            '            unidadmil = 0
            '            cadena = ""
            '            ' Selecciono bloques de a tres cifras empezando desde el final (de la cadena invertida)
            '            Do While posision <= Len(cifra)
            '                ' Selecciono una porción del numero
            '                bloque = Mid(cifra, posision, 3)
            '                ' Transformo el número a cadena
            '                cadena = Convertir(bloque, unidadmil) & " " & cadena.Trim
            '                ' Incremento la cantidad desde donde seleccionar la subcadena
            '                posision = posision + 3
            '                ' Incremento la posisión de la unidad de mil
            '                unidadmil = unidadmil + 1
            '            Loop
            ' Cargo la función
            'Return IIf(decimales = "00", cadena.Trim.ToLower, cadena.Trim.ToLower & " con " & decimales & "/100")
        End Function
        Private Function Convertir(ByVal cadena As String, ByVal unidadmil As Byte) As String
            ' Defino variables
            Dim centena, decena, unidad As Byte
            ' Invierto la subcadena (la original habia sido invertida en el procedimiento NumeroATexto)
            cadena = Invertir(cadena)
            ' Determino la longitud de la cadena
            If Len(cadena) < 3 Then
                cadena = Rellenar(cadena, 3)
            End If
            ' Verifico que la cadena no esté vacía (000)
            If cadena = "000" Then
                Return ""
            End If
            ' Desarmo el numero (empiezo del dígito cero por el manejo de cadenas de VB.NET)
            centena = CType(cadena.Substring(0, 1), Byte)
            decena = CType(cadena.Substring(1, 1), Byte)
            unidad = CType(cadena.Substring(2, 1), Byte)
            cadena = ""
            ' Calculo las centenas
            If centena <> 0 Then
                Dim centenas() As String = {"", IIf(decena = 0 And unidad = 0, "cien", "ciento"), "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos"}
                cadena = centenas(centena)
            End If
            ' Calculo las decenas
            If decena <> 0 Then
                Dim decenas() As String = {"", IIf(unidad = 0, "diez", IIf(unidad >= 6, "dieci", IIf(unidad = 1, "once", IIf(unidad = 2, "doce", IIf(unidad = 3, "trece", IIf(unidad = 4, "catorce", "quince")))))), IIf(unidad = 0, "veinte", "venti"), "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa"}
                cadena = cadena & " " & decenas(decena)
            End If
            ' Calculo las unidades (no pregunten por que este IF es necesario ... simplemente funciona) 
            If decena = 1 And unidad < 6 Then
            Else
                Dim unidades() As String = {"", IIf(decena <> 1, IIf(unidadmil = 1, "un", "uno"), ""), "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve"}
                If decena >= 3 And unidad <> 0 Then
                    cadena = cadena.Trim & " y "
                End If
                If decena = 0 Then
                    cadena = cadena.Trim & " "
                End If
                cadena = cadena & unidades(unidad)
            End If
            ' Evaluo la posision de miles, millones, etc 
            If unidadmil <> 0 Then
                Dim agregado() As String = {"", "mil", IIf((centena = 0) And (decena = 0) And (unidad = 1), "millón", "millones"), "mil millones", "billones", "mil billones"}
                If (centena = 0) And (decena = 0) And (unidad = 1) And unidadmil = 2 Then
                    cadena = "un"
                End If
                cadena = cadena & " " & agregado(unidadmil)
            End If
            ' Cargo la función 
            Return cadena.Trim
        End Function
        Public Function Invertir(ByVal cadena As String) As String
            ' Defino variables 
            Dim retornar As String
            ' Inviero la cadena
            For posision As Short = cadena.Length To 1 Step -1
#Disable Warning BC42104 ' La variable 'retornar' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
                retornar = retornar & cadena.Substring(posision - 1, 1)
#Enable Warning BC42104 ' La variable 'retornar' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
            Next
            ' Retorno la cadena invertida
            Return retornar
        End Function
        Public Function Rellenar(ByVal valor As Object, ByVal cifras As Byte) As String
            ' Defino variables
            Dim cadena As String
            ' Verifico el valor pasado
            If Not IsNumeric(valor) Then
                valor = 0
            Else
                valor = CType(valor, Integer)
            End If
            ' Cargo la cadena
            cadena = valor.ToString.Trim
            ' Relleno con los ceros que sean necesarios para llenar los dígitos pedidos
            For puntero As Byte = (Len(cadena) + 1) To cifras
                cadena = "0" & cadena
            Next puntero
            ' Cargo la función
            Return cadena
        End Function
        ' ================== 3D Secure ===================================================================
        <HttpGet>
        <Route("api/Pagos/Guarda3Dsecure")>
        Public Function Guarda3Dsecure(ByVal FOLIO As String, ByVal NUM_TARJETA As String, ByVal ECI As String, ByVal CardType As String, ByVal XID As String,
                                       ByVal CAVV As String, ByVal Status As String, ByVal Reference3D As String, ByVal NUM_PROCESO As String)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim Cs_3dSecure As New Cs_3DSecure

            Cs_3dSecure.FOLIO = FOLIO
            Cs_3dSecure.NUM_TARJETA = NUM_TARJETA
            Cs_3dSecure.ECI = ECI
            Cs_3dSecure.CardType = CardType
            Cs_3dSecure.XID = XID
            Cs_3dSecure.CAVV = CAVV
            Cs_3dSecure.Status = Status
            Cs_3dSecure.Reference3D = Reference3D
            Cs_3dSecure.NUM_PROCESO = NUM_PROCESO

            Try
                Cs_Respuesta = Mdl_Pagos.Gauarda3DSecure(Cs_3dSecure)
            Catch ex As Exception
                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Correcto"
            End Try

            Return Cs_Respuesta
        End Function

        <HttpGet>
        <Route("api/Pagos/Recupera3Dsecure")>
        Public Function Recupera3Dsecure(ByVal FOLIO As String, ByVal NUM_TARJETA As String)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim Cs_3dSecure As New Cs_3DSecure
            Dim L_Cs_3DSecure As New List(Of Cs_3DSecure)

            Cs_3dSecure.FOLIO = FOLIO
            Cs_3dSecure.NUM_TARJETA = NUM_TARJETA

            Try
                L_Cs_3DSecure = Mdl_Pagos.Recupera3DSecure(Cs_3dSecure)
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Correcto"

                Return Cs_Respuesta
            End Try

            Return L_Cs_3DSecure
        End Function
        Private Function generarCodigo(ByVal texto As String) As String
            Dim codigoBarras As New pdf.Barcode128

            codigoBarras.CodeType = pdf.Barcode.CODE128
            codigoBarras.ChecksumText = True
            codigoBarras.GenerateChecksum = True
            codigoBarras.StartStopText = True
            codigoBarras.Code = texto
            Dim bm As New System.Drawing.Bitmap(codigoBarras.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White))

            Dim bitmapBytes As Byte()
            Using stream As New System.IO.MemoryStream
                bm.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg)
                bitmapBytes = stream.ToArray
            End Using

            Return Convert.ToBase64String(bitmapBytes)
        End Function

        ' ================================================= prueba de estructira del cambio.
        <HttpGet>
        <Route("api/pagos/testEstructuirapago")>
        Public Function testEstructura(ByVal folio As String, ByVal correo As String, ByVal importe As String, ByVal iin_clave As String, foliorecibo As String)
            Dim SqlString As String,
                ES_MULTA As String,
                html As String = ""

            'Dim numero2letras = UCase(letras(importe))

            Dim numero2letras = Mdl_Pagos.RECUPERA_VALOR_STRING("Select dbo.CantidadConLetra(" + importe + ")")
            Dim CodigoBarras = generarCodigo(folio)
            Dim QR = genera_QR(iin_clave)

            SqlString = "EXEC PA_WEB_VERIFICAR_MUTLA_RETORNA_FORMATO_PAGO '" + folio + "'"

            ES_MULTA = Mdl_Pagos.RECUPERA_VALOR_STRING(SqlString)
            If (ES_MULTA = "-1") Then
                html = "NO ES MULTA"
            Else
                html = ES_MULTA
                html = html.Replace("[FECHA]", Format(Now, "dd/MM/yyyy hh:mm tt"))
                html = html.Replace("[FOLIORECIBO]", foliorecibo)
                html = html.Replace("[CODEQR]", QR)
                html = html.Replace("[NUMERO_CON_LETRA]", numero2letras)
                html = html.Replace("[CODE_BARRAS]", "data:image/jpg;base64, " + CodigoBarras)
            End If


            Dim htmlString As String = html
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
            ' doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "Recibos\" & id_folio & ".pdf")
            doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\TEST_" & folio & ".pdf")
            doc.Close()

            'Dim bool As Boolean = ENVIAR_CORREO("APD" & folio, folio, correo, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "documentacion\QR\recibos\" & folio.Replace("-", "") & ".pdf")

            Return html
        End Function

    End Class
End Namespace