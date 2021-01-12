using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

// Copy-pasted from BanGround-Unity repo, enjoy the leaked code lol
namespace BanGround.Utils
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    public class SelectFileDialog
    {
        OPENFILENAMEW ofnw;

        public bool IsSucessful { get; private set; } = false;
        public string File => ofnw.File;

        public SelectFileDialog()
        {
            ofnw = new OPENFILENAMEW
            {
                File = new string(new char[256]),
                MaxFile = 256,
                FileTitle = new string(new char[64]),
                MaxFileTitle = 64,
                Flags = Win32.OFN_EXPLORER | Win32.OFN_PATHMUSTEXIST | Win32.OFN_NOCHANGEDIR
            };

            ofnw.Size = Marshal.SizeOf(ofnw);
        }

        public SelectFileDialog SetFilter(string filter)
        {
            ofnw.Filter = filter;
            return this;
        }

        public SelectFileDialog SetTitle(string title)
        {
            ofnw.Title = title;
            return this;
        }

        public SelectFileDialog SetDefaultExt(string ext)
        {
            ofnw.DefaultExt = ext;
            return this;
        }

        public SelectFileDialog Show()
        {
            IsSucessful = Win32.OpenFileDialog(ref ofnw);
            return this;
        }

        public async Task<SelectFileDialog> ShowAsync()
        {
            await Task.Run(() => IsSucessful = Win32.OpenFileDialog(ref ofnw));
            return this;
        }
    }

    public class SaveFileDialog
    {
        OPENFILENAMEW ofnw;

        public bool IsSucessful { get; private set; } = false;
        public string File => ofnw.File;

        public SaveFileDialog()
        {
            ofnw = new OPENFILENAMEW
            {
                File = new string(new char[256]),
                MaxFile = 256,
                FileTitle = new string(new char[64]),
                MaxFileTitle = 64,
                Flags = Win32.OFN_EXPLORER | Win32.OFN_NOCHANGEDIR | Win32.OFN_HIDEREADONLY
            };

            ofnw.Size = Marshal.SizeOf(ofnw);
        }

        public SaveFileDialog SetFilter(string filter)
        {
            ofnw.Filter = filter;
            return this;
        }

        public SaveFileDialog SetTitle(string title)
        {
            ofnw.Title = title;
            return this;
        }

        public SaveFileDialog SetDefaultExt(string ext)
        {
            ofnw.DefaultExt = ext;
            return this;
        }

        public SaveFileDialog Show()
        {
            IsSucessful = Win32.OpenFileDialog(ref ofnw);
            return this;
        }

        public async Task<SaveFileDialog> ShowAsync()
        {
            await Task.Run(() => IsSucessful = Win32.SaveFileDialog(ref ofnw));
            return this;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OPENFILENAMEW
    {
        public int Size;
        public IntPtr DialogOwner;
        public IntPtr Instance;
        public String Filter;
        public String CustomFilter;
        public int MaxCustFilter;
        public int FilterIndex;
        public String File;
        public int MaxFile;
        public String FileTitle;
        public int MaxFileTitle;
        public String InitialDir;
        public String Title;
        public int Flags;
        public short FileOffset;
        public short FileExtension;
        public String DefaultExt;
        public IntPtr CustomData;
        public IntPtr Hook;
        public String TemplateName;
        public IntPtr ReservedPtr;
        public int ReservedInt;
        public int FlagsEx;
    }

    public static class Win32
    {
        public const int OFN_READONLY = 0x00000001;
        public const int OFN_OVERWRITEPROMPT = 0x00000002;
        public const int OFN_HIDEREADONLY = 0x00000004;
        public const int OFN_NOCHANGEDIR = 0x00000008;
        public const int OFN_SHOWHELP = 0x00000010;
        public const int OFN_ENABLEHOOK = 0x00000020;
        public const int OFN_ENABLETEMPLATE = 0x00000040;
        public const int OFN_ENABLETEMPLATEHANDLE = 0x00000080;
        public const int OFN_NOVALIDATE = 0x00000100;
        public const int OFN_ALLOWMULTISELECT = 0x00000200;
        public const int OFN_EXTENSIONDIFFERENT = 0x00000400;
        public const int OFN_PATHMUSTEXIST = 0x00000800;
        public const int OFN_FILEMUSTEXIST = 0x00001000;
        public const int OFN_CREATEPROMPT = 0x00002000;
        public const int OFN_SHAREAWARE = 0x00004000;
        public const int OFN_NOREADONLYRETURN = 0x00008000;
        public const int OFN_NOTESTFILECREATE = 0x00010000;
        public const int OFN_NONETWORKBUTTON = 0x00020000;
        public const int OFN_NOLONGNAMES = 0x00040000;   // force no long names for 4.x modules
        public const int OFN_EXPLORER = 0x00080000;   // new look commdlg
        public const int OFN_NODEREFERENCELINKS = 0x00100000;
        public const int OFN_LONGNAMES = 0x00200000;   // force long names for 3.x modules
        public const int OFN_ENABLEINCLUDENOTIFY = 0x00400000;   // send include message to callback
        public const int OFN_ENABLESIZING = 0x00800000;
        public const int OFN_DONTADDTORECENT = 0x02000000;
        public const int OFN_FORCESHOWHIDDEN = 0x10000000;  // Show All files including System and hidden files
        public const int OFN_EX_NOPLACESBAR = 0x00000001;
        public const int OFN_SHAREFALLTHROUGH = 2;
        public const int OFN_SHARENOWARN = 1;
        public const int OFN_SHAREWARN = 0;

        [DllImport("Comdlg32.dll", EntryPoint = "GetOpenFileNameW", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
        public static extern bool OpenFileDialog(ref OPENFILENAMEW lpofnw);

        [DllImport("Comdlg32.dll", EntryPoint = "GetSaveFileNameW", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
        public static extern bool SaveFileDialog(ref OPENFILENAMEW lpofnw);

        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern int SHOpenFolderAndSelectItems(
        IntPtr pidlFolder,
        uint cidl,
        [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
        uint dwFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ILCreateFromPath([MarshalAs(UnmanagedType.LPTStr)] string pszPath);

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        public interface IShellLinkW
        {
            [PreserveSig]
            int GetPath(StringBuilder pszFile, int cch, [In, Out] ref WIN32_FIND_DATAW pfd, uint fFlags);

            [PreserveSig]
            int GetIDList([Out] out IntPtr ppidl);

            [PreserveSig]
            int SetIDList([In] ref IntPtr pidl);

            [PreserveSig]
            int GetDescription(StringBuilder pszName, int cch);

            [PreserveSig]
            int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [PreserveSig]
            int GetWorkingDirectory(StringBuilder pszDir, int cch);

            [PreserveSig]
            int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            [PreserveSig]
            int GetArguments(StringBuilder pszArgs, int cch);

            [PreserveSig]
            int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            [PreserveSig]
            int GetHotkey([Out] out ushort pwHotkey);

            [PreserveSig]
            int SetHotkey(ushort wHotkey);

            [PreserveSig]
            int GetShowCmd([Out] out int piShowCmd);

            [PreserveSig]
            int SetShowCmd(int iShowCmd);

            [PreserveSig]
            int GetIconLocation(StringBuilder pszIconPath, int cch, [Out] out int piIcon);

            [PreserveSig]
            int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            [PreserveSig]
            int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

            [PreserveSig]
            int Resolve(IntPtr hwnd, uint fFlags);

            [PreserveSig]
            int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), BestFitMapping(false)]
        public struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        public static void OpenFolderAndSelectFiles(string folder, params string[] filesToSelect)
        {
            IntPtr dir = ILCreateFromPath(folder);

            var filesToSelectIntPtrs = new IntPtr[filesToSelect.Length];
            for (int i = 0; i < filesToSelect.Length; i++)
            {
                filesToSelectIntPtrs[i] = ILCreateFromPath(filesToSelect[i]);
            }

            SHOpenFolderAndSelectItems(dir, (uint)filesToSelect.Length, filesToSelectIntPtrs, 0);
            ReleaseComObject(dir);
            ReleaseComObject(filesToSelectIntPtrs);
        }

        private static void ReleaseComObject(params object[] comObjs)
        {
            foreach (object obj in comObjs)
            {
                if (obj != null && Marshal.IsComObject(obj))
                    Marshal.ReleaseComObject(obj);
            }
        }
    }
#endif
}

