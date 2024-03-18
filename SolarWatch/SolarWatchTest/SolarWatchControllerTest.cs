using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarWatch.Controllers;
using SolarWatch.Data;
using SolarWatch.Services;

namespace SolarWatchTest;

[TestFixture]
public class SolarWatchControllerTest
{
    private Mock<ILogger<SolarWatchController>> _loggerMock;
    private Mock<ICityDataProvider> _coordDataProviderMock;
    private Mock<ISunsetSunriseDataProvider> _sunsetSunriseDataProviderMock;
    private Mock<IWeatherMapJsonProcessor> _jsonProcessorMock;
    private Mock<ISunsetSunriseJsonProcessor> _sunsetSunriseJsonProcessorMock;
    private Mock<SolarWatchContext> _solarWatchContextMock;
    private SolarWatchController _controller;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SolarWatchController>>();
        _coordDataProviderMock = new Mock<ICityDataProvider>();
        _sunsetSunriseDataProviderMock = new Mock<ISunsetSunriseDataProvider>();
        _jsonProcessorMock = new Mock<IWeatherMapJsonProcessor>();
        _sunsetSunriseJsonProcessorMock = new Mock<ISunsetSunriseJsonProcessor>();
        _solarWatchContextMock = new Mock<SolarWatchContext>();
        _controller = new SolarWatchController(_loggerMock.Object, _jsonProcessorMock.Object, _coordDataProviderMock.Object,
            _sunsetSunriseDataProviderMock.Object, _sunsetSunriseJsonProcessorMock.Object, _solarWatchContextMock.Object);
    }

    [Test]
    public async Task GetSunriseTimeNotFoundResultIfSunsetSunriseDataProviderFails()
    {
        _sunsetSunriseDataProviderMock.Setup(
            x => x.GetDataByLongitudeLatitude(It.IsAny<double>(), It.IsAny<double>()))
            .Throws(new Exception());

        #region _coordDataProviderMockRegion
        _coordDataProviderMock.Setup(x => x.GetDataByCity(It.IsAny<string>())).ReturnsAsync(@"
        [
            {
                ""name"": ""Budapest"",
                ""local_names"": {
                    ""li"": ""Boedapes"",
                    ""ga"": ""Búdaipeist"",
                    ""bn"": ""বুদাপেস্ট"",
                    ""tt"": ""Будапешт"",
                    ""it"": ""Budapest"",
                    ""sh"": ""Budimpešta"",
                    ""ps"": ""بوډاپسټ"",
                    ""uk"": ""Будапешт"",
                    ""ht"": ""Boudapès"",
                    ""sq"": ""Budapesti"",
                    ""he"": ""בודפשט"",
                    ""ka"": ""ბუდაპეშტი"",
                    ""av"": ""Будапешт"",
                    ""ku"": ""Budapeşt"",
                    ""ar"": ""بودابست"",
                    ""hy"": ""Բուդապեշտ"",
                    ""th"": ""บูดาเปสต์"",
                    ""ru"": ""Будапешт"",
                    ""fy"": ""Boedapest"",
                    ""es"": ""Budapest"",
                    ""af"": ""Boedapest"",
                    ""hu"": ""Budapest"",
                    ""ro"": ""Budapesta"",
                    ""ca"": ""Budapest"",
                    ""hi"": ""बुडापेस्ट"",
                    ""mn"": ""Будапешт"",
                    ""oc"": ""Budapèst"",
                    ""hr"": ""Budimpešta"",
                    ""kv"": ""Будапешт"",
                    ""de"": ""Budapest"",
                    ""pt"": ""Budapeste"",
                    ""lv"": ""Budapešta"",
                    ""cs"": ""Budapešť"",
                    ""ja"": ""ブダペスト"",
                    ""ml"": ""ബുഡാപെസ്റ്റ്"",
                    ""mk"": ""Будимпешта"",
                    ""mr"": ""बुडापेस्ट"",
                    ""ta"": ""புடாபெஸ்ட்"",
                    ""az"": ""Budapeşt"",
                    ""tr"": ""Budapeşte"",
                    ""kk"": ""Будапешт"",
                    ""uz"": ""Budapesht"",
                    ""pl"": ""Budapeszt"",
                    ""is"": ""Búdapest"",
                    ""bo"": ""བུ་ད་ཕེ་སིད།"",
                    ""ko"": ""부다페스트"",
                    ""bs"": ""Budimpešta"",
                    ""zh"": ""布達佩斯"",
                    ""tg"": ""Будапешт"",
                    ""kn"": ""ಬುಡಾಪೆಸ್ಟ್"",
                    ""nl"": ""Boedapest"",
                    ""eo"": ""Budapeŝto"",
                    ""el"": ""Βουδαπέστη"",
                    ""sr"": ""Будимпешта"",
                    ""la"": ""Budapestinum"",
                    ""sl"": ""Budimpešta"",
                    ""cv"": ""Будапешт"",
                    ""fa"": ""بوداپست"",
                    ""gr"": ""Βουδαπέστη"",
                    ""my"": ""ဗူးဒပက်မြို့"",
                    ""bg"": ""Будапеща"",
                    ""en"": ""Budapest"",
                    ""fr"": ""Budapest"",
                    ""no"": ""Budapest"",
                    ""lt"": ""Budapeštas"",
                    ""os"": ""Будапешт"",
                    ""gu"": ""બુડાપેસ્ટ"",
                    ""am"": ""ቡዳፔስት"",
                    ""ug"": ""Budapésht"",
                    ""be"": ""Будапешт"",
                    ""sk"": ""Budapešť"",
                    ""sv"": ""Budapest"",
                    ""ur"": ""بوداپست"",
                    ""yi"": ""בודאפעשט""
                },
                ""lat"": 47.4979937,
                ""lon"": 19.0403594,
                ""country"": ""HU""
            }
        ]");
        #endregion

        _jsonProcessorMock.Setup(x => x.GetLongitudeLatitude(It.IsAny<string>()))
            .Returns((47.4979937, 19.0403594));

        _sunsetSunriseJsonProcessorMock.Setup(x => x.GetSunrise(It.IsAny<string>())).Returns(new TimeOnly(1, 1));

        var result = await _controller.GetSunriseTime("sdfsfs");
        Console.WriteLine(result.Result);
        
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }
    
        [Test]
    public async Task GetSunsetTimeNotFoundResultIfSunsetSunriseDataProviderFails()
    {
        _sunsetSunriseDataProviderMock.Setup(
            x => x.GetDataByLongitudeLatitude(It.IsAny<double>(), It.IsAny<double>()))
            .Throws(new Exception());

        #region _coordDataProviderMockRegion
        _coordDataProviderMock.Setup(x => x.GetDataByCity(It.IsAny<string>())).ReturnsAsync(@"
        [
            {
                ""name"": ""Budapest"",
                ""local_names"": {
                    ""li"": ""Boedapes"",
                    ""ga"": ""Búdaipeist"",
                    ""bn"": ""বুদাপেস্ট"",
                    ""tt"": ""Будапешт"",
                    ""it"": ""Budapest"",
                    ""sh"": ""Budimpešta"",
                    ""ps"": ""بوډاپسټ"",
                    ""uk"": ""Будапешт"",
                    ""ht"": ""Boudapès"",
                    ""sq"": ""Budapesti"",
                    ""he"": ""בודפשט"",
                    ""ka"": ""ბუდაპეშტი"",
                    ""av"": ""Будапешт"",
                    ""ku"": ""Budapeşt"",
                    ""ar"": ""بودابست"",
                    ""hy"": ""Բուդապեշտ"",
                    ""th"": ""บูดาเปสต์"",
                    ""ru"": ""Будапешт"",
                    ""fy"": ""Boedapest"",
                    ""es"": ""Budapest"",
                    ""af"": ""Boedapest"",
                    ""hu"": ""Budapest"",
                    ""ro"": ""Budapesta"",
                    ""ca"": ""Budapest"",
                    ""hi"": ""बुडापेस्ट"",
                    ""mn"": ""Будапешт"",
                    ""oc"": ""Budapèst"",
                    ""hr"": ""Budimpešta"",
                    ""kv"": ""Будапешт"",
                    ""de"": ""Budapest"",
                    ""pt"": ""Budapeste"",
                    ""lv"": ""Budapešta"",
                    ""cs"": ""Budapešť"",
                    ""ja"": ""ブダペスト"",
                    ""ml"": ""ബുഡാപെസ്റ്റ്"",
                    ""mk"": ""Будимпешта"",
                    ""mr"": ""बुडापेस्ट"",
                    ""ta"": ""புடாபெஸ்ட்"",
                    ""az"": ""Budapeşt"",
                    ""tr"": ""Budapeşte"",
                    ""kk"": ""Будапешт"",
                    ""uz"": ""Budapesht"",
                    ""pl"": ""Budapeszt"",
                    ""is"": ""Búdapest"",
                    ""bo"": ""བུ་ད་ཕེ་སིད།"",
                    ""ko"": ""부다페스트"",
                    ""bs"": ""Budimpešta"",
                    ""zh"": ""布達佩斯"",
                    ""tg"": ""Будапешт"",
                    ""kn"": ""ಬುಡಾಪೆಸ್ಟ್"",
                    ""nl"": ""Boedapest"",
                    ""eo"": ""Budapeŝto"",
                    ""el"": ""Βουδαπέστη"",
                    ""sr"": ""Будимпешта"",
                    ""la"": ""Budapestinum"",
                    ""sl"": ""Budimpešta"",
                    ""cv"": ""Будапешт"",
                    ""fa"": ""بوداپست"",
                    ""gr"": ""Βουδαπέστη"",
                    ""my"": ""ဗူးဒပက်မြို့"",
                    ""bg"": ""Будапеща"",
                    ""en"": ""Budapest"",
                    ""fr"": ""Budapest"",
                    ""no"": ""Budapest"",
                    ""lt"": ""Budapeštas"",
                    ""os"": ""Будапешт"",
                    ""gu"": ""બુડાપેસ્ટ"",
                    ""am"": ""ቡዳፔስት"",
                    ""ug"": ""Budapésht"",
                    ""be"": ""Будапешт"",
                    ""sk"": ""Budapešť"",
                    ""sv"": ""Budapest"",
                    ""ur"": ""بوداپست"",
                    ""yi"": ""בודאפעשט""
                },
                ""lat"": 47.4979937,
                ""lon"": 19.0403594,
                ""country"": ""HU""
            }
        ]");
        #endregion

        _jsonProcessorMock.Setup(x => x.GetLongitudeLatitude(It.IsAny<string>()))
            .Returns((47.4979937, 19.0403594));

        _sunsetSunriseJsonProcessorMock.Setup(x => x.GetSunset(It.IsAny<string>())).Returns(new TimeOnly(1, 1));

        var result = await _controller.GetSunsetTime("sdfsfs");
        Console.WriteLine(result.Result);
        
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }
    
    [Test]
    public async Task GetSunriseTimeNotFoundResultIfCoordDataProviderFails()
    {
        _sunsetSunriseDataProviderMock.Setup(
            x => x.GetDataByLongitudeLatitude(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(@"
            {
                ""results"": {
                    ""sunrise"": ""2024-01-22T06:19:56+00:00"",
                    ""sunset"": ""2024-01-22T15:30:34+00:00"",
                    ""solar_noon"": ""2024-01-22T10:55:15+00:00"",
                    ""day_length"": 33038,
                    ""civil_twilight_begin"": ""2024-01-22T05:47:45+00:00"",
                    ""civil_twilight_end"": ""2024-01-22T16:02:45+00:00"",
                    ""nautical_twilight_begin"": ""2024-01-22T05:10:03+00:00"",
                    ""nautical_twilight_end"": ""2024-01-22T16:40:27+00:00"",
                    ""astronomical_twilight_begin"": ""2024-01-22T04:33:34+00:00"",
                    ""astronomical_twilight_end"": ""2024-01-22T17:16:56+00:00""
                },
                ""status"": ""OK"",
                ""tzid"": ""UTC""
            }");
        
        _coordDataProviderMock.Setup(x => x.GetDataByCity(It.IsAny<string>())).Throws(new Exception());

        _jsonProcessorMock.Setup(x => x.GetLongitudeLatitude(It.IsAny<string>()))
            .Returns((47.4979937, 19.0403594));

        _sunsetSunriseJsonProcessorMock.Setup(x => x.GetSunrise(It.IsAny<string>())).Returns(new TimeOnly(1, 1));

        var result = await _controller.GetSunriseTime("sdfsfs");
        Console.WriteLine(result.Result);
        
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }
}