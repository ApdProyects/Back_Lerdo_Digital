Public Class ModuloGeneral

    ' Variables compañia
    Public Property EmisorNombre
    Public Property EmisorDomicilioCalle
    Public Property EmisorDomicilioColonia
    Public Property EmisorDomicilioMunicipio
    Public Property EmisorDomicilioEstado
    Public Property EmisorDomicilioCp
    Public Property EmisorRfc
    Public Property EmisorPais

    ' Variables cliente
    Public Property ReceptorNombre
    Public Property ReceptorDomicilioCalle
    Public Property ReceptorDomicilioColonia
    Public Property ReceptorDomicilioMunicipio
    Public Property ReceptorDomicilioEstado
    Public Property ReceptorDomicilioCp
    Public Property ReceptorRfc
    Public Property ReceptorPais
    Public Property ReceptorMail
    Public Property ReceptorTelefono


    'Variables producto
    Public Property productos As List(Of Venta_Det)
    Public Property SumatoriaDeIvaEnConceptos

    'Variables fiscales
    Public Property CertificadoSelloDigital
    Public Property LlavePrivada
    Public Property ContraseñaLlavePrivada
    Public Property CarpetaAlmacenXML
    Public Property UsuarioTimbrado
    Public Property ContraseñaTimbrado
    Public Property RegimenFiscal
    Public Property LugarDeExpedicion
    Public Property VersionFE
    Public Property CertificadoPFX
    Public Property ContaseñaCertificadoPFX
    Public Property Timbrador
    Public Property pac_id
    Public Property TipoDeComprobante
    Public Property FE_LUGARDEEXPEDICION

    'Variables prefijo
    Public Property SeriePrefijo

    'Variable mensaje
    Public Property mensaje

    Public Property ReceptorRegimenFiscal
    Public Property ReceptorRegimenFiscal_descripcion
    Public Property EmisorEXPORTACION
End Class
