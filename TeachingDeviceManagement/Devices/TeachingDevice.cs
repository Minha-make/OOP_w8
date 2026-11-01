/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp cơ sở trừu tượng định nghĩa các thuộc tính và hành vi chung cho các thiết bị phòng dạy.
/// Đảm bảo đóng gói event: chỉ nội bộ lớp thiết bị mới có quyền phát sinh event.
/// </summary>
public abstract class TeachingDevice
{
    /// <summary>
    /// Mã định danh duy nhất của thiết bị
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Tên gọi hoặc mô tả thiết bị
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Trạng thái mượn / khả dụng hiện tại của thiết bị (chỉ cập nhật nội bộ)
    /// </summary>
    public DeviceStatus Status { get; private set; }

    /// <summary>
    /// Sự kiện thông báo khi trạng thái của thiết bị thay đổi
    /// </summary>
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// Sự kiện thông báo khi thiết bị phát sinh cảnh báo (pin, tín hiệu, phản hồi)
    /// </summary>
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;

    /// <summary>
    /// Khởi tạo thiết bị giảng dạy với mã và tên
    /// </summary>
    /// <param name="id">Mã thiết bị (không được rỗng)</param>
    /// <param name="name">Tên thiết bị (không được rỗng)</param>
    /// <exception cref="ArgumentException">Ném ra khi id hoặc name là null hoặc khoảng trắng</exception>
    protected TeachingDevice(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Mã thiết bị không được để trống hoặc chỉ chứa khoảng trắng.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên thiết bị không được để trống hoặc chỉ chứa khoảng trắng.", nameof(name));
        }

        Id = id;
        Name = name;
        Status = DeviceStatus.Available;
    }

    /// <summary>
    /// Thay đổi trạng thái thiết bị.
    /// - Kiểm tra tính hợp lệ của enum DeviceStatus.
    /// - Nếu trạng thái mới trùng với trạng thái hiện tại thì bỏ qua không phát event.
    /// - Cập nhật Status trước khi kích hoạt event StatusChanged.
    /// </summary>
    /// <param name="newStatus">Trạng thái mới cần chuyển đổi</param>
    /// <exception cref="ArgumentOutOfRangeException">Ném ra khi giá trị enum không hợp lệ</exception>
    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(DeviceStatus), newStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(newStatus), "Giá trị trạng thái thiết bị không hợp lệ.");
        }

        if (newStatus == Status)
        {
            return;
        }

        DeviceStatus oldStatus = Status;
        Status = newStatus;

        OnStatusChanged(new DeviceStatusChangedEventArgs(
            Id,
            oldStatus,
            newStatus,
            DateTime.Now));
    }

    /// <summary>
    /// Hàm tiện ích dùng trong nội bộ thiết bị để phát sinh cảnh báo
    /// </summary>
    /// <param name="message">Nội dung thông báo cảnh báo</param>
    /// <param name="severity">Mức độ nghiêm trọng của cảnh báo</param>
    protected void RaiseAlert(string message, AlertSeverity severity)
    {
        OnAlertRaised(new DeviceAlertEventArgs(
            Id,
            message,
            severity,
            DateTime.Now));
    }

    /// <summary>
    /// Kích hoạt sự kiện StatusChanged một cách an toàn và độc lập.
    /// Duyệt qua danh sách invocation để đảm bảo nếu một handler ném exception,
    /// các handler tiếp theo vẫn được thực thi đầy đủ. Sau đó ném AggregateException nếu có lỗi.
    /// </summary>
    /// <param name="e">Dữ liệu sự kiện thay đổi trạng thái</param>
    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
    {
        var invocationList = StatusChanged?.GetInvocationList();
        if (invocationList == null || invocationList.Length == 0)
        {
            return;
        }

        List<Exception>? exceptions = null;
        foreach (var handler in invocationList)
        {
            try
            {
                ((EventHandler<DeviceStatusChangedEventArgs>)handler).Invoke(this, e);
            }
            catch (Exception ex)
            {
                exceptions ??= new List<Exception>();
                exceptions.Add(ex);
            }
        }

        if (exceptions != null && exceptions.Count > 0)
        {
            throw new AggregateException("Đã xảy ra một hoặc nhiều ngoại lệ từ subscriber khi xử lý StatusChanged.", exceptions);
        }
    }

    /// <summary>
    /// Kích hoạt sự kiện AlertRaised một cách an toàn và độc lập.
    /// Duyệt qua danh sách invocation để đảm bảo nếu một handler ném exception,
    /// các subscriber khác (như bảo trì, ghi log) vẫn nhận được cảnh báo.
    /// </summary>
    /// <param name="e">Dữ liệu sự kiện cảnh báo</param>
    protected virtual void OnAlertRaised(DeviceAlertEventArgs e)
    {
        var invocationList = AlertRaised?.GetInvocationList();
        if (invocationList == null || invocationList.Length == 0)
        {
            return;
        }

        List<Exception>? exceptions = null;
        foreach (var handler in invocationList)
        {
            try
            {
                ((EventHandler<DeviceAlertEventArgs>)handler).Invoke(this, e);
            }
            catch (Exception ex)
            {
                exceptions ??= new List<Exception>();
                exceptions.Add(ex);
            }
        }

        if (exceptions != null && exceptions.Count > 0)
        {
            throw new AggregateException("Đã xảy ra một hoặc nhiều ngoại lệ từ subscriber khi xử lý AlertRaised.", exceptions);
        }
    }
}
