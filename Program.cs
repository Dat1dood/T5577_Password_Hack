using System;
using System.IO.Ports;
using System.Text;
// B7 B6 B5 B4 B3 B2 B1 B0
SerialPort _serialPort;
byte[] cunterB = new byte[4];
byte[] cunterB0 = new byte[1];
byte[] cunterB1 = new byte[1];
byte[] cunterB2 = new byte[1];
byte[] cunterB3 = new byte[1];
String cunterS = "00000000";
String cunterSB0 = "00";
String cunterSB1 = "00";
String cunterSB2 = "00";
String cunterSB3 = "00";
String packetS;
String password = "00000000";
String readfromSerial = "0";
byte[] packetB;

bool firstFFB0 = false;
bool firstFFB1 = false;
bool firstFFB2 = false;

_serialPort = new SerialPort();
_serialPort.PortName = "/dev/tty.usbserial-0001";
_serialPort.BaudRate = 9600;
_serialPort.Parity = Parity.None;
_serialPort.DataBits = 8;
_serialPort.StopBits = StopBits.One;
_serialPort.Handshake = Handshake.None;
_serialPort.ReadTimeout = 500;
_serialPort.WriteTimeout = 500;
_serialPort.Open();

string bodyS = "00000000";

while (true)
{
    packetS = "RP8" + password;
    // // Uncommment to show sent packet
    Console.WriteLine("Sent Packet: \r\n" + packetS);

    // packetB = StringToByteArray(packetS);
    _serialPort.Write(packetS + "\r\n");
    // Wait 20-500ms
    Thread.Sleep(150);

    // Read device response
    while (_serialPort.BytesToRead > 0)
    {
        readfromSerial = _serialPort.ReadLine();
    }
    // readfromSerial = _serialPort.ReadLine();
    // Console.WriteLine("Rec Packet: \r\n" + readfromSerial);

    if (String.Compare("Err", readfromSerial.Substring(0, 3)) != 0)
    {
        Console.WriteLine("Password: " + password);
        Console.WriteLine("Sent Packet: " + packetS);
        Console.WriteLine("Rec Packet: " + readfromSerial);

        Environment.Exit(0);
    }

    if (cunterB3[0] == 0xFF && cunterB2[0] == 0xFF && cunterB1[0] == 0xFF && cunterB0[0] == 0xFF)
    {
        Console.WriteLine("Done!");
        Environment.Exit(0);
    }
    if (cunterB2[0] == 0xFF && cunterB1[0] == 0xFF && cunterB0[0] == 0xFF)
    {
        cunterB3 = StringToByteArray(cunterSB3);
        cunterB3[0]++;
        cunterB2[0] = 0x00;
        cunterSB3 = ByteArrayToString(cunterB3);
    }

    if (cunterB1[0] == 0xFF && cunterB0[0] == 0xFF)
    {
        cunterB2 = StringToByteArray(cunterSB2);
        cunterB2[0]++;
        cunterB1[0] = 0x00;
        cunterSB2 = ByteArrayToString(cunterB2);
    }
    if (cunterB0[0] == 0xFF)
    {
        cunterB1 = StringToByteArray(cunterSB1);
        cunterB1[0]++;
        cunterSB1 = ByteArrayToString(cunterB1);

    }
    cunterB0 = StringToByteArray(cunterSB0);
    cunterB0[0]++;
    cunterSB0 = ByteArrayToString(cunterB0);

    password = cunterSB3 + cunterSB2 + cunterSB1 + cunterSB0;
    // Console.WriteLine(password);

}


static string ByteArrayToString(byte[] ba)
{
    StringBuilder hex = new StringBuilder(ba.Length * 2);
    foreach (byte b in ba)
        hex.AppendFormat("{0:x2}", b);
    return hex.ToString();
}
static byte[] StringToByteArray(string hex)
{
    return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
}