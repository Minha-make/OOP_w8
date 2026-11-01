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

public class WirelessMicrophoneTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(29)]
    public void UpdateBatteryLevel_ShouldRaiseWarning_WhenBelow30(int batteryLevel)
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateBatteryLevel(batteryLevel);

        // Assert
        Assert.Equal(batteryLevel, mic.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseCritical_WhenZero()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateBatteryLevel(0);

        // Assert
        Assert.Equal(0, mic.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(31)]
    [InlineData(100)]
    public void UpdateBatteryLevel_ShouldNotRaiseAlert_WhenInSafeZone(int safeLevel)
    {
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        bool alertRaised = false;
        mic.AlertRaised += (sender, e) => alertRaised = true;

        mic.UpdateBatteryLevel(safeLevel);

        Assert.False(alertRaised);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(39)]
    public void UpdateSignalQuality_ShouldRaiseWarning_WhenBelow40(int signalQuality)
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateSignalQuality(signalQuality);

        // Assert
        Assert.Equal(signalQuality, mic.SignalQuality);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
    }

    [Fact]
    public void UpdateSignalQuality_ShouldRaiseCritical_WhenZero()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateSignalQuality(0);

        // Assert
        Assert.Equal(0, mic.SignalQuality);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Theory]
    [InlineData(40)]
    [InlineData(41)]
    [InlineData(100)]
    public void UpdateSignalQuality_ShouldNotRaiseAlert_WhenInSafeZone(int safeQuality)
    {
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        bool alertRaised = false;
        mic.AlertRaised += (sender, e) => alertRaised = true;

        mic.UpdateSignalQuality(safeQuality);

        Assert.False(alertRaised);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateBatteryLevel_ShouldThrowInvalidDeviceReadingException_WhenValueOutOfRange(int invalidLevel)
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");

        // Act & Assert
        var ex = Assert.Throws<InvalidDeviceReadingException>(() => mic.UpdateBatteryLevel(invalidLevel));
        Assert.Equal("MIC-01", ex.DeviceId);
        Assert.Equal(invalidLevel, ex.InvalidValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateSignalQuality_ShouldThrowInvalidDeviceReadingException_WhenValueOutOfRange(int invalidSignal)
    {
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        var ex = Assert.Throws<InvalidDeviceReadingException>(() => mic.UpdateSignalQuality(invalidSignal));
        Assert.Equal("MIC-01", ex.DeviceId);
        Assert.Equal(invalidSignal, ex.InvalidValue);
    }

    [Fact]
    public void Constructor_InvalidInitialBatteryOrSignal_ShouldThrowException()
    {
        Assert.Throws<InvalidDeviceReadingException>(() => new WirelessMicrophone("MIC-01", "Micro", initialBattery: 105, initialSignal: 50));
        Assert.Throws<InvalidDeviceReadingException>(() => new WirelessMicrophone("MIC-01", "Micro", initialBattery: 50, initialSignal: -1));
    }
}
