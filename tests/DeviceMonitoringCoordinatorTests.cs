/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Services;
using Xunit;

namespace TeachingDeviceManagement.Tests;

public class DeviceMonitoringCoordinatorTests
{
    [Fact]
    public void Subscribe_ShouldAttachSubscribersToDevices()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu P101");
        var notifier = new ConsoleNotificationService();
        var logger = new DeviceAuditLogger();
        var maintenance = new MaintenanceService();

        var coordinator = new DeviceMonitoringCoordinator(
            new[] { projector },
            notifier,
            logger,
            maintenance);

        // Act
        coordinator.Subscribe();
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0); // Gây ra Critical alert

        // Assert
        Assert.Single(logger.Entries);
        Assert.Single(maintenance.Requests);
    }

    [Fact]
    public void Unsubscribe_ShouldDetachSubscribersFromDevices()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu P101");
        var notifier = new ConsoleNotificationService();
        var logger = new DeviceAuditLogger();
        var maintenance = new MaintenanceService();

        var coordinator = new DeviceMonitoringCoordinator(
            new[] { projector },
            notifier,
            logger,
            maintenance);

        coordinator.Subscribe();
        coordinator.Unsubscribe(); // Hủy đăng ký ngay sau đó

        // Act
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0);

        // Assert: Không ghi nhận thêm bất kỳ log hay request nào
        Assert.Empty(logger.Entries);
        Assert.Empty(maintenance.Requests);
    }

    [Fact]
    public void Dispose_ShouldCallUnsubscribe()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu P101");
        var notifier = new ConsoleNotificationService();
        var logger = new DeviceAuditLogger();
        var maintenance = new MaintenanceService();

        using (var coordinator = new DeviceMonitoringCoordinator(
            new[] { projector },
            notifier,
            logger,
            maintenance))
        {
            coordinator.Subscribe();
        } // coordinator.Dispose() được gọi tại đây

        // Act: Kích hoạt event sau khi dispose
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0);

        // Assert
        Assert.Empty(logger.Entries);
        Assert.Empty(maintenance.Requests);
    }
}
