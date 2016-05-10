using System;

namespace TransferCommon
{
    [Serializable]
    public class UpLoadInfo
    {
        /// <summary>
        /// 标志Id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 上传文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 保存名称
        /// </summary>
        public string SaveName { get; set; }
        /// <summary>
        /// 上传开始位置
        /// </summary>
        public long TransferPos { get; set; }
        /// <summary>
        /// 上传文件长度（大小）
        /// </summary>
        public long TransferLength { get; set; }
        /// <summary>
        /// 已完成的文件长度（大小）
        /// </summary>
        public long TransferedLength { get; set; }
    }
}
