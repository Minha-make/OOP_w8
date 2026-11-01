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
    public void Subscribe_CalledMultipleTimes_ShouldOnlyAttachSubscribersOnce_NoDuplicateEvents()
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

        // Act: Gọi Subscribe nhiều lần liên tiếp
        coordinator.Subscribe();
        coordinator.Subscribe();
        coordinator.Subscribe();

        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0);

        // Assert: Dù Subscribe 3 lần thì mỗi sự kiện chỉ được xử lý đúng 1 lần (Idempotent)
        Assert.Single(logger.Entries);
        Assert.Single(maintenance.Requests);
    }

    [Fact]
    public void Unsubscribe_ShouldDetachSubscribersFromDevices_EvenAfterMultipleSubscribes()
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
        coordinator.Subscribe();
        coordinator.Unsubscribe(); // Hủy đăng ký

        // Act
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0);

        // Assert: Không còn handler nào nhận sự kiện
        Assert.Empty(logger.Entries);
        Assert.Empty(maintenance.Requests);
    }

    [Fact]
    public void Dispose_ShouldCallUnsubscribe_AndBeIdempotent()
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
        coordinator.Dispose();
        coordinator.Dispose(); // Gọi Dispose lần 2 không gây lỗi

        // Act
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.UpdateBatteryLevel(0);

        // Assert
        Assert.Empty(logger.Entries);
        Assert.Empty(maintenance.Requests);
    }

    [Fact]
    public void Subscribe_AfterDispose_ShouldThrowObjectDisposedException()
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

        coordinator.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => coordinator.Subscribe());
    }

    [Fact]
    public void Constructor_NullParameters_ShouldThrowArgumentNullException()
    {
        var devices = new List<TeachingDevice>();
        var notifier = new ConsoleNotificationService();
        var logger = new DeviceAuditLogger();
        var maintenance = new MaintenanceService();

        Assert.Throws<ArgumentNullException>(() => new DeviceMonitoringCoordinator(null!, notifier, logger, maintenance));
        Assert.Throws<ArgumentNullException>(() => new DeviceMonitoringCoordinator(devices, null!, logger, maintenance));
        Assert.Throws<ArgumentNullException>(() => new DeviceMonitoringCoordinator(devices, notifier, null!, maintenance));
        Assert.Throws<ArgumentNullException>(() => new DeviceMonitoringCoordinator(devices, notifier, logger, null!));
    }
}
