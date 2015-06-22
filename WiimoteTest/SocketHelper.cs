using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using WiimoteLib;

namespace WiimoteTest
{
	class SocketHelper
	{
		UdpClient _client;
		IFormatter formatter = new BinaryFormatter();
		public SocketHelper()
		{
			_client = new UdpClient();
			_client.Connect("localhost", 3002);
		}

		public void SendData(WiimoteChangedEventArgs args)
		{
			Send(StructureToByteArray(args.WiimoteState));
		}

		public void SendData(WiimoteExtensionChangedEventArgs args)
		{
			//Send(StructureToByteArray(args));
		}

		void Send(byte[] a)
		{
			_client.Send(a, a.Length);
		}

		byte[] StructureToByteArray(object obj)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, obj);
				return stream.ToArray();
			}
		}
		/*
		byte[] StructureToByteArray(object obj)
		{
			int len = Marshal.SizeOf(obj);

			byte[] arr = new byte[len];
			
			IntPtr ptr = Marshal.AllocHGlobal(len);

			Marshal.StructureToPtr(obj, ptr, true);

			Marshal.Copy(ptr, arr, 0, len);

			Marshal.FreeHGlobal(ptr);
			
			return arr;
		}

		void ByteArrayToStructure(byte[] bytearray, ref object obj)
		{
			int len = Marshal.SizeOf(obj);

			IntPtr i = Marshal.AllocHGlobal(len);

			Marshal.Copy(bytearray, 0, i, len);

			obj = Marshal.PtrToStructure(i, obj.GetType());

			Marshal.FreeHGlobal(i);
		}
		*/
	}
}
