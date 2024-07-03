Imports System.Web.Mvc

Public Class Cs_Respuesta
    Public Property codigo As Integer = 0
    Public Property codigoError As Integer = 0
    Public Property mensaje As String = ""
    Public Property objetoError
    Public Property internalMessage As String
    Public Property internalCode As String


    Public Shared Widening Operator CType(v As Cs_Respuesta) As ActionResult
        Throw New NotImplementedException()
    End Operator
End Class

Public Class Cs_Respuesta_Folios
    Public Property codigo As Integer = 0
    Public Property codigoError As Integer = 0
    Public Property mensaje As String = ""
    Public Property mensajeList As List(Of String)
    Public Property objetoError
    Public Property internalMessage As String
    Public Property internalCode As String


    Public Shared Widening Operator CType(v As Cs_Respuesta_Folios) As ActionResult
        Throw New NotImplementedException()
    End Operator
End Class

