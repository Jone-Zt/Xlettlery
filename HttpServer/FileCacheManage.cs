using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace HttpServer
{
    public class FileInfo
    {
        public HttpContentType ContentType;
        public byte[] FileData;
        public string DownloadFileName;
        public string RealFileName;
        public long EndTicks;
        public bool Changed;
    }

    public class FileCacheManage
    {
        public static int CACHEFILE_FILECOUNT = 50000;//最多缓存文件数量
        public static long CACHEFILE_LIFETIME = 600000000;
        public static int CACHEFILE_FILEMAXSIZE = 1024 * 30;
        public static long CACHEFILE_CHECKLIFESPAN = 600000000L * 10;//1毫秒=10000个100毫微妙
        private static FileCacheManage manage;
        private static object slockObj = new object();
        public static FileCacheManage GetManage()
        {
            if (manage == null)
            {
                lock(slockObj)
                {
                    if (manage == null)
                        manage = new FileCacheManage();
                }
            }
           
            return manage;
        }

        Hashtable myCacheData;
        private long lastCheckLifeEndTime = 0;
        private object lockObj = new object();
        private object lockLbObj = new object();
        private bool isChecing = false;
        private WaitCallback callBackHandle;
        private IlogTool logTool;
        System.IO.FileSystemWatcher watcher;
        List<string> allFiles = new List<string>();
        private FileCacheManage()
        {
            myCacheData = new Hashtable(CACHEFILE_FILECOUNT);
            lastCheckLifeEndTime = DateTime.Now.Ticks;
            callBackHandle = new WaitCallback(Check);
        }

        public void SetRootFilePath(string path)
        {
            if (watcher == null)
            {
                watcher = new System.IO.FileSystemWatcher();
                /* 设置为监视 LastWrite 和 LastAccess 时间方面的更改，以及目录中文本文件的创建、删除或重命名。 */
                watcher.NotifyFilter = System.IO.NotifyFilters.LastAccess | System.IO.NotifyFilters.LastWrite
                              | System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName;
                // 只监控.dll文件  
                watcher.Filter = "*.*";
                // 添加事件处理器。  
                watcher.Changed += Watcher_Changed;
                watcher.Created += Watcher_Created;
                watcher.Deleted += Watcher_Deleted;
                watcher.Renamed += Watcher_Renamed;
            }
            watcher.Path = path;
        }

        private void Watcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            FileInfo fInfo = GetCacheFileInfo(e.OldFullPath);
            if (fInfo != null)
                fInfo.Changed = true;
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            FileInfo fInfo = GetCacheFileInfo(e.FullPath);
            if (fInfo != null)
                fInfo.Changed = true;
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            FileInfo fInfo = GetCacheFileInfo(e.FullPath);
            if (fInfo != null)
                fInfo.Changed = true;
        }

        private void Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            FileInfo fInfo = GetCacheFileInfo(e.FullPath);
            if (fInfo != null)
                fInfo.Changed = true;
        }

        public void SetLogTool(IlogTool tool)
        {
            logTool = tool;
        }

        public void CacheFile(FileInfo fInfo)
        {
            try
            {
                fInfo.Changed = false;
                if (fInfo.FileData.Length > CACHEFILE_FILEMAXSIZE)//文件长度超过缓存限制长度时，不做缓存
                    return;
                if (myCacheData.Count > CACHEFILE_FILECOUNT)//缓存内容超过 最大缓存文件数量时，不做缓存，且启动过期检查
                {
                    BeginCheck();
                    return;
                }
                fInfo.EndTicks = DateTime.Now.Ticks + CACHEFILE_LIFETIME;//缓存文件生存周期
                lock(lockLbObj)
                {
                    myCacheData[fInfo.RealFileName] = fInfo;
                }
                if (!isChecing)
                {
                    if (DateTime.Now.Ticks - lastCheckLifeEndTime > CACHEFILE_CHECKLIFESPAN)//固定间隔时间启动回收
                    {
                        BeginCheck();
                    }
                }
            }
            catch (Exception ex)
            {
                logTool.WriteError("文件缓存数据 保存失败 ", ex);
            }
        }

        public FileInfo GetCacheFileInfo(string fileAllName)
        {
            try
            {
                FileInfo fInfo = myCacheData[fileAllName] as FileInfo;
                if (fInfo != null)
                    fInfo.EndTicks = DateTime.Now.Ticks + CACHEFILE_LIFETIME;
                return fInfo;
            }
            catch (Exception ex)
            {
                logTool.WriteError("文件缓存数据 读取失败 ", ex);
                return null;
            }
        }
        
        private void BeginCheck()
        {
            lock (lockObj)
            {
                if (!isChecing)
                {
                    isChecing = true;
                    System.Threading.ThreadPool.QueueUserWorkItem(callBackHandle);
                }
            }
        }

        private void Check(object status)
        {
            try
            {
                string[] allKeys;
                lock (lockLbObj)
                {
                    allKeys = new string[myCacheData.Count];
                    myCacheData.Keys.CopyTo(allKeys, 0);
                }
                long nowTicks = DateTime.Now.Ticks;
                FileInfo fi = null;
                string fileName;
                for (int i = 0; i < allKeys.Length; i++)
                {
                    fileName = allKeys[i];
                    fi = myCacheData[fileName] as FileInfo;
                    if(fi!=null)
                    {
                        if (fi.Changed || fi.EndTicks < nowTicks)
                        {
                            myCacheData.Remove(fi.RealFileName);
                            i--;
                        }
                    }
                }
                System.GC.Collect();
                isChecing = false;
            }
            catch (Exception ex)
            {
                logTool.WriteError("文件缓存数据生命周期检查处理失败 ", ex);
                isChecing = false;
            }
        }
    }
}
