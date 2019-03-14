using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{

    public class HttpRequest
    {
        public SocketAsyncEventArgs SocketArg { get; set; }

        public bool IsTemp { get; set; }

        private HttpRequestType _RequestType;
        /// <summary>
        /// 请求类型
        /// </summary>
        public HttpRequestType RequestType { get { return _RequestType; } }
       
        private HttpResponse _response;
        /// <summary>
        /// 请求响应对象
        /// </summary>
        public HttpResponse Response { get { return _response; } }

        private int parseLine = 0;
        /// <summary>
        /// TCP接收缓存
        /// </summary>
        public byte[] MyBuffer { get; set; }
        /// <summary>
        /// 请求POST数据
        /// </summary>
        public byte[] MyContentBuffer { get; set; }
        private string _RequestPath;
        /// <summary>
        /// 请求文件地址
        /// </summary>
        public string RequestPath { get { return _RequestPath; } }
        private string _httpVersion;
        /// <summary>
        /// 协议版本号
        /// </summary>
        public string HttpVersion { get { return _httpVersion; } }
        private Dictionary<string,string> _RequestHeaders;
        private Dictionary<string, string> _RequestArges;
        private Socket client;
       
        public HttpRequest(int reciveLength, int sendLength)
        {
            _RequestType = HttpRequestType.Get;
            MyBuffer = new byte[reciveLength];
            _response = new HttpResponse(sendLength);
            _RequestHeaders = new Dictionary<string, string>(20);
            _RequestArges = new Dictionary<string, string>(20);
        }

        private IlogTool logtool;
        public void SetLogTool(IlogTool tool)
        {
            logtool = tool;
        }

        public Dictionary<string, string> RequestArges
        {
            get { return _RequestArges; }
        }

        /// <summary>
        /// 获取Get请求参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                string value = null;
                _RequestArges.TryGetValue(key, out value);
                return value;
            }
        }

        public string GetClientIP()
        {
            System.Net.IPEndPoint address = client.RemoteEndPoint as System.Net.IPEndPoint;
            if (address == null)
                return null;
            return address.Address.ToString();
        }

        public void SetClientSocket(Socket cli)
        {
            client = cli;
        }

        /// <summary>
        /// 请求头参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetHeader(string key)
        {
            string value = null;
            _RequestHeaders.TryGetValue(key.ToLower(), out value);
            return value;
        }
        
        internal bool ParseRequestMsg(int receiveLength)
        {
            try
            {
                bool reciveComplate = false;
                _RequestType = HttpRequestType.Get;
                _RequestHeaders.Clear();
                _RequestArges.Clear();
                _httpVersion = null;
                _RequestPath = null;
                MyContentBuffer = null;
                parseLine = 0;
                string reques;
                int lastPoint = 0;
                int scanPoint = 0;
                while ((lastPoint + scanPoint) < receiveLength)
                {
                    if (MyBuffer[lastPoint + scanPoint] == '\n')
                    {
                        scanPoint++;
                        reques = Encoding.ASCII.GetString(MyBuffer, lastPoint, scanPoint);
                        if (ParseRequestLine(reques.Trim()))
                        {
                            lastPoint += scanPoint;
                            scanPoint = 0;
                            reciveComplate = true;
                            break;
                        }
                        else
                        {
                            lastPoint += scanPoint;
                            scanPoint = 0;
                        }
                    }
                    else
                    {
                        scanPoint++;
                    }
                }
                if (reciveComplate && _RequestType == HttpRequestType.Post)
                {
                    string cLength = GetHeader(HttpHeader.ContentLength);
                    int contentLength = 0;
                    if (!string.IsNullOrEmpty(cLength))
                        contentLength = int.Parse(cLength);
                    if (contentLength > 0)
                    {
                        MyContentBuffer = new byte[contentLength];
                        int contentPoint = 0;
                        if (lastPoint < receiveLength)
                        {
                            Array.Copy(MyBuffer, lastPoint, MyContentBuffer, contentPoint, receiveLength - lastPoint);
                            contentPoint += (receiveLength - lastPoint);
                        }
                        int testCount = 0;
                        while (contentPoint < contentLength && testCount < 5)
                        {
                            testCount++;
                            contentPoint += client.Receive(MyContentBuffer, contentPoint, contentLength - contentPoint, SocketFlags.None);
                        }
                        reciveComplate = (contentPoint >= contentLength);
                    }
                }
                _response.Reset(client, this);
                return reciveComplate;
            }
            catch (Exception ex)
            {
                if (logtool != null)
                    logtool.WriteError("解析http请求失败 ", ex);
                return false;
            }
        }
        

        private bool ParseRequestLine(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            
            if (parseLine == 0)
            {
                string[] requestData = str.Split(' ');
                if (requestData[0] == "GET")
                    _RequestType = HttpRequestType.Get;
                else
                    _RequestType = HttpRequestType.Post;
                _httpVersion = requestData[2];
                ParseRequestPath(requestData[1]);
            }
            else
            {
                int index = str.IndexOf(":");
                if (index > 0)
                {
                    if (str.Length - 2 > index)
                    {
                        _RequestHeaders.Add(str.Substring(0, index).ToLower(), str.Substring(index + 1));
                    }
                    else
                    {
                        _RequestHeaders.Add(str.ToLower(), "");
                    }
                }
                else
                {
                    _RequestHeaders.Add(str.ToLower(), "");
                }
            }
            parseLine++;
            return false;
        }

        private void ParseRequestPath(string str)
        {
            str = System.Web.HttpUtility.UrlDecode(str);
            string[] requestData = str.Split('?');
            _RequestPath = requestData[0];
            if (requestData.Length > 1)
            {
                string[] arges = requestData[1].Split('&');
                foreach (string arge in arges)
                {
                    int index = arge.IndexOf("=");
                    if (index > 0)
                    {
                        if (str.Length - 2 > index)
                        {
                            _RequestArges.Add(arge.Substring(0, index), arge.Substring(index + 1));
                        }
                        else
                        {
                            _RequestArges.Add(arge, "");
                        }
                    }
                    else
                    {
                        _RequestArges.Add(arge, "");
                    }
                }
            }
        }
    }

    public class HttpResponse
    {
        private Socket _clientSocket;
        private string _httpVersion;
        private string _responseCode;
        private string _dateTime;
        public CacheControlType CacheControl { get; set; }
        public string CharSet { get; set; }
        private byte[] buffer;
        private int bLength;
        public HttpResponse(int bufferLength)
        {
            bLength = bufferLength;
            CharSet = "utf-8";
            buffer = new byte[bufferLength];
            CacheControl = CacheControlType.NoCache;
        }

        public bool AllRedirect(string path)
        {
            path = System.Web.HttpUtility.UrlEncode(path);
            _responseCode = HttpRequestErrorCode.AllRedirect;
            StringBuilder responseHeader = new StringBuilder();
            AppendDefaultData(responseHeader, HttpContentType.TextHtml, CharSet, 0);
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            return SendData(buffer, 0, bufferOffset);
        }

        public bool TmpRedirect(string path)
        {
            path = System.Web.HttpUtility.UrlEncode(path);
            _responseCode = HttpRequestErrorCode.TmpRedirect;
            StringBuilder responseHeader = new StringBuilder();
            AppendDefaultData(responseHeader, HttpContentType.TextHtml, CharSet, 0);
            responseHeader.Append(HttpHeader.Location + ":" + path + "\r\n");
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            return SendData(buffer, 0, bufferOffset);
        }

        internal void Reset(Socket client, HttpRequest request)
        {
            CacheControl = CacheControlType.NoCache;
            _clientSocket = client;
            _httpVersion = request.HttpVersion;
            _responseCode = "200";
            _dateTime = DateTime.Now.ToString("r");
        }
        /// <summary>
        /// 直接响应错误码
        /// </summary>
        /// <param name="backCode"></param>
        /// <returns></returns>
        public bool SendResponseByErrorCode(string backCode)
        {
            _responseCode = backCode;
            StringBuilder responseHeader = new StringBuilder();
            AppendDefaultData(responseHeader, HttpContentType.TextHtml, CharSet, 0);
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            return SendData(buffer, 0, bufferOffset);
        }

        /// <summary>
        /// 响应状态码和文字消息
        /// </summary>
        /// <param name="backCode"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public bool SendResponseByErrorCode(string backCode, string Content)
        {
            byte[] contentData = Encoding.GetEncoding(CharSet).GetBytes(Content);
            return SendContent(backCode, contentData, CharSet, HttpContentType.TextPlain);
        }

        /// <summary>
        /// 直接响应HTML文本
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public bool SendResponseHTML(string backCode, string Content)
        {
            byte[] contentData = Encoding.GetEncoding(CharSet).GetBytes(Content);
            return SendContent(backCode, contentData, CharSet, HttpContentType.TextHtml);
        }

        /// <summary>
        /// 直接响应HTML文本
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public bool SendResponseHTML(string Content)
        {
            byte[] contentData = Encoding.GetEncoding(CharSet).GetBytes(Content);
            return SendContent(HttpRequestErrorCode.Complate, contentData, CharSet, HttpContentType.TextHtml);
        }

        /// <summary>
        /// 直接响应XML文本
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public bool SendResponseXML(string Content)
        {
            byte[] contentData = Encoding.GetEncoding(CharSet).GetBytes(Content);
            return SendContent(HttpRequestErrorCode.Complate, contentData, CharSet, HttpContentType.TextXml);
        }

        public bool SendContent(string backCode, byte[] content, string charS, HttpContentType hct)
        {
            _responseCode = backCode;
            StringBuilder responseHeader = new StringBuilder();
            if (content == null || content.Length == 0)
                AppendDefaultData(responseHeader, hct, charS, 0);
            else
                AppendDefaultData(responseHeader, hct, charS, content.Length);
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            content.CopyTo(buffer, bufferOffset);
            bufferOffset += content.Length;
            return SendData(buffer, 0, bufferOffset);
        }

        private int CreateHeaderBuffer(byte[] buffer, StringBuilder responseHeader)
        {
            responseHeader.Append("\r\n");
            string header = responseHeader.ToString();
            return Encoding.ASCII.GetBytes(header, 0, header.Length, buffer, 0);
        }

        private IlogTool logtool;
        public void SetLogTool(IlogTool tool)
        {
            logtool = tool;
        }

        public bool SendData(byte[] data,int offset,int length)
        {
            try
            {
                int sendedLength = 0;
                int tCount = 0;
                while (sendedLength < length && tCount < 3)
                {
                    sendedLength += _clientSocket.Send(data, offset + sendedLength, length - sendedLength, SocketFlags.None);
                    tCount++;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (logtool != null)
                    logtool.WriteError("发送数据失败 ", ex);
                return false;
            }
        }

        public void AppendDefaultData(StringBuilder responseHeader, HttpContentType contentType, string charSet, long contentLength)
        {
            responseHeader.Append("HTTP/1.1 " + _responseCode + " " + HttpRequestErrorCode.GetMsgByCode(_responseCode) + "\r\n");
            responseHeader.Append(HttpHeader.CacheControl + ":" + CacheControlTypeTool.GetArgeValue(CacheControl) + "\r\n");
            if (string.IsNullOrEmpty(charSet))
                responseHeader.Append(HttpHeader.ContentType + ":"+ HttpContentTypeTool.GetHeaderContentTypeName(contentType) + "\r\n");
            else
                responseHeader.Append(HttpHeader.ContentType + ":" + HttpContentTypeTool.GetHeaderContentTypeName(contentType)+ ";charset=" + charSet + "\r\n");
            responseHeader.Append(HttpHeader.Server + ":PISA-HttpServer" + "\r\n");
            responseHeader.Append(HttpHeader.Date + ":" + _dateTime + "\r\n");
            responseHeader.Append(HttpHeader.ContentLength + ":" + contentLength + "\r\n");
        }

        public bool SendResponseFile(string fileName)
        {
            FileInfo fInfo = FileCacheManage.GetManage().GetCacheFileInfo(fileName);
            if (fInfo != null && !fInfo.Changed)
            {
                return SendFileInfo(fInfo);
            }
            else
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
                if (!fi.Exists)
                {
                    return SendResponseByErrorCode(HttpRequestErrorCode.NotFound);
                }
                if (fInfo == null)
                    SendNewFile(fi, true);
                else
                    SendNewFile(fi, !fInfo.Changed);
            }
            return true;
        }

        private bool SendNewFile(System.IO.FileInfo fi, bool needCache)
        {
            bool complate = false;
            using (System.IO.FileStream fs = fi.OpenRead())
            {
                if (fs.Length <= FileCacheManage.CACHEFILE_FILEMAXSIZE)
                {
                    FileInfo newFInfo = new FileInfo();
                    newFInfo.ContentType = HttpContentTypeTool.GetContentType(fi.Extension);
                    newFInfo.RealFileName = fi.FullName;
                    newFInfo.DownloadFileName = fi.Name;
                    newFInfo.FileData = new byte[fs.Length];
                    fs.Read(newFInfo.FileData, 0, newFInfo.FileData.Length);
                    fs.Close();
                    if (needCache && (newFInfo.ContentType == HttpContentType.ImageJpeg || newFInfo.ContentType == HttpContentType.ImagePng))
                        FileCacheManage.GetManage().CacheFile(newFInfo);
                    complate = SendFileInfo(newFInfo);
                }
                else
                {
                    complate = SendBigFile(fs, fi);
                    fs.Close();
                }
            }
            return complate;
        }

        private bool SendBigFile(System.IO.FileStream fs, System.IO.FileInfo fi)
        {
            _responseCode = HttpRequestErrorCode.Complate;
            StringBuilder responseHeader = new StringBuilder();
            HttpContentType cType = HttpContentTypeTool.GetContentType(fi.Extension);
            AppendDefaultData(responseHeader, cType, CharSet, fs.Length);
            if (cType == HttpContentType.ApplicationOctetStream)
                responseHeader.Append(HttpHeader.ContentDisposition + ":attachment;filename=" + fi.Name + "\r\n");
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            if (SendData(buffer, 0, bufferOffset))
            {
                fs.Position = 0;
                int sendCount = 0;
                while (fs.Position < fs.Length)
                {
                    if (fs.Length - fs.Position > bLength)
                        sendCount = fs.Read(buffer, 0, bLength);
                    else
                        sendCount = fs.Read(buffer, 0, (int)(fs.Length - fs.Position));
                    if (!SendData(buffer, 0, sendCount))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool SendFileInfo(FileInfo fInfo)
        {
            if (fInfo == null || fInfo.FileData == null || fInfo.FileData.Length == 0)
            {
                return SendResponseByErrorCode(HttpRequestErrorCode.NotFound);
            }
            else
            {
                _responseCode = HttpRequestErrorCode.Complate;
                StringBuilder responseHeader = new StringBuilder();
                AppendDefaultData(responseHeader, fInfo.ContentType, CharSet, fInfo.FileData.Length);
                if (fInfo.ContentType == HttpContentType.ApplicationOctetStream)
                    responseHeader.Append(HttpHeader.ContentDisposition + ":attachment;filename=" + fInfo.DownloadFileName + "\r\n");
                return SendContent(responseHeader, fInfo.FileData);
            }
        }

        private bool SendContent(StringBuilder responseHeader, byte[] content)
        {
            int bufferOffset = CreateHeaderBuffer(buffer, responseHeader);
            int contentSendedLength = 0;
            int waitSendLength = 0;
            if (content != null && content.Length > 0)
            {
                waitSendLength = content.Length;
                if (waitSendLength > bLength - bufferOffset)
                {
                    Array.Copy(content, contentSendedLength, buffer, bufferOffset, bLength - bufferOffset);
                    contentSendedLength = bLength - bufferOffset;
                    bufferOffset = bLength;
                    waitSendLength -= (bLength - bufferOffset);
                }
                else
                {
                    Array.Copy(content, contentSendedLength, buffer, bufferOffset, waitSendLength);
                    contentSendedLength = waitSendLength;
                    bufferOffset += waitSendLength;
                    waitSendLength = 0;

                }
            }

            while (bufferOffset > 0)
            {
                if (!SendData(buffer, 0, bufferOffset))
                {
                    return false;
                }
                bufferOffset = 0;
                if (waitSendLength > 0)
                {
                    if (waitSendLength > bLength - bufferOffset)
                    {
                        Array.Copy(content, contentSendedLength, buffer, 0, bLength);
                        contentSendedLength = bLength;
                        bufferOffset = bLength;
                        waitSendLength -= bLength;
                    }
                    else
                    {
                        Array.Copy(content, contentSendedLength, buffer, 0, waitSendLength);
                        contentSendedLength += waitSendLength;
                        bufferOffset += waitSendLength;
                        waitSendLength = 0;
                    }
                }
            }
            return true;
        }
    }
}
