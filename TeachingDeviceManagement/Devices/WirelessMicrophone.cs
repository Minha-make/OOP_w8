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
    private enum ValueZone
    {
        Safe,
        Warning,
        Critical
    }

    private ValueZone _currentBatteryZone;
    private ValueZone _currentSignalZone;

    /// <summary>
    /// Mức pin hiện tại của micro (0 - 100)
    /// </summary>
    public int BatteryLevel { get; private set; }

    /// <summary>
    /// Chất lượng sóng / tín hiệu hiện tại của micro (0 - 100)
    /// </summary>
    public int SignalQuality { get; private set; }

    /// <summary>
    /// Khởi tạo micro không dây
    /// </summary>
    /// <param name="id">Mã thiết bị</param>
    /// <param name="name">Tên thiết bị</param>
    /// <param name="initialBattery">Mức pin ban đầu (0 - 100, mặc định 100)</param>
    /// <param name="initialSignal">Chất lượng tín hiệu ban đầu (0 - 100, mặc định 100)</param>
    /// <exception cref="InvalidDeviceReadingException">Ném ra khi pin hoặc tín hiệu nằm ngoài khoảng 0-100</exception>
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

        _currentBatteryZone = DetermineBatteryZone(initialBattery);
        _currentSignalZone = DetermineSignalZone(initialSignal);
    }

    /// <summary>
    /// Cập nhật mức pin của micro không dây.
    /// - Khi pin bằng 0%: phát sinh cảnh báo Critical (chống phát lặp nếu vẫn ở mức 0%).
    /// - Khi pin dưới 30%: phát sinh cảnh báo Warning khi chuyển vào vùng cảnh báo.
    /// - Khi pin >= 30%: vùng an toàn, không phát cảnh báo.
    /// </summary>
    /// <param name="newLevel">Mức pin mới (0 - 100)</param>
    /// <exception cref="InvalidDeviceReadingException">Ném ra khi newLevel nằm ngoài khoảng 0-100</exception>
    public void UpdateBatteryLevel(int newLevel)
    {
        if (newLevel < 0 || newLevel > 100)
        {
            throw new InvalidDeviceReadingException(Id, newLevel);
        }

        BatteryLevel = newLevel;
        ValueZone newZone = DetermineBatteryZone(newLevel);

        if (newZone == ValueZone.Critical)
        {
            if (_currentBatteryZone != ValueZone.Critical)
            {
                _currentBatteryZone = ValueZone.Critical;
                RaiseAlert($"Micro không dây '{Name}' đã cạn pin (0%)!", AlertSeverity.Critical);
            }
        }
        else if (newZone == ValueZone.Warning)
        {
            if (_currentBatteryZone != ValueZone.Warning)
            {
                _currentBatteryZone = ValueZone.Warning;
                RaiseAlert($"Micro không dây '{Name}' mức pin yếu ({newLevel}%)!", AlertSeverity.Warning);
            }
        }
        else
        {
            _currentBatteryZone = ValueZone.Safe;
        }
    }

    /// <summary>
    /// Cập nhật chất lượng tín hiệu của micro không dây.
    /// - Khi tín hiệu bằng 0%: phát sinh cảnh báo Critical (chống phát lặp nếu vẫn ở mức 0%).
    /// - Khi tín hiệu dưới 40%: phát sinh cảnh báo Warning khi chuyển vào vùng cảnh báo.
    /// - Khi tín hiệu >= 40%: vùng an toàn, không phát cảnh báo.
    /// </summary>
    /// <param name="newQuality">Chất lượng tín hiệu mới (0 - 100)</param>
    /// <exception cref="InvalidDeviceReadingException">Ném ra khi newQuality nằm ngoài khoảng 0-100</exception>
    public void UpdateSignalQuality(int newQuality)
    {
        if (newQuality < 0 || newQuality > 100)
        {
            throw new InvalidDeviceReadingException(Id, newQuality);
        }

        SignalQuality = newQuality;
        ValueZone newZone = DetermineSignalZone(newQuality);

        if (newZone == ValueZone.Critical)
        {
            if (_currentSignalZone != ValueZone.Critical)
            {
                _currentSignalZone = ValueZone.Critical;
                RaiseAlert($"Micro không dây '{Name}' mất hoàn toàn tín hiệu kết nối (0%)!", AlertSeverity.Critical);
            }
        }
        else if (newZone == ValueZone.Warning)
        {
            if (_currentSignalZone != ValueZone.Warning)
            {
                _currentSignalZone = ValueZone.Warning;
                RaiseAlert($"Micro không dây '{Name}' chất lượng tín hiệu kém ({newQuality}%)!", AlertSeverity.Warning);
            }
        }
        else
        {
            _currentSignalZone = ValueZone.Safe;
        }
    }

    private static ValueZone DetermineBatteryZone(int battery)
    {
        if (battery == 0)
        {
            return ValueZone.Critical;
        }

        if (battery < 30)
        {
            return ValueZone.Warning;
        }

        return ValueZone.Safe;
    }

    private static ValueZone DetermineSignalZone(int signal)
    {
        if (signal == 0)
        {
            return ValueZone.Critical;
        }

        if (signal < 40)
        {
            return ValueZone.Warning;
        }

        return ValueZone.Safe;
    }
}
