/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Lớp điều phối đăng ký và hủy đăng ký các dịch vụ lắng nghe sự kiện từ thiết bị
/// </summary>
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> devices;
    private readonly ConsoleNotificationService notificationService;
    private readonly DeviceAuditLogger auditLogger;
    private readonly MaintenanceService maintenanceService;

    public DeviceMonitoringCoordinator(
        IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService,
        DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        this.devices = devices.ToList();
        this.notificationService = notificationService;
        this.auditLogger = auditLogger;
        this.maintenanceService = maintenanceService;
    }

    /// <summary>
    /// Đăng ký các handler của các service vào các event của thiết bị
    /// </summary>
    public void Subscribe()
    {
        foreach (var device in devices)
        {
            device.StatusChanged += auditLogger.HandleStatusChanged;
            device.AlertRaised += notificationService.HandleAlert;
            device.AlertRaised += maintenanceService.HandleCriticalAlert;
        }
    }

    /// <summary>
    /// Hủy đăng ký các handler đã gắn vào event của thiết bị
    /// </summary>
    public void Unsubscribe()
    {
        foreach (var device in devices)
        {
            device.StatusChanged -= auditLogger.HandleStatusChanged;
            device.AlertRaised -= notificationService.HandleAlert;
            device.AlertRaised -= maintenanceService.HandleCriticalAlert;
        }
    }

    /// <summary>
    /// Thực hiện hủy đăng ký khi kết thúc vòng đời của coordinator
    /// </summary>
    public void Dispose()
    {
        Unsubscribe();
    }
}
