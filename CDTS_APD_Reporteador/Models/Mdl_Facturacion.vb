Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.IO
Imports System.Net.Http.Formatting
Imports System.Web.Mvc
Imports System.Web.Services.Description

Namespace Models
    Public Class Mdl_Facturacion
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function Recupera_Folio(folio As String) As Cs_Folio_fac
            Dim Cs_Folio_fac As New Cs_Folio_fac
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT TOP 1
                                                        I.IIN_CLAVE,
                                                        I.CDA_FOLIO,
                                                        I.IIN_COBRO_ESTATUS, 
                                                        I.IIN_FORMA_PAGO,
                                                        V.VVD_CLAVE, 
                                                        V.VVD_TOTAL,
                                                        CASE WHEN P.CFC_FOLIO IS NULL THEN FX.CCF_CLAVE_CATASTRAL ELSE P.CFC_FOLIO END AS CFC_FOLIO
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I
                                                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[ING_03_INGRESOS] I2
														ON I2.CDA_FOLIO = I.CDA_FOLIO
	                                                LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAJ_03_COBROS] C
		                                                ON I.IIN_CLAVE = C.IIN_CLAVE
                                                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAJ_03_COBROS] C2
		                                                ON I2.IIN_CLAVE = C2.IIN_CLAVE  
		                                            LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CST_01_COBROS_FOX] FX
														ON FX.CCF_CLAVE = '" + folio.Substring(2, 6).ToString() + "' 
	                                                LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS] F
		                                                ON I.IIN_CLAVE = F.IIN_CLAVE
	                                                LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS] P
		                                                ON I.CDA_FOLIO = P.IIN_CLAVE
                                                    LEFT JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_01_VENTAS_DIARIAS] V
		                                                ON C.VVD_CLAVE = V.VVD_CLAVE
		                                                OR F.VVD_CLAVE = V.VVD_CLAVE
		                                                OR P.VVD_CLAVE = V.VVD_CLAVE
                                                    OR C2.VVD_CLAVE= V.VVD_CLAVE
                                                    WHERE I.CDA_FOLIO = '" + folio.ToString() + "'
                                                    ORDER BY P.VVD_CLAVE DESC
                                                    ", conn) 'I.CCO_CLAVE

                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Folio_fac.IIN_CLAVE = If(IsDBNull(d("IIN_CLAVE")), New Integer, d("IIN_CLAVE"))
                            Cs_Folio_fac.CDA_FOLIO = If(IsDBNull(d("CDA_FOLIO")), New Integer, d("CDA_FOLIO"))
                            Cs_Folio_fac.VVD_CLAVE = If(IsDBNull(d("VVD_CLAVE")), New Integer, d("VVD_CLAVE"))
                            Cs_Folio_fac.IIN_FORMA_PAGO = If(IsDBNull(d("IIN_FORMA_PAGO")), String.Empty, d("IIN_FORMA_PAGO"))
                            Cs_Folio_fac.IIN_COBRO_ESTATUS = If(IsDBNull(d("IIN_COBRO_ESTATUS")), String.Empty, d("IIN_COBRO_ESTATUS"))
                            Cs_Folio_fac.VVD_TOTAL = If(IsDBNull(d("VVD_TOTAL")), New Integer, d("VVD_TOTAL"))
                            Cs_Folio_fac.CLAVE_CATASTRAL = If(IsDBNull(d("CFC_FOLIO")), String.Empty, d("CFC_FOLIO"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Folio_fac
        End Function
        Public Function Recupera_Cliente(ByVal RFC As String) As Cs_cliente_fac
            Dim Cs_Cliente_fac As New Cs_cliente_fac
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT VCL_CLAVE, VCL_FECALTA, VCL_NOMBRE,
                                                    VCL_TIPO_PERSONA, VCL_RFC, VCL_DIRECCION, 
                                                    VCL_COLONIA, CMP_CLAVE, CES_CLAVE, CPA_CLAVE, 
                                                    VCL_CP, VCL_ALIAS, VCL_ESTATUS, VTC_CLAVE, 
                                                    VCL_CONTACTO, VCL_EMAIL, VCL_CELULAR, VCL_DIAS_CREDITO, 
                                                    VCL_LIMITE_CREDITO, VCL_SALDO_ACTUAL, VCL_CREDITO_DISPONIBLE, 
                                                    VCL_DESCUENTO, VCL_CUENTA_CONTABLE, VCL_TARJETA_PUNTOS, 
                                                    VCL_ME_PUNTOS_DISPONIBLES, VCL_ME_PUNTOS_UTILIZADOS, VCL_NOTAS, 
                                                    CEM_CLAVE, CET_NOMBRE, VCL_MOSTRAR, VCL_FUM, VCL_SERIAL
                                                 FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES]
                                                 WHERE VCL_RFC = '" + RFC + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Cliente_fac.VCL_CLAVE = If(IsDBNull(d("VCL_CLAVE")), New Integer, d("VCL_CLAVE"))
                            Cs_Cliente_fac.VCL_FECALTA = If(IsDBNull(d("VCL_FECALTA")), New Date, d("VCL_FECALTA"))
                            Cs_Cliente_fac.VCL_NOMBRE = If(IsDBNull(d("VCL_NOMBRE")), String.Empty, d("VCL_NOMBRE"))
                            Cs_Cliente_fac.VCL_TIPO_PERSONA = If(IsDBNull(d("VCL_TIPO_PERSONA")), String.Empty, d("VCL_TIPO_PERSONA"))
                            Cs_Cliente_fac.VCL_RFC = If(IsDBNull(d("VCL_RFC")), String.Empty, d("VCL_RFC"))
                            Cs_Cliente_fac.VCL_DIRECCION = If(IsDBNull(d("VCL_DIRECCION")), String.Empty, d("VCL_DIRECCION"))
                            Cs_Cliente_fac.VCL_COLONIA = If(IsDBNull(d("VCL_COLONIA")), String.Empty, d("VCL_COLONIA"))
                            Cs_Cliente_fac.CMP_CLAVE = If(IsDBNull(d("CMP_CLAVE")), New Integer, d("CMP_CLAVE"))
                            Cs_Cliente_fac.CES_CLAVE = If(IsDBNull(d("CES_CLAVE")), New Integer, d("CES_CLAVE"))
                            Cs_Cliente_fac.CPA_CLAVE = If(IsDBNull(d("CPA_CLAVE")), New Integer, d("CPA_CLAVE"))
                            Cs_Cliente_fac.VCL_CP = If(IsDBNull(d("VCL_CP")), String.Empty, d("VCL_CP"))
                            Cs_Cliente_fac.VCL_ALIAS = If(IsDBNull(d("VCL_ALIAS")), String.Empty, d("VCL_ALIAS"))
                            Cs_Cliente_fac.VCL_ESTATUS = If(IsDBNull(d("VCL_ESTATUS")), String.Empty, d("VCL_ESTATUS"))
                            Cs_Cliente_fac.VCL_CONTACTO = If(IsDBNull(d("VCL_CONTACTO")), String.Empty, d("VCL_CONTACTO"))
                            Cs_Cliente_fac.VCL_EMAIL = If(IsDBNull(d("VCL_EMAIL")), String.Empty, d("VCL_EMAIL"))
                            Cs_Cliente_fac.VCL_CELULAR = If(IsDBNull(d("VCL_CELULAR")), String.Empty, d("VCL_CELULAR"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Cliente_fac
        End Function
        Public Function Recupera_Cliente_regimen(ByVal RFC As String) As Cs_cliente_fac
            Dim Cs_Cliente_fac As New Cs_cliente_fac
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT FRF_CLAVE,FRF_CLAVE_NOMBRE
                                                 FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES]
                                                 WHERE VCL_RFC = '" + RFC + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Cliente_fac.VCL_FRF_CLAVE = If(IsDBNull(d("FRF_CLAVE")), New Integer, d("FRF_CLAVE"))
                            Cs_Cliente_fac.VCL_FRF_CLAVE_NOMBRE = If(IsDBNull(d("FRF_CLAVE_NOMBRE")), String.Empty, d("FRF_CLAVE_NOMBRE"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Cliente_fac
        End Function
        Public Function Guarda_Cliente(ByVal Cs_cliente_fac As Cs_cliente_fac) As Cs_Respuesta
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES]
                                                    ([VCL_CLAVE]
                                                    ,[VCL_FECALTA]
                                                    ,[VCL_NOMBRE]
                                                    ,[VCL_TIPO_PERSONA]
                                                    ,[VCL_RFC]
                                                    ,[VCL_DIRECCION]
                                                    ,[VCL_COLONIA]
                                                    ,[CMP_CLAVE]
                                                    ,[CES_CLAVE]
                                                    ,[CPA_CLAVE]
                                                    ,[VCL_CP]
                                                    ,[VCL_ALIAS]
                                                    ,[VCL_ESTATUS]
                                                    ,[VTC_CLAVE]
                                                    ,[VCL_CONTACTO]
                                                    ,[VCL_EMAIL]
                                                    ,[VCL_CELULAR]
                                                    ,[VCL_DIAS_CREDITO]
                                                    ,[VCL_LIMITE_CREDITO]
                                                    ,[VCL_SALDO_ACTUAL]
                                                    ,[VCL_CREDITO_DISPONIBLE]
                                                    ,[VCL_DESCUENTO]
                                                    ,[VCL_CUENTA_CONTABLE]
                                                    ,[VCL_TARJETA_PUNTOS]
                                                    ,[VCL_ME_PUNTOS_TOTALES]
                                                    ,[VCL_ME_PUNTOS_DISPONIBLES]
                                                    ,[VCL_ME_PUNTOS_UTILIZADOS]
                                                    ,[VCL_NOTAS]
                                                    ,[CEM_CLAVE]
                                                    ,[CET_NOMBRE]
                                                    ,[VCL_MOSTRAR]
                                                    ,[FRF_CLAVE]
                                                    ,[FRF_CLAVE_NOMBRE]
                                                    ,[VCL_FUM]
                                                    ,[VCL_SERIAL])
                                                  VALUES
                                                    (" + Cs_cliente_fac.VCL_CLAVE.ToString() + "
                                                    ,'" + Cs_cliente_fac.VCL_FECALTA.ToString("yyyy-MM-dd") + "'
                                                    ,UPPER('" + Cs_cliente_fac.VCL_NOMBRE.ToString() + "')
                                                    ,'" + Cs_cliente_fac.VCL_TIPO_PERSONA.ToString() + "'
                                                    ,UPPER('" + Cs_cliente_fac.VCL_RFC.ToString() + "')
                                                    ,UPPER('" + Cs_cliente_fac.VCL_DIRECCION.ToString() + "')
                                                    ,UPPER('" + Cs_cliente_fac.VCL_COLONIA.ToString() + "')
                                                    ," + Cs_cliente_fac.CMP_CLAVE.ToString() + "
                                                    ," + Cs_cliente_fac.CES_CLAVE.ToString() + "
                                                    ," + Cs_cliente_fac.CPA_CLAVE.ToString() + "
                                                    ,'" + Cs_cliente_fac.VCL_CP.ToString() + "'
                                                    ,'" + Cs_cliente_fac.VCL_ALIAS.ToString() + "'
                                                    ,'ACTIVO'
                                                    ," + "2" + "
                                                    ,'" + Cs_cliente_fac.VCL_CONTACTO.ToString() + "'
                                                    ,'" + Cs_cliente_fac.VCL_EMAIL.ToString() + "'
                                                    ,'" + Cs_cliente_fac.VCL_CELULAR.ToString() + "'
                                                    ," + Cs_cliente_fac.VCL_DIAS_CREDITO.ToString() + "
                                                    ," + Cs_cliente_fac.VCL_LIMITE_CREDITO.ToString() + "    
                                                    ," + Cs_cliente_fac.VCL_SALDO_ACTUAL.ToString() + "
                                                    ," + Cs_cliente_fac.VCL_CREDITO_DISPONIBLE.ToString() + "
                                                    ," + Cs_cliente_fac.VCL_DESCUENTO.ToString() + "
                                                    ,'" + Cs_cliente_fac.VCL_CUENTA_CONTABLE.ToString() + "'
                                                    ,'" + Cs_cliente_fac.VCL_TARJETA_PUNTOS.ToString() + "'
                                                    ," + Cs_cliente_fac.VCL_ME_PUNTOS_TOTALES.ToString() + "
                                                    ," + Cs_cliente_fac.VCL_ME_PUNTOS_DISPONIBLES.ToString() + "
                                                    ," + Cs_cliente_fac.VCL_ME_PUNTOS_UTILIZADOS.ToString() + "
                                                    ,'" + Cs_cliente_fac.VCL_NOTAS.ToString() + "'
                                                    ," + Cs_cliente_fac.CEM_CLAVE.ToString() + "
                                                    ,'" + Cs_cliente_fac.CET_NOMBRE.ToString() + "'
                                                    ,'" + Cs_cliente_fac.VCL_MOSTRAR.ToString() + "'
                                                    ," + Cs_cliente_fac.VCL_FRF_CLAVE.ToString() + "
                                                    ,'" + Cs_cliente_fac.VCL_FRF_CLAVE_NOMBRE.ToString() + "'
                                                    ," + Cs_cliente_fac.VCL_FUM.ToString("yyyy-MM-dd") + "
                                                    ,'" + Cs_cliente_fac.VCL_SERIAL.ToString("yyyy-MM-dd") + "')", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.codigoError = 200
                        Cs_Respuesta.mensaje = "RFC registrado correctamente."

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Respuesta
        End Function
        Public Function datos_empresa(ByVal ModuloGeneral As ModuloGeneral) As ModuloGeneral
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT   CAT_14_EMPRESAS.CEP_CLAVE AS NUMEROCIA, 
                                                            CAT_14_EMPRESAS.CEP_NOMBRE AS RAZONSOCIAL, 
                                                            CAT_14_EMPRESAS.CEP_DIRECCION AS DIRECCION, 
                                                            CAT_14_EMPRESAS.CEP_COLONIA AS COLONIA, 
                                                            CAT_03_MUNICIPIOS.CMP_NOMBRE AS CIUDAD, 
                                                            CAT_02_ESTADOS.CES_NOMBRE AS ESTADO, 
                                                            CAT_01_PAISES.CPA_NOMBRE AS PAIS, 
                                                            CAT_14_EMPRESAS.CEP_CP AS CODIGOPOSTAL, 
                                                            CAT_14_EMPRESAS.CEP_TELEFONO AS TELEFONO, 
                                                            CAT_14_EMPRESAS.CEP_EMAIL AS EMAIL, 
                                                            CAT_14_EMPRESAS.CEP_RFC AS RFC,
															CAT_14_EMPRESAS.CEP_EXPORTACION as EXPORTACION
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK) 
                                                     INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_01_PAISES] WITH (NOLOCK) 
                                                        ON CAT_14_EMPRESAS.CPA_CLAVE = CAT_01_PAISES.CPA_CLAVE 
                                                     INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_02_ESTADOS] WITH (NOLOCK) 
                                                        ON CAT_14_EMPRESAS.CES_CLAVE = CAT_02_ESTADOS.CES_CLAVE 
                                                     INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_03_MUNICIPIOS] WITH (NOLOCK) 
                                                        ON CAT_14_EMPRESAS.CMP_CLAVE = CAT_03_MUNICIPIOS.CMP_CLAVE", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            ModuloGeneral.EmisorNombre = If(IsDBNull(d("RAZONSOCIAL")), String.Empty, d("RAZONSOCIAL"))
                            ModuloGeneral.EmisorDomicilioCalle = If(IsDBNull(d("DIRECCION")), String.Empty, d("DIRECCION"))
                            ModuloGeneral.EmisorDomicilioColonia = If(IsDBNull(d("COLONIA")), String.Empty, d("COLONIA"))
                            ModuloGeneral.EmisorDomicilioMunicipio = If(IsDBNull(d("CIUDAD")), String.Empty, d("CIUDAD"))
                            ModuloGeneral.EmisorDomicilioEstado = If(IsDBNull(d("ESTADO")), String.Empty, d("ESTADO"))
                            ModuloGeneral.EmisorDomicilioCp = If(IsDBNull(d("CODIGOPOSTAL")), String.Empty, d("CODIGOPOSTAL"))
                            ModuloGeneral.EmisorRfc = Replace(d("RFC"), "-", "", , , CompareMethod.Text)
                            ModuloGeneral.EmisorPais = If(IsDBNull(d("PAIS")), String.Empty, d("PAIS"))
                            ModuloGeneral.EmisorEXPORTACION = If(IsDBNull(d("EXPORTACION")), String.Empty, d("EXPORTACION"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ModuloGeneral
        End Function
        Public Function datos_cliente(ByVal ModuloGeneral As ModuloGeneral, ByVal RFC As String) As ModuloGeneral
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT   VTA_03_CLIENTES.VCL_CLAVE AS NUMEROCLIENTE, 
                                                            VTA_03_CLIENTES.VCL_NOMBRE AS NOMBRE, 
                                                            VTA_03_CLIENTES.VCL_DIRECCION + ' ' + VTA_03_CLIENTES.VCL_COLONIA AS DIRECCION, 
                                                            CAT_03_MUNICIPIOS.CMP_NOMBRE AS CIUDAD, 
                                                            CAT_02_ESTADOS.CES_NOMBRE AS ESTADO, 
                                                            CAT_01_PAISES.CPA_NOMBRE AS PAIS, 
                                                            VTA_03_CLIENTES.VCL_CP AS CODIGOPOSTAL, 
                                                            VTA_03_CLIENTES.VCL_CELULAR AS TELEFONO, 
                                                            VTA_03_CLIENTES.VCL_EMAIL AS EMAIL, 
                                                            VTA_03_CLIENTES.VCL_RFC AS RFC,
															VTA_03_CLIENTES.FRF_CLAVE_NOMBRE AS REGIMEN_FISCAL,
															FEL_01_REGIMEN_FISCAL.FRF_DESCRIPCION AS REGIMEN_DESCRIPCION
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES] WITH (NOLOCK) 
                                                        INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_01_PAISES] WITH (NOLOCK) 
                                                            ON VTA_03_CLIENTES.CPA_CLAVE = CAT_01_PAISES.CPA_CLAVE 
                                                        INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_02_ESTADOS] WITH (NOLOCK) 
                                                            ON VTA_03_CLIENTES.CES_CLAVE = CAT_02_ESTADOS.CES_CLAVE 
                                                        INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_03_MUNICIPIOS] WITH (NOLOCK) 
                                                            ON VTA_03_CLIENTES.CMP_CLAVE = CAT_03_MUNICIPIOS.CMP_CLAVE 
													    INNER JOIN  [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[FEL_01_REGIMEN_FISCAL] WITH (NOLOCK) 
														    ON FEL_01_REGIMEN_FISCAL.FRF_CLAVE=VTA_03_CLIENTES.FRF_CLAVE
                                                    WHERE (VTA_03_CLIENTES.VCL_RFC = '" & RFC & "')", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            ModuloGeneral.ReceptorNombre = If(IsDBNull(d("NOMBRE")), New Integer, d("NOMBRE"))
                            ModuloGeneral.ReceptorDomicilioCalle = If(IsDBNull(d("DIRECCION")), New Integer, d("DIRECCION"))
                            ModuloGeneral.ReceptorDomicilioMunicipio = If(IsDBNull(d("CIUDAD")), New Integer, d("CIUDAD"))
                            ModuloGeneral.ReceptorDomicilioEstado = If(IsDBNull(d("ESTADO")), New Integer, d("ESTADO"))
                            ModuloGeneral.ReceptorDomicilioCp = If(IsDBNull(d("CODIGOPOSTAL")), New Integer, d("CODIGOPOSTAL"))
                            ModuloGeneral.ReceptorRfc = Replace(d("RFC"), "-", "", , , CompareMethod.Text)
                            ModuloGeneral.ReceptorPais = If(IsDBNull(d("PAIS")), String.Empty, d("PAIS"))
                            ModuloGeneral.ReceptorMail = If(IsDBNull(d("EMAIL")), String.Empty, d("EMAIL"))
                            ModuloGeneral.ReceptorTelefono = If(IsDBNull(d("TELEFONO")), String.Empty, d("TELEFONO"))
                            ModuloGeneral.ReceptorRegimenFiscal = If(IsDBNull(d("REGIMEN_FISCAL")), String.Empty, d("REGIMEN_FISCAL"))
                            ModuloGeneral.ReceptorRegimenFiscal_descripcion = If(IsDBNull(d("REGIMEN_DESCRIPCION")), String.Empty, d("REGIMEN_DESCRIPCION"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ModuloGeneral
        End Function
        Public Function datos_venta_det(ByVal ModuloGeneral As ModuloGeneral, ByVal FE_VVD_CLAVE As String) As ModuloGeneral
            Dim dt As DataTable = New DataTable()
            Dim L_V_Det As New List(Of Venta_Det)

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT  VTA_02_VENTAS_DIARIAS_DET.AAR_DESCRIPCION  AS DESCRIPCION, 
                                                           VTA_02_VENTAS_DIARIAS_DET.AAR_PRECIO_MAXIMO_PUBLICO  AS PRECIOUNITARIO, 
                                                           VTA_02_VENTAS_DIARIAS_DET.VVT_GD_CANTIDAD  AS CANTIDAD, 
                                                           0 AS TIPODESCUENTO, 
                                                           VTA_02_VENTAS_DIARIAS_DET.VVT_GD_SUBTOTAL  AS IMPORTE, 
                                                           FEL_03_UNIDAD_MEDIDA.FUM_NOMBRE  AS DESCRIPCIONUM, 
                                                           VTA_02_VENTAS_DIARIAS_DET.VVD_CLAVE,
                                                           PORCENTAJEIEPS=VVT_GD_IEPS_PORC,
                                                           IMPORTEIEPS=VVT_GD_IEPS_IMPORTE,
                                                           PORCENTAJEIVA=VVT_GD_IVA_PORC,
                                                           IMPORTEIVA=VVT_GD_IVA_IMPORTE,
                                                           CLAVEPRODUCTOSERVICIO=AAR_CODIGO_SAT,
                                                           CODIGODEPRODUCTO=ALM_01_ARTICULOS.AAR_NOMBRE,
                                                           CLAVEUNIDAD=FEL_03_UNIDAD_MEDIDA.FUM_UNIDAD,
                                                           ALM_01_ARTICULOS.AAR_CLAVE,
                                                           AAR_CODIGO_SAT,
                                                           VVT_GD_DESC_IMPORTE,
                                                           VTA_02_VENTAS_DIARIAS_DET.AAR_PRECIO_VENTA,
														   ALM_01_ARTICULOS.AAR_OBJETO_IMP
                                                    FROM [VTA_02_VENTAS_DIARIAS_DET]
                                                     INNER JOIN [ALM_01_ARTICULOS] 
                                                        ON VTA_02_VENTAS_DIARIAS_DET.AAR_CLAVE = ALM_01_ARTICULOS.AAR_CLAVE 
                                                     INNER JOIN [FEL_03_UNIDAD_MEDIDA]
                                                        ON ALM_01_ARTICULOS.FUM_CLAVE = FEL_03_UNIDAD_MEDIDA.FUM_CLAVE 
                                                   WHERE (VTA_02_VENTAS_DIARIAS_DET.VVD_CLAVE = " & FE_VVD_CLAVE & ")", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim Venta_Det As New Venta_Det

                            Venta_Det.Descripcion = d.ItemArray(0)
                            Venta_Det.UnidadDeMedida = d.ItemArray(5)
                            Venta_Det.ValorUnitario = d.ItemArray(1)
                            Venta_Det.Cantidad = d.ItemArray(2)
                            Venta_Det.Importe = d.ItemArray(4)
                            Venta_Det.PorcentajeIeps = d.ItemArray(7)
                            Venta_Det.ImporteIeps = d.ItemArray(8)
                            Venta_Det.PorcentajeIva = d.ItemArray(9)
                            Venta_Det.ImporteIva = d.ItemArray(10)
                            Venta_Det.ClaveProductoServicio = d.ItemArray(11)
                            Venta_Det.CodigoDeProducto = d.ItemArray(12)
                            Venta_Det.ClaveUnidadDeMedida = d.ItemArray(13)
                            Venta_Det.Descuento = d.ItemArray(3)
                            ModuloGeneral.SumatoriaDeIvaEnConceptos = ModuloGeneral.SumatoriaDeIvaEnConceptos + d.Item(10)
                            Venta_Det.Articulo = d.ItemArray(14)
                            Venta_Det.ClaveSAT = d.ItemArray(15)
                            Venta_Det.VVT_GD_DESC_IMPORTE = d.ItemArray(16)
                            Venta_Det.Subtotal = d.ItemArray(17)
                            Venta_Det.AAR_OBJETO_IMP = d.ItemArray(18)
                            L_V_Det.Add(Venta_Det)
                        Next

                        ModuloGeneral.productos = L_V_Det

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ModuloGeneral
        End Function
        Public Function datos_fiscales(ByVal ModuloGeneral As ModuloGeneral) As ModuloGeneral
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT   CEP_CLAVE AS CVO, 
                                                            CEP_FE_RUTA_CERTIFICADO AS CERTIFICADO, 
                                                            '' AS VIGENCIADESDE, 
                                                            '' AS VIGENCIAHASTA, 
                                                            '' AS SERIECERTIFICADO, 
                                                            CEP_FE_RUTA_LLAVE_PRIVADA AS LLAVEPRIVADA, 
                                                            CEP_FE_CONTRASENALLAVEPRIVADA AS CONTRASENALLAVEPRIVADA, 
                                                            CEP_FE_PAC_USUARIOTIMBRE AS USUARIOTIMBRE, 
                                                            CEP_FE_PAC_CONTRASENATIMBRE AS CONTRASENATIMBRE, 
                                                            CEP_FE_CARPETAALMACENXML AS CARPETAALMACENXML, 
                                                            CEP_FE_REGIMENFISCAL AS REGIMENFISCAL, 
                                                            CEP_CP AS LUGARDEEXPEDICION, 
                                                            CEP_FE_VERSIONFE AS VERSIONFE, 
                                                            CEP_FE_TIPODECOMPROBANTE AS TIPODECOMPROBANTE, 
                                                            CEP_FE_SUCURSAL AS SUCURSAL,
                                                            CEP_FE_RUTA_PFX,
                                                            CEP_FE_PAC_ID,
                                                            CEP_FE_CONTRASENAPFX,
                                                            CEM_FE_TIMBRADOR='FACTUREHOY',
                                                            CEP_FE_LUGARDEEXPEDICION AS FE_LUGARDEEXPEDICION
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK)", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            ModuloGeneral.CertificadoSelloDigital = If(IsDBNull(d("CERTIFICADO")), String.Empty, d("CERTIFICADO"))
                            ModuloGeneral.CertificadoSelloDigital = If(IsDBNull(d("CERTIFICADO")), String.Empty, d("CERTIFICADO"))
                            ModuloGeneral.LlavePrivada = If(IsDBNull(d("LLAVEPRIVADA")), String.Empty, d("LLAVEPRIVADA"))
                            ModuloGeneral.ContraseñaLlavePrivada = If(IsDBNull(d("CONTRASENALLAVEPRIVADA")), String.Empty, d("CONTRASENALLAVEPRIVADA"))
                            ModuloGeneral.CarpetaAlmacenXML = If(IsDBNull(d("CARPETAALMACENXML")), String.Empty, d("CARPETAALMACENXML"))
                            ModuloGeneral.UsuarioTimbrado = If(IsDBNull(d("USUARIOTIMBRE")), String.Empty, d("USUARIOTIMBRE"))
                            ModuloGeneral.ContraseñaTimbrado = If(IsDBNull(d("CONTRASENATIMBRE")), String.Empty, d("CONTRASENATIMBRE"))
                            ModuloGeneral.RegimenFiscal = If(IsDBNull(d("REGIMENFISCAL")), String.Empty, d("REGIMENFISCAL"))
                            ModuloGeneral.LugarDeExpedicion = If(IsDBNull(d("LUGARDEEXPEDICION")), String.Empty, d("LUGARDEEXPEDICION"))
                            ModuloGeneral.VersionFE = If(IsDBNull(d("VERSIONFE")), String.Empty, d("VERSIONFE"))
                            ModuloGeneral.CertificadoPFX = If(IsDBNull(d("CEP_FE_RUTA_PFX")), String.Empty, d("CEP_FE_RUTA_PFX"))
                            ModuloGeneral.ContaseñaCertificadoPFX = If(IsDBNull(d("CEP_FE_CONTRASENAPFX")), String.Empty, d("CEP_FE_CONTRASENAPFX"))
                            ModuloGeneral.Timbrador = If(IsDBNull(d("CEM_FE_TIMBRADOR")), String.Empty, d("CEM_FE_TIMBRADOR"))
                            ModuloGeneral.pac_id = If(IsDBNull(d("CEP_FE_PAC_ID")), String.Empty, d("CEP_FE_PAC_ID"))
                            ModuloGeneral.TipoDeComprobante = "I"
                            ModuloGeneral.FE_LUGARDEEXPEDICION = If(IsDBNull(d("FE_LUGARDEEXPEDICION")), String.Empty, d("FE_LUGARDEEXPEDICION"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ModuloGeneral
        End Function
        Public Function datos_prefijo(ByVal ModuloGeneral As ModuloGeneral) As ModuloGeneral
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT   1 AS NOSERIE, 
                                                            'FACTURA' AS DESCRIPCION,
                                                            CEP_FE_SERIE_PREFIJO AS PREFIJO 
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK)", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            ModuloGeneral.SeriePrefijo = If(IsDBNull(d("PREFIJO")), String.Empty, d("PREFIJO"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ModuloGeneral
        End Function
        Public Function numero_factura() As Integer
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT CEP_FE_NUM_FACTURA FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK)", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            numero = If(IsDBNull(d.ItemArray(0)), New Integer, d.ItemArray(0))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return numero
        End Function
        Public Sub ACTUALIZAR_CEP_FE_NUM_FACTURA(ByVal SentSQL As String)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Sub ACTUALIZAR_VVD_FACTURACION_SERIE(ByVal SentSQL As String)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Sub INSERTAR_VTA_10_VD_FACTURACION(ByVal SentSQL As String)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Sub ACTUALIZAR_VTA_01_VENTAS_DIARIAS(ByVal SentSQL As String)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Sub ACTUALIZA_METODOS_FORMAS_PAGO(ByVal SentSQL As String)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Function REGRESA_ULTIMO_REGISTRO(ByVal VVD_CLAVE As Integer) As Integer
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT VVF_CLAVE FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_10_VD_FACTURACION] WHERE VVD_CLAVE = " + VVD_CLAVE.ToString(), conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            numero = If(IsDBNull(d.ItemArray(0)), New Integer, d.ItemArray(0))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return numero
        End Function
        Public Function REGRESA_FEL_04_METODO_PAGO(SentSQLMP As String) As String
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQLMP, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            numero = If(IsDBNull(d.ItemArray(0)), New Integer, d.ItemArray(0))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return numero
        End Function
        Public Function REGRESA_FEL_06_USO_CFDI(SentSQLUC As String) As String
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQLUC, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            numero = If(IsDBNull(d.ItemArray(0)), New Integer, d.ItemArray(0))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return numero
        End Function
        Public Function RecuperaUsoCFDI(ByVal UsoCFDI As String) As String
            Dim n_Uso_CFDI As String
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT FUC_CLAVE, FUC_CLAVE_NOMBRE, FUC_NOMBRE
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[FEL_06_USO_CFDI]
                                                    WHERE FUC_NOMBRE = '" + UsoCFDI.ToString() + "'", conn) 'I.CCO_CLAVE

                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            n_Uso_CFDI = If(IsDBNull(d("FUC_CLAVE_NOMBRE")), String.Empty, d("FUC_CLAVE_NOMBRE"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

#Disable Warning BC42104 ' La variable 'n_Uso_CFDI' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
            Return n_Uso_CFDI
#Enable Warning BC42104 ' La variable 'n_Uso_CFDI' se usa antes de que se le haya asignado un valor. Podría darse una excepción de referencia NULL en tiempo de ejecución.
        End Function
        Public Function max_cliente() As Integer
            Dim dt As DataTable = New DataTable()
            Dim clave As Integer
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT MAX(VCL_CLAVE) As clave
                                                    FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[VTA_03_CLIENTES] WITH (NOLOCK)", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            clave = If(IsDBNull(d("clave")), String.Empty, d("clave"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return clave
        End Function
        Public Function DAME_NUM_FACTURA() As Integer
            Dim dt As DataTable = New DataTable()
            Dim clave As Integer
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT CEP_FE_NUM_FACTURA FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CAT_14_EMPRESAS] WITH (NOLOCK)", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            clave = If(IsDBNull(d("CEP_FE_NUM_FACTURA")), String.Empty, d("CEP_FE_NUM_FACTURA"))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return clave
        End Function
        Public Sub ACTUALIZAR_CODIGO_BIDIMENSIONAL(ByVal SentSQL As String, ByVal CodBidimMs As MemoryStream)
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.Add(New SqlParameter("@IMAGEN", CodBidimMs.ToArray()))

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Public Function procesa_vvd_predial(ByVal clave As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_CAJ_03_INSERT_VENTAS_WEB_PREDIAL] '" + clave + "',11,0,1", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        For Each d In dt.Rows
                            array.Add(d)
                        Next
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return array
        End Function

        Public Function procesa_vvd_un(ByVal clave As String, ByVal folio_dep As String, ByVal folio_ico As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_CAJ_03_INSERT_VENTAS_v2]  '" + clave + "','" + folio_dep + "','" + folio_ico + "',1", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        For Each d In dt.Rows
                            array.Add(d)
                        Next
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return array
        End Function

        Public Function procesa_vvd_varios(ByVal clave As String, ByVal folio_dep As String, ByVal folio_ico As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_CAJ_03_INSERT_VENTAS_v3]  '" + clave + "','" + folio_dep + "',1", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        For Each d In dt.Rows
                            array.Add(d)
                        Next
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return array
        End Function

        Public Function REGRESA_ARRAY(ByVal SQL As String) As ArrayList
            Dim dt As DataTable = New DataTable()
            Dim array As New ArrayList

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            array.Add(d.itemArray)
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return array
        End Function
        Public Function VALIDA_FACTURACION(ByVal folio As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)
            Dim clave As Integer

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        ' Executar este para validar facturacion cuando es tema de fechas:
                        'Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].PA_VALIDA_FACTURACION_MODIFICACION_FECHAS '" + folio + "'", conn)


                        Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_VALIDA_FACTURACION] '" + folio + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        For Each d In dt.Rows
                            clave = If(IsDBNull(d(0)), New Integer, d(0))
                        Next
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return clave
        End Function

        '' =============================================================================================
        '' ===================================== Nueva facturacion =====================================
        '' =============================================================================================
        Public Function RECUPERA_VALOR_NUM(ByVal SentSQL As String) As String
            Dim dt As DataTable = New DataTable()
            Dim numero As Integer = 0

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            numero = If(IsDBNull(d.ItemArray(0)), New Integer, d.ItemArray(0))
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return numero
        End Function

        Public Function RECUPERA_FACTURAS(ByVal CS_Facturas_Lista As CS_Facturas_Lista, sqlString As String) As CS_Facturas_Lista
            'Dim CS_FACTURAS_FACT As New CS_Facturas
            Dim dt As DataTable = New DataTable()
            Dim L_Fact As New List(Of CS_Facturas)
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(sqlString, conn) 'I.CCO_CLAVE
                        cmd.CommandType = CommandType.Text
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            Dim CS_FACTURA As New CS_Facturas
                            CS_FACTURA.NumeroFactura = If(IsDBNull(d("NUM_FACTURA")), New Integer, d("NUM_FACTURA"))
                            CS_FACTURA.FechaFactura = If(IsDBNull(d("FECHA_FACTURA")), String.Empty, d("FECHA_FACTURA"))
                            CS_FACTURA.FolioUUIDFactura = If(IsDBNull(d("FOLIO_UUID")), New Integer, d("FOLIO_UUID"))
                            CS_FACTURA.NombreDeFactura = If(IsDBNull(d("NOMBRE_CLIENTE")), New Integer, d("NOMBRE_CLIENTE"))
                            CS_FACTURA.RFCFacturar = If(IsDBNull(d("RFC_CLIENTE")), String.Empty, d("RFC_CLIENTE"))
                            CS_FACTURA.ImporteDeLaFactura = If(IsDBNull(d("TOTAL_FACTURA")), String.Empty, d("TOTAL_FACTURA"))
                            L_Fact.Add(CS_FACTURA)
                        Next
                        CS_Facturas_Lista.ListaFacturas = L_Fact
                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return CS_Facturas_Lista
        End Function

        Public Function RECUPERA_VALOR_STRING(ByVal SentSQL As String) As String
            Dim dt As DataTable = New DataTable()
            Dim cadena As String = ""

            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            cadena = d.ItemArray(0)
                        Next

                        conn.Close()
                        Return cadena
                    Catch ex As Exception
                        conn.Close()
                        Return ex.Message
                    End Try
                End Using
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        Public Function Recuperar_Datos_fiscales(ByVal Datos_fiscales As Datos_fiscales, sqlString As String) As Datos_fiscales
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(sqlString, conn)
                        cmd.CommandType = CommandType.Text
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            Datos_fiscales.NOMBRE_FISCAL = If(IsDBNull(d("VCL_NOMBRE")), String.Empty, d("VCL_NOMBRE"))
                            Datos_fiscales.CP_FISCAL = If(IsDBNull(d("VCL_CP")), String.Empty, d("VCL_CP"))
                            Datos_fiscales.REGIMEN_FISCAL = If(IsDBNull(d("FRF_CLAVE_NOMBRE")), String.Empty, d("FRF_CLAVE_NOMBRE"))
                            Datos_fiscales.DIRECCION_FISCAL = If(IsDBNull(d("VCL_DIRECCION")), String.Empty, d("VCL_DIRECCION"))
                            Datos_fiscales.Mensaje = "200"
                        Next
                        conn.Close()
                    Catch ex As Exception
                        conn.Close()

                        Datos_fiscales.NOMBRE_FISCAL = ""
                        Datos_fiscales.CP_FISCAL = ""
                        Datos_fiscales.REGIMEN_FISCAL = ""
                        Datos_fiscales.DIRECCION_FISCAL = ""
                        Datos_fiscales.Mensaje = ex.Message & "Error en conexión"

                    End Try
                End Using
            Catch ex As Exception
                Datos_fiscales.NOMBRE_FISCAL = ""
                Datos_fiscales.CP_FISCAL = ""
                Datos_fiscales.REGIMEN_FISCAL = ""
                Datos_fiscales.DIRECCION_FISCAL = ""
                Datos_fiscales.Mensaje = ex.Message & "Error en inicio de sesion"
            End Try
            Return Datos_fiscales
        End Function


        Public Function RECUPERA_VALOR_STRING_REPLICAS(ByVal SentSQL As String) As String
            Dim dt As DataTable = New DataTable()
            Dim cadena As String = ""
            Try
                ConnString = Cs_ConexionBD.CONECT_REPLICAS() ' Esta conecxion apunta a multipagos.
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text
                        cmd.CommandTimeout = 900000
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            cadena = d.ItemArray(0)
                        Next

                        conn.Close()
                        Return cadena
                    Catch ex As Exception
                        conn.Close()
                        Return ex.Message
                    End Try

                End Using

            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

        Public Function RECUPERA_FOLIO_GRID(ByVal Folio_Grid As Folio_Grid, ByVal SQL As String) As Folio_Grid
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SQL, conn)
                        cmd.CommandType = CommandType.Text
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            Folio_Grid.Folio = If(IsDBNull(d("CDA_FOLIO")), String.Empty, d("CDA_FOLIO"))
                            Folio_Grid.Descripcion_Folio = If(IsDBNull(d("AAR_DESCRIPCION")), String.Empty, d("AAR_DESCRIPCION"))
                            Folio_Grid.Importe_Folio = If(IsDBNull(d("IIN_TOTAL")), String.Empty, d("IIN_TOTAL"))
                            Folio_Grid.MENSAJE = ""
                        Next
                        conn.Close()
                    Catch ex As Exception
                        conn.Close()

                        Folio_Grid.Folio = ""
                        Folio_Grid.Descripcion_Folio = ""
                        Folio_Grid.Importe_Folio = ""
                        Folio_Grid.MENSAJE = "ERROR NO CONTROLADO:" + ex.Message
                    End Try
                End Using
            Catch ex As Exception
                Folio_Grid.Folio = ""
                Folio_Grid.Descripcion_Folio = ""
                Folio_Grid.Importe_Folio = ""
                Folio_Grid.MENSAJE = "ERROR NO CONTROLADO: " + ex.Message
            End Try
            Return Folio_Grid
        End Function

        Public Function RECUPERA_FORMAS_PAGO(ByVal Lista_Forma_Pago As Lista_Forma_Pago, sqlString As String) As Lista_Forma_Pago
            'Dim CS_FACTURAS_FACT As New CS_Facturas
            Dim dt As DataTable = New DataTable()
            Dim L_PAGOS As New List(Of Recupera_Forma_Pago)
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(sqlString, conn) 'I.CCO_CLAVE
                        cmd.CommandType = CommandType.Text
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each d In dt.Rows
                            Dim Recupera_Forma_Pago As New Recupera_Forma_Pago
                            Recupera_Forma_Pago.FFP_CLAVE_NOMBRE = If(IsDBNull(d("FFP_CLAVE_NOMBRE")), New Integer, d("FFP_CLAVE_NOMBRE"))
                            Recupera_Forma_Pago.FFP_DESCRIPCION = If(IsDBNull(d("FFP_DESCRIPCION")), String.Empty, d("FFP_DESCRIPCION"))

                            L_PAGOS.Add(Recupera_Forma_Pago)
                        Next
                        Lista_Forma_Pago.ListaFormaPago = L_PAGOS
                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return Lista_Forma_Pago
        End Function
    End Class
End Namespace