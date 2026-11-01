/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Lớp điều phối tập trung việc đăng ký và hủy đăng ký các dịch vụ giám sát thiết bị.
/// Giúp loại bỏ việc rải rác các toán tử += và -= trong mã nguồn.
/// Đảm bảo tính idempotent khi Subscribe/Unsubscribe và an toàn khi Dispose.
/// </summary>
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> _devices;
    private readonly ConsoleNotificationService _notificationService;
    private readonly DeviceAuditLogger _auditLogger;
    private readonly MaintenanceService _maintenanceService;

    private bool _isSubscribed;
    private bool _isDisposed;

    /// <summary>
    /// Cho biết Coordinator hiện tại có đang trong trạng thái đăng ký lắng nghe sự kiện hay không
    /// </summary>
    public bool IsSubscribed => _isSubscribed;

    /// <summary>
    /// Khởi tạo Coordinator điều phối giám sát thiết bị
    /// </summary>
    /// <param name="devices">Danh sách thiết bị cần giám sát</param>
    /// <param name="notificationService">Dịch vụ thông báo console</param>
    /// <param name="auditLogger">Dịch vụ ghi log kiểm toán</param>
    /// <param name="maintenanceService">Dịch vụ bảo trì khẩn cấp</param>
    /// <exception cref="ArgumentNullException">Ném ra khi bất kỳ tham số nào là null</exception>
    public DeviceMonitoringCoordinator(
        IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService,
        DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        ArgumentNullException.ThrowIfNull(devices);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(auditLogger);
        ArgumentNullException.ThrowIfNull(maintenanceService);

        _devices = devices.ToList();
        _notificationService = notificationService;
        _auditLogger = auditLogger;
        _maintenanceService = maintenanceService;
        _isSubscribed = false;
        _isDisposed = false;
    }

    /// <summary>
    /// Đăng ký các handler của các service vào các event của thiết bị.
    /// Có cơ chế chống đăng ký trùng lặp (idempotent): nếu đã đăng ký thì bỏ qua.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Ném ra khi gọi trên đối tượng đã bị Dispose</exception>
    public void Subscribe()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_isSubscribed)
        {
            return;
        }

        foreach (var device in _devices)
        {
            device.StatusChanged += _auditLogger.HandleStatusChanged;
            device.AlertRaised += _notificationService.HandleAlert;
            device.AlertRaised += _maintenanceService.HandleCriticalAlert;
        }

        _isSubscribed = true;
    }

    /// <summary>
    /// Hủy đăng ký các handler khỏi event của thiết bị.
    /// Đảm bảo không hủy thừa hoặc lỗi khi chưa từng đăng ký.
    /// </summary>
    public void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        foreach (var device in _devices)
        {
            device.StatusChanged -= _auditLogger.HandleStatusChanged;
            device.AlertRaised -= _notificationService.HandleAlert;
            device.AlertRaised -= _maintenanceService.HandleCriticalAlert;
        }

        _isSubscribed = false;
    }

    /// <summary>
    /// Giải phóng tài nguyên: tự động gọi Unsubscribe() để tránh rò rỉ bộ nhớ (memory leak).
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Unsubscribe();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
