using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Interop
{
    static class Program
    {
		private static void Test() {
			//SetDllDirectory(@"E:\reko\llvm\build\out\bin");

			TextWriter tw = Console.Out;
			var chg = new CppHeaderGenerator(tw);
			chg.Generate(new[]
				{
					typeof(PrimitiveOp).FullName,
					typeof(DataTypeEnum).FullName,
					typeof(IFactory).FullName,
				});

			Factory fac = new Factory();
			var factory = Marshal.GetIUnknownForObject(fac);
			var iid = new Guid("E40FFD0D-3019-4ADF-AC48-800F3ACFA360");
			IntPtr ifac;
			var hr = Marshal.QueryInterface(factory, ref iid, out ifac);
			Initialize(ifac);

			TestTypes();

#if TEST_LLVM
            var bytes = new byte[30];
            ulong addr = 0x00123400;
            bytes[0] = 0xC3;
			Initialize(ifac);
			SetupTarget("i386");
			Disasm(addr, bytes, 1);
#endif


			/*
            Console.WriteLine(fac.stmts[0].ToString());
            Debug.Print(fac.stmts[0].ToString());
            Debug.Assert(fac.stmts.Count == 1);
			*/
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
        {
			bool doInterface = true;
			if (args.Length > 0 && args[0] != "i")
				doInterface = false;

			if (doInterface)
				Test();
			//TODO: struct header
			/*else
				GenerateStructType(args[0]);*/
		}

#if __MonoCS__
        [DllImport("driver.so", CallingConvention = CallingConvention.Cdecl)]
#else
#endif

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize([In] IntPtr factory);

#if TEST_LLVM
		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SetupTarget([MarshalAs(UnmanagedType.LPStr)] string TripleName);

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Disasm(UInt64 PC, [MarshalAs(UnmanagedType.LPArray)] byte[] bytes, int size);
#endif

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool SetDllDirectory(string lpPathName);

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void TestTypes();
	}
}
