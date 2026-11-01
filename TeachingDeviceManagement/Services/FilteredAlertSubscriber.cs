/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber minh họa việc kết hợp giữa Event và Callback thông qua Delegate.
/// Cho phép lọc các cảnh báo theo tiêu chí tùy chỉnh trước khi thực thi một hành động.
/// </summary>
public class FilteredAlertSubscriber
{
    private readonly AlertFilter _filter;
    private readonly Action<DeviceAlertEventArgs> _action;

    /// <summary>
    /// Khởi tạo subscriber với bộ lọc và hành động tương ứng
    /// </summary>
    /// <param name="filter">Delegate lọc điều kiện cảnh báo (không được null)</param>
    /// <param name="action">Action callback thực thi khi thỏa điều kiện lọc (không được null)</param>
    /// <exception cref="ArgumentNullException">Ném ra khi filter hoặc action là null</exception>
    public FilteredAlertSubscriber(
        AlertFilter filter,
        Action<DeviceAlertEventArgs> action)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(action);

        _filter = filter;
        _action = action;
    }

    /// <summary>
    /// Xử lý sự kiện cảnh báo nhận được từ thiết bị.
    /// Nếu cảnh báo thỏa mãn delegate _filter thì kích hoạt delegate _action.
    /// </summary>
    /// <param name="sender">Đối tượng phát sinh sự kiện</param>
    /// <param name="e">Dữ liệu cảnh báo</param>
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (_filter(e))
        {
            _action(e);
        }
    }
}
