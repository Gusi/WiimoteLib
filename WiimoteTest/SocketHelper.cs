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
	public struct Status
	{
		public float X;
		public float Y;

		public struct ButtonsStruct
		{ 
			public byte A;
			public byte B;
			public byte Minus;
			public byte Plus;
			public byte Home;
			public byte One;
			public byte Two;
			public byte C;
			public byte Z;
			public byte Left;
			public byte Right;
			public byte Up;
			public byte Down;
		}

		public ButtonsStruct Buttons;

		public static bool operator ==(Status c1, Status c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(Status c1, Status c2)
		{
			return !(c1 == c2);
		}
		/*
		public bool ButtonsEqual(Status c1)
		{
			return this.Buttons.Equals(c1.Buttons);
		}

		public bool IsDeadZone()
		{
			return Math.Abs(X) < 0.02 && Math.Abs(Y) < 0.02;
		}
		*/
		public void Normalize()
		{
			if (Math.Abs(X) < 0.015) X = 0.0f;
			if (Math.Abs(Y) < 0.015) Y = 0.0f;
		}
	}

	class SocketHelper
	{
		UdpClient _client;
		IFormatter formatter = new BinaryFormatter();
		Status _status = new Status();
		Status _lastStatus = new Status();

		public SocketHelper()
		{
			_client = new UdpClient();
			_client.Connect("localhost", 3002);
		}

		public void SendData(WiimoteChangedEventArgs args)
		{
			_status.X = args.WiimoteState.NunchukState.Joystick.X;
			_status.Y = args.WiimoteState.NunchukState.Joystick.Y;
			_status.Buttons.A = (byte)(args.WiimoteState.ButtonState.A ? 1 : 0);
			_status.Buttons.B = (byte)(args.WiimoteState.ButtonState.B ? 1 : 0);
			_status.Buttons.Minus = (byte)(args.WiimoteState.ButtonState.Minus ? 1 : 0);
			_status.Buttons.Plus = (byte)(args.WiimoteState.ButtonState.Plus ? 1 : 0);
			_status.Buttons.Home = (byte)(args.WiimoteState.ButtonState.Home ? 1 : 0);
			_status.Buttons.One = (byte)(args.WiimoteState.ButtonState.One ? 1 : 0);
			_status.Buttons.Two = (byte)(args.WiimoteState.ButtonState.Two ? 1 : 0);
			_status.Buttons.C = (byte)(args.WiimoteState.NunchukState.C ? 1 : 0);
			_status.Buttons.Z = (byte)(args.WiimoteState.NunchukState.Z ? 1 : 0);
			_status.Buttons.Left = (byte)(args.WiimoteState.ButtonState.Left ? 1 : 0);
			_status.Buttons.Right = (byte)(args.WiimoteState.ButtonState.Right ? 1 : 0);
			_status.Buttons.Up = (byte)(args.WiimoteState.ButtonState.Up ? 1 : 0);
			_status.Buttons.Down = (byte)(args.WiimoteState.ButtonState.Down ? 1 : 0);

			_status.Normalize();


			//if (!_lastStatus.ButtonsEqual(_status) || 
			//	(_lastStatus != _status && (!_status.IsDeadZone() || (_status.IsDeadZone() != _lastStatus.IsDeadZone()))))
			if (_lastStatus != _status)
			{
				_lastStatus = _status;
				Send(StructureToByteArray(_status));
			}
		}

		public void SendData(WiimoteExtensionChangedEventArgs args)
		{
			//Send(StructureToByteArray(args));
		}

		void Send(byte[] a)
		{
			_client.Send(a, a.Length);
		}
		/*
		byte[] StructureToByteArray(object obj)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, obj);
				return stream.ToArray();
			}
		}*/

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
	}
}
