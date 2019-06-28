Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports NTi.CommonUtility
Imports Microsoft.ApplicationBlocks.Data
Imports System.Data.SqlClient




''' <summary>
''' Class function สำหรับใช้งานร่วมกันภายใน 
''' </summary>
Public Class nutrirtion
    'connection string database
    'Shared strConn As String = ConfigurationManager.ConnectionStrings("epayment").ConnectionString
    Shared strConn As String = ConfigurationManager.ConnectionStrings("APIConnection").ConnectionString

    ''' <summary>
    ''' ตรวจสอบ Token ของระบบที่เรียกใช้งาน
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    Shared Function CheckToken() As Boolean
        Return True
    End Function

    ''' <summary>
    ''' ตรวจสอบ Token ของระบบที่เรียกใช้งานและเก็บประวัติการเรียกใช้งาน api
    ''' </summary>
    ''' <param name="token">apikey</param>
    ''' <returns>Boolean</returns>
    'Shared Function VerifyApiToken(ByVal token As String, ByVal request As HttpRequestMessage) As Boolean
    '    Try

    '        If token <> "" Then
    '            'get request information.
    '            Dim ip As String = GetClientIp(request)
    '            Dim route_name As String = CommonUtility.Get_StringValue(request.GetRouteData().Route.RouteTemplate)
    '            Dim verb As String = CommonUtility.Get_StringValue(request.Method.ToString)
    '            Dim client_info As String = CommonUtility.Get_StringValue(request.Headers.UserAgent.ToString)
    '            Dim url As String = request.RequestUri.PathAndQuery

    '            Dim ds As DataSet
    '            ds = SqlHelper.ExecuteDataset(strConn, CommandType.StoredProcedure, "spT_MoblieCheckAPIToken",
    '                New SqlParameter("@token", token),
    '                New SqlParameter("@ip", ip),
    '                New SqlParameter("@service", route_name),
    '                New SqlParameter("@method", verb),
    '                New SqlParameter("@client_info", client_info),
    '                New SqlParameter("@url", url))

    '            If ds.Tables.Count > 0 Then
    '                If ds.Tables(0).Rows.Count > 0 Then
    '                    Return True
    '                Else
    '                    Return False
    '                End If
    '            Else
    '                Return False
    '            End If
    '        Else
    '            Return False
    '        End If
    '    Catch ex As Exception
    '        Return False
    '    End Try
    'End Function

    ''' <summary>
    ''' ใช้ชั่วคราวสำหรับ NEA เรียกใช้งาน Web service
    ''' </summary>
    ''' <param name="token">token</param>
    ''' <param name="request">Web Request object</param>
    ''' <returns>Boolean</returns>


    ''' <summary>
    ''' Get IP Address
    ''' </summary>
    ''' <param name="request"></param>
    ''' <returns></returns>
    Shared Function GetClientIp(request As HttpRequestMessage) As String
        Dim ip As String = String.Empty
        If request.Properties.ContainsKey("MS_HttpContext") Then
            Dim context As HttpContextBase = DirectCast(request.Properties("MS_HttpContext"), HttpContextBase)
            If context.Request.ServerVariables("HTTP_VIA") IsNot Nothing Then
                ip = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR").ToString()
            Else
                ip = context.Request.ServerVariables("REMOTE_ADDR").ToString()
            End If
        End If
        Return ip
    End Function

    ''' <summary>
    ''' Call Website module
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="jsonDataBytes"></param>
    ''' <param name="contentType"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    Shared Function SendRequest(uri As Uri, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim req As WebRequest = WebRequest.Create(uri)
        'System.Net.ServicePointManager.UseNagleAlgorithm = False
        'System.Net.ServicePointManager.Expect100Continue = False
        req.ContentType = contentType
        req.Method = method
        req.ContentLength = jsonDataBytes.Length

        Dim stream = req.GetRequestStream()
        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
        stream.Close()

        Dim response = req.GetResponse().GetResponseStream()

        Dim reader As New StreamReader(response)
        Dim res = reader.ReadToEnd()
        reader.Close()
        response.Close()

        Return res
    End Function

    ''' <summary>
    ''' แปลงรูปแบบวันที่ เป็นรูปแบบ 2018-07-23T14:30:25.123Z
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Shared Function GetDateToString(ByVal obj As String) As String
        If CommonUtility.Is_DateTime(obj) = True Then
            Return CommonUtility.Get_DateTime(obj).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' แปลงข้อมูลสถานะเป็น Boolean
    ''' </summary>
    ''' <param name="value">สถานะ Y/N</param>
    ''' <returns>Boolean: Y=true/N=False</returns>
    Shared Function GetBoolean(ByVal value As String) As Boolean
        If value = "Y" Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' ตำแหน่งที่เก็บไฟล์
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Shared Function GetPathImage(ByVal file As String) As String
        Dim strImg As String = ""
        If CommonUtility.Get_StringValue(file) <> "" Then
            'Dim host As String = DirectCast(Request.Properties("MS_HttpContext"), HttpContextWrapper).Request.Url.Host
            Dim host As String = ConfigurationManager.AppSettings("url").ToString()
            strImg = host & Replace(Replace(file, " ", "%20"), "~/", "/")
        End If
        Return strImg
    End Function

    ''' <summary>
    ''' ฟังก์ชั่นสำหรับรับข้อมูลจาก Form Data
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="query_name"></param>
    ''' <returns></returns>
    Shared Function GetForm(ByVal context As HttpContext, ByVal query_name As String) As String
        If context.Request.Form.AllKeys.Contains(query_name) = True Then
            Return CommonUtility.Get_StringValue(context.Request.Form(query_name).ToString())
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' ตอบสอบค่า DbNull ที่ไม่มีข้อมูลจาก database
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Shared Function CheckDbNull(ByVal value As Object) As String
        If IsDBNull(value) Then
            Return "0"
        Else
            Return value.ToString
        End If
    End Function

    ''' <summary>
    ''' ตรวจสอบค่า Null ของข้อมูล
    ''' </summary>
    ''' <param name="_Data"></param>
    ''' <returns></returns>
    Shared Function ChkNull(_Data As Object)
        Try
            If DBNull.Value.Equals(_Data) = False Then
                If _Data Is Nothing Then
                    _Data = ""
                End If
            Else
                _Data = ""
            End If
        Catch ex As Exception
            _Data = ""
        End Try
        Return _Data
    End Function

    ''' <summary>
    ''' ตรวจสอบตำแหน่งไฟล์รูปภาพ
    ''' </summary>
    ''' <param name="ImageUrl"></param>
    ''' <param name="_Data"></param>
    ''' <returns></returns>
    Shared Function ChkNull_Image(ImageUrl As String, _Data As Object)
        Try
            If _Data.ToString.Trim = "" Then
                _Data = ""
            Else
                _Data = ImageUrl & _Data
            End If
        Catch ex As Exception
            _Data = ""
        End Try
        Return _Data
    End Function
    Public Function SP_Organigzations(ByVal OrgID As String) As DataTable

    End Function

End Class


