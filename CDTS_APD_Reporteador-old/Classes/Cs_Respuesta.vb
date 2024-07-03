Imports System.Web.Mvc
Imports CpDts_APD_Atencion

Public Class Cs_Respuesta
    Public Property codigo As Integer = 0
    Public Property codigoError As Integer = 0
    Public Property mensaje As String = ""

    Public Shared Widening Operator CType(v As Cs_Respuesta) As ActionResult
        Throw New NotImplementedException()
    End Operator
End Class
