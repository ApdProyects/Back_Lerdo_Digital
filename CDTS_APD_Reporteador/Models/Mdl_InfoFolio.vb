Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_InfoFolio
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function Recupera_datos_folio(ByVal id_folio As String, ByVal id_concepto As Integer, ByVal id_depto As Integer)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim dtDet As DataTable = New DataTable()

            Dim array As New ArrayList
            Dim arrayDetalle As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CBM_BUSCA_FOLIO]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 3000

                        cmd.Parameters.AddWithValue("@FOLIO", id_folio)
                        cmd.Parameters.AddWithValue("@CONCEPTO", id_depto)
                        cmd.Parameters.AddWithValue("@DEPTO", id_concepto)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            array.Add(d.ItemArray)
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
        Public Function RecuperaConcepto(ByVal id_folio As String, ByVal id_depto As Integer, ByVal id_concepto As Integer) As String
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()
            Dim Concepto As String = ""

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT ICO_NOMBRE
                                                   FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_02_CONCEPTOS " +
                                                  "WHERE CDA_CLAVE=" + id_depto.ToString() + " AND ICO_CLAVE=" + id_concepto.ToString(), conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Concepto = If(IsDBNull(d("ICO_NOMBRE")), "SIN CONCEPTO", d("ICO_NOMBRE"))
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

            Return Concepto
        End Function
        Public Function Recupera_Conceptos(ByVal id_folio As String, ByVal id_depto As Integer, ByVal id_concepto As Integer) As List(Of Cs_Conceptos)
            Dim dt As DataTable = New DataTable()
            Dim Conceptos As New List(Of Cs_Conceptos)

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try


                        Dim cmd As New SqlCommand("SELECT C.ICO_NOMBRE,I.IIN_SUBTOTAL,I.IIN_DESCUENTO_IMPORTE,I.IIN_TOTAL
                        FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_02_CONCEPTOS C
                        INNER JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].ING_03_INGRESOS I
                        ON I.ICO_CLAVE = C.ICO_CLAVE
                        WHERE C.CDA_CLAVE=" + id_depto.ToString() + " AND I.CDA_FOLIO = '" + id_folio.ToString() + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim concep = New Cs_Conceptos
                            concep.ICO_NOMBRE = If(IsDBNull(d("ICO_NOMBRE")), "SIN CONCEPTO", d("ICO_NOMBRE"))
                            concep.IIN_SUBTOTAL = If(IsDBNull(d("IIN_SUBTOTAL")), "", d("IIN_SUBTOTAL"))
                            concep.IIN_DESCUENTO_IMPORTE = If(IsDBNull(d("IIN_DESCUENTO_IMPORTE")), "", d("IIN_DESCUENTO_IMPORTE"))
                            concep.IIN_TOTAL = If(IsDBNull(d("IIN_TOTAL")), "", d("IIN_TOTAL"))
                            Conceptos.Add(concep)
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

            Return Conceptos
        End Function
        Public Function Recupera_Conceptos_ODP_corralon(ByVal id_folio As String) As List(Of Cs_Conceptos)
            Dim dt As DataTable = New DataTable()
            Dim Conceptos As New List(Of Cs_Conceptos)

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try


                        Dim cmd As New SqlCommand("SELECT I.IIN_TOTAL,C.ICO_NOMBRE FROM [SRV_VIALIDAD].[APDSGEDB_PL].DBO.ING_03_INGRESOS I
                        JOIN [SRV_VIALIDAD].[APDSGEDB_PL].DBO.ING_02_CONCEPTOS C ON C.ICO_CLAVE=I.ICO_CLAVE
                        WHERE I.CDA_FOLIO  = '" + id_folio.ToString() + "' ORDER BY I.IIN_CLAVE DESC", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim concep = New Cs_Conceptos
                            concep.ICO_NOMBRE = If(IsDBNull(d("ICO_NOMBRE")), "SIN CONCEPTO", d("ICO_NOMBRE"))
                            concep.IIN_TOTAL = If(IsDBNull(d("IIN_TOTAL")), "", d("IIN_TOTAL"))
                            Conceptos.Add(concep)
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

            Return Conceptos
        End Function
        Public Function RecuperaMultas(ByVal folio As String) As Cs_Multas
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()
            Dim Cs_Multas As New Cs_Multas

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT M.VMU_CLAVE,P.VPR_NOMBRE,M.VMU_FOLIO,M.VMU_INFRACCION_LUGAR,M.VMU_PLACAS,M.VMU_PROCEDENCIA,M.VMU_GARANTIA_REFERENCIA," +
                                                  "M.VMU_INFRACCION_FECHA,M.VMU_INFRACCION_HORA,M.VMU_IMPORTE,VGR_NOMBRE " +
                                                  "FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VIA_10_MULTAS M " +
                                                  "JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VIA_09_GARANTIAS G " +
                                                  "ON M.VGR_CLAVE = G.VGR_CLAVE " +
                                                  "JOIN [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VIA_02_PROPIETARIOS P " +
                                                  "ON M.VPR_CLAVE = P.VPR_CLAVE " +
                                                  "WHERE M.VMU_FOLIO = '" + folio + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Multas.VMU_CLAVE = If(IsDBNull(d("VMU_CLAVE")), String.Empty, d("VMU_CLAVE"))
                            Cs_Multas.VMU_FOLIO = If(IsDBNull(d("VMU_FOLIO")), String.Empty, d("VMU_FOLIO"))
                            Cs_Multas.PROPIETARIO = If(IsDBNull(d("VPR_NOMBRE")), String.Empty, d("VPR_NOMBRE"))
                            Cs_Multas.VMU_INFRACCION_LUGAR = If(IsDBNull(d("VMU_INFRACCION_LUGAR")), String.Empty, d("VMU_INFRACCION_LUGAR"))
                            Cs_Multas.VMU_PLACAS = If(IsDBNull(d("VMU_PLACAS")), String.Empty, d("VMU_PLACAS"))
                            Cs_Multas.VMU_PROCEDENCIA = If(IsDBNull(d("VMU_PROCEDENCIA")), String.Empty, d("VMU_PROCEDENCIA"))
                            Cs_Multas.VGR_NOMBRE = If(IsDBNull(d("VGR_NOMBRE")), String.Empty, d("VGR_NOMBRE"))
                            Cs_Multas.VMU_GARANTIA_REFERENCIA = If(IsDBNull(d("VMU_GARANTIA_REFERENCIA")), String.Empty, d("VMU_GARANTIA_REFERENCIA"))
                            Cs_Multas.VMU_INFRACCION_FECHA = If(IsDBNull(d("VMU_INFRACCION_FECHA")), String.Empty, d("VMU_INFRACCION_FECHA"))
                            Cs_Multas.VMU_INFRACCION_HORA = If(IsDBNull(d("VMU_INFRACCION_HORA")), String.Empty, d("VMU_INFRACCION_HORA"))
                            Cs_Multas.VMU_IMPORTE = If(IsDBNull(d("VMU_IMPORTE")), String.Empty, d("VMU_IMPORTE"))
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

            Return Cs_Multas
        End Function
        Public Function RecuperaMultasDet(ByVal clave As String) As List(Of Cs_MultasDet)
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()
            Dim Cs_MultasDet As New List(Of Cs_MultasDet)

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT * FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].VIA_11_MULTAS_DET M " +
                                                  "WHERE M.VMU_CLAVE = '" + clave + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim md = New Cs_MultasDet
                            md.VIN_CLAVE = If(IsDBNull(d("VIN_CLAVE")), String.Empty, d("VIN_CLAVE"))
                            md.VIN_NOMBRE = If(IsDBNull(d("VIN_NOMBRE")), String.Empty, d("VIN_NOMBRE"))
                            md.VIN_DESCRIPCION = If(IsDBNull(d("VIN_DESCRIPCION")), String.Empty, d("VIN_DESCRIPCION"))
                            md.VIN_UMA_MINIMA = If(IsDBNull(d("VIN_UMA_MINIMA")), String.Empty, d("VIN_UMA_MINIMA"))
                            md.VMF_IMPORTE_MINIMO = If(IsDBNull(d("VMF_IMPORTE_MINIMO")), String.Empty, d("VMF_IMPORTE_MINIMO"))
                            md.VIN_UMA_MAXIMA = If(IsDBNull(d("VIN_UMA_MAXIMA")), String.Empty, d("VIN_UMA_MAXIMA"))
                            md.VMF_IMPORTE_MIXIMO = If(IsDBNull(d("VMF_IMPORTE_MAXIMO")), String.Empty, d("VMF_IMPORTE_MAXIMO"))

                            Cs_MultasDet.Add(md)
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

            Return Cs_MultasDet
        End Function
        Public Function RecuperaInfoIngresos(ByVal folio As String, ByVal departamento As String, ByVal Concepto As String) As Cs_Ingreso
            Dim dt As DataTable = New DataTable()
            Dim Cs_Ingreso As New Cs_Ingreso

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("exec PA_CAJ_03_COBROS_CAJA '" + folio + "'," + departamento + "," + Concepto, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Ingreso.IIN_CLAVE = If(IsDBNull(d("IIN_CLAVE")), String.Empty, d("IIN_CLAVE"))
                            Cs_Ingreso.IIN_FECALTA = If(IsDBNull(d("IIN_FECALTA")), String.Empty, d("IIN_FECALTA"))
                            Cs_Ingreso.CDA_FOLIO = If(IsDBNull(d("CDA_FOLIO")), String.Empty, d("CDA_FOLIO"))
                            Cs_Ingreso.IIN_SUBTOTAL = If(IsDBNull(d("IIN_SUBTOTAL")), String.Empty, d("IIN_SUBTOTAL"))
                            Cs_Ingreso.IIN_DESCUENTO_IMPORTE = If(IsDBNull(d("IIN_DESCUENTO_IMPORTE")), String.Empty, d("IIN_DESCUENTO_IMPORTE"))
                            Cs_Ingreso.IIN_TOTAL = If(IsDBNull(d("IIN_TOTAL")), String.Empty, d("IIN_TOTAL"))
                            Cs_Ingreso.NOMBRE = If(IsDBNull(d("NOMBRE")), String.Empty, d("NOMBRE"))
                            Cs_Ingreso.DIRECCION = If(IsDBNull(d("DIRECCION")), String.Empty, d("DIRECCION"))
                            Cs_Ingreso.COLONIA = If(IsDBNull(d("COLONIA")), String.Empty, d("COLONIA"))
                            Cs_Ingreso.MUNICIPIO = If(IsDBNull(d("MUNICIPIO")), String.Empty, d("MUNICIPIO"))
                            Cs_Ingreso.ESTADO = If(IsDBNull(d("ESTADO")), String.Empty, d("ESTADO"))
                            Cs_Ingreso.CP = If(IsDBNull(d("CP")), String.Empty, d("CP"))
                            Cs_Ingreso.CELULAR = If(IsDBNull(d("CELULAR")), String.Empty, d("CELULAR"))
                            Cs_Ingreso.GARANTIA = If(IsDBNull(d("GARANTIA")), String.Empty, d("GARANTIA"))
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

            Return Cs_Ingreso
        End Function
        Public Function Recupera_datos_folio_catastro(ByVal clave As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("PA_CAJ_03_COBROS_CAJA", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 3000

                        cmd.Parameters.AddWithValue("@Folio", clave)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            array.Add(d.ItemArray)
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
        Public Function RecuperarFolioRecibo() As Integer
            Dim dt As DataTable = New DataTable()
            Dim folio As Integer

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT AUF_RECIBO_WEB FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].APD_07_ULTIMOFOLIO", conn)

                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            folio = If(IsDBNull(d("AUF_RECIBO_WEB")), String.Empty, d("AUF_RECIBO_WEB") + 1)
                        Next

                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_07_ULTIMOFOLIO] SET [AUF_RECIBO_WEB] = " + folio.ToString(), conn)
                        'Dim cmd As New SqlCommand("UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[APD_07_ULTIMOFOLIO] SET [AUF_RECIBO_21] = " + folio.ToString(), conn)
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

            Return folio
        End Function
    End Class
End Namespace