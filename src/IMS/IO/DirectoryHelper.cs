using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IO
{
    /// <summary>
    /// 文件夹操作帮助类
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="directory"></param>
        public static void CreateIfNot(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
