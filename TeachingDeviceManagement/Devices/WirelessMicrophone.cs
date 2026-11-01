/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Exceptions;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp micro không dây giảng đường
/// </summary>
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; private set; }
    public int SignalQuality { get; private set; }

    public WirelessMicrophone(string id, string name, int initialBattery = 100, int initialSignal = 100)
        : base(id, name)
    {
        if (initialBattery < 0 || initialBattery > 100)
        {
            throw new InvalidDeviceReadingException(id, initialBattery);
        }

        if (initialSignal < 0 || initialSignal > 100)
        {
            throw new InvalidDeviceReadingException(id, initialSignal);
        }

        BatteryLevel = initialBattery;
        SignalQuality = initialSignal;
    }

    /// <summary>
    /// Cập nhật mức pin của micro không dây.
    /// - Khi pin bằng 0%: phát sinh cảnh báo Critical.
    /// - Khi pin dưới 30%: phát sinh cảnh báo Warning.
    /// </summary>
    public void UpdateBatteryLevel(int newLevel)
    {
        if (newLevel < 0 || newLevel > 100)
        {
            throw new InvalidDeviceReadingException(Id, newLevel);
        }

        BatteryLevel = newLevel;

        if (newLevel == 0)
        {
            RaiseAlert($"Micro không dây '{Name}' đã cạn pin (0%)!", AlertSeverity.Critical);
        }
        else if (newLevel < 30)
        {
            RaiseAlert($"Micro không dây '{Name}' mức pin yếu ({newLevel}%)!", AlertSeverity.Warning);
        }
    }

    /// <summary>
    /// Cập nhật chất lượng tín hiệu của micro không dây.
    /// - Khi tín hiệu bằng 0%: phát sinh cảnh báo Critical.
    /// - Khi tín hiệu dưới 40%: phát sinh cảnh báo Warning.
    /// </summary>
    public void UpdateSignalQuality(int newQuality)
    {
        if (newQuality < 0 || newQuality > 100)
        {
            throw new InvalidDeviceReadingException(Id, newQuality);
        }

        SignalQuality = newQuality;

        if (newQuality == 0)
        {
            RaiseAlert($"Micro không dây '{Name}' mất hoàn toàn tín hiệu kết nối (0%)!", AlertSeverity.Critical);
        }
        else if (newQuality < 40)
        {
            RaiseAlert($"Micro không dây '{Name}' chất lượng tín hiệu kém ({newQuality}%)!", AlertSeverity.Warning);
        }
    }
}
