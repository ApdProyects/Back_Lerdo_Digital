Public Class Cs_Detencion
    Inherits Cs_Dispositivo
    Public Property SDE_CLAVE As Integer = 0
    Public Property SDE_NOMBRE As String = ""
    Public Property SDE_APELLIDOS As String = ""
    Public Property SDE_ALIAS As String = ""
    Public Property SDE_LUG_DETENCION As String = ""
    Public Property SDE_DESTINO As String = ""
    Public Property SDE_LONGITUD As String = ""
    Public Property SDE_LATITUD As String = ""
    Public Property SDE_FOTO As String = ""
    Public Property SDE_FECHA_DETENCION As DateTime
    Public Property SDE_FECHA_CAP As DateTime
    Public Property SDE_ESTATUS As String = ""
    Public Property SDE_ACTUALIZA_PRINCIPAL As Boolean
    Public Property SDE_ACTUALIZA_SP As Boolean
    Public Property SDE_ACTUALIZA_SM As Boolean
    Public Property SUS_CLAVE As Integer = 0
    Public Property SPA_CLAVE As Integer = 0
    Public Property SDE_MOTIVO As String = ""
    Public Property USU_ID_PERFIL As Integer = 0
End Class
