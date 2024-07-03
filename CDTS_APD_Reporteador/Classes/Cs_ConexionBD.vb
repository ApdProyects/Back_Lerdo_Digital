Public Class Cs_ConexionBD
    Public Shared Function Connect() As String
        Dim NombreApp As String = ""
        '============ SERIVIDOR PRODUCTIVO ================================  

        Dim Servidor As String = "SRV_VIALIDAD"    ''  server real ip vialidad
        Dim Usuario As String = "apd_moviles"      ''  SERVIDOR REAL
        Dim Password As String = "azpku03j7m3n"    ''  SERVIDOR REAL
        Dim BaseDatos As String = "APDSGEDB_PL"    ''  SERVIDOR REAL

        'Dim Servidor As String = "10.10.20.75" '"25.66.1.18"
        'Dim Usuario As String = "apd_multipagos"
        'Dim Password As String = "853x2kert3cwp6cvh"
        'Dim BaseDatos As String = "APDSGEDB_PL"
        Dim TiempoEspera As String = "5000"
        Dim Puerto As String = "1433"
        '================================================================== 
        Dim Cadena As String = String.Format("Data source = {0}; initial catalog = {1}; user id = {2}; password = {3};Connection Timeout={4};Connect Timeout={5}",
                                             Servidor, BaseDatos, Usuario, Password, TiempoEspera, TiempoEspera)
        Return Cadena
    End Function
    Public Shared Function myConnect() As String
        Dim NombreApp As String = ""
        '============ SERIVIDOR PRODUCTIVO ================================  

        Dim Servidor As String = "SRV_VIALIDAD" '' server real ' ip vialidad

        'Dim Servidor As String = "25.68.13.14"
        'Dim Servidor As String = "25.68.13.14"
        Dim Usuario As String = "apd_moviles"
        Dim Password As String = "azpku03j7m3n"
        Dim BaseDatos As String = "APDSGEDB_PL"
        Dim TiempoEspera As String = "5000"
        Dim Puerto As String = "3306"
        Dim Cadena As String = String.Format("Data source = {0}; initial catalog = {1}; user id = {2}; password = {3};Connection Timeout={4};Connect Timeout={5}",
                                             Servidor, BaseDatos, Usuario, Password, TiempoEspera, TiempoEspera)
        Return Cadena
    End Function

    Public Shared Function CONECT_REPLICAS() As String
        Dim NombreApp As String = ""
        '============ SERIVIDOR PRODUCTIVO ================================  

        Dim Servidor As String = "SRV_MULTIPAGOS" '' server real
        'Dim Servidor As String = "25.68.13.14"
        'Dim Servidor As String = "25.68.13.14"
        Dim Usuario As String = "apd_multipagos"
        Dim Password As String = "853x2kert3cwp6cvh"
        Dim BaseDatos As String = "APDSGEDB_PL"
        Dim TiempoEspera As String = "5000"
        Dim Puerto As String = "3306"
        Dim Cadena As String = String.Format("Data source = {0}; initial catalog = {1}; user id = {2}; password = {3};Connection Timeout={4};Connect Timeout={5}",
                                             Servidor, BaseDatos, Usuario, Password, TiempoEspera, TiempoEspera)
        Return Cadena
    End Function


    Public Shared Function Connect_local() As String
        Dim NombreApp As String = ""
        Dim clave = ConfigurationManager.AppSettings("IPPublica")
        '============ SERIVIDOR PRODUCTIVO ================================   
        Dim Servidor As String = "APD_PGM_06" 'clave
        'Dim Servidor As String = "SRV_JN"
        Dim Usuario As String = "apduser"
        Dim Password As String = "apdsql"
        'Dim BaseDatos As String = "APDSGEDB_0000"
        Dim BaseDatos As String = "APDSGEDB_0000"

        Dim TiempoEspera As String = "5000"
        Dim Puerto As String = "1433"
        '================================================================== 
        Dim Cadena As String = String.Format("Data source = {0}; initial catalog = {1}; user id = {2}; password = {3};Connection Timeout={4};Connect Timeout={5}",
                                             Servidor, BaseDatos, Usuario, Password, TiempoEspera, TiempoEspera)
        Return Cadena
    End Function

End Class
