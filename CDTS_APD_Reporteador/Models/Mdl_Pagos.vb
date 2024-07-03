Imports System.Data.SqlClient
Imports System.Web.Mvc
Imports Mysqlx

Namespace Models
    Public Class Mdl_Pagos
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function GuardaPago(ByVal Cs_Pago As Cs_Pago, ByVal id_depto As Integer, ByVal id_concepto As Integer) As Cs_Respuesta_pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim dt As DataTable = New DataTable()
            Dim dtFecha As DateTime = DateTime.Now

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS]
                                                           ([CFC_FOLIO]
                                                           ,[CFC_ID_AFILIACION]
                                                           ,[CFC_REFERENCIA]
                                                           ,[CFC_RESULTADO_PAYW]
                                                           ,[CFC_FECHA_REQ_CTE]
                                                           ,[CFC_CODIGO_AUT]
                                                           ,[CFC_FECHA_RSP_CTE]
                                                           ,[CFC_BANCO]
                                                           ,[CFC_FECHA_COBRO]       
                                                           ,[CFC_IMPORTE_COBRADO] 
                                                           ,[CFC_ESTATUS]
                                                           ,[CDA_CLAVE]
                                                           ,[ICO_CLAVE]
                                                           ,[LUS_CLAVE])
                                                   VALUES
                                                          ('" + Cs_Pago.id_folio + "',
                                                           '" + Cs_Pago.ID_AFILIACION + "',
                                                           '" + Cs_Pago.REFERENCIA + "',
                                                           '" + Cs_Pago.RESULTADO_PAYW + "',
                                                           '" + Format(Cs_Pago.FECHA_REQ_CTE, "yyyy-MM-dd hh:MM:ss") + "',
                                                           '" + Cs_Pago.CODIGO_AUT + "', 
                                                           '" + Format(Cs_Pago.FECHA_RSP_CTE, "yyyy-MM-dd hh:MM:ss") + "',
                                                           '" + "BANORTE" + "', 
                                                           '" + Format(dtFecha, "yyyy-MM-dd hh:MM:ss") + "',
                                                           " + Cs_Pago.pago + ", 
                                                           'COBRADA',
                                                           " + id_depto.ToString() + ",
                                                           " + id_concepto.ToString() + ",
                                                           " + Cs_Pago.usuario + ")", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        Cs_Respuesta_pago.codigo = 1
                        Cs_Respuesta_pago.codigoError = 200
                        Cs_Respuesta_pago.mensaje = "Correcto"
                        Cs_Respuesta_pago.folio = ""
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Respuesta_pago
        End Function
        Public Function GeneraCobro(ByVal Cs_Pago As Cs_Pago) As Cs_Respuesta_pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CBM_ACTUALIZA_INGRESOS]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 500
                        cmd.Parameters.AddWithValue("@FOLIO", Cs_Pago.id_folio)
                        cmd.Parameters.AddWithValue("@DEPTO", Cs_Pago.id_depto)
                        cmd.Parameters.AddWithValue("@CONCEPTO", Cs_Pago.id_concepto)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Respuesta_pago.folio = If(IsDBNull(d("IIN_CLAVE")), New Integer, d("IIN_CLAVE"))
                            '  Cs_Respuesta_pago.VQR_CLAVE = If(IsDBNull(d("VQR_CLAVE")), New Integer, d("VQR_CLAVE"))
                        Next

                        conn.Close()

                        Cs_Respuesta_pago.codigo = 1
                        Cs_Respuesta_pago.codigoError = 200
                        Cs_Respuesta_pago.mensaje = "Correcto"
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Respuesta_pago
        End Function
        Public Function actualiza_Qr_validacion(ByVal Cs_Pago As String)
            Dim dt As DataTable = New DataTable()
            ConnString = Cs_ConexionBD.Connect()
            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("UPDATE [SRV_VIALIDAD].[APDSGEDB_PL_BIB_DIGITAL].[dbo].[QR_01_VALIDACION] set VQR_TIPO = 7 WHERE VGR_FOLIO ='" + Cs_Pago.ToString() + "'", conn)
                    cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                    conn.Close()
                Catch ex As Exception
                    conn.Close()
                    Throw ex
                End Try
            End Using
            Return 1
        End Function
        Public Function datos_folio_vialidad(ByVal id_folio As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim dtDet As DataTable = New DataTable()

            Dim array As New ArrayList
            Dim arrayDetalle As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("EXEC [SRV_VIALIDAD].[APDSGEDB_PL_BIB_DIGITAL].[dbo].[PA_CAJ_03_COBROS_CAJA]", conn)
                        cmd.CommandType = CommandType.Text
                        cmd.CommandTimeout = 3000

                        cmd.Parameters.AddWithValue("@Folio", id_folio)

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

        Public Function GuardaPagoCatastro(ByVal Cs_Pago As Cs_Pago, ByVal id_depto As Integer, ByVal id_concepto As Integer) As Cs_Respuesta_pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim dt As DataTable = New DataTable()
            Dim dtFecha As DateTime = DateTime.Now

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("
                            INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_01_FOLIOS_COBRADOS]
                                ([CFC_FOLIO]
                                ,[CFC_ID_AFILIACION]
                                ,[CFC_REFERENCIA]
                                ,[CFC_RESULTADO_PAYW]
                                ,[CFC_FECHA_REQ_CTE]
                                ,[CFC_CODIGO_AUT]
                                ,[CFC_FECHA_RSP_CTE]
                                ,[CFC_BANCO]
                                ,[CFC_FECHA_COBRO]       
                                ,[CFC_IMPORTE_COBRADO] 
                                ,[CFC_ESTATUS]
                                ,[CDA_CLAVE]
                                ,[ICO_CLAVE]
                                ,[LUS_CLAVE])
                            VALUES
                                ('" + Cs_Pago.id_folio + "',
                                 '" + Cs_Pago.ID_AFILIACION + "',
                                 '" + Cs_Pago.REFERENCIA + "',
                                 '" + Cs_Pago.RESULTADO_PAYW + "',
                                 '" + Format(Cs_Pago.FECHA_REQ_CTE, "yyyy-MM-dd hh:MM:ss") + "',
                                 '" + Cs_Pago.CODIGO_AUT + "', 
                                 '" + Format(Cs_Pago.FECHA_RSP_CTE, "yyyy-MM-dd hh:MM:ss") + "',
                                 '" + "BANORTE" + "', 
                                 '" + Format(dtFecha, "yyyy-MM-dd hh:MM:ss") + "',
                                 " + Cs_Pago.pago + ", 
                                 'COBRADA',
                                 " + id_depto.ToString() + ",
                                 " + id_concepto.ToString() + ",
                                 " + Cs_Pago.usuario + ")", conn)

                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        Cs_Respuesta_pago.codigo = 1
                        Cs_Respuesta_pago.codigoError = 200
                        Cs_Respuesta_pago.mensaje = "Correcto"
                        Cs_Respuesta_pago.folio = ""
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Respuesta_pago
        End Function
        Public Function GeneraCobroCatastro(ByVal Cs_Pago As Cs_Pago) As Cs_Respuesta_pago
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        'anterior Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CBM_ACTUALIZA_INGRESOS_PREDIAL]", conn)
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_CBM_ACTUALIZA_INGRESOS_PREDIAL_V4]", conn)

                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 500
                        cmd.Parameters.AddWithValue("@CLAVE_CATASTRAL", Cs_Pago.id_folio)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Respuesta_pago.folio = If(IsDBNull(d("IIN_CLAVE")), New Integer, d("IIN_CLAVE"))
                            ' Cs_Respuesta_pago.VQR_CLAVE = If(IsDBNull(d("VQR_CLAVE")), New Integer, d("VQR_CLAVE"))
                        Next

                        conn.Close()

                        Cs_Respuesta_pago.codigo = 1
                        Cs_Respuesta_pago.codigoError = 200
                        Cs_Respuesta_pago.mensaje = "Correcto"
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Cs_Respuesta_pago
        End Function
        Public Function GeneraODP_corralon(ByVal folio As String) As Integer
            Dim Cs_Respuesta_pago As New Cs_Respuesta_pago
            Dim dt As DataTable = New DataTable()
            Dim resultado As Integer = 0
            'Try
            '    ConnString = Cs_ConexionBD.Connect()
            '    Using conn As New SqlConnection(ConnString)
            '        Try
            '            Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_AYT_01_COBROS_INSERT_CORRALON_WEB]", conn)
            '            cmd.CommandType = CommandType.StoredProcedure
            '            cmd.CommandTimeout = 600
            '            cmd.Parameters.AddWithValue("@FOLIO", folio)
            '            Dim da As New SqlDataAdapter(cmd)
            '            da.Fill(dt)
            '            For Each d In dt.Rows
            '                Cs_Respuesta_pago.folio = If(IsDBNull(d("IIN_CLAVE")), New Integer, d("IIN_CLAVE"))
            '            Next
            '            conn.Close()
            '            Cs_Respuesta_pago.codigo = 1
            '            Cs_Respuesta_pago.codigoError = 200
            '            Cs_Respuesta_pago.mensaje = "Correcto"
            '        Catch ex As Exception
            '            conn.Close()
            '            Throw ex
            '        End Try
            '    End Using
            'Catch ex As Exception
            '    Throw ex
            'End Try
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_AYT_01_COBROS_INSERT_CORRALON_WEB]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 600
                        cmd.Parameters.AddWithValue("@FOLIO", folio)

                        ' Agregar parámetro de retorno
                        Dim returnParameter As SqlParameter = cmd.Parameters.Add("@Result", SqlDbType.Int)
                        returnParameter.Direction = ParameterDirection.ReturnValue
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        resultado = CInt(returnParameter.Value)
                        conn.Close()
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return resultado
        End Function
        ' ================================= 3D Secure ===============================================
        Public Function Gauarda3DSecure(ByVal Cs_3DSecure As Cs_3DSecure) As Cs_Respuesta
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("INSERT INTO [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_02_3D_SECURE]
                                                            ([3DS_FOLIO]
                                                            ,[3DS_NUM_TARJETA]
                                                            ,[3DS_ECI]
                                                            ,[3DS_CardType]
                                                            ,[3DS_XID]
                                                            ,[3DS_CAVV]
                                                            ,[3DS_Status]
                                                            ,[3DS_Reference3D]
                                                            ,[3DS_NUM_PROCESO])
                                                    VALUES
                                                            ('" + Cs_3DSecure.FOLIO + "'
                                                            ,'" + Cs_3DSecure.NUM_TARJETA + "'
                                                            ,'" + Cs_3DSecure.ECI + "'
                                                            ,'" + Cs_3DSecure.CardType + "'
                                                            ,'" + Cs_3DSecure.XID + "'
                                                            ,'" + Cs_3DSecure.CAVV + "'
                                                            ,'" + Cs_3DSecure.Status + "'
                                                            ,'" + Cs_3DSecure.Reference3D + "'
                                                            ," + Cs_3DSecure.NUM_PROCESO + ")", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        conn.Close()

                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.codigoError = 200
                        Cs_Respuesta.mensaje = "Correcto"
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
        Public Function Recupera3DSecure(ByVal Cs_3DSecure As Cs_3DSecure) As List(Of Cs_3DSecure)
            Dim Cs_Respuesta As New Cs_Respuesta
            Dim L_Cs_3DS As New List(Of Cs_3DSecure)
            Dim Cs_3DS As New Cs_3DSecure
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT [3DS_CLAVE]
                                                         ,[3DS_FOLIO]
                                                         ,[3DS_NUM_TARJETA]
                                                         ,[3DS_ECI]
                                                         ,[3DS_CardType]
                                                         ,[3DS_XID]
                                                         ,[3DS_CAVV]
                                                         ,[3DS_Status]
                                                         ,[3DS_Reference3D]
                                                         ,[3DS_NUM_PROCESO]
                                                     FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_02_3D_SECURE]
                                                     WHERE [3DS_FOLIO] = '" + Cs_3DSecure.FOLIO + "' 
                                                        OR [3DS_NUM_TARJETA] = '" + Cs_3DSecure.NUM_TARJETA + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_3DS = New Cs_3DSecure
                            Cs_3DS.NUM_TARJETA = If(IsDBNull(d("3DS_NUM_TARJETA")), New Integer, d("3DS_NUM_TARJETA"))

                            L_Cs_3DS.Add(Cs_3DS)
                        Next

                        conn.Close()

                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.codigoError = 200
                        Cs_Respuesta.mensaje = "Correcto"
                    Catch ex As Exception
                        conn.Close()
                        Throw ex
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return L_Cs_3DS
        End Function

        ' ============================================================================================================
        ' ==================================== Envio en autimatico de informacion ==================================== 
        ' ============================================================================================================
        Public Function RECUPERA_VALOR_STRING_REPLICAS(ByVal SentSQL As String) As String
            Dim dt As DataTable = New DataTable()
            Dim cadena As String = ""
            Try
                ConnString = Cs_ConexionBD.CONECT_REPLICAS()
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
        '
        '           BLOQUE GENERADO PARA RECUPERAR UN VALOR TIPO STRING
        '
        Public Function RECUPERA_VALOR_STRING(ByVal SentSQL As String) As String
            Dim dt As DataTable = New DataTable()
            Dim cadena As String = ""
            Try
                ConnString = Cs_ConexionBD.Connect()
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

    End Class
End Namespace