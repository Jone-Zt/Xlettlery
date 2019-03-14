using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer
{
    public enum HttpRequestType
    {
        Get = 1,
        Post = 2
    }

    public class HttpHeader
    {
        /// <summary>
        /// 可接收文档类型
        /// </summary>
        public const string Accept = "Accept";
        /// <summary>
        /// 客户端语言区域标识
        /// </summary>
        public const string AcceptLanguage = "Accept-Language";
        /// <summary>
        /// 客户端浏览器信息
        /// </summary>
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// 客户端设备CPU类型
        /// </summary>
        public const string UA_CPU = "UA-CPU";
        /// <summary>
        /// 可接收内容压缩方式
        /// </summary>
        public const string AcceptEncoding = "Accept-Encoding";
        /// <summary>
        /// 客户端请求服务器域名地址
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// 响应 缓存标识
        /// </summary>
        public const string CacheControl = "Cache-Control";
        /// <summary>
        /// 服务端应用标签
        /// </summary>
        public const string Server = "Server";
        /// <summary>
        /// 日期时间
        /// </summary>
        public const string Date = "Date";
        /// <summary>
        /// 内容文档类型
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// 内容长度 不包含头信息
        /// </summary>
        public const string ContentLength = "Content-Length";
        /// <summary>
        /// 重定向客户端到指定地址
        /// </summary>
        public const string Location = "Location";
        /// <summary>
        /// 指定内容压缩方式
        /// </summary>
        public const string ContentEncoding = "Content-Encoding";
        /// <summary>
        /// 文件描述 下载时使用
        /// </summary>
        public const string ContentDisposition = "Content-Disposition";
        /// <summary>
        /// 资源最后一次编辑时间
        /// </summary>
        public const string LastModified = "Last-Modified";
    }

    public enum HttpContentType
    {
        Know = -1,
        TextHtml = 0,
        TextPlain = 1,
        TextXml = 2,
        ImageJpeg = 3,
        ImageGif = 4,
        ImagePng = 5,
        ImageTiff = 6,
        TextCss = 7,
        TextJscript = 8,
        ApplicationJavascript = 9,
        ApplicationOctetStream = 10
    }

    public static class HttpContentTypeTool
    {
        public static string GetHeaderContentTypeName(HttpContentType ct)
        {
            switch (ct)
            {
                case HttpContentType.TextPlain:
                    return "text/plain";
                case HttpContentType.TextHtml:
                    return "text/html";
                case HttpContentType.TextXml:
                    return "text/xml";
                case HttpContentType.ImageJpeg:
                    return "image/jpeg";
                case HttpContentType.ImageGif:
                    return "image/gif";
                case HttpContentType.ImagePng:
                    return "image/png";
                case HttpContentType.ImageTiff:
                    return "image/tiff";
                case HttpContentType.TextCss:
                    return "text/css";
                case HttpContentType.TextJscript:
                    return "text/jscript";
                case HttpContentType.ApplicationJavascript:
                    return "application/javascript";
                case HttpContentType.ApplicationOctetStream:
                    return "application/octet-stream";
                default:
                    return "";
            }
        }
        public static HttpContentType GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".htm":
                    return HttpContentType.TextHtml;
                case ".html":
                    return HttpContentType.TextHtml;
                case ".config":
                    return HttpContentType.TextXml;
                case ".xml":
                    return HttpContentType.TextXml;
                case ".jpe":
                    return HttpContentType.ImageJpeg;
                case ".jpeg":
                    return HttpContentType.ImageJpeg;
                case ".jpg":
                    return HttpContentType.ImageJpeg;
                case ".gif":
                    return HttpContentType.ImageGif;
                case ".png":
                    return HttpContentType.ImagePng;
                case ".pnz":
                    return HttpContentType.ImagePng;
                case ".tif":
                    return HttpContentType.ImageTiff;
                case ".tiff":
                    return HttpContentType.ImageTiff;
                case ".css":
                    return HttpContentType.TextCss;
                case ".js":
                    return HttpContentType.ApplicationJavascript;
                default:
                    return HttpContentType.ApplicationOctetStream;
            }
        }
    }

    public class HttpRequestErrorCode
    {
        /// <summary>
        /// 请求处理完成
        /// </summary>
        public const string Complate = "200";
        /// <summary>
        /// 部分内容完成
        /// </summary>
        public const string PartialContent = "206";
        /// <summary>
        /// 访问的资源不存在
        /// </summary>
        public const string NotFound = "404";
        /// <summary>
        /// 永久重定向
        /// </summary>
        public const string AllRedirect = "301";
        /// <summary>
        /// 临时重定向
        /// </summary>
        public const string TmpRedirect = "302";
        /// <summary>
        /// GET重定向
        /// </summary>
        public const string GetRedirect = "303";
        /// <summary>
        /// POST重定向
        /// </summary>
        public const string POSTRedirect = "307";

        public static string GetMsgByCode(string code)
        {
            switch (code)
            {
                case HttpRequestErrorCode.Complate:
                    return "OK";
                case HttpRequestErrorCode.PartialContent:
                    return "Partial Content";
                case HttpRequestErrorCode.AllRedirect:
                    return "Moved Permanently";
                case HttpRequestErrorCode.TmpRedirect:
                    return "Found";
                case HttpRequestErrorCode.GetRedirect:
                    return "Found";
                case HttpRequestErrorCode.POSTRedirect:
                    return "Found";
                case HttpRequestErrorCode.NotFound:
                    return "Not Found";
                default:
                    return "ERROR";
            }
        }
    }

    public enum CacheControlType
    {
        /// <summary>
        /// 指示响应可被任何缓存区缓存
        /// </summary>
        Public = 1,
        /// <summary>
        /// 指示对于单个用户的整个或部分响应消息，不能被共享缓存处理。这允许服务器仅仅描述当用户的部分响应消息，此响应消息对于其他用户的请求无效
        /// </summary>
        Private = 2,
        /// <summary>
        /// 指示请求或响应消息不能缓存 
        /// </summary>
        NoCache = 3,
        /// <summary>
        /// 用于防止重要的信息被无意的发布。在请求消息中发送将使得请求和响应消息都不使用缓存
        /// </summary>
        NoStore = 4,
        /// <summary>
        /// 指示客户机可以接收生存期不大于指定时间（以秒为单位）的响应
        /// </summary>
        MaxAge = 5,
        /// <summary>
        /// 指示客户机可以接收响应时间小于当前时间加上指定时间的响应
        /// </summary>
        MinFresh = 6,
    }

    public static class CacheControlTypeTool
    {
        public static string GetArgeValue(CacheControlType cct)
        {
            switch (cct)
            {
                case CacheControlType.Public:
                    return "public";
                case CacheControlType.Private:
                    return "private";
                case CacheControlType.NoCache:
                    return "no-cache";
                case CacheControlType.NoStore:
                    return "no-store";
                case CacheControlType.MaxAge:
                    return "max-age";
                case CacheControlType.MinFresh:
                    return "min-fresh";
                default:
                    return "no-cache";
            }
        }
    }
}
