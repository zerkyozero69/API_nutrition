Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Xml
Imports Newtonsoft
Imports API_nutrition_vb.nutrirtion
Imports Microsoft.ApplicationBlocks.Data
Imports API_nutrition_vb.Member
Imports API_nutrition_vb.RegisterStatus
Imports NTi.CommonUtility
Imports System.Web.Http

Namespace Controllers
    Public Class RegisterController
        Inherits ApiController
        <Route("Login")>
        <AcceptVerbs("POST")>
        Public Function Login() As HttpResponseMessage

            Try
                'ตรวจสอบ FormData
                Dim UserName As String = "" 'Company Tax ID, Citizen ID

                Dim password As String = "" 'รหัสผ่าน

                If Current.Request.Form.AllKeys.Contains("UserName") = True Then
                    UserName = Current.Request.Form("UserName").ToString()
                End If

                If Current.Request.Form.AllKeys.Contains("password") = True Then
                    password = Current.Request.Form("password").ToString()
                End If

                If UserName <> "" And password <> "" Then
                    'ตรวจสอบข้อมูล User ผ่าน webservice

                    Dim result_post As String = ""
                    Dim jsonSring As String = "{""u"":""" & UserName & """,""p"":""" & password & """}"
                    Dim Uri As Uri = New Uri(ConfigurationManager.AppSettings("HostWeb") & "/DesktopModules/JwtAuth/API/mobile/login")

                    Dim data = Encoding.UTF8.GetBytes(jsonSring)
                    result_post = SendRequest(Uri, data, "application/json", "POST")

                    If result_post <> "" Then
                        Dim jss As New JavaScriptSerializer()
                        Dim dict As Dictionary(Of String, String) = jss.Deserialize(Of Dictionary(Of String, String))(result_post)

                        'Dim doc = New XmlDocument()
                        'doc.LoadXml(result_post)
                        'Dim node As XmlNode = doc.GetElementsByTagName("status")(0)

                        Dim user_id As String = ""
                        user_id = dict("userId").ToString
                        '**************************************************************************************************************************
                        'If Not dict Is Nothing Then
                        '    If node.InnerText = "true" Then
                        '        'Return result
                        '        Dim msg As New [RegisterStatus]
                        '        msg.status = True
                        '        msg.token = doc.GetElementsByTagName("token")(0).InnerText
                        '        'msg.user_type = type
                        '        Return Request.CreateResponse(HttpStatusCode.OK, msg)
                        '    Else
                        '        Dim err As New [Error]
                        '        If doc.GetElementsByTagName("message")(0).InnerText.Contains("Duplicate") = True Then
                        '            err.code = "2" 'user id ซ้ำ
                        '        Else
                        '            err.code = "6" 'error อื่นๆ
                        '        End If
                        '        err.message = doc.GetElementsByTagName("message")(0).InnerText
                        '        'Return resual
                        '        Return Request.CreateResponse(HttpStatusCode.BadRequest, err)
                        '    End If
                        '    '************************** ลง else = ลงทะเบียนสำเร็จ *****************
                        'Else
                        Dim user_info As New [RegisterStatus]
                        If user_id <> "" Then
                            'กรณีดึงข้อมูลสำเร็จ
                            user_info.status = "True" 'doc.GetElementsByTagName("Status")(0).InnerText
                            user_info.UserID = user_id 'doc.GetElementsByTagName("UserID")(0).InnerText
                            user_info.Username = UserName  'doc.GetElementsByTagName("Username")(0).InnerText
                            user_info.FirstName = dict("displayName").ToString 'doc.GetElementsByTagName("FirstName")(0).InnerText
                            user_info.LastName = "" ' doc.GetElementsByTagName("Lastname")(0).InnerText
                            user_info.Email = "" 'doc.GetElementsByTagName("Email")(0).InnerText
                            user_info.Phone = ""
                            user_info.Role = "User" 'doc.GetElementsByTagName("UserRole")(0).InnerText
                            user_info.IsActivate = "True" 'doc.GetElementsByTagName("IsActivate")(0).InnerText
                            user_info.IsLock = "False" 'doc.GetElementsByTagName("IsLock")(0).InnerText
                            user_info.token = UserName 'doc.GetElementsByTagName("Token")(0).InnerText

                            'ตรวจสอบว่าเป็น จนท หรือไม่?
                            Dim strDBConn As String = ConfigurationManager.ConnectionStrings("MocApiMb").ConnectionString
                            Dim dsUser As DataSet
                            dsUser = SqlHelper.ExecuteDataset(strDBConn, CommandType.Text, "select top(1) OrgID, FirstName,LastName,isnull(Email,'') as Email,Phone,case when p.IsHead=1 then 'หัวหน้า' when p.IsDirector=1 then 'ผู้อำนวยการ' else 'เจ้าหน้าที่' end as Position, (SELECT OfficeName From Offices WHERE OfficeId=p.OfficeID) AS OfficeName, (SELECT OrgName FROM Organizations WHERE OrgID=p.OrgID) AS OrgName from Person p where p.isDelete =0 and p.isDisable = 0 and p.UserName=@user", New SqlParameter("@user", UserName))

                            If dsUser.Tables.Count > 0 Then
                                If dsUser.Tables(0).Rows.Count > 0 Then
                                    Dim dt As DataTable = dsUser.Tables(0)
                                    user_info.OrgId = dt.Rows(0)("OrgID")
                                    user_info.OrgName = dt.Rows(0)("OrgName")
                                    user_info.OfficeName = dt.Rows(0)("OfficeName")
                                    user_info.Position = dt.Rows(0)("Position")
                                    user_info.Email = dt.Rows(0)("Email")
                                    user_info.Phone = dt.Rows(0)("Phone")
                                    user_info.Role = "mocuser"
                                End If
                            End If

                            If user_info.status = "True" Then
                                user_info.message = "Login success"
                                'Return resual
                                Return Request.CreateResponse(HttpStatusCode.OK, user_info)
                            Else

                                'ตรวจสอบว่าเป็น จนท หรือไม่?
                                strDBConn = ConfigurationManager.ConnectionStrings("MocApiMb").ConnectionString
                                dsUser = SqlHelper.ExecuteDataset(strDBConn, CommandType.Text, "select top(1) OrgID, FirstName,LastName,isnull(Email,'') as Email,Phone,case when p.IsHead=1 then 'หัวหน้า' when p.IsDirector=1 then 'ผู้อำนวยการ' else 'เจ้าหน้าที่' end as Position, (SELECT OfficeName From Offices WHERE OfficeId=p.OfficeID) AS OfficeName, (SELECT OrgName FROM Organizations WHERE OrgID=p.OrgID) AS OrgName from Person p where p.isDelete =0 and p.isDisable = 0 and p.UserName=@user", New SqlParameter("@user", UserName))

                                If dsUser.Tables.Count > 0 Then
                                    If dsUser.Tables(0).Rows.Count > 0 Then
                                        Dim dt As DataTable = dsUser.Tables(0)
                                        user_info.OrgId = dt.Rows(0)("OrgID")
                                        user_info.OrgName = dt.Rows(0)("OrgName")
                                        user_info.OfficeName = dt.Rows(0)("OfficeName")
                                        user_info.Position = dt.Rows(0)("Position")
                                        user_info.Email = dt.Rows(0)("Email")
                                        user_info.Phone = dt.Rows(0)("Phone")
                                        user_info.Role = "mocuser"
                                        user_info.message = "Login success"
                                        'Return resual
                                        Return Request.CreateResponse(HttpStatusCode.OK, user_info)
                                    End If
                                End If

                                user_info.message = "ข้อมูลชื่อผู้ใช้งานหรือรหัสผ่านไม่ถูกต้อง"
                                'Return resual
                                Return Request.CreateResponse(HttpStatusCode.BadRequest, user_info)
                            End If

                        Else

                            user_info.message = "ข้อมูลชื่อผู้ใช้งานหรือรหัสผ่านไม่ถูกต้อง"
                            'Return resual
                            Return Request.CreateResponse(HttpStatusCode.BadRequest, user_info)
                        End If
                    Else

                        Dim err As New [Error]
                        err.code = "4"
                        err.message = "ไม่มีข้อมูลตอบกลับจากการลงทะเบียนในเว็บหลัก"
                        'Return resual
                        Return Request.CreateResponse(HttpStatusCode.BadRequest, err)
                    End If
                Else
                    'error case เกิดข้อผิดพลาด
                    Dim err As New [Error]
                    err.code = "5" 'กรอกข้อมูลการสมัครไม่ครบ
                    err.message = "กรุณาใส่ข้อมูลชื่อผู้ใช้งานและรหัสผ่านให้ครบถ้วนถูกต้อง, ห้ามเว้นว่างไว้"
                    'Return resual
                    Return Request.CreateResponse(HttpStatusCode.BadRequest, err)

                End If
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