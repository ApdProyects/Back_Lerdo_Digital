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

            Dim array As New ArrayList
            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try

                        Dim cmd As New SqlCommand("PA_CAJ_03_COBROS_CAJA", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@Folio", id_folio)
                        cmd.Parameters.AddWithValue("@IdDepto", id_depto)
                        cmd.Parameters.AddWithValue("@IdConcepto", id_concepto)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            array.Add(d.ItemArray)
                        Next



                        'For Each d In dt.Rows
                        '    Dim c As New Cs_Posts


                        '    c.LPA_CLAVE = If(IsDBNull(d("LPA_CLAVE")), String.Empty, d("LPA_CLAVE"))
                        '    c.LPA_TITULO = If(IsDBNull(d("LPA_TITULO")), String.Empty, d("LPA_TITULO"))
                        '    c.LPA_TIPO = If(IsDBNull(d("LPA_DESCRIPCION")), String.Empty, d("LPA_DESCRIPCION"))
                        '    c.LPA_DESCRIPCION = If(IsDBNull(d("LPA_TIPO")), String.Empty, d("LPA_TIPO"))
                        '    c.LPA_IMAGEN = If(IsDBNull(d("LPA_IMAGEN")), String.Empty, d("LPA_IMAGEN"))

                        '    Dim da As New SqlDataAdapter(c)
                        '    da.Fill(dt)

                        '    For Each d In dt.Rows
                        '        Array.Add(d.ItemArray)

                        '        L_Cs_Post.Add(c)

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