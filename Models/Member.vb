Imports System
Imports System.DateTime

Public Class Member
    Public Structure _member
        'Public id As String
        Public Property User_name As String
        ' Public Property User_Password As String
        Public Property Token_key As String
        'Public Property token As String
        'Public Property Created_by As String
        Public Property Created_date As DateTime
        Public Property message As String

    End Structure
    Public Class [LoginStatus]
        Public Property statusLogin As Integer
        Public Property message As String
    End Class

    Public Class [RegisterStatus]
        Public Property id As Integer
        Public Property status As Boolean
        Public Property token As String
        Public Property user_type As String
    End Class

    Public Class [VerifyStatus]
        Public Property status As Boolean
        Public Property is_register As Boolean
    End Class
End Class
