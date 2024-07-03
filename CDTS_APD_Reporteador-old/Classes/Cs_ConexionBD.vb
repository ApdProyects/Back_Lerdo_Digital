Public Class Cs_ConexionBD
    Public Shared Function Connect() As String
        Dim NombreApp As String = ""
        '============ SERIVIDOR PRODUCTIVO ================================  
        'Dim Servidor As String = "25.66.1.18"
        'Dim Usuario As String = "apduser"
        'Dim Password As String = "apdsql"
        Dim Servidor As String = "25.96.153.115"
        Dim Usuario As String = "sa"
        Dim Password As String = "apdsql"
        Dim BaseDatos As String = "APDSGEDB_PL"
        Dim TiempoEspera As String = "500"
        Dim Puerto As String = "1433"
        '================================================================== 
        Dim Cadena As String = String.Format("Data source = {0}; initial catalog = {1}; user id = {2}; password = {3}",
                                             Servidor, BaseDatos, Usuario, Password)
        Return Cadena
    End Function
End Class
