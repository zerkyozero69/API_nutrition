
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
    Public Class LOGIN_Controller
        Inherits ApiController
        Dim strConn As String = ConfigurationManager.ConnectionStrings("APIConnection").ConnectionString
        <Route("Login")>
        <AcceptVerbs("P           OST")>
        Public Function Login() As HttpResponseMessage

            Try
                'ตรวจสอบ FormData
                Dim User_name As String = "" 'username

                Dim password As String = "" 'รหัสผ่าน

                If Current.Request.Form.AllKeys.Contains("UserName") = True Then
                    User_name = Current.Request.Form("UserName").ToString()
                End If

                If Current.Request.Form.AllKeys.Contains("password") = True Then
                    password = Current.Request.Form("password").ToString()
                End If

                If User_name <> "" And password <> "" Then
                    Dim ds As New DataSet
                    Dim sql As String
                    sql = "SELECT User_Name, User_id FROM User_Register WHERE User_Name = @User_Name" 'ทำการ select sql จาก username 
                    ds = SqlHelper.ExecuteDataset(strConn, CommandType.Text, sql, New SqlParameter("@User_Name", User_name)) 'ทำการเอ็กซีคิว แล้วเทียบพารามิตเตอร์ username 
                    If ds.Tables(0).Rows.Count < 0 Then
                        Dim result_user As New _member 'ประกาศผลเป็น json 
                        result_user.User_name = User_name
                        result_user.Token_key =
                        result_user.Created_date =
                        result_user.message = "เข้าระบบสำเร็จ"
                        Return Request.CreateResponse(HttpStatusCode.OK, result_user)
                    Else
                        Dim err As [Error]
                        err.code = "1" 'กรอกข้อมูลซ้ำ
                        err.message = "ชื่อผู้ใช้งานผิด "
                        Return Request.CreateResponse(HttpStatusCode.BadRequest, err)
                        Dim user_info As New [RegisterStatus]
                    End If

                End If






                    'error case เกิดข้อผิดพลาด
                'Dim err As New [Error]
                'err.code = "5" 'กรอกข้อมูลการสมัครไม่ครบ
                'err.message = "กรุณาใส่ข้อมูลชื่อผู้ใช้งานและรหัสผ่านให้ครบถ้วนถูกต้อง, ห้ามเว้นว่างไว้"
                ''Return resual
                'Return Request.CreateResponse(HttpStatusCode.BadRequest, err)


            Catch ex As Exception
                'error case เกิดข้อผิดพลาด
                Dim err As New [Error]
                err.code = "6" 'error จากสาเหตุอื่นๆ จะมีรายละเอียดจาก system แจ้งกลับ
                err.message = ex.Message
                'Return resual
                Return Request.CreateResponse(HttpStatusCode.BadRequest, err)
            End Try

        End Function
    End Class
End Namespace