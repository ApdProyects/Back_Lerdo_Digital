Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web.Mvc
Imports MySql.Data.MySqlClient

Namespace Models
    Public Class Mdl_Usuarios
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String

        Public Function loginUsuario_NEW(ByVal Cs_Usuario As Cs_Usuario, ByVal TELEFONO_P As String, ByVal password As String) As Cs_Usuario
            'Dim l_Usuario As New List(Of Cs_Usuario)
            'Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_AUTENTICACION_LERDO_DIGITAL] '" & TELEFONO_P & "', '" & password & "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Usuario.LUS_CLAVE = If(IsDBNull(d("LUS_CLAVE")), New Integer, d("LUS_CLAVE"))
                            Cs_Usuario.LRO_CLAVE = If(IsDBNull(d("LRO_CLAVE")), New Integer, d("LRO_CLAVE"))
                            Cs_Usuario.LUS_CORREO = If(IsDBNull(d("LUS_CORREO")), String.Empty, d("LUS_CORREO"))
                            Cs_Usuario.LUS_TELEFONO = If(IsDBNull(d("LUS_TELEFONO")), String.Empty, d("LUS_TELEFONO"))
                            Cs_Usuario.LUS_USUARIO = If(IsDBNull(d("LUS_USUARIO")), String.Empty, d("LUS_USUARIO"))
                            Cs_Usuario.LUS_CONTRASENA = If(IsDBNull(d("LUS_CONTRASENA")), String.Empty, d("LUS_CONTRASENA"))
                            Cs_Usuario.ULTIMO_RFC = If(IsDBNull(d("ULTIMO_RFC")), String.Empty, d("ULTIMO_RFC"))
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

            Return Cs_Usuario
        End Function

        ''NO SE SI SE USA ESTE PROCESO
        Public Function loginUsuario(ByVal TELEFONO_P As String, ByVal password As String)
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_AUTENTICACION_LERDO_DIGITAL] '" & TELEFONO_P & "', '" & password & "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Cs_Usuario.LUS_CLAVE = If(IsDBNull(d("LUS_CLAVE")), New Integer, d("LUS_CLAVE"))
                            Cs_Usuario.LRO_CLAVE = If(IsDBNull(d("LRO_CLAVE")), New Integer, d("LRO_CLAVE"))
                            Cs_Usuario.LUS_CORREO = If(IsDBNull(d("LUS_CORREO")), String.Empty, d("LUS_CORREO"))
                            Cs_Usuario.LUS_TELEFONO = If(IsDBNull(d("LUS_TELEFONO")), String.Empty, d("LUS_TELEFONO"))
                            Cs_Usuario.LUS_USUARIO = If(IsDBNull(d("LUS_USUARIO")), String.Empty, d("LUS_USUARIO"))
                            Cs_Usuario.LUS_CONTRASENA = If(IsDBNull(d("LUS_CONTRASENA")), String.Empty, d("LUS_CONTRASENA"))
                            Cs_Usuario.ULTIMO_RFC = If(IsDBNull(d("ULTIMO_RFC")), String.Empty, d("ULTIMO_RFC"))
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

            Return Cs_Usuario
        End Function

        Public Function Recuperapassword(ByVal telefono As String) As List(Of Cs_Usuario)
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT top 1 * FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS WHERE LUS_TELEFONO = '" + telefono + "'", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim c As New Cs_Usuario
                            c.LUS_CLAVE = If(IsDBNull(d("LUS_CLAVE")), New Integer, d("LUS_CLAVE"))
                            c.LRO_CLAVE = If(IsDBNull(d("LRO_CLAVE")), New Integer, d("LRO_CLAVE"))
                            c.LUS_CORREO = If(IsDBNull(d("LUS_CORREO")), New Date, d("LUS_CORREO"))
                            c.LUS_TELEFONO = If(IsDBNull(d("LUS_TELEFONO")), String.Empty, d("LUS_TELEFONO"))
                            c.LUS_USUARIO = If(IsDBNull(d("LUS_USUARIO")), String.Empty, d("LUS_USUARIO"))
                            c.LUS_CONTRASENA = If(IsDBNull(d("LUS_CONTRASENA")), String.Empty, d("LUS_CONTRASENA"))

                            l_Usuario.Add(c)
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

            Return l_Usuario
        End Function

        Public Function verifiedUsuario(ByVal correo As String, ByVal telefono As String) As Boolean
            Dim l_Usuario As New List(Of Cs_Usuario)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT * FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS WHERE LUS_TELEFONO='" + telefono + "' or LUS_CORREO='" + correo + "' ", conn)
                        cmd.CommandType = CommandType.Text



                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        'LUS_CLAVE Int PRIMARY KEY IDENTITY(1, 1),
                        'LRO_CLAVE Int,
                        'LUS_FUM SMALLDATETIME ,
                        'LUS_CORREO NVARCHAR(20),
                        'LUS_TELEFONO NVARCHAR(20),
                        'LUS_USUARIO NVARCHAR(20),
                        'LUS_CONTRASENA NVARCHAR(50),
                        For Each d In dt.Rows
                            'Dim c As New Cs_Usuario
                            'c.LUS_CLAVE = If(IsDBNull(d("LUS_CLAVE")), New Integer, d("LUS_CLAVE"))
                            'c.LRO_CLAVE = If(IsDBNull(d("LRO_CLAVE")), New Integer, d("LRO_CLAVE"))
                            'c.LUS_CORREO = If(IsDBNull(d("LUS_CORREO")), New Date, d("LUS_CORREO"))
                            'c.LUS_TELEFONO = If(IsDBNull(d("LUS_TELEFONO")), String.Empty, d("LUS_TELEFONO"))
                            'c.LUS_USUARIO = If(IsDBNull(d("LUS_USUARIO")), String.Empty, d("LUS_USUARIO"))
                            'c.LUS_CONTRASENA = If(IsDBNull(d("LUS_CONTRASENA")), String.Empty, d("LUS_CONTRASENA"))


                            'l_Usuario.Add(c)
                            Return False
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
            If (dt.Rows.Count > 0) Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function registroUsuario(ByVal correo As String, ByVal telefono As String, ByVal usuario As String, ByVal contrasena As String)
            Dim dt As DataTable = New DataTable()
            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        '@prmLRO_CLAVE	 INT,
                        '@prmLUS_FUM	 SMALLDATETIME,
                        '   @prmLUS_USUARIO	NVARCHAR(20),
                        '   @prmLUS_CORREO	NVARCHAR(20),
                        '   @prmLUS_TELEFONO	NVARCHAR(20),
                        '   @prmLUS_CONTRASENA	NVARCHAR(20)
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_LDG_04_USUARIOS_ALTAS]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandTimeout = 3000
                        cmd.Parameters.AddWithValue("@prmLUS_CORREO", correo)
                        cmd.Parameters.AddWithValue("@prmLUS_TELEFONO", telefono)
                        cmd.Parameters.AddWithValue("@prmLUS_USUARIO", usuario)
                        cmd.Parameters.AddWithValue("@prmLUS_CONTRASENA", contrasena)
                        cmd.Parameters.AddWithValue("@prmLRO_CLAVE", 2)


                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        Dim Cs_JWT As New Cs_JWT
                        Dim JWT As String
                        For Each d In dt.Rows
                            Dim item = d.ItemArray(0)
                            JWT = Cs_JWT.GenerateToken(item, 1)
                            Dim data = New ArrayList()
                            data.Add(d.ItemArray(0))
                            data.Add(d.ItemArray(1))
                            data.Add(d.ItemArray(2))
                            data.Add(d.ItemArray(3))
                            data.Add(d.ItemArray(4))
                            data.Add(d.ItemArray(5))
                            data.Add(d.ItemArray(6))
                            data.Add(JWT)



                            'd.ItemArray.Add(7, JWT)
                            array.Add(data)
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

        Public Function recuperaResultadosUPD() As List(Of Cs_ResultadosUPD)
            Dim l_Usuario As New List(Of Cs_ResultadosUPD)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.myConnect()

                Using conn As New MySqlConnection(ConnString)
                    Try
                        Dim cmd As New MySqlCommand("SELECT * FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[adm_11_resultados]", conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim c As New Cs_ResultadosUPD
                            c.RES_CLAVE = If(IsDBNull(d("RES_CLAVE")), New Integer, d("RES_CLAVE"))
                            c.USU_CLAVE = If(IsDBNull(d("USU_CLAVE")), New Integer, d("USU_CLAVE"))
                            c.RES_MATRICULA = If(IsDBNull(d("RES_MATRICULA")), String.Empty, d("RES_MATRICULA"))
                            c.RES_NOMBRE = If(IsDBNull(d("RES_NOMBRE")), String.Empty, d("RES_NOMBRE"))
                            c.RES_CORREO = If(IsDBNull(d("RES_CORREO")), String.Empty, d("RES_CORREO"))
                            c.RES_PUNTOS = If(IsDBNull(d("RES_PUNTOS")), String.Empty, d("RES_PUNTOS"))
                            c.RES_CALIFICACION = If(IsDBNull(d("RES_CALIFICACION")), String.Empty, d("RES_CALIFICACION"))
                            c.RES_TELEFONO = If(IsDBNull(d("RES_TELEFONO")), String.Empty, d("RES_TELEFONO"))
                            c.RES_MODALIDAD = If(IsDBNull(d("RES_MODALIDAD")), String.Empty, d("RES_MODALIDAD"))
                            c.RES_RESULTADO = If(IsDBNull(d("RES_RESULTADO")), String.Empty, d("RES_RESULTADO"))
                            c.RES_ENVIADO = If(IsDBNull(d("RES_ENVIADO")), String.Empty, d("RES_ENVIADO"))

                            l_Usuario.Add(c)
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

            Return l_Usuario
        End Function

        Public Function ActualizaEnvio(ByVal RES_CLAVE As Integer) As Cs_Respuesta
            Dim l_Usuario As New List(Of Cs_ResultadosUPD)
            Dim Cs_Usuario As New Cs_Usuario
            Dim dt As DataTable = New DataTable()
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                ConnString = Cs_ConexionBD.myConnect()

                Using conn As New MySqlConnection(ConnString)
                    Try
                        Dim cmd As New MySqlCommand("UPDATE adm_11_resultados SET RES_ENVIADO='SI' WHERE RES_CLAVE = " + RES_CLAVE.ToString(), conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New MySqlDataAdapter(cmd)
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

            Cs_Respuesta.codigo = 1
            Cs_Respuesta.codigoError = 200
            Cs_Respuesta.mensaje = "Correo"

            Return Cs_Respuesta
        End Function

        Public Function ActualizarUsuario(LUS_CLAVE As Integer, LUS_USUARIO As String, LUS_TELEFONO As String, LUS_CORREO As String, OLD_PASS As String, NEW_PASS As String) As String
            Dim cadena As String = ""
            Try
                Dim ConnString As String = Cs_ConexionBD.Connect()
                Dim dt As DataTable = New DataTable()

                Using conn As New SqlConnection(ConnString)
                    Dim cmd As New SqlCommand("EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_ACTUALIZA_REGISTRO_DE_USUARIOS_LDG_04_USUARIOS] 
                                                        '" & LUS_USUARIO & "' , 
                                                        '" & LUS_TELEFONO & "' , 
                                                        '" & LUS_CORREO & "', 
                                                        " & LUS_CLAVE.ToString() & " , 
                                                        '" & OLD_PASS & "' , 
                                                        '" & NEW_PASS & "'  ", conn)
                    'EXEC [SRV_VIALIDAD].[APDSGEDB_PL].[DBO].[PA_ACTUALIZA_REGISTRO_DE_USUARIOS_LDG_04_USUARIOS] '' , '', '', NULL, '1', '1'
                    cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                    For Each d In dt.Rows
                        cadena = d.ItemArray(0)
                    Next

                    Return cadena
                End Using
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function
        Public Function buscarUsuario(ByVal correo As String) As Cs_Usuario
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("SELECT TOP 1 * FROM [SRV_VIALIDAD].[APDSGEDB_PL].[dbo].LDG_04_USUARIOS WHERE  LUS_CORREO=@Correo", conn)
                        cmd.CommandType = CommandType.Text

                        cmd.Parameters.AddWithValue("@Correo", correo)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Dim d = dt.Rows(0)
                            Dim usuario As New Cs_Usuario()
                            usuario.LUS_CORREO = If(IsDBNull(d("LUS_CORREO")), String.Empty, d("LUS_CORREO"))
                            usuario.LUS_TELEFONO = If(IsDBNull(d("LUS_TELEFONO")), String.Empty, d("LUS_TELEFONO"))
                            usuario.LUS_USUARIO = If(IsDBNull(d("LUS_USUARIO")), String.Empty, d("LUS_USUARIO"))
                            usuario.LUS_CONTRASENA = If(IsDBNull(d("LUS_CONTRASENA")), String.Empty, d("LUS_CONTRASENA"))


                            Return usuario
                        End If
                    Catch ex As Exception
                        Throw ex
                    Finally
                        conn.Close()
                    End Try
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return Nothing
        End Function

    End Class
End Namespace