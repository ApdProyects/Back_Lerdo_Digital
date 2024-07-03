Imports System.Web.Mvc

Namespace Models
    Public Class HTML_Recibos
        Inherits Controller

        Function Generico(cabecero, cuantos, numero2letras, Cs_Respuesta_pago, pago, id_folio) As String
            Dim html As String
            Dim variable = cabecero(0)

            html = "<html lang='es'>
                      <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1'>
                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1' crossorigin='anonymous'>

                        <title>Hello, world!</title>
                      </head>
                      </head>
                      <body style='padding-top: 40px; padding-left: 40px; padding-right: 40px; font-family: Verdana; font-size: 13px;'>
                        <div class='container'>
                          <div class='row'>
                            <div class='col-md-12' style='text-align: right; color:red'>
                              <p style='font-size: 16px;'><b>" + variable(0).ToString() + "</b></p>
                            </div> 
                        </div>
                          <div class='row' style='border-bottom: 3px solid gray;'>
                            <div class='col-md-3'>
                                <img src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_p6nto0_amibzm.jpg' alt='...' style='width: 100%; height:130px;'>
                            </div>
                            <div class='col-md-6' style='border-top: 3px solid gray; background-color: #eee9e8;'> 
                                  <div class='row'>
                                    <div class='col-md-12' style='padding-top: 3px; text-align: center;'>
                                        <h5><b>TESORERIA MUNICIPAL</b></h5>
                                        <h5>AV. FRANCISCO SARABIA NO 3 NORTE</h5>
                                        <h5>LERDO, DURANGO C.P. 35150</h5>
                                        <h5><b>COMPROBANTE DE PAGO</b></h5>
                                    </div> 
                                  </div>
                            </div>
                            <div class='col-md-3'>
                                <img src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_2_tqrpf0_v43whg.png' alt='...' style='width: 100%; height: 130px;'>
                            </div>
                          </div>
                          <div class='row' style='padding-top: 30px;'>
                            <div class='col-md-7' style='border: 2px solid black; padding-top:10px; padding-bottom:5px;'>
                              <h6><b>Nombre: </b>" + variable(6).ToString() + "</h6>
                              <h6><b>Estatus: </b>" + variable(14).ToString() + "</h6>
                              <h6><b>Garantîa: </b>" + variable(13).ToString() + "</h6>
                            </div>
                            <div class='col-md-5' style='border-top: 2px solid black; border-right: 2px solid black; border-bottom: 2px solid black; padding-top:10px; padding-bottom:5px;'>
                              <h6><b>Fecha: </b>" + Format(variable(1), "dd/MM/yyyy") + "</h6>
                              <h6><b>FOLIO FACTURACIÓN: </b>" + Cs_Respuesta_pago.folio + "</h6>
                            </div>
                          </div>
                          <div class='row' style='padding-top: 10px;'>
                            <div class='col-md-12' style='text-align:center; border: 2px solid black; background-color: #eee9e8; padding-top: 7px;'>
                              <h6><b>CONCEPTO DE INGRESOS</b></h6>
                            </div>
                          </div>
                          <div class='row'>
                            <div class='col-md-10' style='background-color: #eee9e8; padding-top: 7px; border-bottom: 2px solid black; border-left: 2px solid black;'>
                              <h6><b>CONCEPTO</b></h6>
                            </div>
                            <div class='col-md-2' style='background-color: #eee9e8; padding-top: 7px; border-bottom: 2px solid black; border-right: 2px solid black;text-align: right; padding-right: 10px;'>
                              <h6><b>IMPORTE</b></h6>
                            </div>
                          </div>
                          <div class='row' style='border-right: 2px solid black; border-left: 2px solid black; height: 200px;'>      
                            <div class='col-md-10' style='padding-top: 7px;'>"
            If cuantos > 1 Then
                For Each d In cabecero
                    html = html + "<h6>" + d(18).ToString() + "</h6> "
                Next
            Else
                html = html + "<h6>" + variable(18).ToString() + "</h6> "
            End If
            html = html + "</div> 
                            <div class='col-md-2' style='padding-top: 7px; text-align: right;'> 
                            </div> 
                          </div>
                          <div class='row' style='border-bottom: 2px solid black; border-right: 2px solid black; border-left: 2px solid black;'>
                            <div class='col-md-10' style='padding-top: 7px;'>
                              <h6>TOTAL:</h6>
                            </div> 
                            <div class='col-md-2' style='padding-top: 7px; text-align: right; padding-right: 10px;'>
                              <h6><b>" + FormatCurrency(pago) + "</b></h6>
                            </div>
                          </div>
                          <div class='row' style='border-bottom: 2px solid black; border-right: 2px solid black; border-left: 2px solid black; background-color: #eee9e8;'>
                            <div class='col-md-7' style='padding-top: 10px;'>
                              <h6><b>CANTIDAD CON LETRA:</b></h6>
                              <h6>" + numero2letras + " M.N.</h6>
                            </div> 
                            <div class='col-md-5' style='padding-top: 10px; text-align: right; padding-right: 20px;'>
                              <h6><b>Folio:</b> " + id_folio + "</h6>
                            </div>
                            <br>
                            <div class='col-md-12' style='padding-top: 10px;'>
                              <h6><b>Cobrada el día:</b> " + StrConv(FormatDateTime(Now, DateFormat.LongDate), vbProperCase).ToString() + "</h6>
                            </div>  
                          </div>
                          <div class='row' style='border-bottom: 2px solid black; border-right: 2px solid black; border-left: 2px solid black;'>
                            <div class='col-md-12' style='padding-top: 30px; text-align: center; background-color: gray;'>
                              <p style='font-size: 12px;'><b>APD CONSULTORES EN TECNOLOGÍAS DE LA INFORMACIÓN.</b></p>
                            </div>  
                          </div> 
                      </body>
                    </html>"

            Return html
        End Function
        Function VialidadPermisos(cabecero, detalle, numero2letras, Cs_Respuesta_pago, pago, id_folio) As String
            Dim html As String

            html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head>
                                <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
                                <meta content='width=device-width' name='viewport'>
                                <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                                <title></title>
                                <style type='text/css'>
                                    body {
                                        margin: 0;
                                        padding: 0;
                                    }
                                    table,
                                    td,
                                    tr {
                                        vertical-align: top;
                                        border-collapse: collapse;
                                    }
                                    * {
                                        line-height: inherit;
                                    }
                                    a[x-apple-data-detectors=true] {
                                        color: inherit !important;
                                        text-decoration: none !important;
                                    }
                                </style>
                                <style id='media-query' type='text/css'>
                                    @media (max-width: 720px) { 
                                        .block-grid,
                                        .col {
                                            min-width: 320px !important;
                                            max-width: 100% !important;
                                            display: block !important;
                                        } 
                                        .block-grid {
                                            width: 100% !important;
                                        } 
                                        .col {
                                            width: 100% !important;
                                        } 
                                        .col>div {
                                            margin: 0 auto;
                                        } 
                                        img.fullwidth,
                                        img.fullwidthOnMobile {
                                            max-width: 100% !important;
                                        } 
                                        .no-stack .col {
                                            min-width: 0 !important;
                                            display: table-cell !important;
                                        } 
                                        .no-stack.two-up .col {
                                            width: 50% !important;
                                        } 
                                        .no-stack .col.num4 {
                                            width: 33% !important;
                                        } 
                                        .no-stack .col.num8 {
                                            width: 66% !important;
                                        }
                                        .no-stack .col.num4 {
                                            width: 33% !important;
                                        }
                                        .no-stack .col.num3 {
                                            width: 25% !important;
                                        }
                                        .no-stack .col.num6 {
                                            width: 50% !important;
                                        }
                                        .no-stack .col.num9 {
                                            width: 75% !important;
                                        }
                                        .video-block {
                                            max-width: none !important;
                                        }
                                        .mobile_hide {
                                            min-height: 0px;
                                            max-height: 0px;
                                            max-width: 0px;
                                            display: none;
                                            overflow: hidden;
                                            font-size: 0px;
                                        }
                                        .desktop_hide {
                                            display: block !important;
                                            max-height: none !important;
                                        }
                                    }
                                </style>
                            </head>
                            <body class='clean-body' style='margin: 0; padding: 0; -webkit-text-size-adjust: 100%; background-color: #fbfbfb;'>
                                <table bgcolor='#fbfbfb' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='table-layout: fixed; vertical-align: top; min-width: 320px; Margin: 0 auto; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fbfbfb; width: 100%;' valign='top' width='100%'>
                                    <tbody>
                                        <tr style='vertical-align: top;' valign='top'>
                                            <td style='word-break: break-word; vertical-align: top;' valign='top'>
                                                <div style='background-color:transparent;'>
                                                    <div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>
                                                        <div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>
                                                            <div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>
                                                                <div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>
                                                                            <img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_p6nto0_amibzm.jpg' style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class='col num6' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 348px; width: 350px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>
                                                                            <tbody>
                                                                                <tr style='vertical-align: top;' valign='top'>
                                                                                    <td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 45px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>
                                                                                        <table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #e2dfdf; width: 100%;' valign='top' width='100%'>
                                                                                            <tbody>
                                                                                                <tr style='vertical-align: top;' valign='top'>
                                                                                                    <td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>
                                                                                                </tr>
                                                                                            </tbody>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                        <div style='color:#555555;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:5px;padding-right:10px;padding-bottom:0px;padding-left:10px;'>
                                                                            <div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #555555; mso-line-height-alt: 14px;'>
                                                                                <p style='font-size: 18px; line-height: 1.2; word-break: break-word; text-align: center; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 22px; margin: 0;'>
                                                                                    <span style='text:center;font-size: 12px;'>
                                                                                        <strong style='font-size: 14px;'>TESORERIA MUNICIPAL </strong><br>
                                                                                        AV. FRANCISCO SARABIA NO. 3NTE<br>
                                                                                        R.F.C PMC951010FE3<br>
                                                                                       <strong style='font-size: 15px;'> RECIBO DE PAGO</strong>
                                                                                    </span></p>
                                                                            </div>
                                                                        </div>
                                           
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>
                                                
                                                                            <div style='font-size:1px;line-height:25px'>&nbsp;</div>
                                                
                                                                            <img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_2_tqrpf0_v43whg.png' style='text-decoration:
                                                                            none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;'
                                                                            title='Alternate text' width='175'>
                                             
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div> 
                                                <div style='background-color:transparent;'>
                                                    <div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color:rgba(0,0,0,0);'>
                                                        <div style='border-collapse: collapse;display: table;width: 100%;background-color:rgba(0,0,0,0);'>
                                                            <div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:0px; padding-bottom:0px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div style='font-size:16px;text-align:center;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif'>
                                                                            <table style='width:100%;background:white;margin-bottom:10px;font-size: 11px;;text-align:left '>
                                                                                <tbody> 
                                                                                    <tr style='padding-top:15px; padding-bottom: 15px;'>
                                                                                        <td style='border:2px solid black;    width: 60%;'>
                                                                                            NOMBRE: " +
                                                                                            cabecero(6) + "<br>
                                                                                            ESTATUS: " +
                                                                                            cabecero(14) + "<br>
                                                                                            GARANTÍA: " +
                                                                                            cabecero(13) + "
                                                                                        </td>         
                                                                                        <td style='text-align:left; border:2px solid black;'>
                                                                                            FECHA: " +
                                                                                            Format(cabecero(1), "dd/MM/yyyy") + "<br>
                                                                                            FOLIO FACTURACIÓN: " +
                                                                                            Cs_Respuesta_pago.folio + "<br>
                                                                                    </tr> 
                                                                                </tbody> 
                                                                            </table><table style='width:100%;background:white; border:2px solid black;height: 200px;'>
                                                                                <thead style='background:white;border-bottom:2px solid black'>
                                                                                    <tr style='border-bottom:1px solid black;text-align:left;font-size:10px;    BACKGROUND: #d4d4d4;text-align:center'> 
                                                                                        <th colspan='13' style='text-align:center;border-bottom:1px solid black'>
                                                                                            <strong>CONCEPTO DE INGRESOS</strong></th> 
                                                                                    </tr>
                                                                                    <tr style='border-bottom:1px solid black;text-align:left;font-size:10px; BACKGROUND: #d4d4d4;font-weight:bold'> 
                                                                                        <th colspan='10' style='text-align:left;border-bottom:1px solid black'>
                                                                                           <strong>CONCEPTO</strong> </th> 
                                                                                            <th colspan='3' style='text-align:left;border-bottom:1px solid black'>
                                                                                                <strong></strong> IMPORTE</th>
                                                                                    </tr>
                                                                                </thead>
                                                                                <tbody style='height: 200px;text-align:left;font-size:10px'>
                                                                                    <tr style='padding-top:15px; padding-bottom: 15px;'>"
            For Each d In detalle
                html = html + "<td colspan ='8' style='padding-left: 5px; padding-top: 5px;'>" + d(3) + "</td>
                                                                                        <td colspan ='3'> </td>
                                                                                        <td colspan='3'></td>"
            Next
            html = html + "</tr>
                                                                                </tbody> 
                                                                               <tfoot style='font-size:10px'></tfoot><tbody><tr style='padding-top:15px; padding-bottom: 15px;font-size:10px'>
                                                                                    <td colspan='10'>  </td> 
                                                                                    <td colspan='3' style='text-align:left'>
                                                                                    " +
                    FormatCurrency(pago) + "  
                                                                                    </td>
                                                                                </tr> 
                                                                            </tbody></table>  
                                                                            <div style='width:100%;position:relative;float:left;MARGIN-TOP:5PX;background:#e4e4e4;'>
                                                                                <!-- <div style='width:100%;position:relative;float:left;text-align:left;font-size:10px'>
                                                                                     <br>
                                                                                     <strong>NOTAS IMPORTANTES:</strong>
                                                                                     <br>eSTE RECIBO NO ES VALIDO<br> 
                                                                                 </div>-->
                                                                                 <div style='width:calc(50% - 10px);position:relative;float:left;text-align:left;font-size:12px;background-color:#e4e4e4;position:relative;float:left;padding:5px;'>
                                                                                     <br>
                                                                                     <strong>CANTIDAD CON LETRA:</strong>
                                                                                     <br>SON (" +
                    numero2letras + ") M.N.<br>
                                                                                 </div>
                                                                                   <div style='width:calc(50% - 10px);position:relative;float:left;text-align:center;font-size:12px;background-color:#e4e4e4;position:relative;float:left;padding:5px;'>
                                                                        <h4 style='margin:0px;width:100%;text-align:center'><b>FOLIO: " +
                    id_folio + "</b></h4>
                                                                        </div>
                                                                                 <div style='width:calc(100% - 10px);position:relative;float:left;text-align:center;font-size:12px;background-color:#a3a0a0;position:relative;float:left;padding:5px;'>
                                                                                    <br>
                                                                                    <strong> Desarrollo Web ©Lerdo Digital by APD Consultores en Tecnología de la Información</strong> 
                                                                                </div> 
                                                                             </div>
                                                                        </div> 
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div> 
                                                <br><br> 
                                            </td>
                                        </tr>
                                    </tbody>
                                </table> 
                            </body></html>"

            Return html
        End Function
        Function VialidadExamen(cabecero, detalle, numero2letras, Cs_Respuesta_pago, pago, id_folio) As String
            Dim html As String

            html = "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'><head>
                                <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
                                <meta content='width=device-width' name='viewport'>
                                <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                                <title></title>
                                <style type='text/css'>
                                    body {
                                        margin: 0;
                                        padding: 0;
                                    }
                                    table,
                                    td,
                                    tr {
                                        vertical-align: top;
                                        border-collapse: collapse;
                                    }
                                    * {
                                        line-height: inherit;
                                    }
                                    a[x-apple-data-detectors=true] {
                                        color: inherit !important;
                                        text-decoration: none !important;
                                    }
                                </style>
                                <style id='media-query' type='text/css'>
                                    @media (max-width: 720px) { 
                                        .block-grid,
                                        .col {
                                            min-width: 320px !important;
                                            max-width: 100% !important;
                                            display: block !important;
                                        } 
                                        .block-grid {
                                            width: 100% !important;
                                        } 
                                        .col {
                                            width: 100% !important;
                                        } 
                                        .col>div {
                                            margin: 0 auto;
                                        } 
                                        img.fullwidth,
                                        img.fullwidthOnMobile {
                                            max-width: 100% !important;
                                        } 
                                        .no-stack .col {
                                            min-width: 0 !important;
                                            display: table-cell !important;
                                        } 
                                        .no-stack.two-up .col {
                                            width: 50% !important;
                                        } 
                                        .no-stack .col.num4 {
                                            width: 33% !important;
                                        } 
                                        .no-stack .col.num8 {
                                            width: 66% !important;
                                        }
                                        .no-stack .col.num4 {
                                            width: 33% !important;
                                        }
                                        .no-stack .col.num3 {
                                            width: 25% !important;
                                        }
                                        .no-stack .col.num6 {
                                            width: 50% !important;
                                        }
                                        .no-stack .col.num9 {
                                            width: 75% !important;
                                        }
                                        .video-block {
                                            max-width: none !important;
                                        }
                                        .mobile_hide {
                                            min-height: 0px;
                                            max-height: 0px;
                                            max-width: 0px;
                                            display: none;
                                            overflow: hidden;
                                            font-size: 0px;
                                        }
                                        .desktop_hide {
                                            display: block !important;
                                            max-height: none !important;
                                        }
                                    }
                                </style>
                            </head>
                            <body class='clean-body' style='margin: 0; padding: 0; -webkit-text-size-adjust: 100%; background-color: #fbfbfb;'>
                                <table bgcolor='#fbfbfb' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='table-layout: fixed; vertical-align: top; min-width: 320px; Margin: 0 auto; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fbfbfb; width: 100%;' valign='top' width='100%'>
                                    <tbody>
                                        <tr style='vertical-align: top;' valign='top'>
                                            <td style='word-break: break-word; vertical-align: top;' valign='top'>
                                                <div style='background-color:transparent;'>
                                                    <div class='block-grid three-up' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color: transparent;'>
                                                        <div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>
                                                            <div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>
                                                                <div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>
                                                                            <img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_p6nto0_amibzm.jpg' style='text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;' title='Alternate text' width='175'>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class='col num6' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 348px; width: 350px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <table border='0' cellpadding='0' cellspacing='0' class='divider' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top' width='100%'>
                                                                            <tbody>
                                                                                <tr style='vertical-align: top;' valign='top'>
                                                                                    <td class='divider_inner' style='word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 45px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;' valign='top'>
                                                                                        <table align='center' border='0' cellpadding='0' cellspacing='0' class='divider_content' role='presentation' style='table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-top: 3px solid #e2dfdf; width: 100%;' valign='top' width='100%'>
                                                                                            <tbody>
                                                                                                <tr style='vertical-align: top;' valign='top'>
                                                                                                    <td style='word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;' valign='top'><span></span></td>
                                                                                                </tr>
                                                                                            </tbody>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                        <div style='color:#555555;font-family:Verdana, Geneva, sans-serif;line-height:1.2;padding-top:5px;padding-right:10px;padding-bottom:0px;padding-left:10px;'>
                                                                            <div style='line-height: 1.2; font-size: 12px; font-family: Verdana, Geneva, sans-serif; color: #555555; mso-line-height-alt: 14px;'>
                                                                                <p style='font-size: 18px; line-height: 1.2; word-break: break-word; text-align: center; font-family: Verdana, Geneva, sans-serif; mso-line-height-alt: 22px; margin: 0;'>
                                                                                    <span style='text:center;font-size: 12px;'>
                                                                                        <strong style='font-size: 14px;'>TESORERIA MUNICIPAL </strong><br>
                                                                                        AV. FRANCISCO SARABIA NO. 3NTE<br>
                                                                                        R.F.C PMC951010FE3<br>
                                                                                       <strong style='font-size: 15px;'> RECIBO DE PAGO</strong>
                                                                                    </span></p>
                                                                            </div>
                                                                        </div>
                                           
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class='col num3' style='display: table-cell; vertical-align: top; max-width: 320px; min-width: 174px; width: 175px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div align='center' class='img-container center autowidth' style='padding-right: 0px;padding-left: 0px;'>
                                                
                                                                            <div style='font-size:1px;line-height:25px'>&nbsp;</div>
                                                
                                                                            <img align='center' alt='Alternate text' border='0' class='center autowidth' src='https://res.cloudinary.com/apd/image/upload/v1593444335/apd_digital/logo_2_tqrpf0_v43whg.png' style='text-decoration:
                                                                            none; -ms-interpolation-mode: bicubic; height: auto; border: 0; width: 100%; max-width: 175px; display: block;'
                                                                            title='Alternate text' width='175'>
                                             
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div> 
                                                <div style='background-color:transparent;'>
                                                    <div class='block-grid' style='Margin: 0 auto; min-width: 320px; max-width: 700px; overflow-wrap: break-word; word-wrap: break-word; word-break: break-word; background-color:rgba(0,0,0,0);'>
                                                        <div style='border-collapse: collapse;display: table;width: 100%;background-color:rgba(0,0,0,0);'>
                                                            <div class='col num12' style='min-width: 320px; max-width: 700px; display: table-cell; vertical-align: top; width: 700px;'>
                                                                <div style='width:100% !important;'>
                                                                    <div style='border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:0px; padding-bottom:0px; padding-right: 0px; padding-left: 0px;'>
                                                                        <div style='font-size:16px;text-align:center;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif'>
                                                                            <table style='width:100%;background:white;margin-bottom:10px;font-size: 11px;;text-align:left '>
                                                                                <tbody> 
                                                                                    <tr style='padding-top:15px; padding-bottom: 15px;'>
                                                                                        <td style='border:2px solid black;    width: 60%;'>
                                                                                            NOMBRE: " +
                                                                                            cabecero(6) + "<br>
                                                                                            ESTATUS: " +
                                                                                            cabecero(14) + "<br>
                                                                                            GARANTÍA: " +
                                                                                            cabecero(13) + "
                                                                                        </td>         
                                                                                        <td style='text-align:left; border:2px solid black;'>
                                                                                            FECHA: " +
                                                                                            Format(cabecero(1), "dd/MM/yyyy") + "<br>
                                                                                            FOLIO FACTURACIÓN: " +
                                                                                            Cs_Respuesta_pago.folio + "<br>
                                                                                    </tr> 
                                                                                </tbody> 
                                                                            </table><table style='width:100%;background:white; border:2px solid black;height: 200px;'>
                                                                                <thead style='background:white;border-bottom:2px solid black'>
                                                                                    <tr style='border-bottom:1px solid black;text-align:left;font-size:10px;    BACKGROUND: #d4d4d4;text-align:center'> 
                                                                                        <th colspan='13' style='text-align:center;border-bottom:1px solid black'>
                                                                                            <strong>CONCEPTO DE INGRESOS</strong></th> 
                                                                                    </tr>
                                                                                    <tr style='border-bottom:1px solid black;text-align:left;font-size:10px; BACKGROUND: #d4d4d4;font-weight:bold'> 
                                                                                        <th colspan='10' style='text-align:left;border-bottom:1px solid black'>
                                                                                           <strong>CONCEPTO</strong> </th> 
                                                                                            <th colspan='3' style='text-align:center;border-bottom:1px solid black'>
                                                                                                <strong></strong> IMPORTE</th>
                                                                                    </tr>
                                                                                </thead>
                                                                                <tbody style='height: 200px;text-align:left;font-size:10px'>
                                                                                    <tr style='padding-top:15px; padding-bottom: 15px;'>"
            For Each d In detalle
                html = html + "<td colspan ='10' style='padding-left: 5px; padding-top: 5px;'>Pago de " + d(3) + " correspondiente al folio " + id_folio + "</td>
                                                                                        <td colspan ='2'> </td> "
            Next
            html = html + "</tr>
                                                                                </tbody> 
                                                                               <tfoot style='font-size:10px'></tfoot><tbody><tr style='padding-top:15px; padding-bottom: 15px;font-size:10px'>
                                                                                    <td colspan='10'>  </td> 
                                                                                    <td colspan='2' style='text-align:left'>
                                                                                    " +
                    FormatCurrency(pago) + "  
                                                                                    </td>
                                                                                </tr> 
                                                                            </tbody></table>  
                                                                            <div style='width:100%;position:relative;float:left;MARGIN-TOP:5PX;background:#e4e4e4;'>
                                                                                <!-- <div style='width:100%;position:relative;float:left;text-align:left;font-size:10px'>
                                                                                     <br>
                                                                                     <strong>NOTAS IMPORTANTES:</strong>
                                                                                     <br>eSTE RECIBO NO ES VALIDO<br> 
                                                                                 </div>-->
                                                                                 <div style='width:calc(50% - 10px);position:relative;float:left;text-align:left;font-size:12px;background-color:#e4e4e4;position:relative;float:left;padding:5px;'>
                                                                                     <br>
                                                                                     <strong>CANTIDAD CON LETRA:</strong>
                                                                                     <br>SON (" +
                    numero2letras + ") M.N.<br>
                                                                                 </div>
                                                                                   <div style='width:calc(50% - 10px);position:relative;float:left;text-align:center;font-size:12px;background-color:#e4e4e4;position:relative;float:left;padding:5px;'>
                                                                        <h4 style='margin:0px;width:100%;text-align:center'><b>FOLIO: " +
                    id_folio + "</b></h4>
                                                                        </div>
                                                                                 <div style='width:calc(100% - 10px);position:relative;float:left;text-align:center;font-size:12px;background-color:#a3a0a0;position:relative;float:left;padding:5px;'>
                                                                                    <br>
                                                                                    <strong> Desarrollo Web ©Lerdo Digital by APD Consultores en Tecnología de la Información</strong> 
                                                                                </div> 
                                                                             </div>
                                                                        </div> 
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div> 
                                                <br><br> 
                                            </td>
                                        </tr>
                                    </tbody>
                                </table> 
                            </body></html>"

            Return html
        End Function
    End Class
End Namespace