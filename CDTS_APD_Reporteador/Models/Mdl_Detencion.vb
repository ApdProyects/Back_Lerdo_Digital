Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_Detencion
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function GuardaDetencion(ByVal Detencion As Cs_Detencion) As Cs_Respuesta
            Dim Cs_Resultado As New Cs_Respuesta
            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_SPM_01_GUARDADETENCION", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@SDE_NOMBRE", Detencion.SDE_NOMBRE)
                        cmd.Parameters.AddWithValue("@SDE_APELLIDOS", Detencion.SDE_APELLIDOS)
                        cmd.Parameters.AddWithValue("@SDE_ALIAS", Detencion.SDE_ALIAS)
                        cmd.Parameters.AddWithValue("@SDE_LUG_DETENCION", Detencion.SDE_LUG_DETENCION)
                        cmd.Parameters.AddWithValue("@SDE_DESTINO", Detencion.SDE_DESTINO)
                        cmd.Parameters.AddWithValue("@SDE_LONGITUD", Detencion.SDE_LONGITUD)
                        cmd.Parameters.AddWithValue("@SDE_LATITUD", Detencion.SDE_LATITUD)
                        cmd.Parameters.AddWithValue("@SDE_FOTO", Detencion.SDE_FOTO)
                        cmd.Parameters.AddWithValue("@SDI_IMEI", Detencion.SDI_IMEI)
                        cmd.Parameters.AddWithValue("@SDE_FECHA_DETENCION", Detencion.SDE_FECHA_DETENCION)
                        cmd.Parameters.AddWithValue("@SUS_CLAVE", Detencion.SUS_CLAVE)
                        cmd.Parameters.AddWithValue("@SPA_CLAVE", Detencion.SPA_CLAVE)
                        cmd.Parameters.AddWithValue("@SDE_MOTIVO", Detencion.SDE_MOTIVO)
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_Resultado.CODIGO = If(IsDBNull(D("CODIGO")), New Integer, D("CODIGO"))
                            Cs_Resultado.CODIGOERROR = If(IsDBNull(D("CODIGOERROR")), New Integer, D("CODIGOERROR"))
                            Cs_Resultado.MENSAJE = If(IsDBNull(D("MENSAJE")), String.Empty, D("MENSAJE"))
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
            Return Cs_Resultado
        End Function
        Public Function RecuperaDetenciones(ByVal detencion As Cs_Detencion, Optional ByVal dependencia As String = "") As List(Of Cs_Detencion)
            Dim L_Cs_Detencion As New List(Of Cs_Detencion)
            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_SPM_01_RECUPERADETENCION", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@SDE_NOMBRE", detencion.SDE_NOMBRE)
                        cmd.Parameters.AddWithValue("@SDE_APELLIDOS", detencion.SDE_APELLIDOS)
                        cmd.Parameters.AddWithValue("@SDE_ALIAS", detencion.SDE_ALIAS)
                        cmd.Parameters.AddWithValue("@USU_ID_PERFIL", detencion.USU_ID_PERFIL)
                        cmd.Parameters.AddWithValue("@DEPENDENCIA", dependencia)
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Dim c As New Cs_Detencion
                            c.SDE_CLAVE = If(IsDBNull(D("SDE_CLAVE")), New Integer, D("SDE_CLAVE"))
                            c.SDE_NOMBRE = If(IsDBNull(D("SDE_NOMBRE")), String.Empty, D("SDE_NOMBRE"))
                            c.SDE_APELLIDOS = If(IsDBNull(D("SDE_APELLIDOS")), String.Empty, D("SDE_APELLIDOS"))
                            c.SDE_ALIAS = If(IsDBNull(D("SDE_ALIAS")), String.Empty, D("SDE_ALIAS"))
                            c.SDE_LUG_DETENCION = If(IsDBNull(D("SDE_LUG_DETENCION")), String.Empty, D("SDE_LUG_DETENCION"))
                            c.SDE_DESTINO = If(IsDBNull(D("SDE_DESTINO")), String.Empty, D("SDE_DESTINO"))
                            c.SDE_LONGITUD = If(IsDBNull(D("SDE_LONGITUD")), String.Empty, D("SDE_LONGITUD"))
                            c.SDE_LATITUD = If(IsDBNull(D("SDE_LATITUD")), String.Empty, D("SDE_LATITUD"))
                            c.SDE_FOTO = If(IsDBNull(D("SDE_FOTO")), String.Empty, D("SDE_FOTO"))
                            c.SDE_FECHA_DETENCION = If(IsDBNull(D("SDE_FECHA_DETENCION")), New DateTime, D("SDE_FECHA_DETENCION"))
                            c.SDE_FECHA_CAP = If(IsDBNull(D("SDE_FECHA_CAP")), New DateTime, D("SDE_FECHA_CAP"))
                            c.SDE_ESTATUS = If(IsDBNull(D("SDE_ESTATUS")), String.Empty, D("SDE_ESTATUS"))
                            c.SDE_ACTUALIZA_PRINCIPAL = If(IsDBNull(D("SDE_ACTUALIZA_PRINCIPAL")), New Boolean, D("SDE_ACTUALIZA_PRINCIPAL"))
                            c.SDE_ACTUALIZA_SP = If(IsDBNull(D("SDE_ACTUALIZA_SP")), New Boolean, D("SDE_ACTUALIZA_SP"))
                            c.SDE_ACTUALIZA_SM = If(IsDBNull(D("SDE_ACTUALIZA_SM")), New Boolean, D("SDE_ACTUALIZA_SM"))
                            c.SUS_CLAVE = If(IsDBNull(D("SUS_CLAVE")), New Integer, D("SUS_CLAVE"))
                            c.SPA_CLAVE = If(IsDBNull(D("SPA_CLAVE")), New Integer, D("SPA_CLAVE"))
                            c.SDE_MOTIVO = If(IsDBNull(D("SDE_MOTIVO")), String.Empty, D("SDE_MOTIVO"))
                            L_Cs_Detencion.Add(c)
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
            Return L_Cs_Detencion
        End Function
    End Class
End Namespace