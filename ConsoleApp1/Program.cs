using System;
using System.IO;

class Program
{
    public static void ListSerialDevices()
    {
        // Get the serial devices from /dev/ directory (ttyUSB* and ttyACM*)
        string[] serialDevices = Directory.GetFiles("/dev/", "tty*"); // USB to serial devices

        if (serialDevices.Length == 0)
        {
            Console.WriteLine("No USB or ACM serial devices found.");
            return;
        }

        Console.WriteLine("Serial Devices:");
        foreach (string device in serialDevices)
        {
            // For each serial device, fetch the corresponding USB details
            GetUsbDetailsForDevice(device);
        }
    }

    public static void GetUsbDetailsForDevice(string device)
    {
        try
        {
            // Get the symlink in /sys/class/tty/ for this device
            string sysTtyPath = $"/sys/class/tty/{Path.GetFileName(device)}";

            // The USB information is two levels up from the device directory
            string devicePath = new FileInfo(sysTtyPath).LinkTarget;
            string usbDevicePath = Path.GetFullPath(Path.Combine(sysTtyPath, "../",devicePath, "../../../"));

            // Extract USB information from the files in the device path
            if(File.Exists(Path.Combine(usbDevicePath, "idVendor")) && File.Exists(Path.Combine(usbDevicePath, "idProduct")))
            {
                string vendor = File.ReadAllText(Path.Combine(usbDevicePath, "idVendor")).Trim();
                string product = File.ReadAllText(Path.Combine(usbDevicePath, "idProduct")).Trim();
                string serial = File.Exists(Path.Combine(usbDevicePath, "serial"))
                    ? File.ReadAllText(Path.Combine(usbDevicePath, "serial")).Trim()
                    : "Unknown";
                Console.WriteLine($" - {device}");
                Console.WriteLine($"  -> {usbDevicePath}");
                Console.WriteLine($"  -> Vendor: {vendor}, Product: {product}, Serial: {serial}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting USB details for {device}: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        ListSerialDevices();
    }
}
