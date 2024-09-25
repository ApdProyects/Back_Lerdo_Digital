Imports System.Web.Http
Imports System.Web.Http.Cors
Imports CDTS_APD_Reporteador
Imports CDTS_APD_Reporteador.Models

Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports ThoughtWorks.QRCode.Codec
Imports ThoughtWorks.QRCode
Imports System.Drawing
Imports System.Data.SqlClient
Imports System.Runtime.Serialization.Diagnostics
Imports System.ServiceModel.Diagnostics

Imports SelectPdf
Imports System.Net.Mail
Imports System.Net
Imports System.Net.Http
Imports API_APD_Reporteador.com.facturehoy.wsprod3
Imports System.Diagnostics.Eventing.Reader
Imports System.Web.UI.WebControls.Expressions
Imports System.Resources
Imports System.Globalization

Namespace Controllers

    <EnableCors("*", "*", "*")>
    Public Class FacturacionController
        Inherits ApiController
        Dim culture As CultureInfo = New CultureInfo("en-US") '' AGREGAMOS EL INTERVALO DE REGION NESESARIA PARA GENERAR EL ARCIVO.

        Dim Mdl_Facturacion As New Mdl_Facturacion
        'Const URI_SAT = "http://www.sat.gob.mx/cfd/3"
        'Const xsi = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd"


        Const URI_SAT = "http://www.sat.gob.mx/cfd/4"
        '  Const xsi = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd"
        Const xsi = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd"
        '' "http://www.sat.gob.mx/sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt"



        Public Const CK_KEY = "RSAT34MB34N_7F1CD986683M"
        Public Const NOMBRE_XSLT = "cadenaoriginal_3_3.xslt"
        Public Const NOMBRE_XSLT_4_0 = "cadenaoriginal_4_0.xslt"
        Public Const DIR_SAT = "\02_SAT\"
        Public Const DIR_PKI = "\PKI\"

        Private m_xmlDOM As New XmlDocument
        Private c As Integer

        Dim ModuloGeneral As New ModuloGeneral

        Dim CS_Facturas_Lista As New CS_Facturas_Lista
        Dim Datos_fiscales As New Datos_fiscales
        Dim Lista_Forma_Pago As New Lista_Forma_Pago
        Dim Folio_Grid As New Folio_Grid

        Dim numero_factura As Integer
        Dim FE_MetodoPago As String
        Dim Fe_Txt_MetodoPago As String
        Dim FE_FormaPago As String
        Dim Fe_Txt_FormaPago As String
        Dim FE_UsoCFDI As String
        Dim FE_CondicionesPago As String
        Dim FE_Folio As String
        Dim FE_CFDIRelacionado As String
        Dim FE_TipoVenta As String
        Dim FE_ImporteFactura As String
        Dim FE_ImporteTotal As String
        Dim FE_Lugar As String
        Dim FE_NumFacturita As String

        Dim G_RFC As String
        Dim G_folio As String

        Dim Total As String
        Dim UUID As String
        Dim RFCEmisor As String
        Dim RFCReceptor As String

        Private qrBackColor As Integer = Color.FromArgb(255, 255, 255, 255).ToArgb
        Private qrForeColor As Integer = Color.FromArgb(255, 0, 0, 0).ToArgb
        Dim CBidimensional As Image

        Dim codigoB64 As String
        Dim NoCertificado As String
        Dim NoCertificadoSAT As String
        Dim SelloCFD As String
        Dim SelloSAT As String
        Dim VersionSAT As String
        Dim FechaCertificadoSAT As String

        Private IMPORTE_EXENTO As Decimal = 0
        Private IMPORTE_TASA As Decimal = 0

        '/*PRUEBA DE FUNCIONAMIENTO DE APLI*/
        <HttpGet>
        <Route("api/Facturacion/index")>
        Public Function index(ByVal folio As Integer)
            Return "CREADO BY ING. ARNULFO FERNANDO MARTINEZ ROSALES"
        End Function

        '/*NUEVA FACTURACION*/
        '/* RECUPERAR REGIMEN FISCAL*/
        <HttpGet>
        <Route("api/Facturacion/recupera_regimen")>
        Public Function recupera_regimen()
            Dim Cs_regimen As New List(Of Cs_regimen)
            Dim Cs_Resultado As New Cs_Respuesta
            Dim SentSQL As String
            Dim resultado As ArrayList
            Try
                SentSQL = "select FRF_CLAVE, FRF_CLAVE_NOMBRE, FRF_DESCRIPCION from [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].FEL_01_REGIMEN_FISCAL"
                resultado = Mdl_Facturacion.REGRESA_ARRAY(SentSQL)
            Catch ex As Exception
                Cs_Resultado = New Cs_Respuesta
                Cs_Resultado.codigo = -1
                Cs_Resultado.codigoError = 500
                Cs_Resultado.mensaje = "Error: " + ex.Message
                Return Cs_Resultado
            End Try

            '   Return Cs_Resultado
            Return resultado
        End Function

        'Nueva Facturacion 
        'Recupera facturas
        <HttpGet>
        <Route("api/Facturacion/RecuperaFacturasGrid")>
        Public Function RecuperaFacturas(ByVal lus_clave As Integer)
            Try
                Dim sqlString
                'Dim CS_Facturas_Lista As New CS_Facturas_Lista
                sqlString = "EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_X_WEB_FACTURAS_BY_USUARIO] " & lus_clave & ""
                CS_Facturas_Lista = Mdl_Facturacion.RECUPERA_FACTURAS(CS_Facturas_Lista, sqlString)                     ' RETORNA UN VALOR NUMERIOCO
                Return CS_Facturas_Lista
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        'Nueva Facturacion 
        'Recuperar Datos Fiscales
        <HttpGet>
        <Route("api/Facturacion/RecuperarDatosFiscales")>
        Public Function RecuperarDatosFiscales(ByVal RFC As String)
            Try
                Dim sqlString
                'Dim CS_Facturas_Lista As New CS_Facturas_Lista
                sqlString = "IF (SELECT 'SI' FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[VTA_03_CLIENTES] WHERE VCL_RFC = '" & RFC & "') = 'SI' BEGIN 
                            SELECT 
                                ISNULL(VCL_NOMBRE,'NO INGRESADO') AS VCL_NOMBRE, 
                                ISNULL(VCL_CP, '') AS VCL_CP, 
                                ISNULL(VCL_DIRECCION , '--') AS VCL_DIRECCION,
                                ISNULL(FRF_CLAVE_NOMBRE ,'') AS FRF_CLAVE_NOMBRE
                            FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[VTA_03_CLIENTES]
                            WHERE VCL_RFC = '" & RFC & "'
                            END
                            ELSE BEGIN
	                            SELECT 
	                                'NO INGRESADO' AS VCL_NOMBRE, 
		                            '--'AS VCL_CP, 
		                            '--'AS VCL_DIRECCION,
		                            '--'AS FRF_CLAVE_NOMBRE
                            END"
                Datos_fiscales = Mdl_Facturacion.Recuperar_Datos_fiscales(Datos_fiscales, sqlString)
                Return Datos_fiscales
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        'Nueva Facturacion 
        'Replicar Pago
        <HttpGet>
        <Route("api/Facturacion/Replicar_pago")>
        Public Function Replicar_pago(ByVal id_folio As String)
            Try
                Dim sqlString
                'Dim CS_Facturas_Lista As New CS_Facturas_Lista
                sqlString = "EXEC [DBO].[PA_Traer_ACTUALIZA_DATA_SRV_VIALIDAD_A_SRV_MULTIPAGOS] '" & id_folio & "'"
                'este sp se encuentra en la base de datos de Multipagos. se genero una conexion extra para este caso en particular.

                Dim Aux = Mdl_Facturacion.RECUPERA_VALOR_STRING_REPLICAS(sqlString)    ' RETORNA UN VALOR NUMERIOCO
                Return Aux
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        'Nueva Facturacion 
        'Enviar Factura ENCIAR POR CORREO
        <HttpGet>
        <Route("api/Facturacion/EnviarFactura")>
        Public Function EnviarFactura(luz_clave As Integer, ByVal NumFactura As Integer, ByVal RFC As String)
            Try
                Dim sql As String = "SELECT LUS_CORREO FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].LDG_04_USUARIOS WHERE LUS_CLAVE = " & luz_clave.ToString()
                Dim Correo As String = Mdl_Facturacion.RECUPERA_VALOR_STRING(sql)
                Dim bool As String = RENVIO_ENVIAR_CORREO(
                                            NumFactura,
                                            Correo,
                                            HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(NumFactura)) & "_" & CStr(RFC) & ".pdf",
                                            HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(NumFactura)) & "_" & CStr(RFC) & ".xml")
                Return bool
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        'Nueva Facturacion 
        'Corregir_Datos_Fiscales
        <HttpGet>
        <Route("api/Facturacion/Corregir_Datos_Fiscales")>
        Public Function Corregir_Datos_Fiscales(ByVal RFC As String, ByVal NOMBRE_FISCAL As String, ByVal CP_FISCAL As String, ByVal FRF_CLAVE_NOMBRE As String, ByVal DIRECCION As String)
            Try
                Dim sql As String = "EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_ACTUALIZA_DATOS_FISCALES_VTA_03_CLIENTES] 
                                        '" & NOMBRE_FISCAL & "' , 
                                        '" & CP_FISCAL & "' , 
                                        '" & DIRECCION & "' , 
                                        '" & FRF_CLAVE_NOMBRE & "' , 
                                        '" & RFC & "'"
                Dim respuesta As String = Mdl_Facturacion.RECUPERA_VALOR_STRING(sql)
                Return respuesta
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        'Nueva Facturacion 
        'EnviarFactura DescargarFacturaPDF
        <HttpGet>
        <Route("api/Facturacion/DescargarFacturaPDF")>
        Public Function DescargarFacturaPDF(ByVal NumFactura As Integer, ByVal RFC As String) As HttpResponseMessage
            Try
                Dim ArchivoPDF As String = HttpContext.Current.Request.PhysicalApplicationPath & "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(NumFactura)) & "_" & CStr(RFC) & ".pdf"
                Dim result As New HttpResponseMessage(HttpStatusCode.OK)
                Dim stream As New FileStream(ArchivoPDF, FileMode.Open, FileAccess.Read)
                result.Content = New StreamContent(stream)
                result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                result.Content.Headers.ContentDisposition.FileName = ArchivoPDF
                result.Content.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream")
                Return result
            Catch ex As Exception
                BadRequest(ex.Message)
            End Try
        End Function

        'Nueva Facturacion 
        'EnviarFactura DescargarFacturaXML
        <HttpGet>
        <Route("api/Facturacion/DescargarFacturaXML")>
        Public Function DescargarFacturaXML(ByVal NumFactura As Integer, ByVal RFC As String) As HttpResponseMessage
            Try
                Dim ArchivoXML As String = HttpContext.Current.Request.PhysicalApplicationPath & "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(NumFactura)) & "_" & CStr(RFC) & ".xml"
                Dim result As New HttpResponseMessage(HttpStatusCode.OK)
                Dim stream As New FileStream(ArchivoXML, FileMode.Open, FileAccess.Read)
                result.Content = New StreamContent(stream)
                result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                result.Content.Headers.ContentDisposition.FileName = ArchivoXML
                result.Content.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream")
                Return result
            Catch ex As Exception
                BadRequest(ex.Message)
            End Try
        End Function

        '' Nueva Facturacion
        '' RECUPERAR folio para grid
        <HttpGet>
        <Route("api/Facturacion/Recupera_folio_grid")>
        Public Function Recupera_Folio_Para_Grid(ByVal Folio As String)
            Dim Cs_folio_fac As New Cs_Folio_fac
            Dim Folio_Grid As New Folio_Grid
            Dim folio_pago As String
            Dim respuesta_sp As Integer
            '' inicializamos mensaje
            Folio_Grid.Folio = ""
            Folio_Grid.Descripcion_Folio = ""
            Folio_Grid.Importe_Folio = ""
            Folio_Grid.MENSAJE = ""

            folio_pago = Folio.Substring(3, (Folio.Count() - 3))
            Try
                If (Folio.Count() = 11 Or Folio.Count() = 13 Or Folio.Count() = 16 Or Folio.Count() = 17) Then

                    Cs_folio_fac = Mdl_Facturacion.Recupera_Folio(folio_pago)
                    respuesta_sp = Mdl_Facturacion.VALIDA_FACTURACION(folio_pago)

                    If (respuesta_sp = 0) Then
                        Folio_Grid.MENSAJE = "No se puede realizar la facturación electrónica del folio '" & Folio & "'. No cumple los requerimientos de fecha."
                    End If
                    If (Cs_folio_fac.IIN_COBRO_ESTATUS = "FACTURADA") Then
                        Folio_Grid.MENSAJE = "Este folio '" & Folio & "' ya fue facturado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498"
                    End If
                    If (Cs_folio_fac.IIN_COBRO_ESTATUS = "NO COBRADA") Then
                        Folio_Grid.MENSAJE = "Este folio '" & Folio & "' no ha sido pagado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498"
                    End If
                    Dim AUX_XD As String
                    AUX_XD = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT TOP 1 'SI' FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I WHERE I.CDA_FOLIO = '" & folio_pago & "'")

                    If (AUX_XD <> "SI") Then
                        Folio_Grid.MENSAJE = ("Folio " + Folio + " no registrado en ingresos, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498.")
                    End If

                Else
                    Folio_Grid.MENSAJE = "El Folio '" & Folio & "' no es valido, inténtelo nuevamente"
                End If

                If Folio_Grid.MENSAJE = "" Then
                    Try
                        Dim sql As String = "EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].PA_X_WEB_PL_Recupera_Folio_Para_Grid '" & folio_pago & "'"
                        Folio_Grid = Mdl_Facturacion.RECUPERA_FOLIO_GRID(Folio_Grid, sql)
                    Catch ex As Exception
                        Folio_Grid.MENSAJE = ex.Message
                    End Try
                End If
            Catch ex As Exception
                Folio_Grid.MENSAJE = ex.Message
            End Try

            Return Folio_Grid
        End Function

        ''NUEVA FACTURACION
        '' RETRORNAR FORMAS DE PAGO
        <HttpGet>
        <Route("api/Facturacion/FORMASPAGO")>
        Public Function RETORNA_FORMAS_PAGO()
            Try
                Dim sqlString
                'Dim CS_Facturas_Lista As New CS_Facturas_Lista
                sqlString = "SELECT FFP_CLAVE_NOMBRE, FFP_DESCRIPCION FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].FEL_05_FORMA_PAGO ORDER BY FFP_DESCRIPCION"
                Lista_Forma_Pago = Mdl_Facturacion.RECUPERA_FORMAS_PAGO(Lista_Forma_Pago, sqlString)                     ' RETORNA UN VALOR NUMERIOCO
                Return Lista_Forma_Pago
            Catch ex As Exception
                Return ex.Message
            End Try

            'SELECT FFP_CLAVE_NOMBRE	, FFP_DESCRIPCION FROM FEL_05_FORMA_PAGO ORDER BY FFP_DESCRIPCION
        End Function


        <HttpGet>
        <Route("api/Facturacion/Recuperafolio")>
        Public Function Recuperafolio(ByVal folio As String)
            Dim Cs_Folio_fac As New Cs_Folio_fac
            Dim folio_fac As String = ""
            Dim DEPTO As String = ""

            Try
                If (folio.Count() = 11) Then
                    folio_fac = folio.Substring(3, 8)
                ElseIf (folio.Count() = 13) Then
                    folio_fac = folio.Substring(3, 10)
                ElseIf (folio.Count() = 17) Then
                    folio_fac = folio.Substring(3, 14)
                Else
                    Cs_Folio_fac.codigo = -1
                    Cs_Folio_fac.codigoError = 400
                    Cs_Folio_fac.mensaje = "Folio no valido, inténtelo nuevamente."

                    Return Cs_Folio_fac
                End If

                Cs_Folio_fac = Mdl_Facturacion.Recupera_Folio(folio_fac)

                If (Cs_Folio_fac.CDA_FOLIO > 0) Then
                    Cs_Folio_fac.codigo = 1
                    Cs_Folio_fac.codigoError = 200
                    Cs_Folio_fac.mensaje = "correcto"
                Else
                    Cs_Folio_fac.codigo = -1
                    Cs_Folio_fac.codigoError = 400
                    Cs_Folio_fac.mensaje = "No se encontro folio"
                End If
            Catch ex As Exception
                Cs_Folio_fac = New Cs_Folio_fac
                Cs_Folio_fac.codigo = -1
                Cs_Folio_fac.codigoError = 500
                Cs_Folio_fac.mensaje = "Error: " + ex.Message
            End Try

            Return Cs_Folio_fac
        End Function
        <HttpGet>
        <Route("api/Facturacion/Recuperacliente")>
        Public Function Recuperacliente(ByVal RFC As String)
            Dim Cs_Cliente_fac As New Cs_cliente_fac

            Try
                Cs_Cliente_fac = Mdl_Facturacion.Recupera_Cliente(RFC)

                If (Cs_Cliente_fac.VCL_CLAVE > 0) Then
                    Cs_Cliente_fac.codigo = 1
                    Cs_Cliente_fac.codigoError = 200
                    Cs_Cliente_fac.mensaje = "correcto"
                Else
                    Cs_Cliente_fac.codigo = -1
                    Cs_Cliente_fac.codigoError = 400
                    Cs_Cliente_fac.mensaje = "No se encontro RFC"
                End If
            Catch ex As Exception
                Cs_Cliente_fac = New Cs_cliente_fac
                Cs_Cliente_fac.codigo = -1
                Cs_Cliente_fac.codigoError = 500
                Cs_Cliente_fac.mensaje = "Error: " + ex.Message
            End Try

            Return Cs_Cliente_fac
        End Function

        <HttpGet>
        <Route("api/Facturacion/Recupera_regimen_cliente")>
        Public Function Recupera_regimen_cliente(ByVal RFC As String)
            Dim Cs_Cliente_fac As New Cs_cliente_fac

            Try
                Cs_Cliente_fac = Mdl_Facturacion.Recupera_Cliente_regimen(RFC)

                If (Cs_Cliente_fac.VCL_FRF_CLAVE_NOMBRE = "0" Or Cs_Cliente_fac.VCL_FRF_CLAVE_NOMBRE = Nothing Or Cs_Cliente_fac.VCL_FRF_CLAVE_NOMBRE = "") Then
                    Cs_Cliente_fac.codigo = -1
                    Cs_Cliente_fac.codigoError = 400
                    Cs_Cliente_fac.mensaje = "No se encontro Regimen"
                Else

                    Cs_Cliente_fac.codigo = 1
                    Cs_Cliente_fac.codigoError = 200
                    Cs_Cliente_fac.mensaje = "correcto"
                End If
            Catch ex As Exception
                Cs_Cliente_fac = New Cs_cliente_fac
                Cs_Cliente_fac.codigo = -1
                Cs_Cliente_fac.codigoError = 500
                Cs_Cliente_fac.mensaje = "Error: " + ex.Message
            End Try

            Return Cs_Cliente_fac
        End Function
        <HttpGet>
        <Route("api/Facturacion/ConfirmacionCliente")>
        Public Function ConfirmacionCliente(ByVal RFC As String)
            Dim Cs_Cliente_fac As New Cs_cliente_fac

            Try
                ModuloGeneral = Mdl_Facturacion.datos_cliente(ModuloGeneral, RFC)

                If (ModuloGeneral.ReceptorMail <> Nothing) Then

                    Dim bool As Boolean = ENVIAR_CORREO_RFC(RFC, ModuloGeneral.ReceptorMail)

                    Cs_Cliente_fac.codigo = 1
                    Cs_Cliente_fac.codigoError = 200
                    Cs_Cliente_fac.mensaje = "correcto"
                Else
                    Cs_Cliente_fac.codigo = -1
                    Cs_Cliente_fac.codigoError = 400
                    Cs_Cliente_fac.mensaje = "No se encontro RFC"
                End If
            Catch ex As Exception
                Cs_Cliente_fac = New Cs_cliente_fac
                Cs_Cliente_fac.codigo = -1
                Cs_Cliente_fac.codigoError = 500
                Cs_Cliente_fac.mensaje = "Error: " + ex.Message
            End Try

            Return Cs_Cliente_fac
        End Function

        '/* GUARDAR CLIENTE */
        <HttpGet>
        <Route("api/Facturacion/guardacliente")>
        Public Function guardacliente(RFC As String,
                                      nombre As String,
                                      tipopersona As String,
                                      direccion As String,
                                      colonia As String,
                                      CP As String,
                                      email As String,
                                      celular As String,
                                      estado As String,
                                      municipio As String,
                                      regimen As String)

            Dim Cs_Cliente_fac As New Cs_cliente_fac
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim VCL_CLAVE As Integer
            Dim resultado As ArrayList
            Dim SentSQL As String

            SentSQL = "select FRF_CLAVE_NOMBRE from [APDSGEDB_PL].[dbo].[FEL_01_REGIMEN_FISCAL] WHERE FRF_CLAVE = " + regimen
            resultado = Mdl_Facturacion.REGRESA_ARRAY(SentSQL)
            VCL_CLAVE = Mdl_Facturacion.max_cliente() + 1

            Try
                Cs_Cliente_fac.VCL_CLAVE = VCL_CLAVE
                Cs_Cliente_fac.VCL_RFC = RFC
                Cs_Cliente_fac.VCL_NOMBRE = nombre
                Cs_Cliente_fac.VCL_TIPO_PERSONA = tipopersona
                Cs_Cliente_fac.VCL_DIRECCION = direccion
                Cs_Cliente_fac.VCL_COLONIA = colonia
                Cs_Cliente_fac.VCL_CP = CP
                Cs_Cliente_fac.VCL_EMAIL = email
                Cs_Cliente_fac.VCL_CELULAR = celular
                Cs_Cliente_fac.CES_CLAVE = estado
                Cs_Cliente_fac.CMP_CLAVE = municipio
                Cs_Cliente_fac.CPA_CLAVE = 1
                Cs_Cliente_fac.VCL_FRF_CLAVE_NOMBRE = resultado(0)(0).ToString()
                Cs_Cliente_fac.VCL_FRF_CLAVE = regimen
                Cs_Respuesta = Mdl_Facturacion.Guarda_Cliente(Cs_Cliente_fac)

                If (Cs_Respuesta.codigo <= 0) Then
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "Hubo un problema al guardar el RFC, intentelo nuevamente."
                End If
            Catch ex As Exception
                Cs_Respuesta = New Cs_Respuesta
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 500
                Cs_Respuesta.mensaje = "Error: " + ex.Message
            End Try

            Return Cs_Respuesta
        End Function
        <HttpGet>
        <Route("api/Facturacion/guardacliente_regimen")>
        Public Function guardacliente_regimen(RFC As String, regimen As String)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim resultado As ArrayList
            Dim SentSQL As String
            Try
                SentSQL = "UPDATE [APDSGEDB_PL].[DBO].VTA_03_CLIENTES set FRF_CLAVE=" + regimen + ",FRF_CLAVE_NOMBRE= (select FRF_CLAVE_NOMBRE from [APDSGEDB_PL].[DBO].FEL_01_REGIMEN_FISCAL where FRF_CLAVE =" + regimen + ") WHERE VCL_RFC = '" + RFC + "'"
                resultado = Mdl_Facturacion.REGRESA_ARRAY(SentSQL)


                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Regimen fiscal Actualizado."

            Catch ex As Exception
                Cs_Respuesta = New Cs_Respuesta
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 500
                Cs_Respuesta.mensaje = "Error: " + ex.Message
            End Try
            Return Cs_Respuesta
        End Function
        <HttpGet>
        <Route("api/Facturacion/facturar")>
        Public Function facturar(RFC As String, folio As String, UsoCFDI As String)

            Dim Cs_folio_fac As New Cs_Folio_fac
            Dim Mensaje As String = ""
            Dim i As Integer = 0
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim folio_fac As String = ""
            Dim folio_dep As String = ""
            Dim folio_ico As String = ""
            Dim DEPTO As String = ""
            Dim n_Uso_CFDI As String = ""
            Dim respuesta_sp As Integer
            G_RFC = RFC
            G_folio = folio
            Dim clave_Catastral As String = ""

            'DEPTO = folio.Substring(6, 2) 
            If (folio.Count() = 11) Then
                folio_fac = folio.Substring(3, 8)
                Dim res = Mdl_Facturacion.procesa_vvd_predial(folio_fac)
            ElseIf (folio.Count() = 13) Then
                folio_fac = folio.Substring(3, 10)
                folio_dep = CInt(folio_fac.Substring(0, 2))
                folio_ico = CInt(folio_fac.Substring(2, 4))

                If folio_dep <> 24 And folio_dep <> 56 And folio_dep <> 19 And folio_dep <> 2 And folio_dep <> "5" And folio_dep <> 20 And folio_dep <> 11 Then
                    Dim res = Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                Else
                    Dim res = Mdl_Facturacion.procesa_vvd_varios(folio_fac, folio_dep, folio_ico)
                End If
            ElseIf (folio.Count() = 16) Then
                folio_fac = folio.Substring(3, 13)
                folio_dep = CInt(folio_fac.Substring(0, 2))
                folio_ico = CInt(folio_fac.Substring(2, 4))

                If folio_dep <> 24 And folio_dep <> 56 And folio_dep <> 19 And folio_dep <> 2 And folio_dep <> "5" And folio_dep <> 20 And folio_dep <> 11 Then
                    Dim res = Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                Else
                    Dim res = Mdl_Facturacion.procesa_vvd_varios(folio_fac, folio_dep, folio_ico)
                End If
            ElseIf (folio.Count() = 17) Then
                folio_fac = folio.Substring(3, 14)
                folio_dep = CInt(folio_fac.Substring(0, 2))
                folio_ico = CInt(folio_fac.Substring(2, 4))

                If folio_dep <> 24 And folio_dep <> 56 And folio_dep <> 19 And folio_dep <> 2 And folio_dep <> "2" And folio_dep <> "5" And folio_dep <> 20 And folio_dep <> 11 Then
                    Dim res = Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                Else
                    Dim res = Mdl_Facturacion.procesa_vvd_varios(folio_fac, folio_dep, folio_ico)
                End If
            Else
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Folio no valido, inténtelo nuevamente."

                Return Cs_Respuesta
            End If

            Cs_folio_fac = Mdl_Facturacion.Recupera_Folio(folio_fac)


            respuesta_sp = Mdl_Facturacion.VALIDA_FACTURACION(folio_fac)

            If (respuesta_sp = 0) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "No se puede realizar la facturación electrónica. No cumple los requerimientos de fecha."

                Return Cs_Respuesta
            End If

            If (Cs_folio_fac.VVD_CLAVE = 0 Or Cs_folio_fac.VVD_TOTAL = 0) Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Folio no registrado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498."

                Return Cs_Respuesta
            End If

            If (Cs_folio_fac.IIN_COBRO_ESTATUS = "FACTURADA") Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Este folio ya fue facturado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498"

                Return Cs_Respuesta
            End If

            If (Cs_folio_fac.IIN_COBRO_ESTATUS = "NO COBRADA") Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "Este folio no ha sido pagado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 465-5498"

                Return Cs_Respuesta
            End If

            ModuloGeneral = Mdl_Facturacion.datos_empresa(ModuloGeneral)
            ModuloGeneral = Mdl_Facturacion.datos_cliente(ModuloGeneral, RFC)
            ModuloGeneral = Mdl_Facturacion.datos_venta_det(ModuloGeneral, Cs_folio_fac.VVD_CLAVE)
            ModuloGeneral = Mdl_Facturacion.datos_fiscales(ModuloGeneral)
            ModuloGeneral = Mdl_Facturacion.datos_prefijo(ModuloGeneral)

            Mensaje = ValidaConsulta(ModuloGeneral)

            If Mensaje <> "" Then
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 400
                Cs_Respuesta.mensaje = "No Se Puede Generar La Factura Porque Los Siguientes Datos Están Vacios!" & Mensaje

                Return Cs_Respuesta
            End If

            numero_factura = Mdl_Facturacion.numero_factura()
            FE_MetodoPago = "PUE"
            Fe_Txt_MetodoPago = "PAGO EN UNA SOLA EXHIBICIÓN"


            If (Cs_folio_fac.IIN_FORMA_PAGO = "TARJETA DE CREDITO" Or Cs_folio_fac.IIN_FORMA_PAGO = "TC" Or Cs_folio_fac.IIN_FORMA_PAGO = "TCTRANS") Then
                FE_FormaPago = "04"
                Fe_Txt_FormaPago = "TARJETA DE CREDITO"
            ElseIf (Cs_folio_fac.IIN_FORMA_PAGO = "TARJETA DE DÉBITO") Then
                FE_FormaPago = "28"
                Fe_Txt_FormaPago = "TARJETA DE DÉBITO"
            ElseIf (Cs_folio_fac.IIN_FORMA_PAGO = "EFECTIVO" Or Cs_folio_fac.IIN_FORMA_PAGO = "EFECTIVONC" Or Cs_folio_fac.IIN_FORMA_PAGO = "EFECTIVOCHEQ" Or Cs_folio_fac.IIN_FORMA_PAGO = "EFECTIVOTRANS" Or Cs_folio_fac.IIN_FORMA_PAGO = "EFECTIVOTC" Or Cs_folio_fac.IIN_FORMA_PAGO = "NC") Then
                FE_FormaPago = "01"
                Fe_Txt_FormaPago = "EFECTIVO"
            ElseIf (Cs_folio_fac.IIN_FORMA_PAGO = "TRANS") Then
                FE_FormaPago = "03"
                Fe_Txt_FormaPago = "TRANSFERENCIA ELECTRÓNICA DE FONDOS"
            ElseIf (Cs_folio_fac.IIN_FORMA_PAGO = "CHEQ") Then
                FE_FormaPago = "02"
                Fe_Txt_FormaPago = "CHEQUE NOMINATIVO"
            End If

            n_Uso_CFDI = Mdl_Facturacion.RecuperaUsoCFDI(UsoCFDI)

            FE_UsoCFDI = n_Uso_CFDI

            FE_CondicionesPago = "CONDICION 1"
            FE_Folio = ""
            FE_CFDIRelacionado = ""
            FE_TipoVenta = "REMISION"
            FE_Lugar = ""

            FE_ImporteFactura = Cs_folio_fac.VVD_TOTAL
            FE_ImporteTotal = Cs_folio_fac.VVD_TOTAL

            FE_NumFacturita = Mdl_Facturacion.DAME_NUM_FACTURA() + 1

            ModuloGeneral.LlavePrivada = HttpContext.Current.Request.PhysicalApplicationPath + "XML\CSD_PRESIDENCIA_MUNICIPAL_DE_CIUDAD_LERDO_PMC951010FE3_20180424_094210.key"
            '================================================================================================== CREAR CFDI ======================================================================================================
            Try
                CrearCFD_40(HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")
            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 500
                Cs_Respuesta.mensaje = "No se realizo el timbrado, problemas al generar el CFDI. "
                Cs_Respuesta.objetoError = ex

                Return Cs_Respuesta
            End Try
            '================================================================================================== FIN CREAR CFDI ==================================================================================================
            Try
                '================================================================================================== TIMBRADO XML ==================================================================================================
                Dim queusuariocertus As String = ModuloGeneral.UsuarioTimbrado
                Dim quepasscertus As String = ModuloGeneral.ContraseñaTimbrado
                Dim queproceso As Integer = ModuloGeneral.pac_id
                'Dim queusuariocertus As String = "EWE1709045U0.Test"
                'Dim quepasscertus As String = "Prueba$1"
                'Dim queproceso As Integer = "194876591"
                'Dim MemStream As System.IO.MemoryStream = FileToMemory(ModuloGeneral.CarpetaAlmacenXML & "\" & ModuloGeneral.SeriePrefijo & CStr(folio) & ".xml") 'buscar archivo ya timbrado 
                Dim MemStream As System.IO.MemoryStream = FileToMemory(HttpContext.Current.Request.PhysicalApplicationPath + "XML" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml") 'buscar archivo ya timbrado
                Dim archivo As Byte() = MemStream.ToArray()

                ServicePointManager.Expect100Continue = True
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                'Dim service As New com.facturehoy.wsprod3.WsEmisionTimbrado33
                'Dim service As New com.facturehoy.pruebasclientes.WsEmisionTimbrado40
                'Dim service As New com.facturehoy.wsprod4_0.WsEmisionTimbrado4
                Dim service As New com.facturehoy.wsprod4.WsEmisionTimbrado40
                Dim puerto = service.EmitirTimbrar(queusuariocertus, quepasscertus, queproceso, archivo)

                If Not puerto.XML Is Nothing Then
                    If puerto.isError Then
                        Cs_Respuesta.codigo = -1
                        Cs_Respuesta.codigoError = puerto.isError
                        Cs_Respuesta.mensaje = puerto.message

                        Return Cs_Respuesta
                    Else
                        'File.WriteAllBytes("\\148.235.12.11\Actualizar\APD\02_DOCUMENTOS\08_FE\03_XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml", puerto.XML)
                        File.WriteAllBytes(HttpContext.Current.Request.PhysicalApplicationPath + "XML" & "\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml", puerto.XML)
                    End If
                Else
                    Cs_Respuesta.codigo = -1
                    Cs_Respuesta.codigoError = 400
                    Cs_Respuesta.mensaje = "Error no identificado, favor de checar si el timbrado se efectuo correctamente" + puerto.message

                    Return Cs_Respuesta
                End If
                '================================================================================================ FIN TIMBRADO XML ================================================================================================
                '======================================================================================== ACTUALIZA FOLIO FACTURACION =============================================================================================
                Dim SentSQL As String = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_14_EMPRESAS SET CEP_FE_NUM_FACTURA=" & FE_NumFacturita & ""
                Mdl_Facturacion.ACTUALIZAR_CEP_FE_NUM_FACTURA(SentSQL)
                '==================================================================================== FIN ACTUALIZA FOLIO FACTURACION =============================================================================================
                Dim Diita = CStr(Format(DatePart(DateInterval.Day, Now), 0))
                Dim Yearsitito = CStr(DatePart(DateInterval.Year, Now))

                Dim VVD_FACTURACION_SERIE As String = ModuloGeneral.SeriePrefijo
                Dim VVD_FACTURACION_ESTATUS As String = "TIMBRADA"
                Dim VVD_FACTURACION_NOMBRE As String = ModuloGeneral.SeriePrefijo & CStr(folio)

                'SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VTA_17_FACTURACION_CONTROL " &
                '                    "   SET VVD_FACTURACION_SERIE = ' " & VVD_FACTURACION_SERIE & "', 
                '                            VVD_FACTURACION_NUMERO = " & FE_NumFacturita & ", 
                '                            VVD_FACTURACION_FECHA = GETDATE(), 
                '                            VVD_FACTURACION_ESTATUS ='" & VVD_FACTURACION_ESTATUS & "' " &
                '                    "WHERE  VVD_CLAVE   =  " & Cs_folio_fac.VVD_CLAVE & ""
                'Mdl_Facturacion.ACTUALIZAR_VVD_FACTURACION_SERIE(SentSQL)

                Dim VVF_FECHALTA As Date = Now
                Dim VVD_FACTURACION_NUMERO As Integer = FE_NumFacturita
                Dim VVD_FACTURACION_FECHA As Date = Now
                Dim VVD_FACTURACION_SUPERVISOR As String = ModuloGeneral.ReceptorNombre 'Emp_NombreCorto

                SentSQL = "INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VTA_10_VD_FACTURACION " &
                    "(VVF_FECHALTA, VVD_CLAVE, VVD_FACTURACION_SERIE,VVD_FACTURACION_NUMERO,  VVD_FACTURACION_FECHA, VVD_FACTURACION_SUPERVISOR,VVD_FACTURACION_METODO_PAGO,VVD_FACTURACION_CONDICIONES_PAGO,VVD_FACTURACION_FORMA_PAGO,VVD_FACTURACION_LUGAR_EXPEDICION) " &
                    "VALUES(CONVERT(SMALLDATETIME,GETDATE()), " & Cs_folio_fac.VVD_CLAVE & ", '" & VVD_FACTURACION_SERIE & "', " & VVD_FACTURACION_NUMERO & ", CONVERT(SMALLDATETIME,GETDATE()), '" & VVD_FACTURACION_SUPERVISOR & "','" & Fe_Txt_MetodoPago & "','" & FE_CondicionesPago & "','" & Fe_Txt_FormaPago & "','" & ModuloGeneral.FE_LUGARDEEXPEDICION & "')"
                Mdl_Facturacion.INSERTAR_VTA_10_VD_FACTURACION(SentSQL)

                CodigoBidimensional(Cs_folio_fac.VVD_CLAVE, RFC)

                Dim Fechacertificacion = FechaCertificadoSAT
                Dim VVF_CLAVE As Integer = Mdl_Facturacion.REGRESA_ULTIMO_REGISTRO(Cs_folio_fac.VVD_CLAVE) ''--SCH + 1

                SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VTA_01_VENTAS_DIARIAS SET VVD_FACTURACION_ESTATUS='FACTURADA', VVF_CLAVE=" & VVF_CLAVE & " WHERE VVD_CLAVE=" & Cs_folio_fac.VVD_CLAVE & ""
                Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)

                If (folio.Count() = 11) Then
                    SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_03_INGRESOS SET IIN_COBRO_ESTATUS='FACTURADA'  WHERE CDA_FOLIO='" & ((folio.Substring(3, 8)).ToString()) & "'"
                    Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                ElseIf (folio.Count() = 13) Then
                    SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_03_INGRESOS SET IIN_COBRO_ESTATUS='FACTURADA'  WHERE CDA_FOLIO='" & ((folio.Substring(3, 10)).ToString()) & "'"
                    Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                ElseIf (folio.Count() = 17) Then
                    SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_03_INGRESOS SET IIN_COBRO_ESTATUS='FACTURADA'  WHERE CDA_FOLIO='" & ((folio.Substring(3, 14)).ToString()) & "'"
                    Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                End If


                Dim VFFP_NOMBRE As String = FE_FormaPago
                Dim VFCP_NOMBRE As String = FE_CondicionesPago
                Dim VFMP_NOMBRE As String = FE_MetodoPago ''--cbo_MetodosPago.SelectedValue
                Dim VFUC_NOMBRE As String = FE_UsoCFDI ''--cbo_UsoCFDI.SelectedValue
                Dim VFLE_LUGAR_EXPEDICION As String = ModuloGeneral.FE_LUGARDEEXPEDICION

                Dim SentSQLMP As String = "SELECT  FMP_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].FEL_04_METODO_PAGO WITH (NOLOCK) WHERE  FMP_NOMBRE = '" & FE_MetodoPago & "'"
                VFMP_NOMBRE = Mdl_Facturacion.REGRESA_FEL_04_METODO_PAGO(SentSQLMP)

                Dim SentSQLUC As String = "SELECT  FUC_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].FEL_06_USO_CFDI WITH (NOLOCK) WHERE  FUC_CLAVE_NOMBRE = '" & FE_UsoCFDI & "'"
                VFUC_NOMBRE = Mdl_Facturacion.REGRESA_FEL_06_USO_CFDI(SentSQLUC)

                SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VTA_10_VD_FACTURACION " &
                "SET FFP_CLAVE ='" & VFFP_NOMBRE & "', FCP_CLAVE ='" & 1 & "', FMP_CLAVE ='" & VFMP_NOMBRE & "',FUC_CLAVE= '" & VFUC_NOMBRE & "' " &
                ",FMN_CLAVE =100,VVD_FECHA_TIMBRADA=GETDATE(),VVD_UUID_FACTURA='" & UUID & "'" &
                "WHERE VVD_CLAVE = " & Cs_folio_fac.VVD_CLAVE & ""
                Mdl_Facturacion.ACTUALIZA_METODOS_FORMAS_PAGO(SentSQL)

                Dim numero2letras = UCase(letras(FE_ImporteTotal))

                Dim CadenaOriginalTimbre = "||" & VersionSAT & "|" & UUID & "|" & Fechacertificacion.ToString & "|" & SelloSAT & "|" & NoCertificadoSAT & "||"

                Dim html = "
<html lang='es'>
	<head> 
	  <meta charset='utf-8'>
	  <meta name='viewport' content='width=device-width, initial-scale=1'> 
	  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1' crossorigin='anonymous'>
	</head>
	<body style='padding-top: 40px; padding-left: 40px; padding-right: 40px; font-family: Verdana; font-size: 13px;'>
	  <div class='container'>
		  <div class='row'>
			  <div class='col-md-3'>
				  <img src='C:\inetpub\API_LERDO_DIGITAL\img\lerdologo.jpeg' alt='Lerdo de oportunidades' style='width: 100%; height:130px;'>
			  </div>
			  <div class='col-md-6' style='padding-top:16px;'>
					<div class='row'>
					  <div class='col-md-2'> 
						  <b>EMISOR</b>
					  </div> 
					  <div class='col-md-10'>
						<div class='row' style='padding-top: 5px;'>
							<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
							</div>
						</div>
					  </div> 
					</div>
					<div class='row'>
					  <div class='col-md-12' style='padding-top: 3px;'>
						  PRESIDENCIA MUNICIPAL DE CD LERDO DGO<br>
						  R.F.C. PMC951010FE3<br>
						  AV. FRANCISCO SARABIA N° 3<br>
						  COL. CENTRO C.P. 35150<br>
						  CD LERDO, DURANGO, MÉXICO<br>
						  TEL. (871) 175-0000<br>
						  REGIMEN GENERAL DE LEY DE LAS PERSONAS MORALES<br>
					  </div> 
					</div>
			  </div>
			  <div class='col-md-3'>
				  <img src='C:\inetpub\API_LERDO_DIGITAL\img\apdlogo.png' alt='APD Consultores en tecnología' style='width: 100%; height: 130px;'>
			  </div>
		  </div>
		  <div class='row'>
			  <div class='col-md-1'> 
				 <b>CLIENTE</b>
			  </div>    
			  <div class='col-md-4'>
				<div class='row' style='padding-top: 5px;'>
					<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
				</div>
			  </div>
			  <div class='col-md-2' style='text-align:center;'> 
				  <b>FACTURA</b>
			  </div>    
			  <div class='col-md-5'>  
				<div class='row' style='padding-top: 5px;'>
					<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
				</div>
			  </div>
		  </div>
		  <div class='row'>
			  <div class='col-md-5' style='border: black 1px solid; padding-left: 15px;'>
				  " + ModuloGeneral.ReceptorNombre + "<br>
				  " + ModuloGeneral.ReceptorDomicilioCalle + "<br> 
				  C.P. " + ModuloGeneral.ReceptorDomicilioCp + "<br>
				  " + ModuloGeneral.ReceptorDomicilioMunicipio + ", " + ModuloGeneral.ReceptorDomicilioEstado + "<br>
				  R.F.C.:" + ModuloGeneral.ReceptorRfc + "<br>" + "
				  Régimen Físcal:" + ModuloGeneral.ReceptorRegimenFiscal_descripcion + "
			  </div>
			  <div class='col-md-2' style='padding-left: 25px; padding-top:10px; font-size:10px;'>
				  No. Folio<br>
				  Lugar de Emision<br>
				  Fecha<br>
				  Fecha Certificacion<br>
				  Folio Fiscal
			  </div>
			  <div class='col-md-5' style='border: black 1px solid; padding-left: 15px;'>
				  " + folio + "<br>
				  LERDO, DURANGO<br>
				  " + Now.ToString("dd/MM/yyyy") + "<br>
				  " + Convert.ToDateTime(Fechacertificacion).ToString("dd/MM/yyyy HH:mm:ss") + "<br>
				  " + UUID + "
			  </div>
		  </div>
		  <div class='row'>
			  <div class='col-md-2'> 
				  <b>CFDI RELACIONADOS</b>
			  </div>    
			  <div class='col-md-10'>  
				<div class='row' style='padding-top: 5px;'>
					<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
				</div>
			  </div>
		  </div>
		  <div class='row'>
			  <div class='col-md-6' style='text-align: left; padding-left: 12px; font-size: 12px; padding-top: 3px; padding-bottom: 3px; font-size:10px;'> 
				  UUID
			  </div>    
			  <div class='col-md-6' style='padding-left: 50px; padding-top: 3px; padding-bottom: 3px; font-size:10px;'> 
				  Tipo Relación
			  </div>
		  </div>
		  <div class='row'>
			  <div class='col-md-2'> 
				  <b>CONCEPTOS</b>
			  </div>    
			  <div class='col-md-10'>  
				<div class='row' style='padding-top: 5px;'>
					<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
				</div>
			  </div>
		  </div>
		  <div class='row' style=' padding-top: 10px; padding-bottom:50px;'>
			  <div class='col-md-12' style=' border-top: black 1px solid; border-bottom: black 1px solid; border-right: black 1px solid; border-left: black 1px solid; height: 200px;'>
				  <table  style='width: 100%;'>
					<thead style='border-bottom: black 1px solid;'>
					  <tr>
						<th style='text-align: center; font-size: 9px;'>Art.</th>
						<th style='text-align: center; font-size: 9px;'>Clave SAT</th>
						<th style='text-align: center; font-size: 9px;'>Cant.</th>
						<th style='font-size: 9px;'>Unidad SAT</th>
						<th style='font-size: 9px;'>Descripción</th>
						<th style='text-align: right; font-size: 9px;'>Precio Unit.</th>
						<th style='text-align: right; font-size: 9px;'>Desc.</th>
						<th style='text-align: right; font-size: 9px;'>Importe</th>
					  </tr> 
					</thead>
					<tbody>"
                For Each p In ModuloGeneral.productos
                    html = html + "
					  <tr> 
						<td style='text-align: center; font-size: 9px;'>" + p.Articulo.ToString() + "</td>
						<td style='text-align: center; font-size: 9px;'>" + p.ClaveSAT.ToString() + "</td>
						<td style='text-align: center; font-size: 9px;'>" + CInt(p.Cantidad).ToString() + "</td>
						<td style=' font-size: 9px;'>" + p.UnidadDeMedida + "</td>
						<td style=' font-size: 9px;'>" + p.Descripcion + "</td>
						<td style='text-align: right; font-size: 9px;'>" + FormatCurrency(p.Subtotal) + "</td>
						<td style='text-align: right; font-size: 9px;'>" + FormatCurrency(p.VVT_GD_DESC_IMPORTE) + "</td>
						<td style='text-align: right; font-size: 9px;'>" + FormatCurrency(p.Importe) + "</td>
					  </tr>  "
                Next
                html = html + "
					</tbody>
				  </table>
			  </div>"

                If (folio.Count() = 11) Then
                    html = html + "<div class='col-md-12' style=' border-bottom: black 1px solid; border-right: black 1px solid; border-left: black 1px solid;'>
						<p style='text-align: left; padding-left: 10px; font-size: 10px;'>CLAVE O CUENTA CATASTRAL: " + Cs_folio_fac.CLAVE_CATASTRAL.ToString() + "</p>
					</div>"
                End If

                html = html + "</div>
		  <div class='row'>
			  <div class='col-md-2' style='padding-top:20px;'>
				<img src='data:image/png;base64, " + codigoB64 + "' alt='...' style='width: 150px; height: 150px;'> 
			  </div>
			  <div class='col-md-7'>
				<div class='row'>
					<div class='col-md-4'>  
						<b>IMPORTE CON LETRA</b>
					</div>    
					<div class='col-md-8'>  
						<div class='row' style='padding-top:5px;'>
							<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
					</div> 
				</div>
				<div class='row'>
					<div class='col-md-12' style='text-align: center; padding-top:10px; padding-bottom:5px;'>
						" + numero2letras + " M.N.
					</div>
				</div>
				<div class='row'>
				  <div class='col-md-2' style='padding-top:10px;'> 
					  <b>PAGOS</b>
				  </div>    
				  <div class='col-md-10'>  
					<div class='row' style='padding-top:15px;'>
						<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
					</div>
				  </div>
				</div> 
				<div class='row'>
					<div class='col-md-12' style=' border: black 1px solid; padding-top: 5px; padding-bottom: 5px; padding-left: 10px; font-size: 10px;'>
					 <b>EFECTOS FISCALES AL PAGO&nbsp;&nbsp;&nbsp;&nbsp;" + Fe_Txt_FormaPago + "<br><br>
						METODO DE PAGO:PAGO EN UNA SOLA EXHIBICIÓN<br><br>
						USO CFDI:  " + UsoCFDI + "</b>
						CFDI:<strong>" + " " + ModuloGeneral.VersionFE + "</strong></b>
					</div>
				</div>
				</div>
			  </div>
			  <div class='col-md-3'>
				<div class='row'>
					<div class='col-md-6' style='text-align: center;'> 
						<b>TOTALES</b>
					</div>    
					<div class='col-md-6'>  
						<div class='row' style=' padding-top:5px;'>
							<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
							</div>
						</div>
					</div>
				</div>
				<div class='row'>
					<div class='col-md-6' style='padding-left: 5px; padding-top:5px;'>
						SUBTOTAL:<br><br>
						IVA:<br><br>
						TOTAL:
					</div>
					<div class='col-md-6' style='text-align: right;padding-top:5px;'>
						" + FormatCurrency(FE_ImporteTotal) + "<br><br>
						" + FormatCurrency(0) + "<br><br>
						" + FormatCurrency(FE_ImporteTotal) + "
					</div>
				</div>
				<div class='row'>    
				  <div class='col-md-12'>  
					<div class='row' style='padding-top:5px; padding-left: 5px;'>
						<div class='col-md-12' style='height: 5px; border: 1px solid black; color:black; background-color: black;'>  
						</div>
					</div>
				  </div>
				</div>
			  </div>
		  </div>  
		  <div class='row' style='padding-top:15px;'>
			<div class='col-md-4'>
				<b>SERIE DEL CERTIFICADO DEL CSD:</b>
			</div>
			<div class='col-md-8'>
				" + NoCertificado + "
			</div>
		  </div>
		  <div class='row'>
			<div class='col-md-4'>
				<b>SERIE DEL CERTIFICADO DEL SAT:</b>
			</div>
			<div class='col-md-8'>
				" + NoCertificadoSAT + "
			</div>
		  </div>
		  <div class='row' style='padding-top: 40px; padding-bottom: 10px;'>
			<div class='col-md-12'>
				<b>SELLO DIGITAL DEL CFDI:</b>
			</div><br>
			<div class='col-md-12'>
				<textarea rows='4' style='width: 100%; border-style: none; border-color: Transparent; overflow:hidden; '>" + SelloCFD.ToString() + "</textarea>
			</div> 
		  </div>  
		  <div class='row' style='padding-top: 10px; padding-bottom: 15px;'>
			<div class='col-md-12'>
				<b>SELLO DEL SAT:</b>
			</div><br>
			<div class='col-md-12'>
				<textarea rows='4' style='width: 100%; border-style: none; border-color: Transparent; overflow:hidden; '>" + SelloSAT.ToString() + "</textarea>
			</div> 
		  </div>
		  <div class='row' style='padding-top: 10px; padding-bottom: 15px;'>
			<div class='col-md-12'>
				<b>CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACION DIGITAL DEL SAT:</b>
			</div><br>
			<div class='col-md-12'> 
				<textarea rows='4' style='width: 100%; border-style: none; border-color: Transparent; overflow:hidden; '>" + CadenaOriginalTimbre.ToString() + "</textarea>
			</div>
			<br>
		  </div>
	  </div>
	  <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/js/bootstrap.bundle.min.js' integrity='sha384-ygbV9kiqUc6oa4msXn9868pTtWMgiQaeYH7/t7LECLbyPA2x65Kgf80OJFdroafW' crossorigin='anonymous'></script>
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
                'doc.Save("\\148.235.12.11\Actualizar\APD\02_DOCUMENTOS\08_FE\03_XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".pdf")
                doc.Close()

                htmlString = html
                pdf_page_size = "A4"
                pageSize = DirectCast([Enum].Parse(GetType(PdfPageSize),
                    pdf_page_size, True), PdfPageSize)
                pdf_orientation = "Portrait"
                pdfOrientation = DirectCast(
                    [Enum].Parse(GetType(PdfPageOrientation),
                    pdf_orientation, True), PdfPageOrientation)
                webPageWidth = 1024
                Try
                    webPageWidth = Convert.ToInt32(1024)
                Catch
                End Try
                webPageHeight = 0
                Try
                    webPageHeight = Convert.ToInt32(768)
                Catch
                End Try
                converter = New HtmlToPdf()
                converter.Options.PdfPageSize = pageSize
                converter.Options.PdfPageOrientation = pdfOrientation
                converter.Options.WebPageWidth = webPageWidth
                converter.Options.WebPageHeight = webPageHeight
                doc = converter.ConvertHtmlString(htmlString)
                doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".pdf")
                doc.Close()

                Dim bool As String = ENVIAR_CORREO("", folio, ModuloGeneral.ReceptorMail, "", "", "", "", HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".pdf", HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")

                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Timbrado exitoso " & bool

                Return Cs_Respuesta

            Catch ex As Exception
                Cs_Respuesta.codigo = -1
                Cs_Respuesta.codigoError = 500
                Cs_Respuesta.mensaje = "No se realizo el timbrado, " + ex.Message

                Return Cs_Respuesta
            End Try
        End Function

        'Nueva Facturacion 
        'EnviarFactura
        <HttpGet>
        <Route("api/Facturacion/facturarnew")>
        Public Function facturarNew(RFC As String, Folios As String, UsoCFDI As String, Usuario As Integer, Nombre_FISCAL As String, CP_Fiscal As String, Regimen_Fiscal As String, Direccion_FISCAL As String, Forma_Pago As String, MetodoPago As String)
            Dim Cs_folio_fac As New Cs_Folio_fac
            Dim Mensaje As String = ""
            Dim i As Integer = 0
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim Cs_Respuesta_Folios As New Cs_Respuesta_Folios
            Dim folio_fac As String
            Dim folio_dep As String
            Dim folio As String
            Dim folios_errados As New List(Of String)
            Dim folio_D As String
            Dim folio_ico As String
            Dim DEPTO As String = ""
            Dim n_Uso_CFDI As String = ""
            Dim respuesta_sp As Integer
            Dim clave_Catastral As String = ""

            Dim ACTUALIZAR_DATOS_FISCALES As Boolean
            ACTUALIZAR_DATOS_FISCALES = False
            Dim ClienteId As Integer    '' EL NUMOER DEL CLIENTE VCL_CLAVE new 26/02/2023
            Dim sqlString As String     '' Sentencia SQL new 26/02/2023
            Dim VentaDiaria As Integer  '' numeor de venta diaria

            Dim FolioAr() As String = Split(Folios, ", ")
            RFC = RFC.ToUpper()
            Nombre_FISCAL = Nombre_FISCAL.ToUpper()
            Direccion_FISCAL = Direccion_FISCAL.ToUpper()
            '' RECUPERA EL NUMEOR DEL CLIENTE POR SU RFC new 26/02/2023
            sqlString = "select TOP 1 VCL_CLAVE  from [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES] where VCL_RFC = '" & RFC & "'"
            ClienteId = Mdl_Facturacion.RECUPERA_VALOR_NUM(sqlString)           '' RETORNA UN VALOR NUMERIOCO

            ''BUSCAMSO EL NUMERO DE LA ULTIMA FACTURA FACTURA new 26/02/2023
            sqlString = "SELECT CEP_FE_NUM_FACTURA FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK)"
            G_folio = Mdl_Facturacion.RECUPERA_VALOR_NUM(sqlString).ToString()  '' RETORNA UN VALOR NUMERIOCO de la ultima factura

            sqlString = "SELECT TOP(1) VVD_CLAVE + 1 FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_01_VENTAS_DIARIAS] ORDER BY VVD_CLAVE DESC"
            VentaDiaria = Mdl_Facturacion.RECUPERA_VALOR_NUM(sqlString)         '' RETORNA UN VALOR NUMERIOCO

            Mensaje = ""
            ''logitud de folio ingreso
            For i = 0 To UBound(FolioAr)
                folio = FolioAr(i)
                If (folio.Count() = 11 Or folio.Count() = 13 Or folio.Count() = 16 Or folio.Count() = 17) Then
                    folio_fac = folio.Substring(3, (folio.Count() - 3))
                    Cs_folio_fac = Mdl_Facturacion.Recupera_Folio(folio_fac)
                    respuesta_sp = Mdl_Facturacion.VALIDA_FACTURACION(folio_fac)
                    If (respuesta_sp = 0) Then
                        Mensaje = "No se puede realizar la facturación electrónica del folio '" & folio & "'. No cumple los requerimientos de fecha. "
                        folios_errados.Add(Mensaje)
                    End If
                    If (Cs_folio_fac.IIN_COBRO_ESTATUS = "FACTURADA") Then
                        Mensaje = "Este folio '" & folio & "' ya fue facturado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 134-8604. "
                        folios_errados.Add(Mensaje)
                    End If
                    If (Cs_folio_fac.IIN_COBRO_ESTATUS = "NO COBRADA") Then
                        Mensaje = "Este folio '" & folio & "' no ha sido pagado, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 134-8604. "
                        folios_errados.Add(Mensaje)
                    End If
                    For J = 0 To UBound(FolioAr)
                        folio_D = FolioAr(J)
                        ''validacion de folios duplicados
                        If i <> J Then
                            If folio = folio_D Then
                                Mensaje = "El Folio '" & folio & "' fue enviado mas de una vez, verifica. "
                                folios_errados.Add(Mensaje)
                            End If
                        End If
                    Next
                    Dim AUX_XD As String
                    AUX_XD = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT TOP 1 'SI' FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I WHERE I.CDA_FOLIO = '" & folio_fac & "'")

                    If (AUX_XD <> "SI") Then
                        folios_errados.Add("Folio " + folio + " no registrado en ingresos, favor de comunicarse al CALL CENTER de Lerdo Digital (871) 134-8604. ")
                    End If
                Else
                    Mensaje = "El Folio '" & folio & "' no es valido, inténtelo nuevamente. "
                    folios_errados.Add(Mensaje)
                End If
            Next

            If folios_errados.Count > 0 Then
                Mensaje = ""
                For Each item As String In folios_errados
                    Mensaje &= item & " "
                Next


                Cs_Respuesta_Folios.codigo = 0
                Cs_Respuesta_Folios.codigoError = 400
                Cs_Respuesta_Folios.mensaje = "Error en los siguientes folios: " & Mensaje
                Cs_Respuesta_Folios.mensajeList = folios_errados
                Return Cs_Respuesta_Folios
            End If

            ''insersion de folio
            For i = 0 To UBound(FolioAr)
                folio = FolioAr(i)
                folio_fac = folio.Substring(3, (folio.Count() - 3))
                folio_dep = CInt(folio_fac.Substring(0, 2))
                folio_ico = CInt(folio_fac.Substring(2, 4))
                If (folio.Count() = 11) Then
                    '	Dim res = Mdl_Facturacion.procesa_vvd_predial(folio_fac)
                    '	VARIABLES NESESARIAS PARA EL SP PA_CAJ_03_INSERT_VENTAS_v2_MULTIFUNCIONALIDAD
                    '		@Folio NVARCHAR(20)
                    '		@IdDepto NVARCHAR(2)-- EL DEPARTAMENTO PARA PAGAR PREDIAL
                    '		@VCL_CLAVE INTEGER
                    '		@VVD_CLAVE integer	-- ID del Cliente Seleccionado		
                    Dim res = Mdl_Facturacion.REGRESA_ARRAY("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CAJ_03_INSERT_VENTAS_WEB_PREDIAL_MULTIFUNCIONALIDAD] '" & folio_fac & "' , '11', " & ClienteId.ToString() & " , " & VentaDiaria.ToString())
                ElseIf (folio.Count() = 13 Or folio.Count() = 16) Then
                    If folio_dep <> 24 And folio_dep <> 56 And folio_dep <> 19 And folio_dep <> 2 And folio_dep <> "5" And folio_dep <> 20 And folio_dep <> 11 Then
                        '	Dim res = Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                        '	VARIABLES NESESARIAS PARA EL SP PA_CAJ_03_INSERT_VENTAS_v2_MULTIFUNCIONALIDAD
                        '		@Folio VARCHAR(20)	-- Clave
                        '		@IdDepto INTEGER	-- Folio_dep
                        '		@IdConcepto INTEGER	-- Folio_ico
                        '		@VCL_CLAVE INTEGER	-- ID del Cliente Seleccionado
                        '		@VVD_CLAVE INTEGER	-- ID DE VENTA DIARIA SE MANDA DESDE LA PANTALLA PRINCIPAL.
                        Dim res = Mdl_Facturacion.REGRESA_ARRAY("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CAJ_03_INSERT_VENTAS_v2_MULTIFUNCIONALIDAD] '" & folio_fac & "','" & folio_dep & "'," & folio_ico & "," & ClienteId.ToString() & ", " & VentaDiaria.ToString()) '  Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                    Else
                        '	Dim res = Mdl_Facturacion.procesa_vvd_varios(folio_fac, folio_dep, folio_ico)
                        '	VARIABLES NESESARIAS PARA EL SP PA_CAJ_03_INSERT_VENTAS_v3_MULTIFUNCIONALIDAD
                        '		@Folio VARCHAR(20)	-- Clave
                        '		@IdDepto INTEGER	-- Folio_dep
                        '		@VCL_CLAVE INTEGER	-- ID del Cliente Seleccionado
                        '		@VVD_CLAVE INTEGER	-- ID DE VENTA DIARIA SE MANDA DESDE LA PANTALLA PRINCIPAL.
                        Dim res = Mdl_Facturacion.REGRESA_ARRAY("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CAJ_03_INSERT_VENTAS_v3_MULTIFUNCIONALIDAD] '" & folio_fac & "','" & folio_dep & "'," & ClienteId.ToString() & ", " & VentaDiaria.ToString()) '  Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                    End If
                ElseIf (folio.Count() = 17) Then
                    If folio_dep <> 24 And folio_dep <> 56 And folio_dep <> 19 And folio_dep <> 2 And folio_dep <> "2" And folio_dep <> "5" And folio_dep <> 20 And folio_dep <> 11 Then
                        'Dim res = Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                        Dim res = Mdl_Facturacion.REGRESA_ARRAY("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CAJ_03_INSERT_VENTAS_v2_MULTIFUNCIONALIDAD] '" & folio_fac & "','" & folio_dep & "'," & folio_ico & "," & ClienteId.ToString() & ", " & VentaDiaria.ToString()) '  Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                    Else
                        'Dim res = Mdl_Facturacion.procesa_vvd_varios(folio_fac, folio_dep, folio_ico)
                        Dim res = Mdl_Facturacion.REGRESA_ARRAY("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CAJ_03_INSERT_VENTAS_v3_MULTIFUNCIONALIDAD] '" & folio_fac & "','" & folio_dep & "'," & ClienteId.ToString() & ", " & VentaDiaria.ToString()) '  Mdl_Facturacion.procesa_vvd_un(folio_fac, folio_dep, folio_ico)
                    End If
                End If
            Next

            ModuloGeneral = Mdl_Facturacion.datos_venta_det(ModuloGeneral, VentaDiaria)
            ModuloGeneral = Mdl_Facturacion.datos_empresa(ModuloGeneral)
            ModuloGeneral = Mdl_Facturacion.datos_cliente(ModuloGeneral, RFC)

            'If  Nombre_FISCAL As String, CP_Fiscal As String, Regimen_Fiscal As String, Direccion_FISCAL As String, Forma_Pago As String, MetodoPago As String
            If ModuloGeneral.ReceptorNombre <> Nombre_FISCAL Or
                ModuloGeneral.ReceptorDomicilioCalle <> Direccion_FISCAL Or
                ModuloGeneral.ReceptorDomicilioCp <> CP_Fiscal Or
                ModuloGeneral.ReceptorRegimenFiscal <> Regimen_Fiscal Then

                ModuloGeneral.ReceptorNombre = Nombre_FISCAL
                ModuloGeneral.ReceptorDomicilioCalle = Direccion_FISCAL
                ModuloGeneral.ReceptorDomicilioCp = CP_Fiscal
                ModuloGeneral.ReceptorRegimenFiscal = Regimen_Fiscal
                Dim AUX As String = "SELECT TOP 1 FRF_DESCRIPCION  from [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[FEL_01_REGIMEN_FISCAL] where FRF_CLAVE_NOMBRE = " & Regimen_Fiscal
                ModuloGeneral.ReceptorRegimenFiscal_descripcion = Mdl_Facturacion.RECUPERA_VALOR_STRING(AUX)

                ACTUALIZAR_DATOS_FISCALES = True
            End If

            ModuloGeneral = Mdl_Facturacion.datos_fiscales(ModuloGeneral)
            ModuloGeneral = Mdl_Facturacion.datos_prefijo(ModuloGeneral)

            Mensaje = ValidaConsulta(ModuloGeneral)

            If Mensaje <> "" Then
                Cs_Respuesta_Folios.codigo = 0
                Cs_Respuesta_Folios.codigoError = 400
                Cs_Respuesta_Folios.mensaje = "No Se Puede Generar La Factura Porque Los Siguientes Datos Están Vacios!" & Mensaje

                Return Cs_Respuesta_Folios
            End If

            numero_factura = Mdl_Facturacion.numero_factura()
            'Forma_Pago As String, MetodoPago As String
            FE_MetodoPago = MetodoPago '' PUEDE SER PPD O PUE
            If FE_MetodoPago = "PPD" Then ''PAGO EN PARCIALIDADES O DIFERIDO
                Fe_Txt_MetodoPago = "PAGO EN PARCIALIDADES O DIFERIDO"
                Forma_Pago = 99
            Else    ''PUE
                Fe_Txt_MetodoPago = "PAGO EN UNA SOLA EXHIBICIÓN"
            End If

            ''se genero por defecto las factara con metodo de pago PPD
            FE_FormaPago = Forma_Pago
            If Integer.Parse(FE_FormaPago) < 10 Then
                FE_FormaPago = "0" + FE_FormaPago
            End If
            sqlString = "SELECT FFP_DESCRIPCION FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].FEL_05_FORMA_PAGO Where cast(FFP_CLAVE_NOMBRE as int) = cast('" & Forma_Pago & "' as int) "
            Fe_Txt_FormaPago = Mdl_Facturacion.RECUPERA_VALOR_STRING(sqlString)

            n_Uso_CFDI = Mdl_Facturacion.RecuperaUsoCFDI(UsoCFDI)

            FE_UsoCFDI = n_Uso_CFDI

            FE_CondicionesPago = "CONDICION 1"
            FE_Folio = ""
            FE_CFDIRelacionado = ""
            FE_TipoVenta = "REMISION"
            FE_Lugar = ""

            ' Generara el total de la venta diaria. 
            sqlString = "Select V.VVD_TOTAL From [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_01_VENTAS_DIARIAS] V Where V.VVD_CLAVE = " + VentaDiaria.ToString()
            FE_ImporteFactura = Mdl_Facturacion.RECUPERA_VALOR_STRING(sqlString)
            FE_ImporteTotal = FE_ImporteFactura
            FE_NumFacturita = Mdl_Facturacion.DAME_NUM_FACTURA() + 1
            '' PRUEBA DE CAMBIAR 26/02/2023
            G_folio = FE_NumFacturita
            ModuloGeneral.LlavePrivada = HttpContext.Current.Request.PhysicalApplicationPath + "XML\CSD_PRESIDENCIA_MUNICIPAL_DE_CIUDAD_LERDO_PMC951010FE3_20180424_094210.key"
            '================================================================================================== CREAR CFDI ======================================================================================================
            Try
                CrearCFD_40(HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")
            Catch ex As Exception
                '****************************************************************************'
                'ENVIO DE WHATS APP
                Try
                    Dim SOLUCION As String
                    'puerto.codigoError
                    Dim telefono_cliente As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT  (u.LUS_TELEFONO +'111,111'+  u.LUS_USUARIO) as Tel_usuario FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS U WHERE LUS_CLAVE = " + Usuario.ToString())
                    SOLUCION = "PASARLO A TI NOTA: _NO SE GENERO EL XML, VERIFICA LAS RELACIONES DE LA BASE DE DATOS DE " + Folios + " EN LAS TABLAS ING_03_INGRESOS, ING_02_CONCEPTOS, ALM_01_ARTICULOS, VTA_01_VENTAS_DIARIAS ETC. ETC_"
                    Dim RESPUESTA As String = Envia_Whatsapp_ERROR_FACTURACION(Nombre_FISCAL, telefono_cliente, RFC, "0", ex.Message, SOLUCION)
                Catch ex2 As Exception

                End Try
                '****************************************************************************'
                Cs_Respuesta_Folios.codigo = 0
                Cs_Respuesta_Folios.codigoError = 500
                Cs_Respuesta_Folios.mensaje = "No se realizo el timbrado, problemas al generar el CFDI. Comunicarse al CALL CENTER de Lerdo Digital (871) 134-8604 "
                Cs_Respuesta_Folios.objetoError = ex

                Return Cs_Respuesta_Folios
            End Try
            '================================================================================================== FIN CREAR CFDI ==================================================================================================
            Try
                '================================================================================================== TIMBRADO XML ==================================================================================================
                Dim queusuariocertus As String = ModuloGeneral.UsuarioTimbrado
                Dim quepasscertus As String = ModuloGeneral.ContraseñaTimbrado
                Dim queproceso As Integer = ModuloGeneral.pac_id
                'Dim queusuariocertus As String = "EWE1709045U0.Test"
                'Dim quepasscertus As String = "Prueba$1"
                'Dim queproceso As Integer = "194876591"
                'Dim MemStream As System.IO.MemoryStream = FileToMemory(ModuloGeneral.CarpetaAlmacenXML & "\" & ModuloGeneral.SeriePrefijo & CStr(folio) & ".xml") 'buscar archivo ya timbrado 
                Dim MemStream As MemoryStream  'buscar archivo Para timbrar.
                MemStream = FileToMemory(HttpContext.Current.Request.PhysicalApplicationPath + "XML" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")
                Dim archivo As Byte() = MemStream.ToArray()

                ServicePointManager.Expect100Continue = True
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                Dim service As New com.facturehoy.wsprod4.WsEmisionTimbrado40
                Dim serviceTest As New com.facturehoy.pruebasclientes.WsEmisionTimbrado40
                Dim puerto = service.EmitirTimbrar(queusuariocertus, quepasscertus, queproceso, archivo)

                If Not puerto.XML Is Nothing Then
                    If puerto.isError Then
                        '****************************************************************************'
                        'ENVIO DE WHATS APP
                        Try
                            Dim SOLUCION As String
                            'puerto.codigoError
                            Dim telefono_cliente As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT  (u.LUS_TELEFONO +'111,111'+  u.LUS_USUARIO) as Tel_usuario FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS U WHERE LUS_CLAVE = " + Usuario.ToString())
                            SOLUCION = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT CME_MENSAJE_PARA_USAURIO FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_33_MATRIZ_ERRORES_SAT WHERE CME_CÓDIGO_ERROR = '" + puerto.codigoError.ToString() + "' or CME_CÓDIGO_ERROR = 'CFDI40" + puerto.codigoError.ToString() + "' ")
                            If SOLUCION.Contains("CENTER") = True Then
                                SOLUCION = "Pasar Error a TI."
                            Else
                                SOLUCION = "Lo puede realizar el usuario"
                            End If
                            Dim RESPUESTA As String = Envia_Whatsapp_ERROR_FACTURACION(Nombre_FISCAL, telefono_cliente, RFC, puerto.codigoError, puerto.message, SOLUCION)
                        Catch ex As Exception

                        End Try
                        '****************************************************************************'
                        Cs_Respuesta_Folios.codigo = 0
                        Cs_Respuesta_Folios.codigoError = puerto.isError
                        Cs_Respuesta_Folios.mensaje = puerto.message

                        Return Cs_Respuesta_Folios
                    Else
                        File.WriteAllBytes(HttpContext.Current.Request.PhysicalApplicationPath + "XML" & "\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml", puerto.XML)
                    End If
                Else
                    Dim SOLUCION As String
                    Dim correcto As Boolean = False
                    '/******************************************** SE COMENTO TODO ESTE BLOQUE PARA PODER VER EL ERROR COMPLETO DE PUERTO. ********************************************/
                    '/*     ENVIAR WHATS APP CON EL ERROR A CLAUDIA     */
                    Try
                        'puerto.codigoError
                        Dim telefono_cliente As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT  (u.LUS_TELEFONO +'111,111'+  u.LUS_USUARIO) as Tel_usuario FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS U WHERE LUS_CLAVE = " + Usuario.ToString())
                        SOLUCION = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT CME_MENSAJE_PARA_USAURIO FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_33_MATRIZ_ERRORES_SAT WHERE CME_CÓDIGO_ERROR = '" + puerto.codigoError.ToString() + "' or CME_CÓDIGO_ERROR = 'CFDI40" + puerto.codigoError.ToString() + "' ")
                        Mensaje = SOLUCION
                        correcto = True
                        If SOLUCION.Contains("CENTER") = True Then
                            SOLUCION = "Pasar Error a TI."
                        Else
                            SOLUCION = "Lo puede realizar el usuario"
                        End If
                        Dim RESPUESTA As String = Envia_Whatsapp_ERROR_FACTURACION(Nombre_FISCAL, telefono_cliente, RFC, puerto.codigoError, puerto.message, SOLUCION)
                    Catch ex As Exception

                    End Try

                    If correcto = True Then
                        puerto.message = Mensaje
                    End If

                    Dim error_message As Boolean = False
                    If puerto.message.Contains("El campo Nombre del receptor, debe encontrarse en la lista de RFC inscritos no cancelados en el SAT") Then
                        puerto.message = "El Nombre Fiscal, no es valido"
                        'puerto.codigoError
                        error_message = True
                    End If
                    If puerto.message.Contains("El campo DomicilioFiscalReceptor") Then
                        puerto.message = "El CP Fiscal, no es valido"
                        error_message = True
                    End If
                    If puerto.message.Contains("Of attribute 'DomicilioFiscalReceptor' on element 'cfdi:Receptor' is not valid with respect to its type, '#AnonType_DomicilioFiscalReceptorReceptorComprobante") Then
                        puerto.message = "El CP Fiscal, no es valido"
                        error_message = True
                    End If
                    If puerto.message.Contains("'RegimenFiscalReceptor' del elemento 'cfdi:Receptor' debe contener al menos uno de los siguientes valores ") Then
                        puerto.message = "El Regimen Fiscal del Receptor no es valido."
                        error_message = True
                    End If
                    If puerto.message.Contains("'FormaPago' del elemento 'cfdi:Comprobante' debe contener") Then
                        puerto.message = "La Forma de Pago " & Fe_Txt_FormaPago & " no es valido. para el metodo de pago " & Fe_Txt_MetodoPago
                        error_message = True
                    End If
                    If error_message = False And correcto = False Then
                        puerto.message = "Verifique los datos de Facturación o comunicarse al CALL CENTER de Lerdo Digital (871) 134-8604."
                    End If


                    Cs_Respuesta_Folios.codigo = 0
                    Cs_Respuesta_Folios.codigoError = 400
                    Cs_Respuesta_Folios.mensaje = "Facturación no exitosa, " + puerto.message
                    Cs_Respuesta_Folios.objetoError = puerto
                    Return Cs_Respuesta_Folios

                End If
                '================================================================================================ FIN TIMBRADO XML ================================================================================================

                '======================================================================================== ACTUALIZA FOLIO FACTURACION =============================================================================================
                Dim SentSQL As String = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] SET CEP_FE_NUM_FACTURA = CEP_FE_NUM_FACTURA + 1 " & FE_NumFacturita & ""
                Mdl_Facturacion.ACTUALIZAR_CEP_FE_NUM_FACTURA(SentSQL)
                '==================================================================================== FIN ACTUALIZA FOLIO FACTURACION =============================================================================================
                Dim Diita = CStr(Format(DatePart(DateInterval.Day, Now), 0))
                Dim Yearsitito = CStr(DatePart(DateInterval.Year, Now))

                Dim VVD_FACTURACION_SERIE As String = ModuloGeneral.SeriePrefijo
                Dim VVD_FACTURACION_ESTATUS As String = "TIMBRADA"
                Dim VVD_FACTURACION_NOMBRE As String = ModuloGeneral.SeriePrefijo & CStr(VentaDiaria)

                Dim VVF_FECHALTA As Date = Now
                Dim VVD_FACTURACION_NUMERO As Integer = FE_NumFacturita
                Dim VVD_FACTURACION_FECHA As Date = Now
                Dim VVD_FACTURACION_SUPERVISOR As String = ModuloGeneral.ReceptorNombre 'Emp_NombreCorto

                SentSQL = "INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_10_VD_FACTURACION] (
                    VVF_FECHALTA, 
                    VVD_CLAVE, 
                    VVD_FACTURACION_SERIE,
                    VVD_FACTURACION_NUMERO,  
                    VVD_FACTURACION_FECHA, 
                    VVD_FACTURACION_SUPERVISOR,
                    VVD_FACTURACION_METODO_PAGO,
                    VVD_FACTURACION_CONDICIONES_PAGO,
                    VVD_FACTURACION_FORMA_PAGO,
                    VVD_FACTURACION_LUGAR_EXPEDICION,
                    VCL_CLAVE,
                    LUS_CLAVE) 
                VALUES
		    	    (CONVERT(SMALLDATETIME,GETDATE()), 
		    	    " & VentaDiaria.ToString() & ", 
		    	    '" & VVD_FACTURACION_SERIE & "', 
		    	    " & VVD_FACTURACION_NUMERO.ToString & ",
		    	    CONVERT(SMALLDATETIME,GETDATE()), 
		    	    '" & VVD_FACTURACION_SUPERVISOR & "',
		    	    '" & Fe_Txt_MetodoPago & "',
		    	    '" & FE_CondicionesPago & "',
		    	    '" & Fe_Txt_FormaPago & "',
		    	    '" & ModuloGeneral.FE_LUGARDEEXPEDICION & "' ,
		    	    " & ClienteId.ToString() & " ,
		    	    " & Usuario.ToString() & ")"

                Mdl_Facturacion.INSERTAR_VTA_10_VD_FACTURACION(SentSQL)

                CodigoBidimensional(VentaDiaria, RFC)

                Dim Fechacertificacion = FechaCertificadoSAT
                Dim VVF_CLAVE As Integer = Mdl_Facturacion.REGRESA_ULTIMO_REGISTRO(VentaDiaria) ''--SCH + 1

                SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_01_VENTAS_DIARIAS] SET VVD_FACTURACION_ESTATUS='FACTURADA', VVF_CLAVE=" & VVF_CLAVE & " WHERE VVD_CLAVE=" & VentaDiaria & ""
                Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)

                For i = 0 To UBound(FolioAr)
                    folio = FolioAr(i)

                    If (folio.Count() = 11) Then
                        SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] SET IIN_COBRO_ESTATUS = 'FACTURADA'  WHERE CDA_FOLIO = '" & ((folio.Substring(3, 8)).ToString()) & "'"
                        Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                    ElseIf (folio.Count() = 13) Then
                        SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] SET IIN_COBRO_ESTATUS  ='FACTURADA'  WHERE CDA_FOLIO = '" & ((folio.Substring(3, 10)).ToString()) & "'"
                        Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                    ElseIf (folio.Count() = 17) Then
                        SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] SET IIN_COBRO_ESTATUS = 'FACTURADA'  WHERE CDA_FOLIO = '" & ((folio.Substring(3, 14)).ToString()) & "'"
                        Mdl_Facturacion.ACTUALIZAR_VTA_01_VENTAS_DIARIAS(SentSQL)
                    End If

                Next

                Dim ACTUALIZA_DATOS_FISCALES_RES As String = ""
                If ACTUALIZAR_DATOS_FISCALES = True Then
                    sqlString = " EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_ACTUALIZAR_DATOS_FISCALES_PL] '" & Nombre_FISCAL & "', '" & CP_Fiscal & "' , '" & Direccion_FISCAL & "','" & Regimen_Fiscal & "','" & RFC & "'"
                    ACTUALIZA_DATOS_FISCALES_RES = Mdl_Facturacion.RECUPERA_VALOR_STRING(sqlString)
                End If

                Dim VFFP_NOMBRE As String = FE_FormaPago
                Dim VFCP_NOMBRE As String = FE_CondicionesPago
                Dim VFMP_NOMBRE As String = FE_MetodoPago ''--cbo_MetodosPago.SelectedValue
                Dim VFUC_NOMBRE As String = FE_UsoCFDI ''--cbo_UsoCFDI.SelectedValue
                Dim VFLE_LUGAR_EXPEDICION As String = ModuloGeneral.FE_LUGARDEEXPEDICION

                Dim SentSQLMP As String = "SELECT  FMP_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[FEL_04_METODO_PAGO] WITH (NOLOCK) WHERE  FMP_NOMBRE = '" & FE_MetodoPago & "'"
                VFMP_NOMBRE = Mdl_Facturacion.REGRESA_FEL_04_METODO_PAGO(SentSQLMP)

                Dim SentSQLUC As String = "SELECT  FUC_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[FEL_06_USO_CFDI] WITH (NOLOCK) WHERE  FUC_CLAVE_NOMBRE = '" & FE_UsoCFDI & "'"
                VFUC_NOMBRE = Mdl_Facturacion.REGRESA_FEL_06_USO_CFDI(SentSQLUC)

                SentSQL = "UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_10_VD_FACTURACION] " &
                            "SET FFP_CLAVE ='" & VFFP_NOMBRE & "'
                                , FCP_CLAVE ='" & 1 & "'
                                , FMP_CLAVE ='" & VFMP_NOMBRE & "'
                                , FUC_CLAVE= '" & VFUC_NOMBRE & "' 
                                , FMN_CLAVE = 100
                                , VVD_FECHA_TIMBRADA = GETDATE()
                                , VVD_UUID_FACTURA='" & UUID & "'" &
                            "WHERE VVD_CLAVE = " & VentaDiaria.ToString() & ""
                Mdl_Facturacion.ACTUALIZA_METODOS_FORMAS_PAGO(SentSQL)

                Dim numero2letras = UCase(letras(FE_ImporteTotal))

                Dim CadenaOriginalTimbre = "||" & VersionSAT & "|" & UUID & "|" & Fechacertificacion.ToString & "|" & SelloSAT & "|" & NoCertificadoSAT & "||"

                Dim html = "<html lang='es'>"
                ''SE PASO TODO EL HTML A VARIABLES
                Dim CUERPO_PDF As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Productos_PDF As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT AFE_HTML_INTERACION1 FROM  [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Productos_PDF_aux As String = ""
                Dim interacion_Catastrales_PDF As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT AFE_HTML_INTERACION2 FROM  [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Catastrales_PDF_aux As String = ""
                Dim TablaProductos As String = ""       '' Remplaza [TablaProductos]
                Dim ClabesCatastrales As String = ""    '' Remplaza [ClabesCatastrales]
                ''correcion del texto del pdf.
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorNombre]", ModuloGeneral.ReceptorNombre)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioCalle]", ModuloGeneral.ReceptorDomicilioCalle)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioCp]", ModuloGeneral.ReceptorDomicilioCp)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioMunicipio]", ModuloGeneral.ReceptorDomicilioMunicipio)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioEstado]", ModuloGeneral.ReceptorDomicilioEstado)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorRfc]", ModuloGeneral.ReceptorRfc)
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorRegimenFiscal_descripcion]", ModuloGeneral.ReceptorRegimenFiscal_descripcion)
                CUERPO_PDF = CUERPO_PDF.Replace("[VentaDiaria]", VentaDiaria.ToString())
                CUERPO_PDF = CUERPO_PDF.Replace("[FechaActual]", Now.ToString("dd/MM/yyyy"))
                CUERPO_PDF = CUERPO_PDF.Replace("[Fechacertificacion]", Convert.ToDateTime(Fechacertificacion).ToString("dd/MM/yyyy HH:mm:ss"))
                CUERPO_PDF = CUERPO_PDF.Replace("[UUID]", UUID)
                CUERPO_PDF = CUERPO_PDF.Replace("[codigoB64]", codigoB64)
                CUERPO_PDF = CUERPO_PDF.Replace("[numero2letras]", numero2letras)
                CUERPO_PDF = CUERPO_PDF.Replace("[Fe_Txt_FormaPago]", Fe_Txt_FormaPago)
                CUERPO_PDF = CUERPO_PDF.Replace("[METODO_PAGO_TEXTO]", Fe_Txt_MetodoPago)
                CUERPO_PDF = CUERPO_PDF.Replace("[VersionFE]", ModuloGeneral.VersionFE)
                CUERPO_PDF = CUERPO_PDF.Replace("[UsoCFDI]", UsoCFDI)
                'CDbl(FE_ImporteFactura).ToString("0.00", culture)
                'CUERPO_PDF = CUERPO_PDF.Replace("[FormatCurrency(0)]", FormatCurrency(0))
                'CUERPO_PDF = CUERPO_PDF.Replace("[FE_ImporteTotal]", FormatCurrency(FE_ImporteTotal))
                CUERPO_PDF = CUERPO_PDF.Replace("[FormatCurrency(0)]", "$" + CDbl(0).ToString("0.00", culture))
                CUERPO_PDF = CUERPO_PDF.Replace("[FE_ImporteTotal]", "$" + CDbl(FE_ImporteTotal).ToString("0.00", culture))
                CUERPO_PDF = CUERPO_PDF.Replace("[NoCertificado]", NoCertificado)
                CUERPO_PDF = CUERPO_PDF.Replace("[NoCertificadoSAT]", NoCertificadoSAT)
                CUERPO_PDF = CUERPO_PDF.Replace("[SelloCFD]", SelloCFD)
                CUERPO_PDF = CUERPO_PDF.Replace("[SelloSAT]", SelloSAT)
                CUERPO_PDF = CUERPO_PDF.Replace("[CadenaOriginalTimbre]", CadenaOriginalTimbre)

                ' insersion de intereacion de la tabla 
                For Each p In ModuloGeneral.productos
                    'cargamos el string
                    interacion_Productos_PDF_aux = interacion_Productos_PDF
                    'corregimos el string 
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Articulo]", p.Articulo.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.ClaveSAT]", p.ClaveSAT.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Cantidad]", CInt(p.Cantidad).ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.UnidadDeMedida]", p.UnidadDeMedida)
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Descripcion]", p.Descripcion)
                    'interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Subtotal]", FormatCurrency(p.Subtotal))
                    'interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.VVT_GD_DESC_IMPORTE]", FormatCurrency(p.VVT_GD_DESC_IMPORTE))
                    'interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Importe]", FormatCurrency(p.Importe))
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Subtotal]", "$" + CDbl(p.Subtotal).ToString("0.00", culture))
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.VVT_GD_DESC_IMPORTE]", "$" + CDbl(p.VVT_GD_DESC_IMPORTE).ToString("0.00", culture))
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Importe]", "$" + CDbl(p.Importe).ToString("0.00", culture))
                    'insertamos el string en la tabla
                    TablaProductos = TablaProductos + interacion_Productos_PDF_aux
                Next
                CUERPO_PDF = CUERPO_PDF.Replace("[TablaProductos]", TablaProductos)

                ''insercion de clave catastral
                For i = 0 To UBound(FolioAr)
                    folio = FolioAr(i)
                    If (folio.Count() = 11) Then
                        folio_fac = folio.Substring(3, (folio.Count() - 3))
                        sqlString = "SELECT TOP 1 
                        CASE WHEN P.CFC_FOLIO IS NULL THEN 
                            FX.CCF_CLAVE_CATASTRAL 
                            ELSE P.CFC_FOLIO 
                        END AS CFC_FOLIO 
		    			FROM  
		    			[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I
                        LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I2
		    			    ON I2.CDA_FOLIO = I.CDA_FOLIO
	                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAJ_03_COBROS] C
		                    ON I.IIN_CLAVE = C.IIN_CLAVE
                        LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAJ_03_COBROS] C2
		                    ON I2.IIN_CLAVE = C2.IIN_CLAVE  
		                LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CST_01_COBROS_FOX] FX
		    			    ON FX.CCF_CLAVE = '" & folio_fac.Substring(2, 6).ToString() & "' 
	                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS] F
		                    ON I.IIN_CLAVE = F.IIN_CLAVE
	                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS] P
		                    ON I.CDA_FOLIO = P.IIN_CLAVE
                        LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_01_VENTAS_DIARIAS] V
		                    ON C.VVD_CLAVE = V.VVD_CLAVE
		                    OR F.VVD_CLAVE = V.VVD_CLAVE
		                    OR P.VVD_CLAVE = V.VVD_CLAVE
                        OR C2.VVD_CLAVE= V.VVD_CLAVE
                        WHERE I.CDA_FOLIO = '" & folio_fac.ToString() & "' 
                        ORDER BY P.VVD_CLAVE DESC"
                        clave_Catastral = Mdl_Facturacion.RECUPERA_VALOR_STRING(sqlString)

                        'cargamos el string
                        interacion_Catastrales_PDF_aux = interacion_Catastrales_PDF
                        'corregimos el string
                        interacion_Catastrales_PDF_aux = interacion_Catastrales_PDF_aux.Replace("[clave_Catastral]", clave_Catastral)
                        'insertamos el string en la tabla
                        ClabesCatastrales = ClabesCatastrales + interacion_Catastrales_PDF_aux
                    End If
                Next

                CUERPO_PDF = CUERPO_PDF.Replace("[ClabesCatastrales]", ClabesCatastrales)

                Dim htmlString As String = CUERPO_PDF
                Dim pdf_page_size As String = "A4"
                Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize), pdf_page_size, True), PdfPageSize)
                Dim pdf_orientation As String = "Portrait"
                Dim pdfOrientation As PdfPageOrientation = DirectCast([Enum].Parse(GetType(PdfPageOrientation),
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
                doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".pdf")
                doc.Close()

                Dim sql As String = "SELECT LUS_CORREO FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].LDG_04_USUARIOS WHERE LUS_CLAVE = " & Usuario.ToString()
                Dim Correo As String = Mdl_Facturacion.RECUPERA_VALOR_STRING(sql)

                Dim bool As String = ENVIAR_CORREO("",
                                                   VentaDiaria.ToString(),
                                                   Correo,
                                                   "", "", "", "",
                                                   HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".pdf",
                                                   HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & "\APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")

                Cs_Respuesta.codigo = 1
                Cs_Respuesta.codigoError = 200
                Cs_Respuesta.mensaje = "Timbrado exitoso " & bool & ACTUALIZA_DATOS_FISCALES_RES

                Return Cs_Respuesta

            Catch ex As Exception
                Cs_Respuesta_Folios.codigo = 0
                Cs_Respuesta_Folios.codigoError = 500
                Cs_Respuesta_Folios.mensaje = "No se realizo el timbrado, " + ex.Message

                Return Cs_Respuesta_Folios
            End Try
        End Function

        'Nueva Facturacion 
        'EnviarFactura
        <HttpGet>
        <Route("api/Facturacion/TesteEstructura")>
        Public Function testestructura() As String
            Dim htmlString As String = ""
            Try
                Dim html = "<html lang='es'>"
                ''SE PASO TODO EL HTML A VARIABLES
                Dim CUERPO_PDF = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT TOP 1 AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Productos_PDF As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT  TOP 1 AFE_HTML_INTERACION1 FROM  [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Productos_PDF_aux As String = ""
                Dim interacion_Catastrales_PDF As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT TOP 1 AFE_HTML_INTERACION2 FROM  [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 1")
                Dim interacion_Catastrales_PDF_aux As String = ""
                Dim TablaProductos As String = ""       '' Remplaza [TablaProductos]
                Dim ClabesCatastrales As String = ""    '' Remplaza [ClabesCatastrales]
                ''correcion del texto del pdf.
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorNombre]", "Fernando MARTINEZ")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioCalle]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioCp]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioMunicipio]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorDomicilioEstado]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorRfc]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[ReceptorRegimenFiscal_descripcion]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[VentaDiaria]", 1)
                CUERPO_PDF = CUERPO_PDF.Replace("[FechaActual]", Now.ToString("dd/MM/yyyy"))
                CUERPO_PDF = CUERPO_PDF.Replace("[Fechacertificacion]", Convert.ToDateTime(Now).ToString("dd/MM/yyyy HH:mm:ss"))
                CUERPO_PDF = CUERPO_PDF.Replace("[UUID]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[codigoB64]", "")
                CUERPO_PDF = CUERPO_PDF.Replace("[numero2letras]", "")
                CUERPO_PDF = CUERPO_PDF.Replace("[Fe_Txt_FormaPago]", "")
                CUERPO_PDF = CUERPO_PDF.Replace("[METODO_PAGO_TEXTO]", Fe_Txt_MetodoPago)
                CUERPO_PDF = CUERPO_PDF.Replace("[UsoCFDI]", "")
                CUERPO_PDF = CUERPO_PDF.Replace("[VersionFE]", "")
                CUERPO_PDF = CUERPO_PDF.Replace("[FormatCurrency(0)]", "$" + CDbl(0).ToString("0.00", culture))
                CUERPO_PDF = CUERPO_PDF.Replace("[FE_ImporteTotal]", "$" + CDbl(10).ToString("0.00", culture))
                CUERPO_PDF = CUERPO_PDF.Replace("[NoCertificado]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[NoCertificadoSAT]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[SelloCFD]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[SelloSAT]", "XD")
                CUERPO_PDF = CUERPO_PDF.Replace("[CadenaOriginalTimbre]", "XD")

                ' insersion de intereacion de la tabla 
                For i = 0 To 10
                    'cargamos el string
                    interacion_Productos_PDF_aux = interacion_Productos_PDF
                    'corregimos el string 
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Articulo]", i.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.ClaveSAT]", i.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Cantidad]", CInt(i).ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.UnidadDeMedida]", i.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Descripcion]", i.ToString())
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Subtotal]", "$" + CDbl(i).ToString("0.00", culture))
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.VVT_GD_DESC_IMPORTE]", "$" + CDbl(i).ToString("0.00", culture))
                    interacion_Productos_PDF_aux = interacion_Productos_PDF_aux.Replace("[p.Importe]", "$" + CDbl(i).ToString("0.00", culture))
                    'insertamos el string en la tabla
                    TablaProductos = TablaProductos + interacion_Productos_PDF_aux
                Next
                CUERPO_PDF = CUERPO_PDF.Replace("[TablaProductos]", TablaProductos)

                ''insercion de clave catastral
                For i = 0 To 5
                    If (i Mod 2 = 0) Then
                        'cargamos el string
                        interacion_Catastrales_PDF_aux = interacion_Catastrales_PDF
                        'corregimos el string
                        interacion_Catastrales_PDF_aux = interacion_Catastrales_PDF_aux.Replace("[clave_Catastral]", i)
                        'insertamos el string en la tabla
                        ClabesCatastrales = ClabesCatastrales + interacion_Catastrales_PDF_aux
                    End If
                Next
                CUERPO_PDF = CUERPO_PDF.Replace("[ClabesCatastrales]", ClabesCatastrales)
                htmlString = CUERPO_PDF
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
                doc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "\XML\" & "APD_TEST_01.pdf")
                doc.Close()
                Return htmlString
            Catch ex As Exception
                Return htmlString + ": ERROR: " + ex.Message
            End Try
        End Function
        Private Function ValidaConsulta(ByVal ModuloGeneral As ModuloGeneral) As String
            Dim Mensaje As String = ""
            Dim i As Integer = 0
            If Trim(ModuloGeneral.EmisorNombre) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Nombre del emisor"
            ElseIf Trim(ModuloGeneral.EmisorDomicilioCalle) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Direccion del emisor"
            ElseIf Trim(ModuloGeneral.EmisorDomicilioColonia) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Colonia del emisor"
            ElseIf Trim(ModuloGeneral.EmisorDomicilioMunicipio) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Municipio del emisor"
            ElseIf Trim(ModuloGeneral.EmisorDomicilioEstado) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Estado del emisor"
            ElseIf Trim(ModuloGeneral.EmisorDomicilioCp) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "cp del emisor"
            ElseIf Trim(ModuloGeneral.EmisorRfc) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Rfc del emisor"
            ElseIf Trim(ModuloGeneral.EmisorPais) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Pais del emisor"
            ElseIf Trim(ModuloGeneral.ReceptorNombre) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Nombre del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorDomicilioCalle) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Direccion del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorDomicilioMunicipio) = " " Then
                Mensaje = Mensaje & vbCrLf & "" & "Ciudad del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorDomicilioEstado) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Estado del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorDomicilioCp) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Codigo postal del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorRfc) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Rfc del receptor"
            ElseIf Trim(ModuloGeneral.ReceptorPais) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Pais del receptor"
            ElseIf Trim(ModuloGeneral.CertificadoSelloDigital) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Certificado de sello digital"
            ElseIf Trim(ModuloGeneral.LlavePrivada) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Llave privada"
            ElseIf Trim(ModuloGeneral.ContraseñaLlavePrivada) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Contraseña de llave privada"
            ElseIf Trim(ModuloGeneral.CarpetaAlmacenXML) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Carpeta para almacenimiento de los XML"
            ElseIf Trim(ModuloGeneral.RegimenFiscal) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Regimen fiscal del emisor"
            ElseIf Trim(ModuloGeneral.LugarDeExpedicion) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Lugar de expedicion de los comprobantes"
            ElseIf Trim(ModuloGeneral.VersionFE) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Version de la factura electronica"
            ElseIf Trim(ModuloGeneral.TipoDeComprobante) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Tipo de comprobante"
            ElseIf Trim(ModuloGeneral.SeriePrefijo) = "" Then
                Mensaje = Mensaje & vbCrLf & "" & "Prefijo para archivos XML"
            End If

            For Each p In ModuloGeneral.productos
                i = i + 1

                If Trim(p.Cantidad) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "Cantidad en la fila " & i & " de los productos"
                ElseIf Trim(p.Descripcion) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "Descripcion en la fila " & i & " de los productos"
                ElseIf Trim(p.UnidadDeMedida) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "UnidadDeMedida en la fila " & i & " de los productos"
                ElseIf Trim(p.ValorUnitario) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "ValorUnitario en la fila " & i & " de los productos"
                ElseIf Trim(p.Importe) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "Importe en la fila " & i & " de los productos"
                    ''--SCH
                ElseIf Trim(p.PorcentajeIeps) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "PorcentajeIeps en la fila " & i & " de los productos"
                ElseIf Trim(p.ImporteIeps) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "ImporteIeps en la fila " & i & " de los productos"
                ElseIf Trim(p.PorcentajeIva) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "PorcentajeIva en la fila " & i & " de los productos"
                ElseIf Trim(p.ImporteIva) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "ImporteIva en la fila " & i & " de los productos"
                ElseIf Trim(p.ClaveProductoServicio) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "ClaveProductoServicio en la fila " & i & " de los productos"
                ElseIf Trim(p.CodigoDeProducto) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "CodigoDeProducto en la fila " & i & " de los productos"
                ElseIf Trim(p.ClaveUnidadDeMedida) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "ClaveUnidadDeMedida en la fila " & i & " de los productos"
                ElseIf Trim(p.Descuento) = "" Then
                    Mensaje = Mensaje & vbCrLf & "" & "Descuento en la fila " & i & " de los productos"
                End If
            Next

            Return Mensaje
        End Function
        ' ModuloGeneraCFDI ========================================================================================================================
        Public Sub CrearCFD(ByVal RutaXML As String)
            Dim Comprobante As XmlNode

            Try
                m_xmlDOM = CrearDOM()
                'Creamos el nodo raiz "Comprobante"
                Comprobante = CrearNodoComprobante()
                m_xmlDOM.AppendChild(Comprobante)
                IndentarNodo(Comprobante, 1)
                ''--SCH Crear Nodos relacionados Refacturación
                If FE_Folio <> "" Then
                    CrearNodoCfdiRelacionado(Comprobante)
                End If

                CrearNodoEmisor(Comprobante)
                IndentarNodo(Comprobante, 1)

                CrearNodoReceptor(Comprobante)
                IndentarNodo(Comprobante, 1)

                CrearNodoConceptos(Comprobante)
                IndentarNodo(Comprobante, 1)
                If ModuloGeneral.SumatoriaDeIvaEnConceptos > 0 Then
                    CrearNodoImpuestos(Comprobante)

                    IndentarNodo(Comprobante, 1)
                End If
                'CrearNodoComplemento(Comprobante)

                IndentarNodo(Comprobante, 0)

                'quitando esta linea no agrega ni certificado ni sello
                ''--CON EL CHILKAT 2021/10/27 SellarCFD(Comprobante)
                ''--2021/10/30 Sellar con el PFX
                SellarCFDConPfx(Comprobante)
                m_xmlDOM.InnerXml = (Replace(m_xmlDOM.InnerXml, "schemaLocation", "xsi:schemaLocation", , , CompareMethod.Text))

                m_xmlDOM.Save(RutaXML)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Sub CrearCFD_40(ByVal RutaXML As String)
            Dim Comprobante As XmlNode
            IMPORTE_EXENTO = 0
            IMPORTE_TASA = 0
            'Inicializamos la variable para que contenga el DOM del CFD
            m_xmlDOM = CrearDOM()
            'Creamos el nodo raiz "Comprobante"
            Comprobante = CrearNodoComprobante()
            m_xmlDOM.AppendChild(Comprobante)
            IndentarNodo(Comprobante, 1)
            ''--SCH 2023/03/06
            ''CrearNodoInformacionGlobal(Comprobante)
            ''--SCH Crear Nodos relacionados Refacturación
            If FE_Folio <> "" Then
                CrearNodoCfdiRelacionado(Comprobante)
            End If


            CrearNodoEmisor(Comprobante)
            IndentarNodo(Comprobante, 1)

            CrearNodoReceptor(Comprobante)
            IndentarNodo(Comprobante, 1)

            CrearNodoConceptos(Comprobante)
            IndentarNodo(Comprobante, 0)
            ''--If SumatoriaDeIvaEnConceptos > 0 Then
            CrearNodoImpuestos(Comprobante)

            IndentarNodo(Comprobante, 1)
            ''--End If
            ''--CrearNodoComplemento(Comprobante)

            '' IndentarNodo(Comprobante, 0)

            'quitando esta linea no agrega ni certificado ni sello
            ''--CON EL CHILKAT 2021/10/27  SellarCFD(Comprobante)
            ''--2021/10/27 Sellar con el PFX
            SellarCFDConPfx(Comprobante)
            m_xmlDOM.InnerXml = (Replace(m_xmlDOM.InnerXml, "schemaLocation", "xsi:schemaLocation", , , CompareMethod.Text))

            m_xmlDOM.Save(RutaXML)
        End Sub
        Private Function CrearDOM() As XmlDocument
            Dim oDOM As New XmlDocument
            Dim Nodo As XmlNode
            Nodo = oDOM.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""utf-8""")
            oDOM.AppendChild(Nodo)
            Nodo = Nothing
            CrearDOM = oDOM
        End Function
        Private Function CrearNodoComprobante() As XmlNode
            'Dim Comprobante As XmlNode
            Dim Comprobante As XmlElement
            Comprobante = m_xmlDOM.CreateElement("cfdi:Comprobante", URI_SAT)
            CrearAtributosComprobante(Comprobante)
            CrearNodoComprobante = Comprobante
        End Function
        Private Sub CrearAtributosComprobante(ByVal Nodo As XmlElement)
            Nodo.SetAttribute("xmlns:cfdi", URI_SAT)
            Nodo.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")
            'Nodo.SetAttribute("xsi:schemaLocation", "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv3.xsd", ")
            Nodo.SetAttribute("xsi:schemaLocation", "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd")
            'Nodo.SetAttribute("Version", ModuloGeneral.VersionFE)
            Nodo.SetAttribute("Version", ModuloGeneral.VersionFE)
            Nodo.SetAttribute("Serie", ModuloGeneral.SeriePrefijo)
            'Nodo.SetAttribute("folio", String.Format("{0:000000}", Convert.ToString(FE_NumFacturita)))
            Nodo.SetAttribute("Folio", CStr(G_folio))
            Nodo.SetAttribute("Fecha", Format(Now(), "yyyy-MM-ddThh:mm:ss"))
            Nodo.SetAttribute("Sello", "")
            'Nodo.SetAttribute("noAprobacion", "1")
            'Nodo.SetAttribute("anoAprobacion", "2011")
            ''--SCH
            ''FE_FormaPago = "PAGO EN UNA SOLA EXHIBICION"
            ''Nodo.SetAttribute("formaDePago", FE_FormaPago)
            ''FE_MetodoPago = "EFECTIVO"
            ''FE_ImporteFactura = FE_ImporteTotal ''--SCH FormatCurrency(CDec(FE_ImporteTotal / (1 + IVA_CONST)), 2)
            Nodo.SetAttribute("FormaPago", String.Format("{0:00}", FE_FormaPago))
            Nodo.SetAttribute("NoCertificado", "")
            Nodo.SetAttribute("Certificado", "")
            Nodo.SetAttribute("CondicionesDePago", FE_CondicionesPago) 'CREDITO, CONTADO, CREDITO A 3 MESES ETC
            ''--Nodo.SetAttribute("SubTotal", FE_ImporteFactura)
            Nodo.SetAttribute("SubTotal", CDbl(FE_ImporteFactura).ToString("0.00", culture))
            Nodo.SetAttribute("Total", CDbl(FE_ImporteTotal).ToString("0.00", culture))
            Nodo.SetAttribute("Moneda", "MXN")
            Nodo.SetAttribute("TipoDeComprobante", ModuloGeneral.TipoDeComprobante)
            Nodo.SetAttribute("Exportacion", ModuloGeneral.EmisorEXPORTACION)
            ''--Nodo.SetAttribute("metodoDePago", FE_MetodoPago)
            Nodo.SetAttribute("MetodoPago", String.Format("{0:00}", FE_MetodoPago))
            Nodo.SetAttribute("LugarExpedicion", ModuloGeneral.LugarDeExpedicion)

        End Sub
        Private Sub IndentarNodo(ByVal Nodo As XmlNode, ByVal Nivel As Long)
            Nodo.AppendChild(m_xmlDOM.CreateTextNode(vbNewLine & New String(ControlChars.Tab, Nivel)))
        End Sub
        Private Sub CrearNodoCfdiRelacionado(ByVal Nodo As XmlNode)

            Dim CfdiRelacionados As XmlElement
            Dim CfdiRelacionado As XmlElement

            CfdiRelacionados = CrearNodo("cfdi:CfdiRelacionados")
            If FE_TipoVenta = "NOTACREDITO" Then ''--SCH Nota de Credito
                CfdiRelacionados.SetAttribute("TipoRelacion", "01")
                IndentarNodo(CfdiRelacionados, 1)
            Else
                CfdiRelacionados.SetAttribute("TipoRelacion", "04")
                IndentarNodo(CfdiRelacionados, 1)
            End If

            ''--SCH si son varios ducumentos relacionados --- Realizar el Ciclo para agregarlos
            CfdiRelacionado = CrearNodo("cfdi:CfdiRelacionado")
            CfdiRelacionado.SetAttribute("UUID", FE_CFDIRelacionado)
            'IndentarNodo(CfdiRelacionado, 2)
            CfdiRelacionados.AppendChild(CfdiRelacionado)

            Nodo.AppendChild(CfdiRelacionados)
        End Sub
        Private Function CrearNodo(ByVal Nombre As String) As XmlNode
            CrearNodo = m_xmlDOM.CreateNode(XmlNodeType.Element, Nombre, URI_SAT)
        End Function
        Private Sub CrearNodoEmisor(ByVal Nodo As XmlNode)
            Dim Emisor As XmlElement
#Disable Warning BC42024 ' Variable local sin usar: 'DomFiscal'.
            Dim DomFiscal As XmlElement
#Enable Warning BC42024 ' Variable local sin usar: 'DomFiscal'.
            'Dim ExpedidoEn As XmlElement
#Disable Warning BC42024 ' Variable local sin usar: 'RegimenFiscal'.
            Dim RegimenFiscal As XmlElement
#Enable Warning BC42024 ' Variable local sin usar: 'RegimenFiscal'.

            Emisor = CrearNodo("cfdi:Emisor")
            Emisor.SetAttribute("Nombre", ModuloGeneral.EmisorNombre)
            Emisor.SetAttribute("Rfc", ModuloGeneral.EmisorRfc)
            Emisor.SetAttribute("RegimenFiscal", ModuloGeneral.RegimenFiscal)
            ''--SCH
            ''Emisor.SetAttribute("nombre", ModuloGeneral.EmisorNombre)
            ''Emisor.SetAttribute("rfc", ModuloGeneral.EmisorRfc)
            ''IndentarNodo(Emisor, 2)

            ''DomFiscal = CrearNodo("cfdi:DomicilioFiscal")
            ''DomFiscal.SetAttribute("calle", ModuloGeneral.EmisorDomicilioCalle)
            ''DomFiscal.SetAttribute("codigoPostal", ModuloGeneral.EmisorDomicilioCp)
            ''DomFiscal.SetAttribute("colonia", ModuloGeneral.EmisorDomicilioColonia)
            ''DomFiscal.SetAttribute("estado", ModuloGeneral.EmisorDomicilioEstado)
            ''DomFiscal.SetAttribute("municipio", ModuloGeneral.EmisorDomicilioMunicipio)
            ''DomFiscal.SetAttribute("pais", ModuloGeneral.EmisorPais)
            ''Emisor.AppendChild(DomFiscal)
            ''IndentarNodo(Emisor, 2)

            'ExpedidoEn = CrearNodo("ExpedidoEn")
            'ExpedidoEn.SetAttribute("calle", ModuloGeneral.EmisorExpedidoEnCalle)
            'ExpedidoEn.SetAttribute("codigoPostal", ModuloGeneral.EmisorExpedidoEnCp)
            'ExpedidoEn.SetAttribute("colonia", ModuloGeneral.EmisorExpedidoEnColonia)
            'ExpedidoEn.SetAttribute("estado", ModuloGeneral.EmisorExpedidoEnEstado)
            'ExpedidoEn.SetAttribute("localidad", ModuloGeneral.EmisorExpedidoEnLocalidad)
            'ExpedidoEn.SetAttribute("municipio", ModuloGeneral.EmisorExpedidoEnMunicipio)
            'ExpedidoEn.SetAttribute("noExterior", ModuloGeneral.EmisorExpedidoEnNoExterior)
            'ExpedidoEn.SetAttribute("pais", ModuloGeneral.EmisorExpedidoEnPais)
            'Emisor.AppendChild(ExpedidoEn)
            'IndentarNodo(Emisor, 1)

            ''--SCH
            ''RegimenFiscal = CrearNodo("cfdi:RegimenFiscal")
            ''RegimenFiscal.SetAttribute("Regimen", ModuloGeneral.RegimenFiscal)
            ''Emisor.AppendChild(RegimenFiscal)
            ''IndentarNodo(Emisor, 1)

            Nodo.AppendChild(Emisor)
        End Sub
        Private Sub CrearNodoReceptor(ByVal Nodo As XmlNode)
            Dim Receptor As XmlElement
#Disable Warning BC42024 ' Variable local sin usar: 'Domicilio'.
            Dim Domicilio As XmlElement
#Enable Warning BC42024 ' Variable local sin usar: 'Domicilio'.

            Receptor = CrearNodo("cfdi:Receptor")
            Receptor.SetAttribute("Rfc", ModuloGeneral.ReceptorRfc)
            Receptor.SetAttribute("Nombre", ModuloGeneral.ReceptorNombre)
            Receptor.SetAttribute("UsoCFDI", FE_UsoCFDI) ''-- "G01" Este valor debemos de traerlo de la tabla de FEL_06_USO_CFDI





            'Xml 4.0
            Receptor.SetAttribute("DomicilioFiscalReceptor", ModuloGeneral.ReceptorDomicilioCp)
            '****************************************************************************************************

            '**************************************************************************************************
            'Xml 4.0
            Receptor.SetAttribute("RegimenFiscalReceptor", ModuloGeneral.ReceptorRegimenFiscal)
            '****************************************************************************************************

            ''--SCH
            ''Receptor.SetAttribute("nombre", ModuloGeneral.ReceptorNombre)
            ''Receptor.SetAttribute("rfc", ModuloGeneral.ReceptorRfc)
            ''IndentarNodo(Receptor, 2)

            ''Domicilio = CrearNodo("cfdi:Domicilio")
            ''Domicilio.SetAttribute("calle", ModuloGeneral.ReceptorDomicilioCalle)
            ''Domicilio.SetAttribute("codigoPostal", ModuloGeneral.ReceptorDomicilioCp)
            ''Domicilio.SetAttribute("estado", ModuloGeneral.ReceptorDomicilioEstado)
            ''Domicilio.SetAttribute("municipio", ModuloGeneral.ReceptorDomicilioMunicipio)
            ''Domicilio.SetAttribute("pais", ModuloGeneral.ReceptorPais)
            ''Receptor.AppendChild(Domicilio)
            ''IndentarNodo(Receptor, 1)

            Nodo.AppendChild(Receptor)
        End Sub
        Private Sub CrearNodoConceptos(ByVal Nodo As XmlNode)
            Dim Conceptos As XmlElement
            Dim Concepto As XmlElement

            Dim Impuestos As XmlElement
            Dim Traslados As XmlElement
            Dim Traslado As XmlElement
#Disable Warning BC42024 ' Variable local sin usar: 'Retenciones'.
            Dim Retenciones As XmlElement
#Enable Warning BC42024 ' Variable local sin usar: 'Retenciones'.
#Disable Warning BC42024 ' Variable local sin usar: 'Retencion'.
            Dim Retencion As XmlElement
#Enable Warning BC42024 ' Variable local sin usar: 'Retencion'.
            Dim DescuentoDetalle As Double

            Conceptos = CrearNodo("cfdi:Conceptos")
            IndentarNodo(Conceptos, 2)

            For Each p In ModuloGeneral.productos
                ''Concepto = CrearNodo("cfdi:Concepto")
                ''Concepto.SetAttribute("cantidad", Cantidad(c))
                ''Concepto.SetAttribute("descripcion", Descripcion(c))
                ''Concepto.SetAttribute("unidad", UnidadDeMedida(c))
                ''Concepto.SetAttribute("valorUnitario", ValorUnitario(c))
                ''Concepto.SetAttribute("importe", Importe(c))
                ''Conceptos.AppendChild(Concepto)
                ''IndentarNodo(Conceptos, 2)
                ''Concepto = Nothing 
                DescuentoDetalle = 0
                Concepto = CrearNodo("cfdi:Concepto")
                Concepto.SetAttribute("ClaveProdServ", p.ClaveProductoServicio)
                Concepto.SetAttribute("NoIdentificacion", p.CodigoDeProducto)
                Concepto.SetAttribute("Cantidad", CDbl(p.Cantidad).ToString("0.00", culture))
                Concepto.SetAttribute("ClaveUnidad", p.ClaveUnidadDeMedida)
                Concepto.SetAttribute("Unidad", p.UnidadDeMedida)
                Concepto.SetAttribute("Descripcion", p.Descripcion)
                Concepto.SetAttribute("ValorUnitario", CDbl(p.ValorUnitario).ToString("0.000000", culture))
                Concepto.SetAttribute("Importe", CDbl(p.Importe).ToString("0.000000", culture))
                ''If Descuento(c) <0 Then
                ''    Concepto.SetAttribute("Descuento", Importe(c) * (Descuento(c) / 100))
                ''End If
                If p.Descuento > 0 Then
                    DescuentoDetalle = CDbl(p.ValorUnitario * (p.Descuento / 100) * p.Cantidad).ToString("0.000000", culture)
                    Concepto.SetAttribute("Descuento", DescuentoDetalle)
                End If
                'Xml 4.0
                Concepto.SetAttribute("ObjetoImp", p.AAR_OBJETO_IMP)
                If p.ImporteIva > 0 Or p.ImporteIva = 0 Then ''--SCH Or IvaExento(c) = "True" Or ImporteIepsXConcepto(c) > 0 Then
                    Impuestos = CrearNodo("cfdi:Impuestos")
                    IndentarNodo(Impuestos, 4)

                    Traslados = CrearNodo("cfdi:Traslados")
                    IndentarNodo(Traslados, 5)

                    If p.ImporteIva > 0 Or p.ImporteIva = 0 Then ''--SCH Or IvaExento(c) = "True" Then
                        Traslado = CrearNodo("cfdi:Traslado")
                        ''If ImporteIepsXConcepto(c) > 0 Then
                        ''    If Descuento(c) > 0 Then
                        ''        'Traslado.SetAttribute("Base", Format(CDbl(Importe(c) + ImporteIepsXConcepto(c)), "0.00"))
                        ''        Traslado.SetAttribute("Base", Format(CDbl((ValorUnitario(c) * Cantidad(c)) - DescuentoDetalle + ImporteIepsXConcepto(c)), "0.00"))
                        ''    Else
                        ''        'Traslado.SetAttribute("Base", Format(CDbl(Importe(c) + ImporteIepsXConcepto(c)), "0.00"))
                        ''        Traslado.SetAttribute("Base", Format(CDbl((ValorUnitario(c) * Cantidad(c)) - DescuentoDetalle + ImporteIepsXConcepto(c)), "0.00"))
                        ''    End If
                        ''Else
                        If p.Descuento > 0 Then
                            'Traslado.SetAttribute("Base", Format(CDbl(Importe(c)), "0.00"))
                            Traslado.SetAttribute("Base", CDbl((p.ValorUnitario * p.Cantidad) - DescuentoDetalle).ToString("0.000000", culture))
                        Else
                            'Traslado.SetAttribute("Base", Format(CDbl(Importe(c)), "0.00"))
                            Traslado.SetAttribute("Base", CDbl(p.ValorUnitario * p.Cantidad).ToString("0.000000", culture))
                        End If
                        ''End If
                        Traslado.SetAttribute("Impuesto", "002")
                        If p.ImporteIva = 0 Then ''--IvaExento(c) = "True" Then
                            Traslado.SetAttribute("TipoFactor", "Exento")
                            IMPORTE_EXENTO += CDbl(p.ValorUnitario * p.Cantidad)
                        ElseIf p.ImporteIva > 0 Then
                            Traslado.SetAttribute("TipoFactor", "Tasa")
                            'Traslado.SetAttribute("TasaOCuota", String.Format("{0:000000}", PorcentajeIva(c)))
                            Traslado.SetAttribute("TasaOCuota", Convert.ToString(p.PorcentajeIva) & "0000")
                            'Traslado.SetAttribute("Importe", Format(CDbl(ImporteIva(c)), "#.#000"))
                            Traslado.SetAttribute("Importe", CDbl(p.ImporteIva).ToString("0.000000", culture))
                            IMPORTE_TASA += CDbl(p.ValorUnitario * p.Cantidad)
                        ElseIf p.ImporteIva = 0 Then
                            Traslado.SetAttribute("TipoFactor", "Tasa")
                            Traslado.SetAttribute("TasaOCuota", "0.000000")
                            Traslado.SetAttribute("Importe", CDbl(p.ImporteIva).ToString("0.000000", culture))
                        End If
                        Traslados.AppendChild(Traslado)
                    End If
                    Impuestos.AppendChild(Traslados)
                    IndentarNodo(Traslados, 4)

                    Concepto.AppendChild(Impuestos)
                    IndentarNodo(Impuestos, 3)
                End If
                Conceptos.AppendChild(Concepto)
                If p.ImporteIva > 0 Then
                    IndentarNodo(Concepto, 2)
                End If
                IndentarNodo(Conceptos, 1)
                Concepto = Nothing
            Next

            Nodo.AppendChild(Conceptos)
        End Sub
        Private Sub CrearNodoImpuestos(ByVal Nodo As XmlNode)
            Dim Impuestos As XmlElement
            Dim Traslados As XmlElement
            Dim Traslado As XmlElement
            ''--
            Impuestos = CrearNodo("cfdi:Impuestos")

            ''If FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIsr.Text > 0 Or FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIvaFederal.Text > 0 Then
            ''    Impuestos.SetAttribute("TotalImpuestosRetenidos", Format((CDbl(FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIsr.Text) + CDbl(FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIvaFederal.Text)), "0.00"))
            ''Else
            ''    Impuestos.SetAttribute("TotalImpuestosRetenidos", "0.00")
            ''End If
            ''--SCH IEPS Impuestos.SetAttribute("TotalImpuestosTrasladados", Format(CDbl(SumatoriaDeIvaEnConceptos + SumatoriaDeIepsEnConceptos), "0.00"))
            If IMPORTE_TASA > 0 Then
                Impuestos.SetAttribute("TotalImpuestosTrasladados", Format(Math.Truncate(ModuloGeneral.SumatoriaDeIvaEnConceptos * 100) / 100, "0.00"))
            End If
            '	Impuestos.SetAttribute("TotalImpuestosTrasladados", Format(Math.Truncate(ModuloGeneral.SumatoriaDeIvaEnConceptos * 100) / 100, "0.00"))
            IndentarNodo(Impuestos, 2)

            'ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
            ''If (FrmFacturaElectronicaIvaXProducto.CboSerie.SelectedValue = 3 Or FrmFacturaElectronicaIvaXProducto.CboSerie.SelectedValue = 4 Or FrmFacturaElectronicaIvaXProducto.CboSerie.SelectedValue = 1 And FrmFacturaElectronicaIvaXProducto.ChkFacturaConIvaRetenidoFederal.Checked = True) _
            ''    And FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIsr.Text > 0 Or FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIvaFederal.Text > 0 Then

            ''    Dim Retenciones As XmlElement
            ''    Dim Retencion As XmlElement

            ''    Retenciones = CrearNodo("cfdi:Retenciones")
            ''    IndentarNodo(Retenciones, 3)
            ''    'Impuestos.AppendChild(Retenciones)

            ''    If FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIsr.Text > 0 Then
            ''        Retencion = CrearNodo("cfdi:Retencion")
            ''        Retencion.SetAttribute("Impuesto", "001")
            ''        Retencion.SetAttribute("Importe", Format(CDbl(FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIsr.Text), "0.00"))
            ''        Retenciones.AppendChild(Retencion)
            ''    End If

            ''    If FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIvaFederal.Text > 0 Then
            ''        'Retenciones = CrearNodo("cfdi:Retencion")
            ''        'IndentarNodo(Retenciones, 3)
            ''        'Impuestos.AppendChild(Retenciones)
            ''        Retencion = CrearNodo("cfdi:Retencion")
            ''        Retencion.SetAttribute("Impuesto", "002")
            ''        Retencion.SetAttribute("Importe", Format(CDbl(FrmFacturaElectronicaIvaXProducto.TxtImporteRetencionIvaFederal.Text), "0.00"))
            ''        Retenciones.AppendChild(Retencion)
            ''    End If

            ''    IndentarNodo(Retenciones, 2)
            ''    Impuestos.AppendChild(Retenciones)
            ''End If
            ''IndentarNodo(Impuestos, 1)
            'ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
            ''--SCH If FrmFacturaElectronicaIvaXProducto.TxtImporteTotalIeps.Text > 0 Or FrmFacturaElectronicaIvaXProducto.TxtImporteTotalIva.Text > 0 Then
            ''If SumatoriaDeIvaEnConceptos > 0 Then
            Traslados = CrearNodo("cfdi:Traslados")
            IndentarNodo(Traslados, 3)
            Impuestos.AppendChild(Traslados)






            If IMPORTE_EXENTO > 0 Then
                Traslado = CrearNodo("cfdi:Traslado")
                Traslado.SetAttribute("Base", CDbl(IMPORTE_EXENTO).ToString("0.00", culture))
                Traslado.SetAttribute("Impuesto", "002")
                Traslado.SetAttribute("TipoFactor", "Exento")
                Traslados.AppendChild(Traslado)
            End If
            If IMPORTE_TASA > 0 Then
                Traslado = CrearNodo("cfdi:Traslado")
                Traslado.SetAttribute("Base", CDbl(IMPORTE_TASA).ToString("0.00", culture))
                Traslado.SetAttribute("Impuesto", "002")
                Traslado.SetAttribute("TipoFactor", "Tasa")
                Traslado.SetAttribute("TasaOCuota", "0.160000")
                Traslado.SetAttribute("Importe", Format(Math.Truncate(ModuloGeneral.SumatoriaDeIvaEnConceptos * 100) / 100, "0.00"))
                Traslados.AppendChild(Traslado)
            End If

            ''If SumatoriaDeIvaEnConceptos > 0 Then
            ''For Each oDatarowIeps As DataRow In oDatasetFE.Tables("Iva").Rows
            'Traslado = CrearNodo("cfdi:Traslado")
            'Traslado.SetAttribute("Impuesto", "002")
            'Traslado.SetAttribute("TipoFactor", "Tasa")
            'If ModuloGeneral.SumatoriaDeIvaEnConceptos = 0 Then
            '	Traslado.SetAttribute("TasaOCuota", "0.000000")   ''--SCH condicionar cuando sea el IVA a cero
            'Else
            '	''Traslado.SetAttribute("TasaOCuota", Convert.ToString(PorcentajeIva(0)) & "000000")
            '	Traslado.SetAttribute("TasaOCuota", "0.160000")
            'End If

            'Traslado.SetAttribute("Importe", Format(Math.Truncate(ModuloGeneral.SumatoriaDeIvaEnConceptos * 100) / 100, "0.00"))

            'Traslados.AppendChild(Traslado)
            ''Next
            ''End If
            ''If FrmFacturaElectronicaIvaXProducto.TxtImporteTotalIeps.Text > 0 Then
            ''    For Each oDatarowIeps As DataRow In oDatasetFE.Tables("Ieps").Rows
            ''        Traslado = CrearNodo("cfdi:Traslado")
            ''        Traslado.SetAttribute("Impuesto", "003")
            ''        Traslado.SetAttribute("TipoFactor", "Tasa")
            ''        Traslado.SetAttribute("TasaOCuota", Format(CDbl(oDatarowIeps("PORCENTAJEIEPS")), "0.000000"))
            ''        Traslado.SetAttribute("Importe", Format(CDbl(oDatarowIeps("SUMAIMPORTEIEPS")), "0.00"))
            ''        Traslados.AppendChild(Traslado)
            ''    Next
            ''End If

            IndentarNodo(Traslados, 2)
            Impuestos.AppendChild(Traslados)
            IndentarNodo(Impuestos, 1)
            ''End If
            Nodo.AppendChild(Impuestos)
            ''--
            ''Impuestos = CrearNodo("cfdi:Impuestos")
            ''Impuestos.SetAttribute("TotalImpuestosTrasladados", Format(CDbl(SumatoriaDeIvaEnConceptos), "0.00"))
            ''IndentarNodo(Impuestos, 2)

            ''Traslados = CrearNodo("cfdi:Traslados")
            ''IndentarNodo(Traslados, 3)
            ''Impuestos.AppendChild(Traslados)

            ''Traslado = CrearNodo("cfdi:Traslado")
            ''Traslado.SetAttribute("Impuesto", "002")
            ''Traslado.SetAttribute("TipoFactor", "Tasa")

            ''Traslado.SetAttribute("TasaOCuota", "0.000000")   ''--SCH condicionar cuando sea el IVA a cero
            ''Traslado.SetAttribute("Importe", Format(CDbl(SumatoriaDeIvaEnConceptos), "0.00"))



            ''Traslados.AppendChild(Traslado)
            ''IndentarNodo(Traslados, 2)
            ''Impuestos.AppendChild(Traslados)
            ''IndentarNodo(Impuestos, 1)

            ''Nodo.AppendChild(Impuestos)
        End Sub
        ''--SCH 2021/10/30 Se comento por que es por chilkat
        ''''        Public Sub SellarCFD(ByVal NodoComprobante As XmlElement)
        ''''            'Instanciar el objeto X509Certificate2 
        ''''            Dim objCert As New X509Certificate2()
        ''''            'y pasarle el nombre y ruta del Cerfificado para obtener la información en bytes
        ''''            'Dim bRawData As Byte() = ReadFile(ModuloGeneral.CertificadoSelloDigital)
        ''''            Dim bRawData As Byte() = ReadFile(HttpContext.Current.Request.PhysicalApplicationPath + "XML\CSD_SAP990715_SAP990715876_20180119_130338s.cer")
        ''''            'Importamos la información 
        ''''            objCert.Import(bRawData)
        ''''            'Retornamos la información del certificado en Base64
        ''''            Dim cadena As String = Convert.ToBase64String(bRawData)

        ''''            ''la variable cadena tiene la informacion del certificado en Base64 y es la que hay que poner en el atributo certificado del nodo Complemento
        ''''            ''comentando las dos lineas siguientes no agrega el certificado al comprobante xml
        ''''            'Dim xmlComprobante = NodoComprobante.GetElementsByTagName("cfdi:Comprobante")
        ''''            'xmlComprobante.Item(0).Attributes("noCertificado").Value = FormatearSerieCert(objCert.SerialNumber)
        ''''            'xmlComprobante.Item(0).Attributes("certificado").Value = Convert.ToBase64String(bRawData)
        ''''            ''comentando la siguiente linea no agregar el sello al comprobante xml
        ''''            'NodoComprobante.SetAttribute("sello", GenerarSello())

        ''''            'comentando las dos lineas siguientes no agrega el certificado al comprobante xml
        ''''            NodoComprobante.SetAttribute("NoCertificado", FormatearSerieCert(objCert.SerialNumber))
        ''''            NodoComprobante.SetAttribute("Certificado", Convert.ToBase64String(bRawData))
        ''''#Disable Warning BC40000 ' 'Public Overridable Overloads Function GetName() As String' está obsoleto: 'This method has been deprecated.  Please use the Subject property instead.  http://go.microsoft.com/fwlink/?linkid=14202'.
        ''''            Dim R As String = objCert.GetName
        ''''#Enable Warning BC40000 ' 'Public Overridable Overloads Function GetName() As String' está obsoleto: 'This method has been deprecated.  Please use the Subject property instead.  http://go.microsoft.com/fwlink/?linkid=14202'.
        ''''            'comentando la siguiente linea no agregar el sello al comprobante xml
        ''''            NodoComprobante.SetAttribute("Sello", GenerarSello())
        ''''        End Sub

        ''--SCH 2021/10/30
        Public Sub SellarCFDConPfx(ByVal NodoComprobante As XmlElement)
            Dim objCert As New X509Certificate2()
            ''--SCH LOCAL Dim bRawData As Byte() = ReadFile(ModuloGeneral.CertificadoSelloDigital) 
            Dim bRawData As Byte() = ReadFile(HttpContext.Current.Request.PhysicalApplicationPath + "XML\00001000000410501522.cer")
            objCert.Import(bRawData)
            Dim cadena As String = Convert.ToBase64String(bRawData)
            NodoComprobante.SetAttribute("NoCertificado", FormatearSerieCert(objCert.SerialNumber))
            NodoComprobante.SetAttribute("Certificado", Convert.ToBase64String(bRawData))
            NodoComprobante.SetAttribute("Sello", GenerarSelloConPfx())
        End Sub
        Private Function GenerarSelloConPfx() As String
            ''--SCH LOCAL Dim privateCert As New X509Certificate2(CType(ModuloGeneral.CertificadoPFX, Byte()), CStr(ModuloGeneral.ContaseñaCertificadoPFX), X509KeyStorageFlags.Exportable)   
            Dim privateCert As New X509Certificate2(HttpContext.Current.Request.PhysicalApplicationPath + "XML\00001000000410501522.pfx", CStr(ModuloGeneral.ContaseñaCertificadoPFX), X509KeyStorageFlags.MachineKeySet Or X509KeyStorageFlags.PersistKeySet Or X509KeyStorageFlags.Exportable)
            Dim privateKey As RSACryptoServiceProvider = DirectCast(privateCert.PrivateKey, RSACryptoServiceProvider)
            Dim privateKey1 As New RSACryptoServiceProvider()
            privateKey1.ImportParameters(privateKey.ExportParameters(True))
            Dim stringCadenaOriginal() As Byte = System.Text.Encoding.UTF8.GetBytes(GetCadenaOriginal(m_xmlDOM.InnerXml))
            Dim signature As Byte() = privateKey1.SignData(stringCadenaOriginal, "SHA256")
            Dim sello256 As String = Convert.ToBase64String(signature)
            'para verificar el sello
            Dim isValid As Boolean = privateKey1.VerifyData(stringCadenaOriginal, "SHA256", signature)
            GenerarSelloConPfx = sello256
        End Function
        ''--SCH
        Function ReadFile(ByVal strArchivo As String) As Byte()
            Dim f As New FileStream(strArchivo, FileMode.Open, FileAccess.Read)
            Dim size As Integer = CInt(f.Length)
            Dim data As Byte() = New Byte(size - 1) {}
            size = f.Read(data, 0, size)
            f.Close()
            Return data
        End Function

        ''--SCH 2021/10/30 Se comento por que es por chilkat
        ''''        Private Function GenerarSello() As String
        ''''            Try
        ''''                ''--SCH
        ''''                ''Dim objCertPfx As New X509Certificate2("C:\APD\02_DOCUMENTOS\08_FE\01_PKI\00001000000405844691.pfx", "SER09112")
        ''''                ''Dim lRSA As RSACryptoServiceProvider = objCertPfx.PrivateKey
        ''''                ''Dim lhasher As New SHA1CryptoServiceProvider()
        ''''                ''Dim bytesFirmados As Byte() = lRSA.SignData(System.Text.Encoding.UTF8.GetBytes(GetCadenaOriginal(m_xmlDOM.InnerXml)), lhasher)
        ''''                ''Return Convert.ToBase64String(bytesFirmados)

        ''''                Dim pkey As New Chilkat.PrivateKey
        ''''                Dim pkeyXml As String
        ''''                Dim rsa As New Chilkat.Rsa
        ''''                pkey.LoadPkcs8EncryptedFile(ModuloGeneral.LlavePrivada, ModuloGeneral.ContraseñaLlavePrivada)
        ''''                pkeyXml = pkey.GetXml()
        ''''                rsa.UnlockComponent(CK_KEY)
        ''''                rsa.ImportPrivateKey(pkeyXml)
        ''''                rsa.Charset = "utf-8"
        ''''                rsa.EncodingMode = "base64"
        ''''                rsa.LittleEndian = 0
        ''''                Dim base64Sig As String
        ''''                base64Sig = rsa.SignStringENC(GetCadenaOriginal(m_xmlDOM.InnerXml), "sha256")
        ''''                Dim Prueba1 As String = "MX7pXYsjwzPzuLaj9/cy80bVb0YQ35pnt4zNd6RAe7+VOsB3SVzbjenfCkuiS+prE6Q8W9Iw/9Xax13ct2ftMPl9iZQYGEMazWBNspFeXH2MZ1nMu9lsiNcNqIf2G2SOrjsHIgkcUArYhaxBJokLEWHZ7cZawT74w4+D+RP8cGjJS11GDibmCduQHPB7fdgXj5e5sm4JUO1X74azdrP5rZmXaaEGuFzsD1IDUkzCg//drar7lGikYMVrgxDEYsUEzWgfkbOiAAeQ1sirBVcKgT6CnfgY/CE7jc+Pi5FR1B3AAUNmOoiWcc9DrMxP0T+a9QmHiv1e0BVvieCPHTbAfw=="
        ''''                GenerarSello = base64Sig

        ''''            Catch oExcep As Exception
        ''''                ' MsgBox(oExcep.Message)
        ''''            Finally

        ''''            End Try
        ''''#Disable Warning BC42105 ' La función 'GenerarSello' no devuelve un valor en todas las rutas de acceso de código. Puede producirse una excepción de referencia NULL en tiempo de ejecución cuando se use el resultado.
        ''''        End Function
        Public Function GetCadenaOriginal(ByVal xmlCFD As String) As String
            Dim xslt As New System.Xml.Xsl.XslCompiledTransform
            Dim xmldoc As New System.Xml.XmlDocument
            Dim navigator As System.Xml.XPath.XPathNavigator
            Dim output As New IO.StringWriter
            xmldoc.LoadXml(xmlCFD)
            navigator = xmldoc.CreateNavigator()
            xslt.Load(HttpContext.Current.Request.PhysicalApplicationPath + "XML\" & NOMBRE_XSLT_4_0)
            xslt.Transform(navigator, Nothing, output)
            GetCadenaOriginal = output.ToString
            'FrmApartirDeCadenaOriginal.TxtCadenaOriginalFinal.Text = GetCadenaOriginal
        End Function
        Public Function FormatearSerieCert(ByVal Serie As String) As String
            Dim Resultado As String = ""
            Dim I As Integer
            For I = 2 To Len(Serie) Step 2
                Resultado = Resultado & Mid(Serie, I, 1)
            Next
            FormatearSerieCert = Resultado
        End Function
        ' ModuloGeneraCFDI ========================================================================================================================
        Private Function FileToMemory(ByVal Filename As String) As IO.MemoryStream
            'Throw New NotImplementedException
            Dim FS As New System.IO.FileStream(Filename, IO.FileMode.Open)
            Dim MS As New System.IO.MemoryStream
            Dim BA(FS.Length - 1) As Byte
            FS.Read(BA, 0, BA.Length)
            FS.Close()
            MS.Write(BA, 0, BA.Length)
            Return MS
        End Function
        Private Sub CodigoBidimensional(ByVal VVD_CLAVE As String, ByVal RFC As String)
            'Module1.ConsultaTimbre() 'creo que esto no va porque los datos para generar el codigobid los obtiene leyendo el xml
            'RemoveHandler ComboBox1.SelectedIndexChanged, AddressOf ComboBox1_SelectedIndexChanged
            Dim CadenaCodigoBidimensional As String
            LeerDatos(RFC)
            Dim CodBidimMs As New MemoryStream
            CadenaCodigoBidimensional = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id=" & UUID & "&re=" & RFCEmisor & "&rr=" & RFCReceptor & "&tt=" & Total & "&fe=" + SelloCFD.Substring(SelloCFD.Length - 8, 8)
            Dim qrCodeEncoder As QRCodeEncoder = New QRCodeEncoder
            qrCodeEncoder.QRCodeEncodeMode = Codec.QRCodeEncoder.ENCODE_MODE.BYTE
            qrCodeEncoder.QRCodeScale = 6
            qrCodeEncoder.QRCodeErrorCorrect = Codec.QRCodeEncoder.ERROR_CORRECTION.L
            'La versión "0" calcula automáticamente el tamaño
            qrCodeEncoder.QRCodeVersion = 0
            '' --------- Forzar una determinada version -----------
            ''En caso de querer forzar una determinada version (tamaño) el siguiente código devuelve la
            ''versión mínima para el texto que se quiere códificar:
            'Dim iVersion As Integer = AdjustQRVersion(TextBox1.Text, qrCodeEncoder.QRCodeErrorCorrect)
            qrCodeEncoder.QRCodeBackgroundColor = System.Drawing.Color.FromArgb(qrBackColor)
            qrCodeEncoder.QRCodeForegroundColor = System.Drawing.Color.FromArgb(qrForeColor)
            Try
                CBidimensional = qrCodeEncoder.Encode(CadenaCodigoBidimensional, System.Text.Encoding.UTF8)
                'PictureBox1.Image = CBidimensional
                ''PictureBox1.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg)
                'CBidimensional.Save("C:\SISCOVBNETACCESSFECFDIEDICOMV32\SISCOVBNET\bin\Debug\CBIDIMENSIONALES\" & String.Format("{0:000000}", Convert.ToInt32(TxtFactura.Text)) & ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg)
                CBidimensional.Save(CodBidimMs, System.Drawing.Imaging.ImageFormat.Jpeg)

                GuardarCBidimensional(CodBidimMs, VVD_CLAVE)

                Dim filebytes = CodBidimMs.ToArray()
                codigoB64 = Convert.ToBase64String(filebytes)

                '   AddHandler ComboBox1.SelectedIndexChanged, AddressOf ComboBox1_SelectedIndexChanged
                'MsgBox("El codigo bidimensional fue obtenido con exito")
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Private Sub LeerDatos(ByVal RFC As String)
            Dim FlujoReader As System.Xml.XmlTextReader = Nothing
            Dim i As Integer
            ' leer del fichero e ignorar los nodos vacios
            'FlujoReader = New XmlTextReader(ModuloGeneral.CarpetaAlmacenXML & "\" & SeriePrefijo & String.Format("{0:000000}", Convert.ToString(FE_NumFacturita)) & ".xml")

            FlujoReader = New XmlTextReader(HttpContext.Current.Request.PhysicalApplicationPath + "XML" & "\" & "APD_" & String.Format("{0:000000}", Convert.ToInt32(FE_NumFacturita)) & "_" & CStr(RFC) & ".xml")
            FlujoReader.WhitespaceHandling = WhitespaceHandling.None
            ' analizar el fichero y presentar cada nodo
            While FlujoReader.Read()
                Select Case FlujoReader.NodeType
                    Case XmlNodeType.Element
                        If FlujoReader.Name = "cfdi:Comprobante" Then
                            For i = 0 To FlujoReader.AttributeCount - 1
                                FlujoReader.MoveToAttribute(i)
                                If FlujoReader.Name = "Total" Then
                                    Total = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "NoCertificado" Then
                                    NoCertificado = FlujoReader.Value
                                End If
                            Next
                        ElseIf FlujoReader.Name = "cfdi:Emisor" Then
                            For i = 0 To FlujoReader.AttributeCount - 1
                                FlujoReader.MoveToAttribute(i)
                                If FlujoReader.Name = "Rfc" Then
                                    RFCEmisor = FlujoReader.Value
                                End If
                            Next
                        ElseIf FlujoReader.Name = "cfdi:Receptor" Then
                            For i = 0 To FlujoReader.AttributeCount - 1
                                FlujoReader.MoveToAttribute(i)
                                If FlujoReader.Name = "Rfc" Then
                                    RFCReceptor = FlujoReader.Value
                                End If
                            Next
                        ElseIf FlujoReader.Name = "tfd:TimbreFiscalDigital" Then
                            For i = 0 To FlujoReader.AttributeCount - 1
                                FlujoReader.MoveToAttribute(i)
                                If FlujoReader.Name = "Version" Then
                                    VersionSAT = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "FechaTimbrado" Then
                                    FechaCertificadoSAT = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "UUID" Then
                                    UUID = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "NoCertificadoSAT" Then
                                    NoCertificadoSAT = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "SelloCFD" Then
                                    SelloCFD = FlujoReader.Value
                                End If
                                If FlujoReader.Name = "SelloSAT" Then
                                    SelloSAT = FlujoReader.Value
                                End If
                            Next
                        End If
                End Select
            End While
        End Sub
        Private Sub GuardarCBidimensional(ByVal CodBidimMs, ByVal VVD_CLAVE)
            Try
                Dim SentSQL As String = ""
                SentSQL = "UPDATE [APDSGEDB_PL].[dbo].VTA_10_VD_FACTURACION SET VVD_CODIGO_BIDIMENSIONAL=@IMAGEN WHERE VVD_CLAVE=" & VVD_CLAVE & ""
                Mdl_Facturacion.ACTUALIZAR_CODIGO_BIDIMENSIONAL(SentSQL, CodBidimMs)

            Catch Excep As Exception
                Throw Excep
            Finally

            End Try
        End Sub

        '/*USADO PARA NUEVA FACTURACION*/
        Public Function ENVIAR_CORREO(ByVal EMAIL_PIE_PAGINA As String, ByVal EMAIL_ENCABEZADO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_MENSAJITO As String, ByVal EMAIL_ENCABEZADO_SUPERVISOR As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String, ByVal archivoxml As String) As String

            Dim EMAIL_CUERPO As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 2")
            EMAIL_CUERPO = EMAIL_CUERPO.Replace("[EMAIL_ENCABEZADO]", EMAIL_ENCABEZADO)
            Dim Band As String = ENVIA_MAIL(EMAIL_CUERPO, EMAIL_DESTINATARIO, EMAIL_ENCABEZADO, LLEVA_ATACHMENT, NOM_ATACHMENT, archivopdf, archivoxml)

            Return Band
        End Function

        '/*USADO PARA NUEVA FACTURACION*/
        Public Function RENVIO_ENVIAR_CORREO(ByVal EMAIL_ENCABEZADO As String, ByVal EMAIL_DESTINATARIO As String, ByVal archivopdf As String, ByVal archivoxml As String) As String
            Try
                Dim EMAIL_CUERPO As String = ""
                EMAIL_CUERPO = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_16_FUNCIONALIDAD_ESTRUCTURAS] WHERE AFE_CLAVE = 2")
                EMAIL_CUERPO = EMAIL_CUERPO.Replace("[EMAIL_ENCABEZADO]", EMAIL_ENCABEZADO)
                Dim Band As String = RENVIO_ENVIA_MAIL(EMAIL_CUERPO, EMAIL_DESTINATARIO, EMAIL_ENCABEZADO, archivopdf, archivoxml)
                Return Band
            Catch ex As Exception
                Return ex.Message & "ERROR EN RENVIO_ENVIAR_CORREO"
            End Try
        End Function
        '/*USADO PARA NUEVA FACTURACION*/
        Public Function RENVIO_ENVIA_MAIL(ByVal EMAIL_CUERPO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_ENCABEZADO As String, ByVal archivopdf As String, ByVal archivoxml As String) As String
            Try
                Dim RESPUESTA_AR() As String = Split(Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT (CCW_CORREO + ';;--;;'+ CCW_PASSWORD_APLICACION + ';;--;;' + CCW_PUERTO + ';;--;;' +CCW_HOST) RESPUESTA
                                                                                            FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_34_CORREOS_WEB WHERE CCW_CLAVE = 1 "), ";;--;;")

                'Dim fromaddr As String = "lerdofacturacion@gmail.com" 'OLD
                Dim fromaddr As String = RESPUESTA_AR(0)
                Dim toaddr As String = EMAIL_DESTINATARIO
                'Dim password As String = "vhvwxygdvclxnatx" ' OLD
                Dim password As String = RESPUESTA_AR(1)

                If String.IsNullOrEmpty(EMAIL_DESTINATARIO) Then
                    Return "El email del destinatario no puede estar vacío."
                End If

                If String.IsNullOrEmpty(archivopdf) OrElse String.IsNullOrEmpty(archivoxml) Then
                    Return "Los archivos adjuntos no pueden estar vacíos."
                End If

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "FACTURACION FOLIO " + EMAIL_ENCABEZADO + " PORTAL LERDO DIGITAL"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)

                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                msg.Attachments.Add(New Attachment(archivopdf))
                msg.Attachments.Add(New Attachment(archivoxml))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                'smtp.Host = "smtp.gmail.com"   'OLD
                'smtp.Port = 587                'OLD
                smtp.Host = RESPUESTA_AR(3)
                smtp.Port = Integer.Parse(RESPUESTA_AR(2))

                smtp.UseDefaultCredentials = False
                smtp.EnableSsl = True
                'smtp.TargetName = "smtp.gmail.com"

                Dim nc As New System.Net.NetworkCredential(fromaddr, password)
                smtp.Credentials = nc
                smtp.Send(msg)
                Return "Correo Enviado"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function
        '/*USADO PARA NUEVA FACTURACION*/
        Public Function ENVIA_MAIL(ByVal EMAIL_CUERPO As String, ByVal EMAIL_DESTINATARIO As String, ByVal EMAIL_ENCABEZADO As String, ByVal LLEVA_ATACHMENT As String, ByVal NOM_ATACHMENT As String, ByVal archivopdf As String, ByVal archivoxml As String) As String
            Try
                Dim RESPUESTA_AR() As String = Split(Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT (CCW_CORREO + ';;--;;'+ CCW_PASSWORD_APLICACION + ';;--;;' + CCW_PUERTO + ';;--;;' +CCW_HOST) RESPUESTA
                                                                                            FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].CAT_34_CORREOS_WEB WHERE CCW_CLAVE = 1 "), ";;--;;")

                'Dim fromaddr As String = "lerdofacturacion@gmail.com" 'OLD
                Dim fromaddr As String = RESPUESTA_AR(0)
                Dim toaddr As String = EMAIL_DESTINATARIO
                Dim password As String = RESPUESTA_AR(1)

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "FACTURACION FOLIO " + EMAIL_ENCABEZADO + " PORTAL LERDO DIGITAL"
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

                msg.Attachments.Add(New Attachment(archivopdf))
                msg.Attachments.Add(New Attachment(archivoxml))

                msg.To.Add(New System.Net.Mail.MailAddress(EMAIL_DESTINATARIO))
                Dim smtp As New System.Net.Mail.SmtpClient
                'smtp.Host = "smtp.gmail.com"   'OLD
                'smtp.Port = 587                'OLD
                smtp.Host = RESPUESTA_AR(3)
                smtp.Port = Integer.Parse(RESPUESTA_AR(2))

                smtp.UseDefaultCredentials = False
                smtp.EnableSsl = True

                Dim nc As New System.Net.NetworkCredential(fromaddr, password)
                smtp.Credentials = nc
                smtp.Send(msg)
                Return "Correo Enviado"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Public Function ENVIAR_CORREO_RFC(ByVal RFC As String, ByVal EMAIL_DESTINATARIO As String) As Boolean
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
							yuimg {
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
							.text-services .meta{
								text-transform: uppercase;
								font-size: 14px;
							} 
							.text-testimony .name{
								margin: 0;
							}
							.text-testimony .position{
								color: rgba(0,0,0,.3);

							} 
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
								</div>
								<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
									<!-- BEGIN BODY -->
								  <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>
									<tr>
									  <td valign='top' class='bg_white' style='padding: 1em 2.5em;'>
										<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
											<tr>
												<td width='40%' class='logo' style='text-align: center;'>
													<h1><a href='#'><b>LerdoDigital</b></a></h1>
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
													<div class='text' style='padding: 0 3em;'>
														<h2>INFORMACIÓN DEL RFC " + RFC + "</h2>
														<h3 ><strong>NOMBRE</strong>: " + ModuloGeneral.ReceptorNombre + "</h3> 
														<h3 ><strong>DIRECCIÓN</strong>: " + ModuloGeneral.ReceptorDomicilioCalle + "</h3>
														<h3 ><strong>CIUDAD</strong>: " + ModuloGeneral.ReceptorDomicilioMunicipio + "</h3>
														<h3 ><strong>ESTADO</strong>: " + ModuloGeneral.ReceptorDomicilioEstado + "</h3>
														<h3 ><strong>PAIS</strong>: " + ModuloGeneral.ReceptorPais + "</h3>
														<h3 ><strong>CÓDIGO POSTAL</strong>: " + ModuloGeneral.ReceptorDomicilioCp + "</h3>
														<h3 ><strong>TELÉFONO</strong>: " + ModuloGeneral.ReceptorTelefono + "</h3>
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
												</tr>
													</table>
												</td>
											  </tr><!-- end: tr -->
											  <tr>
												<td class='primary email-section' style='text-align:center;'>
													<div class='heading-section heading-section-white' style='    padding: 10px;
																						padding-top: 20px;background:#3cac34;'>
													<h3 style='color:white;font-weight: bold'>Contactanos</h3>
													<p>Si presentas algún problema con tu información o duda enviar un correo solicitando las modificaciones al siguiente correo:</p>
													<p><a  class='btn btn-white-outline'>soporte@lerdodigital.mx</a></p>
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
            Dim Band As Boolean = ENVIA_MAIL_RFC(EMAIL_CUERPO, RFC, EMAIL_DESTINATARIO)
            Return Band
        End Function
        Public Function ENVIA_MAIL_RFC(ByVal EMAIL_CUERPO As String, ByVal RFC As String, ByVal EMAIL_DESTINATARIO As String) As Boolean
            Try
                Dim fromaddr As String = "lerdofacturacion@gmail.com"
                Dim toaddr As String = EMAIL_DESTINATARIO
                Dim password As String = "jsvyumtxkhhtzycm"

                Dim msg As New System.Net.Mail.MailMessage
                msg.Subject = "VALIDACIÓN DE LA INFORMACIÓN CORRESPONDIENTE AL RFC " + RFC
                msg.From = New System.Net.Mail.MailAddress(fromaddr)
                msg.Body = EMAIL_CUERPO
                msg.IsBodyHtml = True

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
        Public Function letras(ByVal nCifra As Object) As String

            ' Defino variables

            Dim cifra, bloque, decimales, cadena As String

#Disable Warning BC42024 ' Variable local sin usar: 'longituid'.
            Dim longituid, posision, unidadmil As Byte
#Enable Warning BC42024 ' Variable local sin usar: 'longituid'.



            ' En caso de que unidadmil sea:

            ' 0 = cientos

            ' 1 = miles

            ' 2 = millones

            ' 3 = miles de millones

            ' 4 = billones

            ' 5 = miles de billones



            ' Reemplazo el símbolo decimal por un punto (.) y luego guardo la parte entera y la decimal por separado

            ' Es necesario poner el cero a la izquierda del punto así si el valor es de sólo decimales, se lo fuerza

            ' a colocar el cero para que no genere error

            cifra = Format(CType(nCifra, Decimal), "###############0.#0")

            decimales = Mid(cifra, Len(cifra) - 1, 2)

            cifra = Left(cifra, Len(cifra) - 3)



            ' Verifico que el valor no sea cero

            If cifra = "0" Then

                Return IIf(decimales = "00", "cero", "cero con " & decimales & "/100")

            End If



            ' Evaluo su longitud (como mínimo una cadena debe tener 3 dígitos)

            If Len(cifra) < 3 Then

                cifra = Rellenar(cifra, 3)

            End If



            ' Invierto la cadena

            cifra = Invertir(cifra)



            ' Inicializo variables

            posision = 1

            unidadmil = 0

            cadena = ""



            ' Selecciono bloques de a tres cifras empezando desde el final (de la cadena invertida)

            Do While posision <= Len(cifra)

                ' Selecciono una porción del numero

                bloque = Mid(cifra, posision, 3)



                ' Transformo el número a cadena

                cadena = Convertir(bloque, unidadmil) & " " & cadena.Trim



                ' Incremento la cantidad desde donde seleccionar la subcadena

                posision = posision + 3



                ' Incremento la posisión de la unidad de mil

                unidadmil = unidadmil + 1

            Loop



            ' Cargo la función

            Return IIf(decimales = "00", cadena.Trim.ToLower, cadena.Trim.ToLower & " con " & decimales & "/100")

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


        '/* NUEVA FACTURACION */
        <HttpGet>
        <Route("api/Facturacion/RecuperaEstados")>
        Public Function RecuperaEstados()
            Dim SentSQL As String
            Dim resultado As ArrayList
            SentSQL = "SELECT CPA_CLAVE,CES_CLAVE,CES_NOMBRE FROM [APDSGEDB_PL].[DBO].CAT_02_ESTADOS"
            resultado = Mdl_Facturacion.REGRESA_ARRAY(SentSQL)

            Return resultado
        End Function
        '/* NUEVA FACTURACION*/
        <HttpGet>
        <Route("api/Facturacion/RecuperaMunicipios")>
        Public Function RecuperaMunicipios()
            Dim SentSQL As String
            Dim resultado As ArrayList
            SentSQL = "SELECT CPA_CLAVE,CES_CLAVE,CMP_CLAVE,CMP_NOMBRE FROM [APDSGEDB_PL].[DBO].CAT_03_MUNICIPIOS"
            resultado = Mdl_Facturacion.REGRESA_ARRAY(SentSQL)

            Return resultado
        End Function

        '/*     NUEVA FACTURACION 
        '       PRUEAB DE ENVIO DE WHATSAPP DESDE API INTERNA*/
        <HttpGet>
        <Route("api/facturacion/EnviaWhatsapp_test")>
        Public Function enviawhatsapp_test(tel As String, mensaje As String)
            Dim SentSQL As String
            Dim resultado As String = ""
            Dim random As New System.Random()
            Dim Codigo As String = random.Next(1000, 10000).ToString()
            Try
                'https://www.waboxapp.com/api/send/chat?token=d8d12c3380f80c46f969637ede039d786622d09aaa46a&uid=5218713913346&to=521[TELEFONO_DESTINO]&custom_uid=PL-[CODIGO]-[TIEMPO]&text=[MENSAJE]
                SentSQL = "SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].APD_16_FUNCIONALIDAD_ESTRUCTURAS WHERE AFE_CLAVE = 3"
                resultado = Mdl_Facturacion.RECUPERA_VALOR_STRING(SentSQL)



                If resultado.Contains("https") = True Then
                    resultado = resultado.Replace("[CODIGO]", Codigo)
                    resultado = resultado.Replace("[TIEMPO]", Now().ToString("ddMMyyyyHHmmss"))
                    resultado = resultado.Replace("[TELEFONO_DESTINO]", tel.ToString())
                    resultado = resultado.Replace("[MENSAJE]", mensaje)
                    Dim oRequest As WebRequest = WebRequest.Create(resultado)
                    Dim oResponse As WebResponse = oRequest.GetResponse()
                    Dim sr As StreamReader = New StreamReader(oResponse.GetResponseStream())
                    Return "ENVIO EXITOSO"
                Else
                    resultado = "sin conexion a la base de datos."
                End If

                Return resultado
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        '/*     NUEVA FACTURACION 
        '       PRUEAB DE ENVIO DE WHATSAPP DESDE API INTERNA*/
        Public Function Envia_Whatsapp_ERROR_FACTURACION(NOMBRE_FISCAL As String, TELEFONO_CLIENTE As String, RFC_CLIENTE As String, CODE_ERROR As String, DESCIRPCION_ERROR As String, SOLUCION As String) As String
            Dim SentSQL As String
            Dim resultado As String = ""
            Dim Resultado_envio As String = ""
            Dim random As New System.Random()
            Try
                'https://www.waboxapp.com/api/send/chat?token=d8d12c3380f80c46f969637ede039d786622d09aaa46a&uid=5218713913346&to=[TELEFONO_DESTINO]&custom_uid=PL-[CODIGO]-[TIEMPO]&text=/////////////////////////////////////
                '*ERROR EN FACTURACION*
                'NOMBRE FISCAL: *[NOMBRE_FISCAL]*
                'RFC: *[RFC_CLIENTE]*
                'NOMBRE DEL CLIENTE: *[NOMBRE_CLIENTE]*
                'TELEFONO: *[TELEFONO_CLIENTE]*
                'CODIGO DE ERROR: *[CODE_ERROR]*
                'DESCIRPCION ERROR: *[DESCIRPCION_ERROR]*
                '
                'SOLUCION: *[SOLUCION]*
                '/////////////////////////////////////
                '
                SentSQL = "SELECT AFE_HTML_BODY FROM [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].APD_16_FUNCIONALIDAD_ESTRUCTURAS WHERE AFE_CLAVE = 4"
                resultado = Mdl_Facturacion.RECUPERA_VALOR_STRING(SentSQL)
                Dim telefonos_APD As String = Mdl_Facturacion.RECUPERA_VALOR_STRING("SELECT	
                ( AFE_HTML_INTERACION1 +','+ AFE_HTML_INTERACION2+','+ AFE_HTML_INTERACION3+','+AFE_HTML_INTERACION4 ) AS TELEFONOS_APD
                FROM 
	                [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].APD_16_FUNCIONALIDAD_ESTRUCTURAS
                WHERE AFE_CLAVE = 4")
                Dim TELEFONOS() As String = Split(telefonos_APD, ",")
                Dim TELEFONO_DESTINO
                Dim CLIENTE() As String = Split(TELEFONO_CLIENTE, "111,111")
                Dim TEL_CLIENTE As String = CLIENTE(0)
                Dim NOMBRE_CLIENTE As String = CLIENTE(1)
                If resultado.Contains("https") = True Then
                    For i = 0 To UBound(TELEFONOS)
                        Resultado_envio = resultado
                        TELEFONO_DESTINO = TELEFONOS(i)
                        Dim Codigo As String = Random.Next(1000, 10000).ToString()
                        '[TELEFONO_DESTINO]
                        '[NOMBRE_FISCAL]*
                        '[RFC_CLIENTE] *
                        '[NOMBRE_CLIENTE] *
                        '[TELEFONO_CLIENTE] *
                        '[CODE_ERROR] *
                        '[DESCIRPCION_ERROR] *
                        '[SOLUCION] *
                        Resultado_envio = Resultado_envio.Replace("[CODIGO]", Codigo)
                        Resultado_envio = Resultado_envio.Replace("[TIEMPO]", Now().ToString("ddMMyyyyHHmmss"))
                        Resultado_envio = Resultado_envio.Replace("[TELEFONO_DESTINO]", TELEFONO_DESTINO)
                        Resultado_envio = Resultado_envio.Replace("[NOMBRE_FISCAL]", NOMBRE_FISCAL)
                        Resultado_envio = Resultado_envio.Replace("[RFC_CLIENTE]", RFC_CLIENTE)
                        Resultado_envio = Resultado_envio.Replace("[NOMBRE_CLIENTE]", NOMBRE_CLIENTE)
                        Resultado_envio = Resultado_envio.Replace("[TELEFONO_CLIENTE]", TEL_CLIENTE)
                        Resultado_envio = Resultado_envio.Replace("[CODE_ERROR]", CODE_ERROR)
                        Resultado_envio = Resultado_envio.Replace("[DESCIRPCION_ERROR]", DESCIRPCION_ERROR)
                        Resultado_envio = Resultado_envio.Replace("[SOLUCION]", SOLUCION)

                        Dim oRequest As WebRequest = WebRequest.Create(Resultado_envio)
                        Dim oResponse As WebResponse = oRequest.GetResponse()
                        Dim sr As StreamReader = New StreamReader(oResponse.GetResponseStream())
                    Next
                    resultado = "ENVIO EXITOSO"
                Else
                    resultado = "sin conexion a la base de datos."
                End If

                Return resultado
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function
    End Class
End Namespace


