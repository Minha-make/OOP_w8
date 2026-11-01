/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Exceptions;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp điều khiển máy chiếu trong phòng dạy
/// </summary>
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; private set; }

    public ProjectorRemote(string id, string name, int initialBattery = 100)
        : base(id, name)
    {
        if (initialBattery < 0 || initialBattery > 100)
        {
            throw new InvalidDeviceReadingException(id, initialBattery);
        }

        BatteryLevel = initialBattery;
    }

    /// <summary>
    /// Cập nhật mức pin của điều khiển máy chiếu.
    /// Mức pin phải từ 0 đến 100.
    /// - Khi pin bằng 0%: phát sinh cảnh báo Critical.
    /// - Khi pin dưới 20%: phát sinh cảnh báo Warning.
    /// - Nếu ở vùng an toàn (>= 20%): không phát sinh cảnh báo.
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
            RaiseAlert($"Điều khiển máy chiếu '{Name}' đã cạn pin (0%)!", AlertSeverity.Critical);
        }
        else if (newLevel < 20)
        {
            RaiseAlert($"Điều khiển máy chiếu '{Name}' mức pin yếu ({newLevel}%)!", AlertSeverity.Warning);
        }
    }
}
