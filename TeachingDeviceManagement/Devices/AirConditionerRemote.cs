/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp điều khiển điều hòa nhiệt độ trong phòng học
/// </summary>
public class AirConditionerRemote : TeachingDevice
{
    /// <summary>
    /// Phòng học mà điều khiển này được bàn giao
    /// </summary>
    public string AssignedRoom { get; }

    /// <summary>
    /// Trạng thái phản hồi tín hiệu của điều hòa (true: có phản hồi, false: mất kết nối)
    /// </summary>
    public bool IsResponsive { get; private set; }

    /// <summary>
    /// Khởi tạo điều khiển điều hòa
    /// </summary>
    /// <param name="id">Mã thiết bị</param>
    /// <param name="name">Tên thiết bị</param>
    /// <param name="assignedRoom">Tên phòng học</param>
    /// <exception cref="ArgumentException">Ném ra khi assignedRoom null hoặc rỗng</exception>
    public AirConditionerRemote(string id, string name, string assignedRoom)
        : base(id, name)
    {
        if (string.IsNullOrWhiteSpace(assignedRoom))
        {
            throw new ArgumentException("Phòng được bàn giao không được để trống hoặc chỉ chứa khoảng trắng.", nameof(assignedRoom));
        }

        AssignedRoom = assignedRoom;
        IsResponsive = true; // Khởi tạo ban đầu thiết bị phản hồi bình thường
    }

    /// <summary>
    /// Cập nhật trạng thái phản hồi của điều hòa.
    /// - Khi chuyển từ phản hồi (true) sang không phản hồi (false): phát sinh cảnh báo Critical.
    /// - Không phát sinh lặp lại nếu trạng thái trước đó đã là không phản hồi.
    /// </summary>
    /// <param name="isResponsive">Trạng thái phản hồi mới</param>
    public void UpdateResponseState(bool isResponsive)
    {
        if (IsResponsive && !isResponsive)
        {
            IsResponsive = false;
            RaiseAlert($"Điều khiển điều hòa tại phòng {AssignedRoom} không phản hồi tín hiệu!", AlertSeverity.Critical);
        }
        else
        {
            IsResponsive = isResponsive;
        }
    }
}
