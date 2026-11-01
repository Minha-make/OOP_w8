/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber hiển thị cảnh báo từ thiết bị ra màn hình console theo định dạng chuẩn
/// </summary>
public class ConsoleNotificationService
{
    /// <summary>
    /// Xử lý sự kiện cảnh báo từ thiết bị và xuất ra màn hình console
    /// </summary>
    /// <param name="sender">Đối tượng thiết bị gửi cảnh báo</param>
    /// <param name="e">Dữ liệu cảnh báo</param>
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        Console.WriteLine($"[{e.Severity}] {e.DeviceId}: {e.Message}");
    }
}
