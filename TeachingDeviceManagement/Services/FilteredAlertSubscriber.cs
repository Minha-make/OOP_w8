/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber lọc thông báo sử dụng delegate AlertFilter trước khi kích hoạt hành động xử lý
/// </summary>
public class FilteredAlertSubscriber
{
    private readonly AlertFilter filter;
    private readonly Action<DeviceAlertEventArgs> action;

    public FilteredAlertSubscriber(
        AlertFilter filter,
        Action<DeviceAlertEventArgs> action)
    {
        this.filter = filter;
        this.action = action;
    }

    /// <summary>
    /// Xử lý sự kiện cảnh báo: kiểm tra điều kiện bộ lọc trước khi thực thi callback
    /// </summary>
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (filter(e))
        {
            action(e);
        }
    }
}
