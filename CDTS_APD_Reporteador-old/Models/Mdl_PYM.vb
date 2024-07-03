Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_PYM
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String

        Public Function Recupera_RptPYM(ByVal opcion As String, ByVal fechainicio As String, ByVal fechafin As String, ByVal estatus As String)
            Dim dt As DataTable = New DataTable()
            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_IND_01_PYM", conn)
                        cmd.CommandType = CommandType.StoredProcedure

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
End Namespace