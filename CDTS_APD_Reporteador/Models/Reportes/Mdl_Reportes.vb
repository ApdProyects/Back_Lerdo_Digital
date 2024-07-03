Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models.Reportes
    Public Class Mdl_Reportes
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function RecuperaReporteSexo() As Cs_RptSexo
            Dim Cs_RptSexo As New Cs_RptSexo
            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORSEXO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptSexo.FEMENINO = If(IsDBNull(D("FEMENINO")), New Integer, D("FEMENINO"))
                            Cs_RptSexo.MASCULINO = If(IsDBNull(D("MASCULINO")), New Integer, D("MASCULINO"))
                            Cs_RptSexo.TOTAL = If(IsDBNull(D("TOTAL")), New Integer, D("TOTAL"))
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
            Return Cs_RptSexo
        End Function
        Public Function RecuperaReporteEdad() As Cs_RptEdad
            Dim Cs_RptEdad As New Cs_RptEdad
            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPOREDAD", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptEdad.MENORES = If(IsDBNull(D("MENORES")), New Integer, D("MENORES"))
                            Cs_RptEdad.ADULTO = If(IsDBNull(D("ADULTO")), New Integer, D("ADULTO"))
                            Cs_RptEdad.ADULTO_MAYOR = If(IsDBNull(D("ADULTO_MAYORES")), New Integer, D("ADULTO_MAYORES"))
                            Cs_RptEdad.TOTAL = If(IsDBNull(D("TOTAL")), New Integer, D("TOTAL"))
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
            Return Cs_RptEdad
        End Function
        Public Function RecuperaReporteHorario() As Cs_RptHorario
            Dim Cs_RptHorario As New Cs_RptHorario
            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORHORARIO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptHorario.PRIMER_TURNO = If(IsDBNull(D("PRIMER_TURNO")), New Integer, D("PRIMER_TURNO"))
                            Cs_RptHorario.SEGUNDO_TURNO = If(IsDBNull(D("SEGUNDO_TURNO")), New Integer, D("SEGUNDO_TURNO"))
                            Cs_RptHorario.TERCER_TURNO = If(IsDBNull(D("TERCER_TURNO")), New Integer, D("TERCER_TURNO"))
                            Cs_RptHorario.TOTAL = If(IsDBNull(D("TOTAL")), New Integer, D("TOTAL"))
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
            Return Cs_RptHorario
        End Function
        Public Function RecuperaReporteMotivo() As List(Of Cs_RptMotivo)
            Dim L_Cs_RptMotivo As New List(Of Cs_RptMotivo)
            Dim Cs_RptMotivo As New Cs_RptMotivo

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORMOTIVO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptMotivo = New Cs_RptMotivo
                            Cs_RptMotivo.SDE_MOTIVO = If(IsDBNull(D("SDE_MOTIVO")), String.Empty, D("SDE_MOTIVO"))
                            Cs_RptMotivo.CANTIDAD = If(IsDBNull(D("CANTIDAD")), New Integer, D("CANTIDAD"))

                            L_Cs_RptMotivo.Add(Cs_RptMotivo)
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
            Return L_Cs_RptMotivo
        End Function
        Public Function RecuperaReportePago() As List(Of Cs_RptPago)
            Dim L_Cs_RptPago As New List(Of Cs_RptPago)
            Dim Cs_RptPago As New Cs_RptPago

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORPAGO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptPago = New Cs_RptPago
                            Cs_RptPago.SDE_MOTIVO_SALIDA = If(IsDBNull(D("SDE_MOTIVO_SALIDA")), String.Empty, D("SDE_MOTIVO_SALIDA"))
                            Cs_RptPago.CANTIDAD = If(IsDBNull(D("CANTIDAD")), New Integer, D("CANTIDAD"))

                            L_Cs_RptPago.Add(Cs_RptPago)
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
            Return L_Cs_RptPago
        End Function
        Public Function RecuperaReporteTiempos() As List(Of Cs_RptTiempos)
            Dim L_Cs_RptTiempos As New List(Of Cs_RptTiempos)
            Dim Cs_RptTiempos As New Cs_RptTiempos

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORTIEMPOS", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptTiempos = New Cs_RptTiempos
                            Cs_RptTiempos.SDE_NOMBRE = If(IsDBNull(D("SDE_NOMBRE")), String.Empty, D("SDE_NOMBRE"))
                            Cs_RptTiempos.SDE_APELLIDOS = If(IsDBNull(D("SDE_APELLIDOS")), String.Empty, D("SDE_APELLIDOS"))
                            Cs_RptTiempos.SDE_ALIAS = If(IsDBNull(D("SDE_ALIAS")), String.Empty, D("SDE_ALIAS"))
                            Cs_RptTiempos.SOS_EDAD = If(IsDBNull(D("SOS_EDAD")), New Integer, D("SOS_EDAD"))
                            Cs_RptTiempos.DIFERENCIA_SM = If(IsDBNull(D("DIFERENCIA_SM")), String.Empty, D("DIFERENCIA_SM"))
                            Cs_RptTiempos.DIFERENCIA_SP = If(IsDBNull(D("DIFERENCIA_SP")), String.Empty, D("DIFERENCIA_SP"))
                            Cs_RptTiempos.ULTIMO_ESTATUS = If(IsDBNull(D("ULTIMO ESTATUS")), String.Empty, D("ULTIMO ESTATUS"))
                            Cs_RptTiempos.PAGO = If(IsDBNull(D("PAGO")), String.Empty, D("PAGO"))

                            L_Cs_RptTiempos.Add(Cs_RptTiempos)
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
            Return L_Cs_RptTiempos
        End Function
        Public Function RecuperaReporteCobro() As List(Of Cs_RptCobro)
            Dim L_Cs_RptCobro As New List(Of Cs_RptCobro)
            Dim Cs_RptCobro As New Cs_RptCobro

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_SPM_REPORTEPORCOBRO", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptCobro = New Cs_RptCobro
                            Cs_RptCobro.CLAVE_DETENIDO = If(IsDBNull(D("CLAVE_DETENIDO")), New Integer, D("CLAVE_DETENIDO"))
                            Cs_RptCobro.NOMBRE_DETENIDO = If(IsDBNull(D("NOMBRE_DETENIDO")), String.Empty, D("NOMBRE_DETENIDO"))
                            Cs_RptCobro.APELLIDOS_DETENIDO = If(IsDBNull(D("APELLIDOS_DETENIDO")), String.Empty, D("APELLIDOS_DETENIDO"))
                            Cs_RptCobro.FECHA_DETENCION = If(IsDBNull(D("FECHA_DETENCION")), New DateTime, D("FECHA_DETENCION"))
                            Cs_RptCobro.FOLIO_REMISION = If(IsDBNull(D("FOLIO_REMISION")), New Integer, D("FOLIO_REMISION"))
                            Cs_RptCobro.JUEZ_COBRO = If(IsDBNull(D("JUEZ_COBRO")), String.Empty, D("JUEZ_COBRO"))
                            Cs_RptCobro.IMPORTE = If(IsDBNull(D("IMPORTE")), New Integer, D("IMPORTE"))

                            L_Cs_RptCobro.Add(Cs_RptCobro)
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
            Return L_Cs_RptCobro
        End Function
        Public Function RecuperaReporteMultasAgentes(ByVal FechaInicio As DateTime, ByVal FechaFin As DateTime) As List(Of Cs_RptVialidad)
            Dim L_Cs_RptVialidad As New List(Of Cs_RptVialidad)
            Dim Cs_RptVialidad As New Cs_RptVialidad

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_X_WEB_VIA_REPORTEMULTASPORAGENTE]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@FecInicial", FechaInicio)
                        cmd.Parameters.AddWithValue("@FecFinal", FechaFin)
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptVialidad = New Cs_RptVialidad
                            Cs_RptVialidad.VAG_NUM_AGENTE = If(IsDBNull(D("VAG_NUM_AGENTE")), New Integer, D("VAG_NUM_AGENTE"))
                            Cs_RptVialidad.VAG_SUMA = If(IsDBNull(D("VAG_SUMA")), String.Empty, D("VAG_SUMA"))

                            L_Cs_RptVialidad.Add(Cs_RptVialidad)
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
            Return L_Cs_RptVialidad
        End Function
        Public Function RecuperaReporteMultasDinero(ByVal FechaInicio As DateTime, ByVal FechaFin As DateTime) As List(Of Cs_RptVialidad)
            Dim L_Cs_RptVialidad As New List(Of Cs_RptVialidad)
            Dim Cs_RptVialidad As New Cs_RptVialidad

            Dim dt As DataTable = New DataTable()
            Try
                ConnString = Cs_ConexionBD.Connect()
                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[dbo].[PA_X_WEB_VIA_REPORTEMULTASPORDINERO]", conn)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@FecInicial", FechaInicio)
                        cmd.Parameters.AddWithValue("@FecFinal", FechaFin)
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        For Each D In dt.Rows
                            Cs_RptVialidad = New Cs_RptVialidad
                            Cs_RptVialidad.VAG_NUM_AGENTE = If(IsDBNull(D("VAG_NUM_AGENTE")), New Integer, D("VAG_NUM_AGENTE"))
                            Cs_RptVialidad.VAG_SUMA = If(IsDBNull(D("VAG_SUMA")), String.Empty, D("VAG_SUMA"))

                            L_Cs_RptVialidad.Add(Cs_RptVialidad)
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
            Return L_Cs_RptVialidad
        End Function
    End Class
End Namespace