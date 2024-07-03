Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_Posts
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function Recupera_posts(ByVal type As String) As List(Of Cs_Posts)
            Dim L_Cs_Post As New List(Of Cs_Posts)

            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_RECUPERAPOSTS", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@TIPO", type)
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim c As New Cs_Posts


                            c.LPA_CLAVE = If(IsDBNull(d("LPA_CLAVE")), String.Empty, d("LPA_CLAVE"))
                            c.LPA_TITULO = If(IsDBNull(d("LPA_TITULO")), String.Empty, d("LPA_TITULO"))
                            c.LPA_TIPO = If(IsDBNull(d("ACI_NOMBRE")), String.Empty, d("ACI_NOMBRE"))
                            c.LPA_DESCRIPCION = If(IsDBNull(d("LPA_TIPO")), String.Empty, d("LPA_TIPO"))
                            c.LPA_IMAGEN = If(IsDBNull(d("LPA_IMAGEN")), String.Empty, d("LPA_IMAGEN"))



                            L_Cs_Post.Add(c)
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

            Return L_Cs_Post
        End Function

        Public Function Mod_Posts(ByVal CURP As String, ByVal password As String) As Cs_Respuesta
            Dim Cs_Ciudadano As New Cs_Ciudadano_AC
            Dim Cs_Respuesta As New Cs_Respuesta

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_MODPASSCIUDADANO_AC", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@CURP", CURP)
                        cmd.Parameters.AddWithValue("@password", password)

                        conn.Open()
                        cmd.ExecuteNonQuery()

                        Cs_Respuesta.codigo = 1
                        Cs_Respuesta.mensaje = "Correcto"

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
    End Class
End Namespace