using System;
using System.Runtime.InteropServices;

namespace CpsCopy
{
    /// <summary>
    /// Base class for FileCopy.
    /// </summary>
    class FileCopy
    {
        /// <summary>
        /// The CopyFileEx method for FileCopy class.
        /// </summary>
        /// <param name="lpExistingFileName">The existing file name.</param>
        /// <param name="lpNewFileName">The new file name.</param>
        /// <param name="lpProgressRoutine">The progress routine.</param>
        /// <param name="lpData">The data.</param>
        /// <param name="pbCancel">The cancel.</param>
        /// <param name="dwCopyFlags">The copy flags.</param>
        /// <returns>The outcome of the method.</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, CopyFileFlags dwCopyFlags);

        /// <summary>
        /// The CopyProgressRoutine delegate for FileCopy class.
        /// </summary>
        /// <param name="TotalFileSize">The total file size.</param>
        /// <param name="TotalBytesTransferred">The total bytes transferred.</param>
        /// <param name="StreamSize">The stream size.</param>
        /// <param name="StreamBytesTransferred">The stream bytes transferred.</param>
        /// <param name="dwStreamNumber">The stream number.</param>
        /// <param name="dwCallbackReason">The callback reason.</param>
        /// <param name="hSourceFile">The source file.</param>
        /// <param name="hDestinationFile">The destination file.</param>
        /// <param name="lpData">The data.</param>
        /// <returns>The copy progress result.</returns>
        public delegate CopyProgressResult CopyProgressRoutine(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, 
            uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

        /// <summary>
        /// The CopyProgressResult enum for FileCopy class.
        /// </summary>
        public enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        /// <summary>
        /// The CopyProgressCallbackReason enum for FileCopy class.
        /// </summary>
        public enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        /// <summary>
        /// The CopyFileFlags enum for FileCopy class.
        /// </summary>
        [Flags]
        public enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        /// <summary>
        /// The Copy method for FileCopy class.
        /// </summary>
        /// <param name="oldFile">The old file.</param>
        /// <param name="newFile">The new file.</param>
        /// <param name="callback">The callback.</param>
        public static void Copy(string oldFile, string newFile, CopyProgressRoutine callback)
        {
            int pbCancel = 0;

            CopyFileEx(oldFile, newFile, callback, IntPtr.Zero, ref pbCancel, CopyFileFlags.COPY_FILE_RESTARTABLE);
        }
    }
}
