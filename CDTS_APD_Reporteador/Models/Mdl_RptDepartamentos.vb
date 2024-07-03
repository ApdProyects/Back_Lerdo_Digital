Imports System.Data.SqlClient
Imports System.Web.Mvc

Public Class Mdl_RptDepartamentos

    Dim Cs_ConexionDB As Cs_ConexionBD()
    Dim ConnString As String

    Public Function Recupera_RptSeguridad(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_SEGURIDAD", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptParquimetros(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_PQM", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptPYM(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_PYM", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptSM(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_SM", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptPS(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_PS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptMA(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_MA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptPC(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_PC", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptRST(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_RST", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptDeportes(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_DEPORTES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptDUB(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_DUB", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptAlcoholes(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_ALCOHOLES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptFE(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_FE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
    Public Function Recupera_RptArte(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
        Dim dt As DataTable = New DataTable()
        Dim array As New ArrayList
        Try
            ConnString = Cs_ConexionBD.Connect()

            Using conn As New SqlConnection(ConnString)
                Try
                    Dim cmd As New SqlCommand("PA_IND_01_ARTECULTURA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 3000
                    cmd.Parameters.AddWithValue("@Opcion", opcion)
                    cmd.Parameters.AddWithValue("@FechaIni", fechainicio)
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin)
                    cmd.Parameters.AddWithValue("@Estatus", estatus)

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
End Class
