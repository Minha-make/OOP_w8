/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using TeachingDeviceManagement.Exceptions;
using Xunit;

namespace TeachingDeviceManagement.Tests;

public class ProjectorRemoteTests
{
    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseWarning_WhenBatteryIsBelow20()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        DeviceAlertEventArgs? alertArgs = null;
        projector.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        projector.UpdateBatteryLevel(15);

        // Assert
        Assert.Equal(15, projector.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
        Assert.Equal("PRJ-01", alertArgs.DeviceId);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseCritical_WhenBatteryIsZero()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        DeviceAlertEventArgs? alertArgs = null;
        projector.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        projector.UpdateBatteryLevel(0);

        // Assert
        Assert.Equal(0, projector.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldThrowInvalidDeviceReadingException_WhenValueIsOutOfRange()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");

        // Act & Assert
        var ex = Assert.Throws<InvalidDeviceReadingException>(() => projector.UpdateBatteryLevel(120));
        Assert.Equal("PRJ-01", ex.DeviceId);
        Assert.Equal(120, ex.InvalidValue);
    }
}
