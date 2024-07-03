Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_Catastro
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function Recupera_telefono_Catastro(ByVal clave As String) As String
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            'Try
            '    ConnString = Cs_ConexionBD.Connect()

            '    Using conn As New SqlConnection(ConnString)
            '        Try

            '            Dim cmd As New SqlCommand("" + clave + "'", conn)
            '            cmd.CommandType = CommandType.Text

            '            Dim da As New SqlDataAdapter(cmd)
            '            da.Fill(dt)

            '            For Each d In dt.Rows
            '                array.Add(d.ItemArray)
            '            Next

            '            conn.Close()
            '        Catch ex As Exception
            '            conn.Close()
            '            Throw ex
            '        End Try
            '    End Using
            'Catch ex As Exception
            '    Throw ex
            'End Try
            telefono = "8712619496"

            Return telefono
        End Function
        Public Function Actualiza_Codigo_Catastro(ByVal clave As String, ByVal codigo As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim hora As Date
            hora = Date.Now

            Dim horamas As Date
            horamas = Date.Now
            horamas = hora.AddMinutes(5)

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("UPDATE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_02_CODIGO_VERIFICACION]
                                                  SET  [CVV_CODIGO] = " + codigo + "
                                                    ,[CVV_FECHA_CREACION] = '" + hora.ToString("yyyy-MM-dd HH:mm:ss") + "'
                                                    ,[CVV_FECHA_EXPIRACION] = '" + horamas.ToString("yyyy-MM-dd HH:mm:ss") + "'
                                                  WHERE [CCV_CLAVE_CATASTRAL] = '" + clave + "'", conn)
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
            Return 1
        End Function
        Public Function Valida_Codigo_Catastro(ByVal clave As String, ByVal codigo As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("SELECT [CCV_CLAVE]
                                                        ,[CCV_CLAVE_CATASTRAL]
                                                        ,[CVV_TELEFONO]
                                                        ,[CVV_CODIGO]
                                                        ,[CVV_FECHA_CREACION]
                                                        ,[CVV_FECHA_EXPIRACION]
                                                FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[CBM_02_CODIGO_VERIFICACION]
                                                WHERE CCV_CLAVE_CATASTRAL = '" + clave + "'", conn)
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
        Public Function traer_info_catastro(ByVal clave As String, ByVal tipo As String)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()
            Dim telefono As String = ""

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        'Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].PA_CST_01_CALCULO_SELECT_V1	'" + clave + "'," + tipo + ",0", conn)
                        'Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].PA_CST_01_CALCULO_SELECT_V4  '" + clave + "'," + tipo + ",0", conn)
                        Dim cmd As New SqlCommand("EXECUTE [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].PA_CST_01_CALCULO_SELECT_V4 @CLAVECATASTRAL, @TIPOCUENTA, @INSERT", conn)

                        cmd.Parameters.AddWithValue("@CLAVECATASTRAL", clave)
                        cmd.Parameters.AddWithValue("@TIPOCUENTA", tipo)
                        cmd.Parameters.AddWithValue("@INSERT", 0)
                        'cmd.CommandType = CommandType.Text

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
    End Class
End Namespace