Public Class Cs_Respuesta_pago

    Public Property codigo As Integer = 0
    Public Property codigoError As Integer = 0
    Public Property mensaje As String = ""
    Public Property folio As String = ""
    Public Property VQR_CLAVE As String = "1"
    Public Property Cs_InfoFolio As ArrayList
    ''CAMBIOS PARA VERIFICAR LOS PAGOS ENLAZADOS AL CORRALON
    Public Property CORRALON As String = ""
End Class
