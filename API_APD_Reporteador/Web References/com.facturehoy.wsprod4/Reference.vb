﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Este código fue generado por una herramienta.
'     Versión de runtime:4.0.30319.42000
'
'     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
'     se vuelve a generar el código.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
'
Namespace com.facturehoy.wsprod4
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="WsEmisionTimbrado40PortBinding", [Namespace]:="http://cfdi.ws4.facturehoy.certus.com/")>  _
    Partial Public Class WsEmisionTimbrado40
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private EmitirTimbrarOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.API_APD_Reporteador.My.MySettings.Default.API_APD_Reporteador_com_facturehoy_wsprod4_WsEmisionTimbrado40
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event EmitirTimbrarCompleted As EmitirTimbrarCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace:="http://cfdi.ws4.facturehoy.certus.com/", ResponseNamespace:="http://cfdi.ws4.facturehoy.certus.com/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function EmitirTimbrar(<System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> ByVal usuario As String, <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> ByVal contrasenia As String, <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> ByVal idServicio As Integer, <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType:="base64Binary", IsNullable:=true)> ByVal xml() As Byte) As <System.Xml.Serialization.XmlElementAttribute("return", Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> wsResponseBO
            Dim results() As Object = Me.Invoke("EmitirTimbrar", New Object() {usuario, contrasenia, idServicio, xml})
            Return CType(results(0),wsResponseBO)
        End Function
        
        '''<remarks/>
        Public Overloads Sub EmitirTimbrarAsync(ByVal usuario As String, ByVal contrasenia As String, ByVal idServicio As Integer, ByVal xml() As Byte)
            Me.EmitirTimbrarAsync(usuario, contrasenia, idServicio, xml, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub EmitirTimbrarAsync(ByVal usuario As String, ByVal contrasenia As String, ByVal idServicio As Integer, ByVal xml() As Byte, ByVal userState As Object)
            If (Me.EmitirTimbrarOperationCompleted Is Nothing) Then
                Me.EmitirTimbrarOperationCompleted = AddressOf Me.OnEmitirTimbrarOperationCompleted
            End If
            Me.InvokeAsync("EmitirTimbrar", New Object() {usuario, contrasenia, idServicio, xml}, Me.EmitirTimbrarOperationCompleted, userState)
        End Sub
        
        Private Sub OnEmitirTimbrarOperationCompleted(ByVal arg As Object)
            If (Not (Me.EmitirTimbrarCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent EmitirTimbrarCompleted(Me, New EmitirTimbrarCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://cfdi.ws4.facturehoy.certus.com/")>  _
    Partial Public Class wsResponseBO
        
        Private acuseField() As Byte
        
        Private arregloAcuseField()() As Byte
        
        Private cadenaOriginalField As String
        
        Private cadenaOriginalTimbreField As String
        
        Private codigoErrorField As Integer
        
        Private fechaHoraTimbradoField As Date
        
        Private fechaHoraTimbradoFieldSpecified As Boolean
        
        Private folioUDDIField As String
        
        Private isErrorField As Boolean
        
        Private isErrorFieldSpecified As Boolean
        
        Private messageField As String
        
        Private pDFField() As Byte
        
        Private rutaDescargaPDFField As String
        
        Private rutaDescargaXMLField As String
        
        Private selloDigitalEmisorField As String
        
        Private selloDigitalTimbreSATField As String
        
        Private xMLField() As Byte
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType:="base64Binary")>  _
        Public Property acuse() As Byte()
            Get
                Return Me.acuseField
            End Get
            Set
                Me.acuseField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("arregloAcuse", Form:=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType:="base64Binary", IsNullable:=true)>  _
        Public Property arregloAcuse() As Byte()()
            Get
                Return Me.arregloAcuseField
            End Get
            Set
                Me.arregloAcuseField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property cadenaOriginal() As String
            Get
                Return Me.cadenaOriginalField
            End Get
            Set
                Me.cadenaOriginalField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property cadenaOriginalTimbre() As String
            Get
                Return Me.cadenaOriginalTimbreField
            End Get
            Set
                Me.cadenaOriginalTimbreField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property codigoError() As Integer
            Get
                Return Me.codigoErrorField
            End Get
            Set
                Me.codigoErrorField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property fechaHoraTimbrado() As Date
            Get
                Return Me.fechaHoraTimbradoField
            End Get
            Set
                Me.fechaHoraTimbradoField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property fechaHoraTimbradoSpecified() As Boolean
            Get
                Return Me.fechaHoraTimbradoFieldSpecified
            End Get
            Set
                Me.fechaHoraTimbradoFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property folioUDDI() As String
            Get
                Return Me.folioUDDIField
            End Get
            Set
                Me.folioUDDIField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property isError() As Boolean
            Get
                Return Me.isErrorField
            End Get
            Set
                Me.isErrorField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property isErrorSpecified() As Boolean
            Get
                Return Me.isErrorFieldSpecified
            End Get
            Set
                Me.isErrorFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property message() As String
            Get
                Return Me.messageField
            End Get
            Set
                Me.messageField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType:="base64Binary")>  _
        Public Property PDF() As Byte()
            Get
                Return Me.pDFField
            End Get
            Set
                Me.pDFField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property rutaDescargaPDF() As String
            Get
                Return Me.rutaDescargaPDFField
            End Get
            Set
                Me.rutaDescargaPDFField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property rutaDescargaXML() As String
            Get
                Return Me.rutaDescargaXMLField
            End Get
            Set
                Me.rutaDescargaXMLField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property selloDigitalEmisor() As String
            Get
                Return Me.selloDigitalEmisorField
            End Get
            Set
                Me.selloDigitalEmisorField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
        Public Property selloDigitalTimbreSAT() As String
            Get
                Return Me.selloDigitalTimbreSATField
            End Get
            Set
                Me.selloDigitalTimbreSATField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType:="base64Binary")>  _
        Public Property XML() As Byte()
            Get
                Return Me.xMLField
            End Get
            Set
                Me.xMLField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")>  _
    Public Delegate Sub EmitirTimbrarCompletedEventHandler(ByVal sender As Object, ByVal e As EmitirTimbrarCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class EmitirTimbrarCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As wsResponseBO
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),wsResponseBO)
            End Get
        End Property
    End Class
End Namespace