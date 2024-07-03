Public Class Datos_fiscales
    Public Property NOMBRE_FISCAL
    Public Property CP_FISCAL
    Public Property REGIMEN_FISCAL
    Public Property DIRECCION_FISCAL
    Public Property Mensaje
End Class

Public Class Recupera_Forma_Pago
    Public Property FFP_CLAVE_NOMBRE
    Public Property FFP_DESCRIPCION
End Class

Public Class Lista_Forma_Pago
    Public Property ListaFormaPago As List(Of Recupera_Forma_Pago)

End Class
