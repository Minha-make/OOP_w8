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
    private enum BatteryZone
    {
        Safe,       // >= 20%
        Warning,    // 1% - 19%
        Critical    // 0%
    }

    private BatteryZone _currentZone;

    /// <summary>
    /// Mức pin hiện tại của điều khiển (0 - 100)
    /// </summary>
    public int BatteryLevel { get; private set; }

    /// <summary>
    /// Khởi tạo điều khiển máy chiếu
    /// </summary>
    /// <param name="id">Mã thiết bị</param>
    /// <param name="name">Tên thiết bị</param>
    /// <param name="initialBattery">Mức pin ban đầu (0 - 100, mặc định 100)</param>
    /// <exception cref="InvalidDeviceReadingException">Ném ra khi mức pin ban đầu không hợp lệ</exception>
    public ProjectorRemote(string id, string name, int initialBattery = 100)
        : base(id, name)
    {
        if (initialBattery < 0 || initialBattery > 100)
        {
            throw new InvalidDeviceReadingException(id, initialBattery);
        }

        BatteryLevel = initialBattery;
        _currentZone = DetermineZone(initialBattery);
    }

    /// <summary>
    /// Cập nhật mức pin của điều khiển máy chiếu.
    /// Quy tắc:
    /// - Mức pin phải từ 0 đến 100.
    /// - Khi pin bằng 0%: phát sinh cảnh báo Critical (chống phát lặp nếu vẫn ở mức 0%).
    /// - Khi pin dưới 20%: phát sinh cảnh báo Warning khi chuyển từ vùng khác vào vùng cảnh báo.
    /// - Nếu ở trong vùng an toàn (>= 20%): không phát sinh cảnh báo.
    /// </summary>
    /// <param name="newLevel">Mức pin mới (0 - 100)</param>
    /// <exception cref="InvalidDeviceReadingException">Ném ra khi newLevel nằm ngoài 0-100</exception>
    public void UpdateBatteryLevel(int newLevel)
    {
        if (newLevel < 0 || newLevel > 100)
        {
            throw new InvalidDeviceReadingException(Id, newLevel);
        }

        BatteryLevel = newLevel;
        BatteryZone newZone = DetermineZone(newLevel);

        if (newZone == BatteryZone.Critical)
        {
            if (_currentZone != BatteryZone.Critical)
            {
                _currentZone = BatteryZone.Critical;
                RaiseAlert($"Điều khiển máy chiếu '{Name}' đã cạn pin (0%)!", AlertSeverity.Critical);
            }
        }
        else if (newZone == BatteryZone.Warning)
        {
            if (_currentZone != BatteryZone.Warning)
            {
                _currentZone = BatteryZone.Warning;
                RaiseAlert($"Điều khiển máy chiếu '{Name}' mức pin yếu ({newLevel}%)!", AlertSeverity.Warning);
            }
        }
        else
        {
            // Vùng an toàn (>= 20%): cập nhật vùng, không phát cảnh báo
            _currentZone = BatteryZone.Safe;
        }
    }

    private static BatteryZone DetermineZone(int battery)
    {
        if (battery == 0)
        {
            return BatteryZone.Critical;
        }

        if (battery < 20)
        {
            return BatteryZone.Warning;
        }

        return BatteryZone.Safe;
    }
}
