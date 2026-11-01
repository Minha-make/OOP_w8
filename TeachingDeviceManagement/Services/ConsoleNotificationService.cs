/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber hiển thị cảnh báo từ thiết bị ra màn hình console
/// </summary>
public class ConsoleNotificationService
{
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        Console.WriteLine($"[{e.Severity}] {e.DeviceId}: {e.Message}");
    }
}
