
Imports System.Data.SqlClient.SqlDataReader
Imports System.IO
Imports System.Net
Imports System.Uri
Imports System.Net.Http
Imports System.Web.HttpContext
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports System.Xml
Imports Newtonsoft
Imports System.Data
Imports System.Configuration
Imports Microsoft.ApplicationBlocks.Data
Imports API_nutrition_vb.Member
Imports API_nutrition_vb.RegisterStatus
Imports API_nutrition_vb.nutrirtion
Imports NTi.CommonUtility
Imports Newtonsoft.Json
Imports System.Data.SqlClient

Namespace Controllers
    Public Class MemberController

        Inherits ApiController
        Dim strConn As String = ConfigurationManager.ConnectionStrings("APIConnection").ConnectionString
        <Route("register")>
        <AcceptVerbs("POST")>
        Public Function Accountregister() As HttpResponseMessage
            Try


                'Dim user_name As String = GetForm(HttpContext.Current, "User_Name")
                'Dim password As String = GetForm(HttpContext.Current, "User_Password")
                'Dim GU_ID As String = GetForm(HttpContext.Current, "GU_ID")
                'Dim Created_by As String = GetForm(HttpContext.Current, "Created_by")
                Dim User_name As String = ""
                Dim User_Password As String = ""
                'Dim token As String = ""
                'Dim GU_ID As String = Nothing
                Dim Created_by As String = ""

                If Current.Request.Form.AllKeys.Contains("User_Name") = True Then
                    User_name = Current.Request.Form("User_Name").ToString()
                End If
                If Current.Request.Form.AllKeys.Contains("User_Password") = True Then
                    User_Password = Current.Request.Form("User_Password").ToString()
                End If
                'If Current.Request.Form.AllKeys.Contains("GU_ID") = True Then
                '    GU_ID = Current.Request.Form("GU_ID").ToString()
                'End If
                If Current.Request.Form.AllKeys.Contains("Created_by") = True Then
                    Created_by = Current.Request.Form("Created_by").ToString()
                End If


                'ตรวจสอบว่าส่งพารามิเตอร์ครบไหม              
                Dim ds As New DataSet
                Dim sql As String
                sql = "SELECT User_Name, User_id FROM User_Register WHERE User_Name = @User_Name"
                ds = SqlHelper.ExecuteDataset(strConn, CommandType.Text, sql, New SqlParameter("@User_Name", User_name))

                If ds.Tables(0).Rows.Count > 0 Then
                    Dim err As [Error]
                    err.code = "1" 'กรอกข้อมูลซ้ำ
                    err.message = "ชื่อผู้ใช้งานมีในระบบอยู่แล้ว "
                    Return Request.CreateResponse(HttpStatusCode.BadRequest, err)
                Else

                    SqlHelper.ExecuteNonQuery(strConn, CommandType.StoredProcedure, "spT_User_PersonInsert",
                                                                      New SqlParameter("@User_Name", User_name),
                                                                      New SqlParameter("@User_Password ", User_Password),
                                                                     New SqlParameter("@Created_by ", Created_by))
                    'Return result ลงทะเบียนสำเร็จ
                    Dim dt As DataSet
                    Dim sqlG As String
                    sqlG = "SELECT User_Name,User_Password, Token_key,Created_date FROM User_Register   ORDER BY User_ID desc"
                    ds = SqlHelper.ExecuteDataset(strConn, CommandType.Text, sqlG)
                    Dim dt2 As DataTable
                    dt2 = ds.Tables(0)
                    Dim result_user As New _member
                    result_user.User_name = dt2.Rows(0).Item("User_Name")
                    result_user.Token_key = dt2.Rows(0).Item("Token_key")
                    result_user.Created_date = dt2.Rows(0).Item("Created_date")
                    result_user.message = "ลงทะเบียนสำเร็จ"
                    Return Request.CreateResponse(HttpStatusCode.OK, result_user)
                End If



            Catch ex As Exception
                'error case เกิดข้อผิดพลาด
                Dim err As New [Error]
                err.code = "2" 'username ซ้ำ
                err.message = "Username ซ้ำ"
                'Return resual
                Return Request.CreateResponse(HttpStatusCode.BadRequest, err)

            End Try
        End Function
        'Public Function getjson() As HttpRequestMessage
        '    If Current.Request Then

        'End Function

    End Class


End Namespace