/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using TeachingDeviceManagement.Exceptions;
using TeachingDeviceManagement.Services;
using Xunit;

namespace TeachingDeviceManagement.Tests;

public class SubscriberTests
{
    [Fact]
    public void DeviceAuditLogger_ShouldRecordStatusChanges()
    {
        // Arrange
        var logger = new DeviceAuditLogger();
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        projector.StatusChanged += logger.HandleStatusChanged;

        // Act
        projector.ChangeStatus(DeviceStatus.Borrowed);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Contains("PRJ-01", logger.Entries[0]);
        Assert.Contains("Available -> Borrowed", logger.Entries[0]);
    }

    [Fact]
    public void MaintenanceService_ShouldRecordOnlyCriticalAlerts()
    {
        // Arrange
        var maintenance = new MaintenanceService();
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        projector.AlertRaised += maintenance.HandleCriticalAlert;

        // Act: Gây ra Warning (15%) -> Không tạo yêu cầu bảo trì
        projector.UpdateBatteryLevel(15);
        Assert.Empty(maintenance.Requests);

        // Act: Gây ra Critical (0%) -> Có tạo yêu cầu bảo trì
        projector.UpdateBatteryLevel(0);

        // Assert
        Assert.Single(maintenance.Requests);
        Assert.Contains("Maintenance request for PRJ-01", maintenance.Requests[0]);
    }

    [Fact]
    public void FilteredAlertSubscriber_ShouldOnlyInvokeAction_WhenFilterMatches()
    {
        // Arrange
        int callCount = 0;
        var subscriber = new FilteredAlertSubscriber(
            alert => alert.Severity == AlertSeverity.Critical,
            alert => callCount++
        );

        var warningAlert = new DeviceAlertEventArgs("DEV-01", "Pin yếu", AlertSeverity.Warning, DateTime.Now);
        var criticalAlert = new DeviceAlertEventArgs("DEV-01", "Hết pin", AlertSeverity.Critical, DateTime.Now);

        // Act
        subscriber.HandleAlert(null, warningAlert);
        Assert.Equal(0, callCount); // Warning không thỏa điều kiện filter

        subscriber.HandleAlert(null, criticalAlert);
        Assert.Equal(1, callCount); // Critical thỏa điều kiện filter
    }

    [Fact]
    public void FilteredAlertSubscriber_NullFilterOrAction_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FilteredAlertSubscriber(null!, alert => { }));
        Assert.Throws<ArgumentNullException>(() => new FilteredAlertSubscriber(alert => true, null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void InvalidDeviceReadingException_NullOrWhitespaceDeviceId_ShouldThrowArgumentException(string? invalidDeviceId)
    {
        Assert.Throws<ArgumentException>(() => new InvalidDeviceReadingException(invalidDeviceId!, 50));
    }
}
