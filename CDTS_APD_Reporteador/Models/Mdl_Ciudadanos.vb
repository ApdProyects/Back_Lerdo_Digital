Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web.Mvc

Namespace Models
    Public Class Mdl_Ciudadanos
        Inherits Controller

        Dim Cs_ConexionDB As Cs_ConexionBD()
        Dim ConnString As String
        Public Function Recupera_Ciudadanos(ByVal CURP As String, ByVal usuario As String, ByVal password As String) As List(Of Cs_Ciudadano_AC)
            Dim L_Cs_Ciudadano As New List(Of Cs_Ciudadano_AC)
            Dim Cs_Ciudadano As New Cs_Ciudadano_AC
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("PA_X_WEB_RECUPERACIUDADANOS_AC", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@CURP", CURP)
                        cmd.Parameters.AddWithValue("@usuario", usuario.ToString())
                        cmd.Parameters.AddWithValue("@password", password)

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            Dim c As New Cs_Ciudadano_AC

                            c.ACI_CLAVE = If(IsDBNull(d("ACI_CLAVE")), New Integer, d("ACI_CLAVE"))
                            c.ACI_FECALTA = If(IsDBNull(d("ACI_FECALTA")), New Date, d("ACI_FECALTA"))
                            c.ACI_NOMBRE = If(IsDBNull(d("ACI_NOMBRE")), String.Empty, d("ACI_NOMBRE"))
                            c.ACI_APELLIDO_PAT = If(IsDBNull(d("ACI_APELLIDO_PAT")), String.Empty, d("ACI_APELLIDO_PAT"))
                            c.ACI_APELLIDO_MAT = If(IsDBNull(d("ACI_APELLIDO_MAT")), String.Empty, d("ACI_APELLIDO_MAT"))
                            c.ACI_NOMBRE_COMPLETO = If(IsDBNull(d("ACI_NOMBRE_COMPLETO")), String.Empty, d("ACI_NOMBRE_COMPLETO"))
                            c.ACI_FECNAC = If(IsDBNull(d("ACI_FECNAC")), New Date, d("ACI_FECNAC"))
                            c.ACI_SEXO = If(IsDBNull(d("ACI_SEXO")), String.Empty, d("ACI_SEXO"))
                            c.ACI_DIRECCION = If(IsDBNull(d("ACI_DIRECCION")), String.Empty, d("ACI_DIRECCION"))
                            c.CCN_CLAVE = If(IsDBNull(d("CCN_CLAVE")), New Integer, d("CCN_CLAVE"))
                            c.CMP_CLAVE = If(IsDBNull(d("CMP_CLAVE")), New Integer, d("CMP_CLAVE"))
                            c.CES_CLAVE = If(IsDBNull(d("CES_CLAVE")), New Integer, d("CES_CLAVE"))
                            c.CPA_CLAVE = If(IsDBNull(d("CPA_CLAVE")), New Integer, d("CPA_CLAVE"))
                            c.ACI_CP = If(IsDBNull(d("ACI_CP")), String.Empty, d("ACI_CP"))
                            c.ACI_TELEFONO = If(IsDBNull(d("ACI_TELEFONO")), String.Empty, d("ACI_TELEFONO"))
                            c.ACI_CELULAR = If(IsDBNull(d("ACI_CELULAR")), String.Empty, d("ACI_CELULAR"))
                            c.ACI_CLAVE_ELECTOR = If(IsDBNull(d("ACI_CLAVE_ELECTOR")), String.Empty, d("ACI_CLAVE_ELECTOR"))
                            c.ACI_CURP = If(IsDBNull(d("ACI_CURP")), String.Empty, d("ACI_CURP"))
                            c.ACI_YEAR_REGISTRO = If(IsDBNull(d("ACI_YEAR_REGISTRO")), String.Empty, d("ACI_YEAR_REGISTRO"))
                            c.ACI_YEAR_REGISTRO_MES = If(IsDBNull(d("ACI_YEAR_REGISTRO_MES")), String.Empty, d("ACI_YEAR_REGISTRO_MES"))
                            c.ACI_ESTADO = If(IsDBNull(d("ACI_ESTADO")), String.Empty, d("ACI_ESTADO"))
                            c.ACI_MUNICIPIO = If(IsDBNull(d("ACI_MUNICIPIO")), String.Empty, d("ACI_MUNICIPIO"))
                            c.ACI_SECCION = If(IsDBNull(d("ACI_SECCION")), String.Empty, d("ACI_SECCION"))
                            c.ACI_LOCALIDAD = If(IsDBNull(d("ACI_LOCALIDAD")), String.Empty, d("ACI_LOCALIDAD"))
                            c.ACI_EMISION = If(IsDBNull(d("ACI_EMISION")), String.Empty, d("ACI_EMISION"))
                            c.ACI_VIGENCIA = If(IsDBNull(d("ACI_VIGENCIA")), String.Empty, d("ACI_VIGENCIA"))
                            c.ACI_NUMERO_CIC = If(IsDBNull(d("ACI_NUMERO_CIC")), String.Empty, d("ACI_NUMERO_CIC"))
                            c.ACI_NUMERO_OCR = If(IsDBNull(d("ACI_NUMERO_OCR")), String.Empty, d("ACI_NUMERO_OCR"))
                            c.ACI_UBICACION_X = If(IsDBNull(d("ACI_UBICACION_X")), String.Empty, d("ACI_UBICACION_X"))
                            c.ACI_UBICACION_Y = If(IsDBNull(d("ACI_UBICACION_Y")), String.Empty, d("ACI_UBICACION_Y"))
                            c.ACI_OBSERVACIONES = If(IsDBNull(d("ACI_OBSERVACIONES")), String.Empty, d("ACI_OBSERVACIONES"))
                            c.CEM_CLAVE = If(IsDBNull(d("CEM_CLAVE")), New Integer, d("CEM_CLAVE"))
                            c.CET_NOMBRE = If(IsDBNull(d("CET_NOMBRE")), String.Empty, d("CET_NOMBRE"))
                            c.ACI_MOSTRAR = If(IsDBNull(d("ACI_MOSTRAR")), String.Empty, d("ACI_MOSTRAR"))
                            c.ACI_FUM = If(IsDBNull(d("ACI_FUM")), New Date, d("ACI_FUM"))
                            c.ACI_SR = If(IsDBNull(d("ACI_SR")), New Boolean, d("ACI_SR"))
                            c.ACI_ENVIAR = If(IsDBNull(d("ACI_ENVIAR")), String.Empty, d("ACI_ENVIAR"))
                            c.ACI_VERIFICADO = If(IsDBNull(d("ACI_VERIFICADO")), String.Empty, d("ACI_VERIFICADO"))
                            c.ACI_PASSWORD = If(IsDBNull(d("ACI_PASSWORD")), String.Empty, d("ACI_PASSWORD"))

                            L_Cs_Ciudadano.Add(c)
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

            Return L_Cs_Ciudadano
        End Function
        Public Function verifica_qr(ByVal iin_clave As String) As Cs_Datos_QR
            Dim QR_Ciudadano As New Cs_Datos_QR
            Dim dt As DataTable = New DataTable()

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand("[SRV_VIALIDAD].[APDSGEDB_PL].[DBO].PA_X_WEB_VALIDA_QR", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@IIN_CLAVE", iin_clave)



                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            ' Dim c As New Cs_Datos_QR

                            QR_Ciudadano.QR_CONCEPTO = If(IsDBNull(d("CONCEPTO")), New Integer, d("CONCEPTO"))
                            QR_Ciudadano.QR_FECHA_COBRO = If(IsDBNull(d("FECHA_COBRO")), New Date, d("FECHA_COBRO"))
                            QR_Ciudadano.QR_ESTATUS = If(IsDBNull(d("ESTATUS")), String.Empty, d("ESTATUS"))
                            QR_Ciudadano.QR_FOLIO = If(IsDBNull(d("FOLIO")), String.Empty, d("FOLIO"))
                            QR_Ciudadano.QR_IMAGEN = If(IsDBNull(d("IMAGEN")), String.Empty, d("IMAGEN"))
                            QR_Ciudadano.QR_NOMBRE = If(IsDBNull(d("NOMBRE")), String.Empty, d("NOMBRE"))
                            QR_Ciudadano.VQR_TIPO = If(IsDBNull(d("VQR_TIPO")), String.Empty, d("VQR_TIPO"))
                            QR_Ciudadano.QR_RECIBO = If(IsDBNull(d("RECIBO")), String.Empty, d("RECIBO"))


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

            Return QR_Ciudadano
        End Function
        Public Function Mod_Pass_Ciudadanos(ByVal CURP As String, ByVal password As String) As Cs_Respuesta
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
        Function Ejecuta_sql_select(ByVal SentSQL As String) As ArrayList
            Dim dt As DataTable = New DataTable()
            Dim array As New ArrayList

            Try
                ConnString = Cs_ConexionBD.Connect()

                Using conn As New SqlConnection(ConnString)
                    Try
                        Dim cmd As New SqlCommand(SentSQL, conn)
                        cmd.CommandType = CommandType.Text

                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)

                        For Each d In dt.Rows
                            array.Add(d.itemArray)
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