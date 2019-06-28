Imports System.Net
Imports System.Web.Http
Imports API_nutrition_vb.demo

Namespace Controllers

    Public Class demoController
        Inherits ApiController
        <Route("register/user")>
        <AcceptVerbs("POST")>
        Public Function textReturn() As register
            Dim User As String = HttpContext.Current.Request.Form("user")
            Dim userID As register = New register()
            userID.User = "OK"
            userID.Password = "Hi"
            Return userID
        End Function
    End Class
End Namespace